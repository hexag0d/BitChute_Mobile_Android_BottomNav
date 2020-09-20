
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
using static BitChute.Classes.FileBrowser;
using Android.Runtime;
using System;
using Android.OS;

namespace MediaCodecHelper {
    [Service]
	public class FileToMp4 : Service {

		private Context _context;
        private static string _workingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
        bool VERBOSE = false;
		private int _width;
		private int _height;
		private int _fps;
		private int _secondPerIFrame;
		private static int _bitRate;
        /// <summary>
        /// frame count
        /// </summary>
        private static int _fC;
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
		private MediaCodec _encoder;
        private MediaCodec _decoder;
        private static ByteBuffer _dbf;

        private static BufferInfo _dbi;
        public static Tuple<MediaExtractor, MediaFormat> DecoderFormat;
        private static int _videoBufferSize;
		private InputSurface _inputSurface;
		private MediaMuxer _muxer;
		private int _trackIndex;
		public static bool MuxerStarted;

		// camera state
		private MediaPlayer _mediaPlayer;
		private OutputSurface _outputSurface;

		// allocate one of these up front so we don't need to do it every time
        /// <summary>
        /// MediaCodec bufferinfo 
        /// </summary>
		private MediaCodec.BufferInfo _bfi;

        /// <summary>
        /// encoded bits so far
        /// </summary>
        private static long _ebt = 0;

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
            _ebt += encoded * 8; 
            return _ebt;
        }

        /// <summary>
        /// estimated total size of output video track
        /// </summary>
        private static long _eTS = 0; //trimmed for memory conservation
        public static long EstimateTotalSize(int length, int bitrate)
        {
            _eTS = ((length/1000 * bitrate));
            return _eTS;
        }

        public static long CorrectForStartTime(int firstBlockBits, long firstBlockTimeStamp)
        {
            var startingTime = ((decimal)firstBlockBits / (decimal)_bitRate); /* start time in ms */
            var startTimeLong = (long)(startingTime * 1000 /*ms*/ * 1000 /*us*/ * 1000 /*ns*/);
            return (firstBlockTimeStamp - startTimeLong);
        }

        public static System.Threading.Tasks.Task<Tuple<MediaExtractor, MediaFormat>> GetVideoTrackFormat(string filepath, Android.Net.Uri inputUri = null)
        {
            MediaExtractor extractor = new MediaExtractor();
            if (inputUri != null) { extractor.SetDataSource(Android.App.Application.Context, inputUri, null); }
            else if (filepath != null) { extractor.SetDataSource(filepath); }
            int trackCount = extractor.TrackCount;
            int bufferSize = -1;
            for (int i = 0; i < trackCount; i++)
            {
                MediaFormat format = extractor.GetTrackFormat(i);
                string mime = format.GetString(MediaFormat.KeyMime);
                bool selectCurrentTrack = false;
                if (mime.StartsWith("audio/")) { selectCurrentTrack = false; } //routed in muxer
                else if (mime.StartsWith("video/")) { selectCurrentTrack = true; }
                if (selectCurrentTrack)
                {
                    extractor.SelectTrack(i);
                    if (format.ContainsKey(MediaFormat.KeyMaxInputSize))
                    {
                        int newSize = format.GetInteger(MediaFormat.KeyMaxInputSize);
                        _videoBufferSize = newSize > bufferSize ? newSize : bufferSize;
                    }
                    DecoderFormat = new Tuple<MediaExtractor, MediaFormat>(extractor, format);
                    return System.Threading.Tasks.Task.FromResult(DecoderFormat);
                }
            }
            return null;
        }


        public void Start(Android.Net.Uri inputUri, string outputPath, string inputPath = null) {
            BitChute.Classes.FileBrowser.GetExternalPermissions();
             EncodeCameraToMp4(inputPath, outputPath, true, inputUri); 
		}

        // For audio: http://stackoverflow.com/questions/22673011/how-to-extract-pcm-samples-from-mediacodec-decoders-output


