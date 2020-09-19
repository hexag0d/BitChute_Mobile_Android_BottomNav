
using Java.Nio;
using Camera = Android.Hardware.Camera;
using Android.Util;
using EGL14 = Android.Opengl.EGL14;
using GLES20 = Android.Opengl.GLES20;
using GLES11Ext = Android.Opengl.GLES11Ext;
using GLSurfaceView = Android.Opengl.GLSurfaceView;
using IEGL10 = Javax.Microedition.Khronos.Egl.IEGL10;
using EGL10 = Javax.Microedition.Khronos.Egl.EGL10;
using EGLConfig = Javax.Microedition.Khronos.Egl.EGLConfig;
using EGLContext = Javax.Microedition.Khronos.Egl.EGLContext;
using EGLDisplay = Javax.Microedition.Khronos.Egl.EGLDisplay;
using EGLSurface = Javax.Microedition.Khronos.Egl.EGLSurface;
using GL = Javax.Microedition.Khronos.Opengles.IGL;
using GL10 = Javax.Microedition.Khronos.Opengles.GL10; // IGL10?
using Java.Lang;
using Android.Media;
using System.IO;
using MediaCodecHelper;
using Android.Content;
using System.Drawing;
using BitChute;
using BitChute.VideoEncoding;
using static Android.Media.MediaCodec;
using BitChute.Fragments;
using BitChute.Classes;
using Android.App;

namespace MediaCodecHelper {
    [Service]
	public class FileToMp4  {

		private Context _context;
        private static string _workingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
        bool VERBOSE = false;
		private int _width;
		private int _height;
		private int _fps;
		private int _secondPerIFrame;
		private static int _bitRate;
        private static int _frameCount;
        public static string LatestOutputPath = "";
        public static string LatestInputPath = "";
        public static int LatestAudioTrackIndex;
        public static int LatestInputVideoLength = -1;
        public static MediaFormat LatestAudioInputFormat;

        public static bool AudioEncodingInProgress;
        public static bool VideoEncodingInProgress;

        public delegate void VideoEncoderEventDelegate(EncoderEventArgs _args);
        public event VideoEncoderEventDelegate Progress;

        public static string GetWorkingDirectory(string wd = null)
        {
            if (wd != null) { _workingDirectory = wd; }
            return _workingDirectory;
        }

        public FileToMp4(Context context, int fps, int secondPerIFrame, System.Drawing.Size? outputSize, int bitRate = 600000) {
			_context = context;

			if (outputSize.HasValue) {
				_width = outputSize.Value.Width;
				_height = outputSize.Value.Height;
			}
            _width = 854;
            _height = 480;
			_secondPerIFrame = secondPerIFrame;
			_fps = fps;
			_bitRate = bitRate;
			
		}
        
		private const string TAG = "CameraToMpegTest";

		// parameters for the encoder
		const string MIME_TYPE = "video/avc";    // H.264 Advanced Video Coding
		const int FRAME_RATE = 24;               // 30fps
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
		private MediaMuxer mMuxer;
		private int mTrackIndex;
		public static bool MuxerStarted;

		// camera state
		private MediaPlayer _mediaPlayer;
		private OutputSurface _outputSurface;

		// allocate one of these up front so we don't need to do it every time
		private MediaCodec.BufferInfo mBufferInfo;
        private static long _encodedBits = 0;
        public static long EncodedBits(int encoded)
        {
            _encodedBits += encoded * 8; 
            return _encodedBits;
        }

        private static long _estimatedTotalSize = 0;
        public static long EstimateTotalSize(int length, int bitrate)
        {
            _estimatedTotalSize = ((length/1000 * bitrate));
            return _estimatedTotalSize;
        }

		public void Start(string inputPath, string outputPath) {
            BitChute.Classes.FileBrowser.GetExternalPermissions();
             EncodeCameraToMp4(inputPath, outputPath); 
		}

        // For audio: http://stackoverflow.com/questions/22673011/how-to-extract-pcm-samples-from-mediacodec-decoders-output

