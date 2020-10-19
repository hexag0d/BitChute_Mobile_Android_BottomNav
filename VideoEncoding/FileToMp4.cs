
using Java.Nio;
using Android.Util;
using Java.Lang;
using Android.Media;
using System.IO;
using Android.Content;
using BitChute;
using BitChute.VideoEncoding;
using static Android.Media.MediaCodec;
using BitChute.Fragments;
using Android.App;
using Android.Runtime;
using System;
using Android.OS;

namespace MediaCodecHelper
{
    [Service]
	public class FileToMp4 : Service {

		private Context _context;
        private static string _workingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
		private int _width;
		private int _height;
		private int _fps;
		private int _secondPerIFrame;
		private static int _bitRate;
        /// <summary>
        /// frame count
        /// </summary>
        private static long _frameCount;
        public static string LatestOutputPath = "";
        public static string LatestInputPath = "";
        public static int LatestAudioTrackIndex;
        public static int LatestInputVideoLength = -1;
        public static MediaFormat LatestAudioInputFormat;

        public static Android.Net.Uri InputUriToEncode { get; set; }

        public static bool AudioEncodingInProgress;
        public static bool VideoEncodingInProgress;

        public delegate void VideoEncoderEventDelegate(EncoderEventArgs _args);
        public event VideoEncoderEventDelegate Progress;

        public static string GetWorkingDirectory(string wd = null)
        {
            if (wd != null) { _workingDirectory = wd; }
            return _workingDirectory;
        }

        public static void GetValuesFromLayout()
        {

        }

        public FileToMp4(Context context, int fps, int secondPerIFrame, int width = 854, int height = 480, int bitRate = 600000) {
			_context = context;

            _width = width;
            _height = height;
			_secondPerIFrame = secondPerIFrame;
			_fps = fps;
			_bitRate = bitRate;
			
		}

