
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
using System.Linq;
using static Android.Media.MediaCodec;

namespace MediaCodecHelper
{

    public class FileToMp4
    {

        private Context _context;
        private string _workingDirectory;
        bool VERBOSE = true;
        private int _width;
        private int _height;
        private int _fps;
        private int _secondPerIFrame;
        private int _bitRate;

        public FileToMp4(Context context, int fps, int secondPerIFrame, System.Drawing.Size? outputSize, int bitRate = 6000000)
        {
            _context = context;

            if (outputSize.HasValue)
            {
                _width = outputSize.Value.Width;
                _height = outputSize.Value.Height;
            }

            _secondPerIFrame = secondPerIFrame;
            _fps = fps;
            _bitRate = bitRate;
            _workingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
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

        //const string FRAGMENT_SHADER2 =
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
        private static MediaCodec _audioEncoder;
        private static MediaCodec _audioDecoder;
        private InputSurface _inputSurface;
        private MediaMuxer mMuxer;
        public static MediaMuxer StaticMuxer;
        private int mTrackIndex;
        private static int mAudioTrackIndex;
        private bool mMuxerStarted;
        public static bool mAudioMuxerStarted;
        public static string FileToReEncodePath;

        // camera state
        private MediaPlayer _mediaPlayer;
        private OutputSurface _outputSurface;
        private string _outputPath = "";
        private static MediaFormat _newAudioFormat;
        private MediaExtractor _audioExtractor;
        private string _videoSource = "";

        // allocate one of these up front so we don't need to do it every time
        private MediaCodec.BufferInfo mBufferInfo;
        private static MediaCodec.BufferInfo _audioBufferInfo;

        public void Start()
        {
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.Main, Android.Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.Main, new string[] { Android.Manifest.Permission.WriteExternalStorage }, 0);
            }

            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.Main, Android.Manifest.Permission.ReadExternalStorage) != (int)Android.Content.PM.Permission.Granted)
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.Main, new string[] { Android.Manifest.Permission.ReadExternalStorage }, 0);
            }
            EncodeCameraToMp4();
        }

        public static MediaMuxer GetCurrentMuxer()
        {
            return StaticMuxer;
        }
        

        // For audio: http://stackoverflow.com/questions/22673011/how-to-extract-pcm-samples-from-mediacodec-decoders-output

        private async void EncodeCameraToMp4()
        {

            // arbitrary but popular values

            try
            {
                prepareMediaPlayer();
                prepareEncoder();
                _inputSurface.MakeCurrent();
                prepareSurfaceTexture();

                _mediaPlayer.Start();

                var st = _outputSurface.SurfaceTexture;
                int frameCount = 0;

                bool isCompleted = false;
                _outputSurface.ChangeFragmentShader(FRAGMENT_SHADER1);
                _mediaPlayer.Completion += (object sender, System.EventArgs e) =>
                {
                    isCompleted = true;
                };
                while (!isCompleted)
                {
                    // Feed any pending encoder output into the muxer.

                    drainEncoder(false);

                    frameCount++;

                    // Acquire a new frame of input, and render it to the Surface.  If we had a
                    // GLSurfaceView we could switch EGL contexts and call drawImage() a second
                    // time to render it on screen.  The texture can be shared between contexts by
                    // passing the GLSurfaceView's EGLContext as eglCreateContext()'s share_context
                    // argument.
                    if (!_outputSurface.AwaitNewImage())
                    {
                        break;
                    }
                    _outputSurface.DrawImage();

                    // Set the presentation time stamp from the SurfaceTexture's time stamp.  This
                    // will be used by MediaMuxer to set the PTS in the video.

                    _inputSurface.SetPresentationTime(st.Timestamp);

                    // Submit it to the encoder.  The eglSwapBuffers call will block if the input
                    // is full, which would be bad if it stayed full until we dequeued an output
                    // buffer (which we can't do, since we're stuck here).  So long as we fully drain
                    // the encoder before supplying additional input, the system guarantees that we
                    // can supply another frame without blocking.
                    if (VERBOSE) Log.Debug(TAG, "sending frame to encoder");
                    _inputSurface.SwapBuffers();
                }
                isCompleted = false;
                
                BufferInfo info = new BufferInfo();
                ByteBuffer buff = null;
                var index = 0;
                await PrepareAudioEncoder();
                _audioEncoder.Start();
                buff = _audioEncoder.GetInputBuffer(index);
                info.Size = buff.Capacity();
                info.Offset = buff.ArrayOffset();
                mMuxer.WriteSampleData(mAudioTrackIndex, buff, info);
                index++;
                while (buff.HasRemaining)
                {
                    buff = _audioEncoder.GetInputBuffer(index);
                    info.Offset = buff.ArrayOffset();
                    mMuxer.WriteSampleData(mAudioTrackIndex, buff, info);
                    index++;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            finally
            {
                //// release everything we grabbed
                //releaseMediaPlayer();
                //releaseEncoder();
                //releaseSurfaceTexture();
            }
            var fileExists = File.Exists(_outputPath);
        }

        private void prepareMediaPlayer()
        {

            _mediaPlayer = new MediaPlayer();
            _videoSource = Path.Combine(_workingDirectory, "car_audio_sample.mp4");
            FileToReEncodePath = _videoSource;
            _mediaPlayer.SetDataSource(Path.Combine(_workingDirectory, "car_audio_sample.mp4"));
            _mediaPlayer.Prepare();
            if (_width == 0 || _height == 0)
            {
                _width = _mediaPlayer.VideoWidth;
                _height = _mediaPlayer.VideoHeight;
            }
        }

        /**
	     * Stops camera preview, and releases the camera to the system.
	     */
        private void releaseMediaPlayer()
        {
            if (VERBOSE) Log.Debug(TAG, "releasing camera");
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Release();
                _mediaPlayer = null;
            }
        }

        private MediaCodec PrepareMediaExtractor(string path)
        {
            _audioExtractor = new MediaExtractor();
            _audioExtractor.SetDataSource(path);
            var codecs = BitChute.VideoEncoding.Sound.Extraction.GetAudioCodecByFilePath(path).Result.First();
            _audioDecoder = codecs.Value;
            return codecs.Value;
        }

        /**
	     * Configures SurfaceTexture for camera preview.  Initializes mStManager, and sets the
	     * associated SurfaceTexture as the Camera's "preview texture".
	     * <p>
	     * Configure the EGL surface that will be used for output before calling here.
	     */
        private void prepareSurfaceTexture()
        {
            _outputSurface = new OutputSurface();
            try
            {
                _mediaPlayer.SetSurface(_outputSurface.Surface);
            }
            catch (System.Exception e)
            {
                throw new System.Exception("setPreviewTexture failed:" + e.Message);
            }
        }

        /**
	     * Releases the SurfaceTexture.
	     */
        private void releaseSurfaceTexture()
        {
            if (_outputSurface != null)
            {
                _outputSurface.Release();
                _outputSurface = null;
            }
        }

        /**
	     * Configures encoder and muxer state, and prepares the input Surface.  Initializes
	     * mEncoder, mMuxer, mInputSurface, mBufferInfo, mTrackIndex, and mMuxerStarted.
	     */
        private void prepareEncoder()
        {
            mBufferInfo = new MediaCodec.BufferInfo();

            MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, _width, _height);

            // Set some properties.  Failing to specify some of these can cause the MediaCodec
            // configure() call to throw an unhelpful exception.
            format.SetInteger(MediaFormat.KeyColorFormat, (int)MediaCodecCapabilities.Formatsurface);
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

            // Output filename.  Ideally this would use Context.getFilesDir() rather than a
            // hard-coded output directory.
            _outputPath = (Android.OS.Environment.ExternalStorageDirectory.Path
                      + "/download/" + "_encoderTest" + new System.Random().Next(0, 666666) + ".mp4");


            var _fileExists = File.Exists(_outputPath);
            Log.Info(TAG, "Output file is " + _outputPath);

            // Create a MediaMuxer.  We can't add the video track and start() the muxer here,
            // because our MediaFormat doesn't have the Magic Goodies.  These can only be
            // obtained from the encoder after it has started processing data.
            //
            // We're not actually interested in multiplexing audio.  We just want to convert
            // the raw H.264 elementary stream we get from MediaCodec into a .mp4 file.
            try
            {
                mMuxer = new MediaMuxer(_outputPath, MediaMuxer.OutputFormat.MuxerOutputMpeg4);
                StaticMuxer = mMuxer;
            }
            catch (System.Exception e)
            {
                throw new System.Exception(e.Message);
            }

            mTrackIndex = -1;
            mMuxerStarted = false;
        }

        /**
 * Configures encoder and muxer state, and prepares the input Surface.  Initializes
 * mEncoder, mMuxer, mInputSurface, mBufferInfo, mTrackIndex, and mMuxerStarted.
 */
        public static async System.Threading.Tasks.Task<MediaCodec> PrepareAudioEncoder(int bitrate = 96000)
        {
            _audioBufferInfo = new MediaCodec.BufferInfo();
            await System.Threading.Tasks.Task.Run(() =>
            {
                _audioEncoder = MediaCodec.CreateEncoderByType(MediaFormat.MimetypeAudioAac);
                _audioEncoder.Configure(_newAudioFormat, null, null, MediaCodecConfigFlags.Encode);
            });
            return _audioEncoder;
        }


        /**
	     * Releases encoder resources.
	     */
        private void releaseEncoder()
        {
            if (VERBOSE) Log.Debug(TAG, "releasing encoder objects");
            if (mEncoder != null)
            {
                mEncoder.Stop();
                mEncoder.Release();
                mEncoder = null;
            }
            if (_inputSurface != null)
            {
                _inputSurface.Release();
                _inputSurface = null;
            }
            //if (mMuxer != null)
            //{
            //    mMuxer.Stop();
            //    mMuxer.Release();
            //    mMuxer = null;
            //}
        }

        public static void ReleaseMuxer()
        {
            if (StaticMuxer != null)
            {
                StaticMuxer.Stop();
                StaticMuxer.Release();
                StaticMuxer = null;
            }
        }

        static int TIMEOUT_USEC = 100000;

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
        private void drainEncoder(bool endOfStream)
        {
            if (VERBOSE) Log.Debug(TAG, "drainEncoder(" + endOfStream + ")");

            if (endOfStream)
            {
                if (VERBOSE) Log.Debug(TAG, "sending EOS to encoder");
                mEncoder.SignalEndOfInputStream();
            }

            ByteBuffer[] encoderOutputBuffers = mEncoder.GetOutputBuffers();
            while (true)
            {
                int encoderStatus = mEncoder.DequeueOutputBuffer(mBufferInfo, TIMEOUT_USEC);
                if (encoderStatus == (int)MediaCodec.InfoTryAgainLater)
                {
                    // no output available yet
                    if (!endOfStream)
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
                    encoderOutputBuffers = mEncoder.GetOutputBuffers();
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputFormatChanged)
                {
                    // should happen before receiving buffers, and should only happen once
                    if (mMuxerStarted)
                    {
                        throw new RuntimeException("format changed twice");
                    }
                    MediaFormat newFormat = mEncoder.OutputFormat;
                    
                    Log.Debug(TAG, "encoder output format changed: " + newFormat);
                    
                    mTrackIndex = mMuxer.AddTrack(newFormat);
                    _newAudioFormat = MediaFormat.CreateAudioFormat(MediaFormat.MimetypeAudioAac, 48000, 1);
                    _newAudioFormat.SetInteger(MediaFormat.KeyBitRate, 96000);
                    //_newAudioFormat.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
                    mAudioTrackIndex = mMuxer.AddTrack(_newAudioFormat);
                    mMuxer.Start();
                    mMuxerStarted = true;
                }
                else if (encoderStatus < 0)
                {
                    Log.Warn(TAG, "unexpected result from encoder.dequeueOutputBuffer: " +
                        encoderStatus);
                    // let's ignore it
                }
                else
                {
                    ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
                    if (encodedData == null)
                    {
                        throw new RuntimeException("encoderOutputBuffer " + encoderStatus +
                            " was null");
                    }

                    if ((mBufferInfo.Flags & MediaCodec.BufferFlagCodecConfig) != 0)
                    {
                        // The codec config data was pulled out and fed to the muxer when we got
                        // the INFO_OUTPUT_FORMAT_CHANGED status.  Ignore it.
                        if (VERBOSE) Log.Debug(TAG, "ignoring BUFFER_FLAG_CODEC_CONFIG");
                        mBufferInfo.Size = 0;
                    }

                    if (mBufferInfo.Size != 0)
                    {
                        if (!mMuxerStarted)
                        {
                            throw new RuntimeException("muxer hasn't started");
                        }

                        // adjust the ByteBuffer values to match BufferInfo (not needed?)
                        encodedData.Position(mBufferInfo.Offset);
                        encodedData.Limit(mBufferInfo.Offset + mBufferInfo.Size);

                        mMuxer.WriteSampleData(mTrackIndex, encodedData, mBufferInfo);
                        if (VERBOSE) Log.Debug(TAG, "sent " + mBufferInfo.Size + " bytes to muxer");
                    }

                    mEncoder.ReleaseOutputBuffer(encoderStatus, false);

                    if ((mBufferInfo.Flags & MediaCodec.BufferFlagEndOfStream) != 0)
                    {
                        if (!endOfStream)
                        {
                            Log.Warn(TAG, "reached end of stream unexpectedly");
                        }
                        else
                        {
                            if (VERBOSE) Log.Debug(TAG, "end of stream reached");
                        }
                        break;      // out of while
                    }
                }
            }
        }

        public static bool AudioEncodingInProgress = false;

        public static async System.Threading.Tasks.Task<bool> DrainAudioEncoder(bool endOfStream, ByteBuffer byteBuffer, MediaCodec.BufferInfo buffInfo)
        {
            if (endOfStream)
            {
                _audioEncoder.SignalEndOfInputStream();
                ReleaseMuxer();
                return false;
            }


                if (byteBuffer == null && !AudioEncodingInProgress)
                {
                    return false;
                }

                AudioEncodingInProgress = true;

                if ((buffInfo.Flags & MediaCodec.BufferFlagCodecConfig) != 0)
                {
                    buffInfo.Size = 0;
                    return false;
                }

                if (buffInfo.Size != 0)
                {
                    if (!mAudioMuxerStarted)
                    {
                        throw new RuntimeException("muxer hasn't started");
                    }

                    StaticMuxer.WriteSampleData(mAudioTrackIndex, byteBuffer, buffInfo);
                    return true;
                }

                return false;
                // mEncoder.ReleaseOutputBuffer(encoderStatus, false);

            return false;
        }
    }
}