        private string EncodeCameraToMp4(string inputPath, string outputPath, bool encodeAudio = true, Android.Net.Uri inputUri = null) {
            LatestInputVideoLength = MuxerEncoding.GetVideoLength(inputPath);
            LatestAudioInputFormat = MuxerEncoding.GetAudioTrackFormat(inputPath);
            EstimateTotalSize(LatestInputVideoLength, _bitRate);
            Android.Graphics.SurfaceTexture st = null;
            try
            {
                prepareMediaPlayer(inputPath);
                prepareEncoder(outputPath);
                _inputSurface.MakeCurrent();
                prepareSurfaceTexture();
                _mediaPlayer.Start();
                _mediaPlayer.SetAudioStreamType(Android.Media.Stream.VoiceCall);
                _mediaPlayer.SetVolume(0, 0);
                _frameCount = 0;
                st = _outputSurface.SurfaceTexture;
            }
            catch (Exception ex) { Log.Debug("VideoEncoder", ex.Message); }
            VideoEncodingInProgress = true;
            while (true)
               {
                    drainEncoder(false);
                    _frameCount++;
                if (_frameCount >= 30 && AppSettings.Logging.SendToConsole) 
                System.Console.WriteLine($"FileToMp4 while @ {_firstKnownBuffer} exited @ {st.Timestamp}  | encoded bits {_encodedBits} of estimated {_estimatedTotalSize}");
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

                    // Set the presentation time stamp from the SurfaceTexture's time stamp.  This
                    // will be used by MediaMuxer to set the PTS in the video.

                    _inputSurface.SetPresentationTime(st.Timestamp);
                    
                    //if (VERBOSE) Log.Debug("MediaLoop", "Set Time " + st.Timestamp);
                    // Submit it to the encoder.  The eglSwapBuffers call will block if the input
                    // is full, which would be bad if it stayed full until we dequeued an output
                    // buffer (which we can't do, since we're stuck here).  So long as we fully drain
                    // the encoder before supplying additional input, the system guarantees that we
                    // can supply another frame without blocking.
                    //if (VERBOSE) Log.Debug(TAG, "sending frame to encoder:");
                    _inputSurface.SwapBuffers();
                if (_encodedBits >= _estimatedTotalSize) { break; }
               }
               drainEncoder(true);
            VideoEncodingInProgress = false;
            if (AppSettings.Logging.SendToConsole)
                System.Console.WriteLine($"DrainEncoder started @ {_firstKnownBuffer} exited @ {st.Timestamp}  | encoded bits {_encodedBits} of estimated {_estimatedTotalSize}");
            try
            {
                releaseMediaPlayer();
                releaseEncoder();
                releaseSurfaceTexture();
            }catch { }
            _firstKnownBuffer = 0; //this stores the audio encoder offset long
            if (!AudioEncodingInProgress)
            {
                mMuxer.Stop(); // if the audio encoding isn't still running then we'll stop everything and return
                mMuxer.Release();
                mMuxer = null;
                if (File.Exists(outputPath)) {
                    this.Progress.Invoke(new EncoderEventArgs(EncodedBits(mBufferInfo.Size), _estimatedTotalSize, true, false, outputPath));
                    return outputPath; }
            }
            this.Progress.Invoke(new EncoderEventArgs(EncodedBits(mBufferInfo.Size), _estimatedTotalSize, false, false, null));
            return null; //file isn't finished processing yet
        }

		private void prepareMediaPlayer(string inputPath) {
			_mediaPlayer = new MediaPlayer ();
			_mediaPlayer.SetDataSource (inputPath);
            LatestInputPath = inputPath; //for tracking purposes but can probably be axed eventually
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
			if (VERBOSE) Log.Debug(TAG, "releasing camera");
			if (_mediaPlayer != null) {
				_mediaPlayer.Stop();
				_mediaPlayer.Release();
				_mediaPlayer = null;
			}
		}

		/**
	     * Configures SurfaceTexture for camera preview.  Initializes mStManager, and sets the
	     * associated SurfaceTexture as the Camera's "preview texture".
	     * <p>
	     * Configure the EGL surface that will be used for output before calling here.
	     */
		private void prepareSurfaceTexture() {
			_outputSurface = new OutputSurface();
			var st = _outputSurface.Surface;
			try {
				_mediaPlayer.SetSurface(st);
			} catch (System.Exception e) {
				throw new System.Exception("setPreviewTexture failed:" + e.Message);
			}
		}

		/**
	     * Releases the SurfaceTexture.
	     */
		private void releaseSurfaceTexture() {
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
			mBufferInfo = new MediaCodec.BufferInfo();
            LatestOutputPath = outputPath;
			MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, _width, _height);

			// Set some properties.  Failing to specify some of these can cause the MediaCodec
			// configure() call to throw an unhelpful exception.
			format.SetInteger(MediaFormat.KeyColorFormat, (int) MediaCodecCapabilities.Formatsurface);
			format.SetInteger(MediaFormat.KeyBitRate, _bitRate);
			format.SetInteger(MediaFormat.KeyFrameRate, FRAME_RATE);
			format.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
			if (VERBOSE) Log.Debug(TAG, "format: " + format);

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