        /**
	     * Configures encoder and muxer state, and prepares the input Surface.  Initializes
	     * mEncoder, mMuxer, mInputSurface, mBufferInfo, mTrackIndex, and mMuxerStarted.
	     */
        private void prepareEncoder(string outputPath, Android.Net.Uri inputUri = null)
        {
            if (inputUri == null) { return; }
            
            _bfi = new MediaCodec.BufferInfo();
            LatestOutputPath = outputPath;
            GetVideoTrackFormat(null, inputUri); // get the input video track format for decoder
            MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, _width, _height); // this is the output encoder video format
            //set the encoder values that we want to output video for
            format.SetInteger(MediaFormat.KeyColorFormat, (int)MediaCodecCapabilities.Formatsurface);
            format.SetInteger(MediaFormat.KeyBitRate, _bitRate);
            format.SetInteger(MediaFormat.KeyFrameRate, FRAME_RATE);
            format.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
            format.SetInteger(MediaFormat.KeyHeight, _height);
            format.SetInteger(MediaFormat.KeyWidth, _width);
            if (AppSettings.Logging.SendToLogCat) { Log.Info(TAG, "format: " + format); }
            // Create a MediaCodec encoder, and configure it with our format.  Get a Surface
            // we can use for input and wrap it with a class that handles the EGL work.

            // If you want to have two EGL contexts -- one for display, one for recording --
            // you will likely want to defer instantiation of CodecInputSurface until after the
            // "display" EGL context is created, then modify the eglCreateContext call to
            // take eglGetCurrentContext() as the share_context argument.
            _encoder = MediaCodec.CreateEncoderByType(MIME_TYPE); // create generic video encoder from MIME
            
            _encoder.Configure(format, null, null, MediaCodecConfigFlags.Encode); // configure the encoder
            _inputSurface = new InputSurface(_encoder.CreateInputSurface()); // assign input

            _encoder.Start(); //start the encoder