///**
//* Extracts all pending data from the encoder and forwards it to the muxer.
//* <p>
//* If endOfStream is not set, this returns when there is no more data to drain.  If it
//* is set, we send EOS to the encoder, and then iterate until we see EOS on the output.
//* Calling this with endOfStream set should be done once, right before stopping the muxer.
//* <p>
//* We're just using the muxer to get a .mp4 file (instead of a raw H.264 stream).  We're
//* not recording audio.
//*/
//private void DrainAudioEncoder(bool endOfStream)
//{

//    if (VERBOSE) Log.Debug(TAG, "drainEncoder(" + endOfStream + ")");

//    if (endOfStream)
//    {
//        if (VERBOSE) Log.Debug(TAG, "sending EOS to encoder");
//        mAudioEncoder.SignalEndOfInputStream();
//    }

//    ByteBuffer[] encoderOutputBuffers = mAudioEncoder.GetOutputBuffers();
//    while (true)
//    {
//        int encoderStatus = mAudioEncoder.DequeueOutputBuffer(mBufferInfo, TIMEOUT_USEC);
//        if (encoderStatus == (int)MediaCodec.InfoTryAgainLater)
//        {
//            // no output available yet
//            if (!endOfStream)
//            {
//                break;      // out of while
//            }
//            else
//            {
//                if (VERBOSE) Log.Debug(TAG, "no output available, spinning to await EOS");
//            }
//        }
//        else if (encoderStatus == (int)MediaCodec.InfoOutputBuffersChanged)
//        {
//            encoderOutputBuffers = mAudioEncoder.GetOutputBuffers();
//        }
//        else if (encoderStatus == (int)MediaCodec.InfoOutputFormatChanged)
//        {
//            if (mMuxerStarted)
//            {