			// Create a MediaMuxer.  We can't add the video track and start() the muxer here,
			// because our MediaFormat doesn't have the Magic Goodies.  These can only be
			// obtained from the encoder after it has started processing data.
			//
			// We're not actually interested in multiplexing audio.  We just want to convert
			// the raw H.264 elementary stream we get from MediaCodec into a .mp4 file.
			try {
				mMuxer = new MediaMuxer(outputPath, MediaMuxer.OutputFormat.MuxerOutputMpeg4);
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
			if (VERBOSE) Log.Debug(TAG, "releasing encoder objects");
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

        public void StartAudioEncoder(long calculatedOffset)
        {
            MuxerEncoding mxe = new MuxerEncoding();
            mxe.Progress += SettingsFrag.OnMuxerProgress;
            mxe.HybridMuxingTrimmer(0, LatestInputVideoLength, LatestInputPath, mMuxer, LatestAudioTrackIndex, null, LatestOutputPath, calculatedOffset);
        }

        private static long _firstKnownBuffer;

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
		private void drainEncoder(bool endOfStream) {
			int TIMEOUT_USEC = 10000;
			if (VERBOSE) Log.Debug(TAG, "drainEncoder(" + endOfStream + ")");
			if (endOfStream) {
				if (VERBOSE) Log.Debug(TAG, "sending EOS to encoder");
				mEncoder.SignalEndOfInputStream();
                this.Progress.Invoke(new EncoderEventArgs(_encodedBits, _estimatedTotalSize, true));
            }
            ByteBuffer[] encoderOutputBuffers = mEncoder.GetOutputBuffers();
			while (true) {
				int encoderStatus = mEncoder.DequeueOutputBuffer(mBufferInfo, TIMEOUT_USEC);
				if (encoderStatus == (int) MediaCodec.InfoTryAgainLater) {
					// no output available yet
					if (!endOfStream) {
						break;      // out of while
					} else {
						if (VERBOSE) Log.Debug(TAG, "no output available, spinning to await EOS");
					}
				} else if (encoderStatus == (int) MediaCodec.InfoOutputBuffersChanged) {
					// not expected for an encoder
					encoderOutputBuffers = mEncoder.GetOutputBuffers();
				} else if (encoderStatus == (int) MediaCodec.InfoOutputFormatChanged) {
					// should happen before receiving buffers, and should only happen once
					if (MuxerStarted) {
						throw new RuntimeException("format changed twice");
					}
					MediaFormat newFormat = mEncoder.OutputFormat;
					if (VERBOSE) Log.Debug(TAG, "encoder output format changed: " + newFormat);

					// now that we have the Magic Goodies, start the muxer
					mTrackIndex = mMuxer.AddTrack(newFormat);
                    LatestAudioTrackIndex = mMuxer.AddTrack(LatestAudioInputFormat); // @TODO No processing on this yet
					mMuxer.Start();
					MuxerStarted = true;
                }
                else if (encoderStatus < 0) {
					Log.Warn(TAG, "unexpected result from encoder.dequeueOutputBuffer: " +
						encoderStatus);
					// let's ignore it
				} else {
					ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
					if (encodedData == null) {
						throw new RuntimeException("encoderOutputBuffer " + encoderStatus +
							" was null");
					}

					if ((mBufferInfo.Flags & MediaCodec.BufferFlagCodecConfig) != 0) {
						// The codec config data was pulled out and fed to the muxer when we got
						// the INFO_OUTPUT_FORMAT_CHANGED status.  Ignore it.
						if (VERBOSE) Log.Debug(TAG, "ignoring BUFFER_FLAG_CODEC_CONFIG");
						mBufferInfo.Size = 0;
					}

                    if (mBufferInfo.Size != 0)
                    {
                        if (!MuxerStarted)
                        {
                            throw new RuntimeException("muxer hasn't started");
                        }
                        // adjust the ByteBuffer values to match BufferInfo
                        encodedData.Position(mBufferInfo.Offset);
                        encodedData.Limit(mBufferInfo.Offset + mBufferInfo.Size);
                        mMuxer.WriteSampleData(mTrackIndex, encodedData, mBufferInfo);
                        EncodedBits(mBufferInfo.Size);
                        if (_firstKnownBuffer == 0)
                        {
                            _firstKnownBuffer = mBufferInfo.PresentationTimeUs;
                            this.StartAudioEncoder(_firstKnownBuffer);
                            System.Console.WriteLine($"started draining @ {mBufferInfo.PresentationTimeUs}");
                        } //we don't want to flood the system with EventArgs so only send once every 30 frames
                        if (_frameCount >= 30) { this.Progress.Invoke(new EncoderEventArgs(_encodedBits, _estimatedTotalSize)); _frameCount = 0; }
                        if (VERBOSE) Log.Debug(TAG, "sent " + mBufferInfo.Size + " bytes to muxer"+ @" @ pt = " + mBufferInfo.PresentationTimeUs);
					}

					mEncoder.ReleaseOutputBuffer(encoderStatus, false);

					if ((mBufferInfo.Flags & MediaCodec.BufferFlagEndOfStream) != 0) {
						if (!endOfStream) { Log.Warn(TAG, "reached end of stream unexpectedly");}
                        else { if (VERBOSE) Log.Debug(TAG, "end of stream reached"); }
                        this.Progress.Invoke(new EncoderEventArgs(_encodedBits, _estimatedTotalSize, true, false));
                        break;      // out of while
                    }
				}
			}
        }
	}
}