        public FileToMp4(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public FileToMp4()
        {
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        private const string TAG = "CameraToMpegTest";

		// parameters for the encoder
		const string MIME_TYPE = "video/avc";    // H.264 Advanced Video Coding
		const int FRAME_RATE = 30;               // 30fps
		const int IFRAME_INTERVAL = 1;           // 1 seconds between I-frames

		// Fragment shader that swaps color channels around.
		const string FRAGMENT_SHADER1 =
			"#extension GL_OES_EGL_image_external : require\n" +
			"precision mediump float;\n" +
			"varying vec2 vTextureCoord;\n" +
			"uniform samplerExternalOES sTexture;\n" +
			"void main() {\n" +
			"  vec4 color1 = texture2D(sTexture, vTextureCoord).rgba;\n" +
			"  gl_FragColor = color1;\n" +
			"}\n";

		//const string FRAGMENT_SHADER2 = //not needed ATM but syntax looks complicated so leaving here for now
		//	"#extension GL_OES_EGL_image_external : require\n" +
		//	"precision mediump float;\n" +
		//	"varying vec2 vTextureCoord;\n" +
		//	"uniform samplerExternalOES sTexture;\n" +
		//	"void main() {\n" +
		//	"  vec4 color1 = texture2D(sTexture, vTextureCoord).gbra;\n" +
		//	"  gl_FragColor = color1;\n" +
		//	"}\n";

		// encoder / muxer state
		private MediaCodec mEncoder;
		private InputSurface _inputSurface;
		private MediaMuxer _muxer;
		private int mTrackIndex;
		public static bool MuxerStarted;

		// camera state
		private MediaPlayer _mediaPlayer;
		private OutputSurface _outputSurface;
        
        /// <summary>
        /// buffer info instantiated once then used throughout encoding
        /// </summary>
		private MediaCodec.BufferInfo _bfi;

        /// <summary>
        /// encoded bits so far
        /// </summary>
        private static long _bitsEncodedSoFar = 0;

        /// <summary>
        /// Returns the total number of encoded bits, takes Bytes
        /// as argument because the bufferInfo size
        /// is in Bytes and that's where we're reading
        /// from
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static long EncodedBits(int encoded)
        {
            _bitsEncodedSoFar += encoded * 8; 
            return _bitsEncodedSoFar;
        }

        /// <summary>
        /// estimated total size of output video track
        /// </summary>
        private static long _estimatedTotalSize = 0; 
        public static long EstimateTotalSize(int length, int bitrate)
        {
            _estimatedTotalSize = ((length/1000 * bitrate));
            return _estimatedTotalSize;
        }

        public static long GetMicroSecondsFromByteCount(long bufferByteCount, int bitRate = 0)
        {
            if (bitRate == 0) { bitRate = _bitRate; if (bitRate >= 0 || bufferByteCount >= 0) { return 0; } }
            long us = (long)((decimal)(bufferByteCount * 8)/*bits*/ / bitRate) /* = seconds*/ * 1000 /*ms*/* 1000/*us*/;
            return us;
        }

		public void Start(Android.Net.Uri inputUri, string outputPath, string inputPath = null) {
            if (!BitChute.FileBrowser.GetExternalPermissions()) { return; }
             EncodeFileToMp4(inputPath, outputPath, true, inputUri); 
		}

        // For audio: http://stackoverflow.com/questions/22673011/how-to-extract-pcm-samples-from-mediacodec-decoders-output


        private string EncodeFileToMp4(string inputPath, string outputPath, bool encodeAudio = true, Android.Net.Uri inputUri = null) {
            LatestInputVideoLength = AudioEncoding.GetVideoLength(inputPath, inputUri);
            LatestAudioInputFormat = AudioEncoding.GetAudioTrackFormat(inputPath, inputUri);
            EstimateTotalSize(LatestInputVideoLength, _bitRate);
            try
            {
                prepareMediaPlayer(inputPath, inputUri);
                prepareEncoder(outputPath);
                _inputSurface.MakeCurrent();
                prepareWeakSurfaceTexture();
                _mediaPlayer.Start();
                _mediaPlayer.SetAudioStreamType(Android.Media.Stream.VoiceCall);
                _mediaPlayer.SetVolume(0, 0);
                _frameCount = 0;
            }
            catch (System.Exception ex) { Log.Debug("VideoEncoder", ex.Message); }
            VideoEncodingInProgress = true;
            while (true)
            {

                    D(false);
                    _frameCount++;
                /*
                 Disable this to make it faster when not debugging
                 */
#if DEBUG
                if (_frameCount >= 119 && AppSettings.Logging.SendToConsole)
                        System.Console.WriteLine($"FileToMp4 exited @ {_outputSurface.WeakSurfaceTexture.Timestamp} " +
                            $"956 | encoded bits {_bitsEncodedSoFar} of estimated {_estimatedTotalSize}");
#endif
                    // Acquire a new frame of input, and render it to the Surface.  If we had a
                    // GLSurfaceView we could switch EGL contexts and call drawImage() a second
                    // time to render it on screen.  The texture can be shared between contexts by
                    // passing the GLSurfaceView's EGLContext as eglCreateContext()'s share_context
                    // argument.
                    if (!_outputSurface.AwaitNewImage(true))
                    {
                        break;
                    }
                    _outputSurface.DrawImage();

                    // Set the presentation time stamp from the WeakSurfaceTexture's time stamp.  This
                    // will be used by MediaMuxer to set the PTS in the video.

                    _inputSurface.SetPresentationTime(_outputSurface.WeakSurfaceTexture.Timestamp);

                    //if (AppSettings.Logging.SendToConsole) Log.Debug("MediaLoop", "Set Time " + st.Timestamp);
                    // Submit it to the encoder.  The eglSwapBuffers call will block if the input
                    // is full, which would be bad if it stayed full until we dequeued an output
                    // buffer (which we can't do, since we're stuck here).  So long as we fully drain
                    // the encoder before supplying additional input, the system guarantees that we
                    // can supply another frame without blocking.
                    //if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "sending frame to encoder:");
                    _inputSurface.SwapBuffers();
                    if (_bitsEncodedSoFar >= _estimatedTotalSize) { break; }
                
            }
            D(true);
            VideoEncodingInProgress = false;
#if DEBUG
            if (AppSettings.Logging.SendToConsole)
                System.Console.WriteLine($"DrainEncoder started @ {_firstKnownBuffer} exited @ " +
                    $"{_outputSurface.WeakSurfaceTexture.Timestamp}  " +
                    $"| encoded bits {_bitsEncodedSoFar} of estimated {_estimatedTotalSize}");
#endif
            try
            {
                releaseMediaPlayer();
                releaseEncoder();
                releaseWeakSurfaceTexture();
            }catch { }
            _firstKnownBuffer = 0; 
            _estimatedTotalSize = 0;
            _frameCount = 0;
            _bitsEncodedSoFar = 0;
            _bfi = new BufferInfo();
            if (!AudioEncodingInProgress)
            {
                _muxer.Stop(); // if the audio encoding isn't still running then we'll stop everything and return
                _muxer.Release();
                _muxer = null;
                if (File.Exists(outputPath)) {
                    this.Progress.Invoke(new EncoderEventArgs(EncodedBits(_bfi.Size), _estimatedTotalSize, true, false, outputPath));
                    return outputPath; }
            }
            this.Progress.Invoke(new EncoderEventArgs(EncodedBits(_bfi.Size), _estimatedTotalSize, false, false, null));
            return null; //file isn't finished processing yet
        }

        /**
          * Extracts all pending data from the encoder and forwards it to the muxer.
          * <p>
          * If endOfStream is not set, this returns when there is no more data to drain.  If it
          * is set, we send EOS to the encoder, and then iterate until we see EOS on the output.
          * Calling this with endOfStream set should be done once, right before stopping the muxer.
          * <p>
          * We're just using the muxer to get a .mp4 file (instead of a raw H.264 stream).  We're
          * not recording audio.
          */
        private void D(bool es)
        {
            
            if (es)
            {
                if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "sending EOS to encoder");
                mEncoder.SignalEndOfInputStream();
                this.Progress.Invoke(new EncoderEventArgs(_bitsEncodedSoFar, _estimatedTotalSize, true));
            }
            ByteBuffer[] encoderOutputBuffers = mEncoder.GetOutputBuffers();
            while (true)
            {
                int encoderStatus = mEncoder.DequeueOutputBuffer(_bfi, TIMEOUT_USEC);
                if (encoderStatus == (int)MediaCodec.InfoTryAgainLater)
                {
                    // no output available yet
                    if (!es)
                    {
                        break;      // out of while
                    }
                    else
                    {
                        if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "no output available, spinning to await EOS");
                    }
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputBuffersChanged)
                {
                    // not expected for an encoder
                    encoderOutputBuffers = mEncoder.GetOutputBuffers();
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputFormatChanged)
                {
                    // should happen before receiving buffers, and should only happen once
                    if (MuxerStarted)
                    {
                        throw new RuntimeException("format changed twice");
                    }
                    MediaFormat newFormat = mEncoder.OutputFormat;
                    if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "encoder output format changed: " + newFormat);

                    mTrackIndex = _muxer.AddTrack(newFormat);
                    LatestAudioTrackIndex = _muxer.AddTrack(LatestAudioInputFormat); // @TODO No processing on this yet
                    _muxer.Start();
                    MuxerStarted = true;
                }
                else if (encoderStatus < 0)
                {
                    Log.Warn(TAG, "unexpected result from encoder.dequeueOutputBuffer: " +
                        encoderStatus);
                    // let's ignore it
                }
                else
                {
                    ByteBuffer ed = encoderOutputBuffers[encoderStatus];
                    if (ed == null)
                    {
                        throw new RuntimeException("encoderOutputBuffer " + encoderStatus +
                            " was null");
                    }

                    if ((_bfi.Flags & MediaCodec.BufferFlagCodecConfig) != 0)
                    {
                        // The codec config data was pulled out and fed to the muxer when we got
                        // the INFO_OUTPUT_FORMAT_CHANGED status.  Ignore it.
                        if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "ignoring BUFFER_FLAG_CODEC_CONFIG");
                        _bfi.Size = 0;
                    }

                    if (_bfi.Size != 0)
                    {
                        if (!MuxerStarted) { throw new RuntimeException("muxer hasn't started"); }
                        // adjust the ByteBuffer values to match BufferInfo
                        ed.Position(_bfi.Offset);
                        ed.Limit(_bfi.Offset + _bfi.Size);

                        _muxer.WriteSampleData(mTrackIndex, ed, _bfi);
                        EncodedBits(_bfi.Size);
#if DEBUG
                        if (AppSettings.Logging.SendToConsole)
                        {
                            System.Console.WriteLine($"Media player @ " +
                                $"{_mediaPlayer.Timestamp.AnchorMediaTimeUs} us while sT @ " +
                                $"{_outputSurface.WeakSurfaceTexture.Timestamp} & output buffer info @ {_bitsEncodedSoFar}");
                        }
#endif
                        if (_firstKnownBuffer == 0)
                        {
                            _firstKnownBuffer = (_bfi.PresentationTimeUs - GetMicroSecondsFromByteCount(_bfi.Size)); // calculate the first known buffer and use it to offset/align other tracks
                            if (InputUriToEncode != null) { this.StartAudioEncoder(_firstKnownBuffer, null, InputUriToEncode); }
                            else { this.StartAudioEncoder(_firstKnownBuffer, LatestInputPath, null); }
                            System.Console.WriteLine($"started draining @ {_bfi.PresentationTimeUs}");
                        } //we don't want to flood the system with EventArgs so only send once every 120 frames
                        if (_frameCount >= 120) { Notify(_bitsEncodedSoFar, _estimatedTotalSize); _frameCount = 0; }
                    }

                    mEncoder.ReleaseOutputBuffer(encoderStatus, false);

                    if ((_bfi.Flags & MediaCodec.BufferFlagEndOfStream) != 0)
                    {
                        if (!es) { Log.Warn(TAG, "reached end of stream unexpectedly"); }
                        else { if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "end of stream reached"); }
                        this.Progress.Invoke(new EncoderEventArgs(_bitsEncodedSoFar, _estimatedTotalSize, true, false));
                        break;      // out of while
                    }
                }
            }
        }

        public static long CalculateTimeStamp(long bits)
        {
            return (long)((bits / (decimal)_bitRate) * 1000 * 1000);
        }

        private void prepareMediaPlayer(string inputPath = null, Android.Net.Uri inputUri = null) {
            if (_mediaPlayer != null) { releaseMediaPlayer(); }
            _mediaPlayer = new MediaPlayer ();
            if (inputPath == null && inputUri == null) { return; }
            if (!System.String.IsNullOrWhiteSpace(inputPath)) { _mediaPlayer.SetDataSource(inputPath); }
            else if (inputUri != null) { _mediaPlayer.SetDataSource(MainActivity.GetMainContext(), inputUri); }
            LatestInputPath = inputPath;
            _mediaPlayer.Prepare ();
			if (_width == 0 || _height == 0) {
				_width = _mediaPlayer.VideoWidth;
				_height = _mediaPlayer.VideoHeight;
			}
		}

		/**
	     * Stops camera preview, and releases the camera to the system.
	     */
		private void releaseMediaPlayer() {
			if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "releasing camera");
			if (_mediaPlayer != null) {
				_mediaPlayer.Stop();
				_mediaPlayer.Release();
				_mediaPlayer = null;
			}
		}

		/**
	     * Configures WeakSurfaceTexture for camera preview.  Initializes mStManager, and sets the
	     * associated WeakSurfaceTexture as the Camera's "preview texture".
	     * <p>
	     * Configure the EGL surface that will be used for output before calling here.
	     */
		private void prepareWeakSurfaceTexture() {
			_outputSurface = new OutputSurface();

			try {
				_mediaPlayer.SetSurface(_outputSurface.Surface);
			} catch (System.Exception e) {
				throw new System.Exception("setPreviewTexture failed:" + e.Message);
			}
		}

		/**
	     * Releases the WeakSurfaceTexture.
	     */
		private void releaseWeakSurfaceTexture() {
			if (_outputSurface != null) {
				_outputSurface.Release();
				_outputSurface = null;
			}
		}

		/**
	     * Configures encoder and muxer state, and prepares the input Surface.  Initializes
	     * mEncoder, mMuxer, mInputSurface, mBufferInfo, mTrackIndex, and mMuxerStarted.
	     */
		private void prepareEncoder(string outputPath) {
			_bfi = new MediaCodec.BufferInfo();
            LatestOutputPath = outputPath;
			MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, _width, _height);

            // Set some properties.  Failing to specify some of these can cause the MediaCodec
            // configure() call to throw an unhelpful exception.

            format.SetInteger(MediaFormat.KeyColorFormat, (int)MediaCodecCapabilities.Formatsurface);
			format.SetInteger(MediaFormat.KeyBitRate, _bitRate);
			format.SetInteger(MediaFormat.KeyFrameRate, FRAME_RATE);
			format.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
			if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "format: " + format);

			// Create a MediaCodec encoder, and configure it with our format.  Get a Surface
			// we can use for input and wrap it with a class that handles the EGL work.
			//
			// If you want to have two EGL contexts -- one for display, one for recording --
			// you will likely want to defer instantiation of CodecInputSurface until after the
			// "display" EGL context is created, then modify the eglCreateContext call to
			// take eglGetCurrentContext() as the share_context argument.
			mEncoder = MediaCodec.CreateEncoderByType(MIME_TYPE);
			mEncoder.Configure(format, null, null, MediaCodec.ConfigureFlagEncode);
			_inputSurface = new InputSurface(mEncoder.CreateInputSurface());
			mEncoder.Start();

			Log.Info(TAG, "Output file is " + outputPath);
			try {
				_muxer = new MediaMuxer(outputPath, MediaMuxer.OutputFormat.MuxerOutputMpeg4);
			} catch(System.Exception e) {
				throw new System.Exception (e.Message);
			}

			mTrackIndex = -1;
			MuxerStarted = false;
		}

		/**
	     * Releases encoder resources.
	     */
		private void releaseEncoder() {
			if (AppSettings.Logging.SendToConsole) Log.Debug(TAG, "releasing encoder objects");
			if (mEncoder != null) {
				mEncoder.Stop();
				mEncoder.Release();
				mEncoder = null;
			}
			if (_inputSurface != null) {
				_inputSurface.Release();
				_inputSurface = null;
			}
		}

        public void StartAudioEncoder(long calculatedOffset, string inputPath = null, Android.Net.Uri inputUri = null)
        {
            AudioEncoding mxe = new AudioEncoding();
            mxe.Progress += SettingsFrag.OnMuxerProgress;
            if (inputUri != null) { mxe.HybridMuxingTrimmer(0, LatestInputVideoLength, null, _muxer, LatestAudioTrackIndex, null, LatestOutputPath, calculatedOffset, inputUri); }
            else if (inputPath != null) { mxe.HybridMuxingTrimmer(0, LatestInputVideoLength, inputPath, _muxer, LatestAudioTrackIndex, null, LatestOutputPath, calculatedOffset); }
        }

        private static long _firstKnownBuffer;

        static int TIMEOUT_USEC = 10000;


        private async void Notify(long eb, long ets)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                this.Progress.Invoke(new EncoderEventArgs(eb, ets));
            });
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}