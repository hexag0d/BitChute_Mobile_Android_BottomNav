﻿
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
using BitChute.Classes;

namespace MediaCodecHelper
{
    public class FileToMp4
    {
        private Context _context;
        private string _workingDirectory;
        bool LOGGING = true;
        private int _fps;
        private int _secondPerIFrame;
        private int _bitRate;

        public FileToMp4(Context context, int fps, int secondPerIFrame, System.Drawing.Size? outputSize, int bitRate = 6000000)
        {
            _context = context;
            
            _secondPerIFrame = secondPerIFrame;
            _fps = fps;
            _bitRate = bitRate;
            _workingDirectory = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
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
        
        private MediaCodec mEncoder;
        private InputSurface _inputSurface;
        private MediaMuxer mMuxer;
        private int mTrackIndex;
        private bool mMuxerStarted;
        
        private MediaPlayer _mediaPlayer;
        private OutputSurface _outputSurface;
      
        private MediaCodec.BufferInfo mBufferInfo;
        public static string LatestFileOutputPath = "";

        public async void Start()
        {
            await FileBrowser.GetExternalPermissions();
            var path = Path.Combine(_workingDirectory, "car_audio_sample.mp4");
            EncodeCameraToMp4(path);
        }

        // For audio: http://stackoverflow.com/questions/22673011/how-to-extract-pcm-samples-from-mediacodec-decoders-output

        private void EncodeCameraToMp4(string filepath)
        {
            var audioFormat = MuxerEncoding.GetAudioTrackFormat(filepath);
            try
            {
                prepareMediaPlayer(filepath);
                PrepareEncoder();
                _inputSurface.MakeCurrent();
                prepareSurfaceTexture();
                _mediaPlayer.Start();
                var st = _outputSurface.SurfaceTexture;
                int frameCount = 0;
                bool isCompleted = false;
                _mediaPlayer.Completion += (object sender, System.EventArgs e)=>{isCompleted=true;};
                while (!isCompleted)
                {
                    drainEncoder(false, filepath, audioFormat);
                    frameCount++;
                    if (!_outputSurface.AwaitNewImage()) { break; }
                    _outputSurface.DrawImage();
                    _inputSurface.SetPresentationTime(st.Timestamp);
                    _inputSurface.SwapBuffers();
                }
                drainEncoder(true, filepath, audioFormat);
            }
            catch (Exception ex){ System.Console.WriteLine(ex); }
            finally { releaseMediaPlayer(); releaseEncoder();  releaseSurfaceTexture(); }
            MuxerEncoding mxe = new MuxerEncoding();
            mxe.HybridMuxingTrimmer(0, MuxerEncoding.GetVideoLength(filepath), filepath, mMuxer, 1);
            var success = File.Exists(LatestFileOutputPath);
        }

        private void prepareMediaPlayer(string filepath)
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.SetDataSource(Path.Combine(_workingDirectory, "car_audio_sample.mp4"));
            _mediaPlayer.SetVolume(0, 0);
            _mediaPlayer.SetAudioStreamType(Android.Media.Stream.VoiceCall);
            _mediaPlayer.Prepare();
        }

        /**
	     * Stops camera preview, and releases the camera to the system.
	     */
        private void releaseMediaPlayer()
        {
            if (LOGGING) Log.Debug(TAG, "releasing camera");
            if (_mediaPlayer != null)
            {
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
        private void prepareSurfaceTexture()
        {
            _outputSurface = new OutputSurface();
            var st = _outputSurface.Surface;
            try { _mediaPlayer.SetSurface(st); }
            catch (System.Exception e) { throw new System.Exception("setPreviewTexture failed:" + e.Message); }
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
        private void PrepareEncoder(int bitRate = 500000, int frameRate = 30, int width = 854, int height = 480)
        {
            mBufferInfo = new MediaCodec.BufferInfo();
            MediaFormat format = MediaFormat.CreateVideoFormat(MIME_TYPE, width, height);
            format.SetInteger(MediaFormat.KeyColorFormat, (int)MediaCodecCapabilities.Formatsurface);
            format.SetInteger(MediaFormat.KeyBitRate, _bitRate);
            format.SetInteger(MediaFormat.KeyFrameRate, FRAME_RATE);
            format.SetInteger(MediaFormat.KeyIFrameInterval, IFRAME_INTERVAL);
            if (LOGGING) Log.Debug(TAG, "format: " + format);
            mEncoder = MediaCodec.CreateEncoderByType(MIME_TYPE);
            mEncoder.Configure(format, null, null, MediaCodec.ConfigureFlagEncode);
            _inputSurface = new InputSurface(mEncoder.CreateInputSurface());
            mEncoder.Start();
            string outputPath = (Android.OS.Environment.ExternalStorageDirectory.Path
                      + "/download/" + "_encoderTest" + new System.Random().Next(0, 66666666) + ".mp4");
            LatestFileOutputPath = outputPath;
            try{ mMuxer = new MediaMuxer(outputPath, MuxerOutputType.Mpeg4); }
            catch (System.Exception e) { throw new System.Exception(e.Message);  }
            mTrackIndex = -1;
            mMuxerStarted = false;
        }

        /**
	     * Releases encoder resources.
	     */
        private void releaseEncoder()
        {
            if (LOGGING) Log.Debug(TAG, "releasing encoder objects");
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
        private void drainEncoder(bool endOfStream, string filepath, MediaFormat audioFormat)
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
                    if (!endOfStream) { break; }
                    else { }
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputBuffersChanged)
                {
                    encoderOutputBuffers = mEncoder.GetOutputBuffers();
                }
                else if (encoderStatus == (int)MediaCodec.InfoOutputFormatChanged)
                {
                    if (mMuxerStarted) { throw new RuntimeException("format changed twice");  }
                    MediaFormat newFormat = mEncoder.OutputFormat;
                    mTrackIndex = mMuxer.AddTrack(newFormat);
                    var mAudioTrackIndex = mMuxer.AddTrack(audioFormat);
                    mMuxer.Start();
                    mMuxerStarted = true;
                }
                else if (encoderStatus < 0) {  }
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
                        if (LOGGING) Log.Debug(TAG, "ignoring BUFFER_FLAG_CODEC_CONFIG");
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
                        if (LOGGING) Log.Debug(TAG, "sent " + mBufferInfo.Size + " bytes to muxer");
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
                            if (LOGGING) Log.Debug(TAG, "end of stream reached");
                        }
                        break;      // out of while
                    }
                }
            }
        }
    }
}