//            }

//            mMuxer.Start();
//            mMuxerStarted = true;
//        }
//        else if (encoderStatus < 0)
//        {
//            Log.Warn(TAG, "unexpected result from encoder.dequeueOutputBuffer: " +
//                encoderStatus);
//            // let's ignore it
//        }
//        else
//        {
//            ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
//            if (encodedData == null)
//            {
//                throw new RuntimeException("encoderOutputBuffer " + encoderStatus +
//                    " was null");
//            }

//            if ((mBufferInfo.Flags & MediaCodec.BufferFlagCodecConfig) != 0)
//            {
//                // The codec config data was pulled out and fed to the muxer when we got
//                // the INFO_OUTPUT_FORMAT_CHANGED status.  Ignore it.
//                if (VERBOSE) Log.Debug(TAG, "ignoring BUFFER_FLAG_CODEC_CONFIG");
//                mBufferInfo.Size = 0;
//            }

//            if (mBufferInfo.Size != 0)
//            {
//                if (!mMuxerStarted)
//                {
//                    throw new RuntimeException("muxer hasn't started");
//                }

//                // adjust the ByteBuffer values to match BufferInfo (not needed?)
//                encodedData.Position(mBufferInfo.Offset);
//                encodedData.Limit(mBufferInfo.Offset + mBufferInfo.Size);

//                mMuxer.WriteSampleData(mAudioTrackIndex, encodedData, mAudioBufferInfo);
//                //if (VERBOSE) Log.Debug(TAG, "sent " + mAudioBufferInfo.Size + " bytes to muxer");
//            }

//            mEncoder.ReleaseOutputBuffer(encoderStatus, false);

//            if ((mBufferInfo.Flags & MediaCodec.BufferFlagEndOfStream) != 0)
//            {
//                if (!endOfStream)
//                {
//                    Log.Warn(TAG, "reached end of stream unexpectedly");
//                }
//                else
//                {
//                    if (VERBOSE) Log.Debug(TAG, "end of stream reached");
//                }
//                break;      // out of while
//            }
//        }
//    }
//}