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
using static Android.Media.MediaCodec;

namespace MediaCodecHelper
{

    public class EncoderToMpegTest
    {

        private Context _context;
        private string _workingDirectory;
        bool VERBOSE = true;
        public EncoderToMpegTest(Context context)
        {
            _context = context;
            _workingDirectory = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }

        private const string TAG = "EncoderToMpegTest";
        // lots of logging

        // parameters for the encoder
        const string MIME_TYPE = "video/avc";    // H.264 Advanced Video Coding
        const int FRAME_RATE = 24;               // 30fps
        const int IFRAME_INTERVAL = 1;           // 5 seconds between I-frames
        const long DURATION_SEC = 10;             // 8 seconds of video

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
        
        private InputSurface _inputSurface;
        private OutputSurface _outputSurface;

        // allocate one of these up front so we don't need to do it every time
        private MediaCodec.BufferInfo mBufferInfo;


        private void MediaCodecMuxerEncode(MediaMuxer mMuxer, int trackIndex, string filepath, MediaCodec inputCodec, MediaCodec outputCodec, 
            MediaFormat inputFormat, MediaFormat outputFormat, BufferInfo inputBufferInfo)
        {
            // arbitrary but popular values
            int encWidth = 640;
            int encHeight = 480;
            int encBitRate = 6000000;      // Mbps
            MediaCodec mc = null;
            try
            {
                mc = prepareEncoder(encWidth, encHeight, encBitRate);
                _inputSurface.MakeCurrent();
                var mp = PrepareMediaPlayer(filepath);
                prepareSurfaceTexture(mp);
                int frameCount = 0;
                var st = _outputSurface.SurfaceTexture;
                _outputSurface.ChangeFragmentShader(FRAGMENT_SHADER1);
                var isCompleted = false;
                mp.Completion += (object sender, System.EventArgs e) =>
                {
                    isCompleted = true;
                };
                mp.Start();
                while (!isCompleted)
                {
                    drainEncoder(false, mMuxer, trackIndex, mBufferInfo, mc);
                    frameCount++;
                    _outputSurface.AwaitNewImage();
                    _outputSurface.DrawImage();
                    _inputSurface.SetPresentationTime(st.Timestamp);
                    _inputSurface.SwapBuffers();
                }
                drainEncoder(true, mMuxer, trackIndex, mBufferInfo, mc);
            }
            finally
            {
                releaseEncoder(mc);
                releaseSurfaceTexture();
            }
        }

        /**
	     * Configures SurfaceTexture for camera preview.  Initializes mStManager, and sets the
	     * associated SurfaceTexture as the Camera's "preview texture".
	     * <p>
	     * Configure the EGL surface that will be used for output before calling here.
	     */
        private void prepareSurfaceTexture(MediaPlayer mp)
        {
            _outputSurface = new OutputSurface();
            mp.SetSurface(_outputSurface.Surface);
        }

        public static MediaPlayer PrepareMediaPlayer(string filePath)
        {
            var _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(filePath);
            _mediaPlayer.Prepare();
            return _mediaPlayer;
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
        private MediaCodec prepareEncoder(int width, int height, int bitRate)
        {
            mBufferInfo = new MediaCodec.BufferInfo();
            MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, width, height);
            format.SetInteger(MediaFormat.KeyColorFormat, (int)MediaCodecCapabilities.Formatsurface);
            format.SetInteger(MediaFormat.KeyBitRate, bitRate);
            format.SetInteger(MediaFormat.KeyFrameRate, FRAME_RATE);
            format.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
            var mEncoder = MediaCodec.CreateEncoderByType(MIME_TYPE);
            mEncoder.Configure(format, null, null, MediaCodec.ConfigureFlagEncode);
            _inputSurface = new InputSurface(mEncoder.CreateInputSurface());
            mEncoder.Start();
            string outputPath = System.IO.Path.Combine(_workingDirectory, "test." + width + "x" + height + ".mp4");
            return mEncoder;
        }

        /**
	     * Releases encoder resources.
	     */
        private void releaseEncoder(MediaCodec mEncoder)
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
        private void drainEncoder(bool endOfStream, MediaMuxer mMuxer, int trackIndex, BufferInfo inputBuffer, MediaCodec mEncoder)
        {
            int TIMEOUT_USEC = 10000;
            if (endOfStream)
            {
                mEncoder.SignalEndOfInputStream();
            }

            ByteBuffer[] encoderOutputBuffers = mEncoder.GetOutputBuffers();
            while (true)
            {
                int encoderStatus = mEncoder.DequeueOutputBuffer(mBufferInfo, TIMEOUT_USEC);
                if (encoderStatus == (int)MediaCodec.InfoTryAgainLater)
                {
                    if (!endOfStream)
                    {
                        break;      // out of while
                    }
                    else
                    {
                    }
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputBuffersChanged)
                {
                    encoderOutputBuffers = mEncoder.GetOutputBuffers();
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputFormatChanged)
                {
                    MediaFormat newFormat = mEncoder.OutputFormat;
                    
                }
                else if (encoderStatus < 0)  { }
                else
                {
                    ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
                    if (encodedData == null)
                    {
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
                        // adjust the ByteBuffer values to match BufferInfo (not needed?)
                        encodedData.Position(mBufferInfo.Offset);
                        encodedData.Limit(mBufferInfo.Offset + mBufferInfo.Size);
                        mMuxer.WriteSampleData(trackIndex, encodedData, mBufferInfo);
                    }

                    mEncoder.ReleaseOutputBuffer(encoderStatus, false);

                    if ((mBufferInfo.Flags & MediaCodec.BufferFlagEndOfStream) != 0)
                    {
                        if (!endOfStream)
                        {
                        }
                        else
                        {
                        }
                        break;      // out of while
                    }
                }
            }
        }
    }
}