            if (AppSettings.Logging.SendToLogCat) { Log.Info(TAG, "Output file is " + outputPath); }
            _inputSurface.MakeCurrent();
            // Create a MediaMuxer.  We can't add the video track and start() the muxer here,
            // because our MediaFormat doesn't have the Magic Goodies.  These can only be
            // obtained from the encoder after it has started processing data.
            //
            // We're not actually interested in multiplexing audio.  We just want to convert
            // the raw H.264 elementary stream we get from MediaCodec into a .mp4 file.
            try
            {//we'll later pass this onto the audio extractor; right now audio is not re-encoded
                _muxer = new MediaMuxer(outputPath, Android.Media.MuxerOutputType.Mpeg4); 
            }
            catch (System.Exception e) { throw new System.Exception(e.Message); }
            _trackIndex = -1; // reset static track index
            MuxerStarted = false; //keep track of this so that the app doesn't crash from trying to start twice
        }

        private void PrepareDecoder(Android.Net.Uri inputUri)
        {
            _decoder = MediaCodec.CreateDecoderByType(MIME_TYPE); // create generic decoder from MIME
            _decoder.Configure(DecoderFormat.Item2, _outputSurface.Surface, null, MediaCodecConfigFlags.None); // configure our decoder with the extracted mediaformat

        }

        private string EncodeCameraToMp4(string inputPath, string outputPath, bool encodeAudio = true, Android.Net.Uri inputUri = null) {
            LatestInputVideoLength = MuxerEncoding.GetVideoLength(inputPath, inputUri);
            LatestAudioInputFormat = MuxerEncoding.GetAudioTrackFormat(inputPath, inputUri);
            EstimateTotalSize(LatestInputVideoLength, _bitRate);
            Android.Graphics.SurfaceTexture st = null;
            try
            {
                var decBuf = new BufferInfo();
                //prepareMediaPlayer(inputPath, inputUri); // @DEBUG I think this is underrunning on my S7, trying with MediaCodec buffer
                prepareEncoder(outputPath, inputUri);
                
                //DecoderFormat.Item1.ReadSampleData()
                prepareSurfaceTexture();

                PrepareDecoder(inputUri);
                //_mediaPlayer.Start(); // MP disabled due to lockups, trying mediacodec buffers
                //_mediaPlayer.SetAudioStreamType(Android.Media.Stream.VoiceCall);
                //_mediaPlayer.SetVolume(0, 0);
                _fC = 0; // reset the frame count
                _decoder.Start();
            }
            catch (System.Exception ex) {
                Log.Debug("VideoEncoder", ex.Message);
            }
            _dbf = ByteBuffer.Allocate(_videoBufferSize);
            _dbi = new BufferInfo();
            _dbi.Offset = 0;
            VideoEncodingInProgress = true;
            while (true)
               {

                    R(); // read from _decoder buffers, nested while loop

                    _fC++; // add to the frame count for tracking purposes
                    D(false); // after reading from decoder, drain to the encoder

                    /*
                     Disabled this to make it faster when not debugging
                     */
                    //if (_frameCount >= 30 && AppSettings.Logging.SendToConsole)  
                    //System.Console.WriteLine($"FileToMp4 while @ {_firstKnownBuffer} exited @ {st.Timestamp}  | encoded bits {_encodedBits} of estimated {_estimatedTotalSize}");
                    
                    // Acquire a new frame of input, and render it to the Surface.  If we had a
                    // GLSurfaceView we could switch EGL contexts and call drawImage() a second
                    // time to render it on screen.  The texture can be shared between contexts by
                    // passing the GLSurfaceView's EGLContext as eglCreateContext()'s share_context
                    // argument.
                    if (!_outputSurface.AwaitNewImage(false)) // @DEBUG no timeout
                    {
                        break;
                    }
                    _outputSurface.DrawImage(); // this might not be needed here @TODO

                    // Set the presentation time stamp from the SurfaceTexture's time stamp.  This
                    // will be used by MediaMuxer to set the PTS in the video.
                    _inputSurface.SetPresentationTime(_outputSurface.SurfaceTexture.Timestamp);

                //if (VERBOSE) Log.Debug("MediaLoop", "Set Time " + _outputSurface.SurfaceTexture.Timestamp);
                // Submit it to the encoder.  The eglSwapBuffers call will block if the input
                // is full, which would be bad if it stayed full until we dequeued an output
                // buffer (which we can't do, since we're stuck here).  So long as we fully drain
                // the encoder before supplying additional input, the system guarantees that we
                // can supply another frame without blocking.
                //if (VERBOSE) Log.Debug(TAG, "sending frame to encoder:");
                _inputSurface.SwapBuffers();
                
                if (_ebt >= _eTS) { break; }
            }
               D(true);
            VideoEncodingInProgress = false;
            if (AppSettings.Logging.SendToConsole)
                System.Console.WriteLine($"DrainEncoder started @ {_firstKnownBuffer} exited @ {st.Timestamp}  | encoded bits {_ebt} of estimated {_eTS}");
            try
            {
                releaseMediaPlayer();
                releaseEncoder();
                releaseSurfaceTexture();
            }catch { }
            _firstKnownBuffer = 0; //this stores the audio encoder offset long
            _eTS = 0;
            _fC = 0;
            _ebt = 0;
            _bfi = null;
            if (!AudioEncodingInProgress)
            {
                _muxer.Stop(); // if the audio encoding isn't still running then we'll stop everything and return
                _muxer.Release();
                _muxer = null;
                if (File.Exists(outputPath)) {
                    this.Progress.Invoke(new EncoderEventArgs(EncodedBits(_bfi.Size), _eTS, true, false, outputPath));
                    return outputPath; }
            }
            this.Progress.Invoke(new EncoderEventArgs(EncodedBits(_bfi.Size), _eTS, false, false, null));
            return null; //file isn't finished processing yet
        }

        private void R()
        {
            int i = _decoder.DequeueInputBuffer(_tus);
            Log.Info("ReadCodec", $" queued i");
            while (true)
            {
                if (i != -1)
                {
                    _dbf = _decoder.GetInputBuffer(i);
                    _dbi.Size = DecoderFormat.Item1.ReadSampleData(_dbf, 0);
                    if (AppSettings.Logging.SendToLogCat) { Log.Info("ReadCodec", $" read {_dbi.Size} to {_dbf.Position()} @ {i} index"); }
                }
                if (_dbi.Size <= 0) { break; }
                else {
                    var f = MF2MCB(DecoderFormat.Item1.SampleFlags);
                    _dbi.PresentationTimeUs = DecoderFormat.Item1.SampleTime;
                    _decoder.QueueInputBuffer(i, _dbi.Offset, _dbi.Size, _dbi.PresentationTimeUs, f);
                    if (AppSettings.Logging.SendToLogCat) { Log.Info("ReadCodec", $" read {_dbi.Size} to {_dbi.PresentationTimeUs} @ {i} index"); }
                    int s = _decoder.DequeueOutputBuffer(_dbi, _tus);
                    if (AppSettings.Logging.SendToLogCat) { Log.Info("ReadCodec", $" dequeue status = {s} for {_dbi} @ {_dbi.PresentationTimeUs} "); }
                    if (s == (int)MediaCodecInfoState.TryAgainLater) { break; }
                    else if (s == (int)MediaCodecInfoState.OutputBuffersChanged)
                    {
                        //_dbf = _decoder.GetOutputBuffer(s);
                    }
                    else if (s == (int)MediaCodecInfoState.OutputFormatChanged)
                    {   /**/      }
                    else if (s < 0)
                    {  /*???*/}
                    if (_dbf.HasRemaining)
                    {
                        _decoder.ReleaseOutputBuffer(s, _bfi.PresentationTimeUs * 1000);
                        if (AppSettings.Logging.SendToLogCat)
                        {
                            Log.Info("ReadCodec", $" release output buffer @ " + $"{_bfi.Size} to {_bfi.PresentationTimeUs * 1000} @ {s} status index");
                        }
                        DecoderFormat.Item1.Advance();
                        break;
                    }
                }
            }
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

            //if (VERBOSE) Log.Debug(TAG, "drainEncoder(" + endOfStream + ")"); @DEBUG, disabled to optimize performance
            if (es)
            {
                if (VERBOSE) Log.Debug(TAG, "sending EOS to encoder");
                _encoder.SignalEndOfInputStream();
                this.Progress.Invoke(new EncoderEventArgs(_ebt, _eTS, true));
            }
            ByteBuffer[] encoderOutputBuffers = _encoder.GetOutputBuffers();
            while (true)
            {
                int encoderStatus = _encoder.DequeueOutputBuffer(_bfi, _tus);
                if (encoderStatus == (int)MediaCodec.InfoTryAgainLater)
                {
                    // no output available yet
                    if (!es)
                    {
                        break;      // out of while
                    }
                    else
                    {
                        if (VERBOSE) Log.Debug(TAG, "no output available, spinning to await EOS");
                    }
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputBuffersChanged)
                {
                    // not expected for an encoder
                    encoderOutputBuffers = _encoder.GetOutputBuffers();
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputFormatChanged)
                {
                    // should happen before receiving buffers, and should only happen once
                    if (MuxerStarted)
                    {
                        throw new RuntimeException("format changed twice");
                    }
                    MediaFormat newFormat = _encoder.OutputFormat;
                    if (VERBOSE) Log.Debug(TAG, "encoder output format changed: " + newFormat);

                    _trackIndex = _muxer.AddTrack(newFormat);
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
                        if (VERBOSE) Log.Debug(TAG, "ignoring BUFFER_FLAG_CODEC_CONFIG");
                        _bfi.Size = 0;
                    }

                    if (_bfi.Size != 0)
                    {
                        if (!MuxerStarted)
                        {
                            throw new RuntimeException("muxer hasn't started");
                        }
                        // adjust the ByteBuffer values to match BufferInfo
                        ed.Position(_bfi.Offset);
                        ed.Limit(_bfi.Offset + _bfi.Size);
                        _bfi.PresentationTimeUs = CalculateTimeStamp(EncodedBits(_bfi.Size)); // the surface PT starts with a massive long so trying this instead of passing this to audio encoder
                        _muxer.WriteSampleData(_trackIndex, ed, _bfi);
                        //System.Console.WriteLine($"Drain {_bfi.Size} @ {_bfi.PresentationTimeUs}");
                        
                        if (_firstKnownBuffer == 0)
                        {
                            _firstKnownBuffer = _bfi.PresentationTimeUs;
                            //var cst = CorrectForStartTime(fbit, _firstKnownBuffer);
                            //Don't take these as arguments because it'll use up more memory; they should remain static while video encodes
                            if (InputUriToEncode != null) { this.StartAudioEncoder(_firstKnownBuffer, null, InputUriToEncode); }
                            else { this.StartAudioEncoder(_firstKnownBuffer, LatestInputPath, null); }
                            System.Console.WriteLine($"started draining @ {_bfi.PresentationTimeUs}");
                        } //we don't want to flood the system with EventArgs so only send once every 120 frames
                        if (_fC >= 120) { Notify(_ebt, _eTS); _fC = 0; }
                        /*
                     disabled when not debugging because this is locking up if the file is too big    @DEBUG
                     */
                        //if (VERBOSE) Log.Debug(TAG, "sent " + mBufferInfo.Size + " bytes to muxer"+ @" @ pt = " + mBufferInfo.PresentationTimeUs);
                    }

                    _encoder.ReleaseOutputBuffer(encoderStatus, false);

                    if ((_bfi.Flags & MediaCodec.BufferFlagEndOfStream) != 0)
                    {
                        if (!es) { Log.Warn(TAG, "reached end of stream unexpectedly"); }
                        else { if (VERBOSE) Log.Debug(TAG, "end of stream reached"); }
                        this.Progress.Invoke(new EncoderEventArgs(_ebt, _eTS, true, false));
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
			_mediaPlayer = new MediaPlayer ();
            if (inputPath == null && inputUri == null) { return; }
            if (!System.String.IsNullOrWhiteSpace(inputPath)) { _mediaPlayer.SetDataSource(inputPath); }
            else if (inputUri != null) { _mediaPlayer.SetDataSource(MainActivity.GetMainContext(), inputUri); }
            LatestInputPath = inputPath; //for tracking purposes but can probably be axed eventually @TODO 
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
			try {
                
				//_mediaPlayer.SetSurface(st);
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
	     * Releases encoder resources.
	     */
		private void releaseEncoder() {
			if (VERBOSE) Log.Debug(TAG, "releasing encoder objects");
			if (_encoder != null) {
				_encoder.Stop();
				_encoder.Release();
				_encoder = null;
			}
			if (_inputSurface != null) {
				_inputSurface.Release();
				_inputSurface = null;
			}
		}

        public void StartAudioEncoder(long calculatedOffset, string inputPath = null, Android.Net.Uri inputUri = null)
        {
            MuxerEncoding mxe = new MuxerEncoding();
            mxe.Progress += SettingsFrag.OnMuxerProgress;
            if (inputUri != null) { mxe.HybridMuxingTrimmer(0, LatestInputVideoLength, null, _muxer, LatestAudioTrackIndex, null, LatestOutputPath, calculatedOffset, inputUri); }
            else if (inputPath != null) { mxe.HybridMuxingTrimmer(0, LatestInputVideoLength, inputPath, _muxer, LatestAudioTrackIndex, null, LatestOutputPath, calculatedOffset); }
        }

        private static long _firstKnownBuffer;

        /// <summary>
        /// timeout microseconds
        /// </summary>
        static int _tus = 10000;

        /// <summary>
        /// invoke encoder eventargs for any operations
        /// listening, like the progress bar
        /// </summary>
        /// <param name="eb"></param>
        /// <param name="ets"></param>
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


        private MediaCodecBufferFlags MF2MCB(MediaExtractorSampleFlags mfg)
        {
            switch (mfg)
            {
                case MediaExtractorSampleFlags.None:
                    return MediaCodecBufferFlags.None;
                case MediaExtractorSampleFlags.Encrypted:
                    return MediaCodecBufferFlags.KeyFrame;
                case MediaExtractorSampleFlags.Sync:
                    return MediaCodecBufferFlags.SyncFrame;
                default:
                    throw new NotImplementedException("ConvertMediaExtractorSampleFlagsToMediaCodecBufferFlags");
            }
        }
    }
}