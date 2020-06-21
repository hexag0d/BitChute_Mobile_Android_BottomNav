/**
   Copyright (c) 2014 Rory Hool
   
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at
   
       http://www.apache.org/licenses/LICENSE-2.0
   
   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFMpeg;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BitChute.Classes;
using Java.IO;
using System.Data;
using Android.Media;
using static Android.Media.MediaCodec;

public class VideoResampler
{

    private static int TIMEOUT_USEC = 10000;

    public static int WIDTH_QCIF = 176;
    public static int HEIGHT_QCIF = 144;
    public static int BITRATE_QCIF = 1000000;

    public static int WIDTH_QVGA = 320;
    public static int HEIGHT_QVGA = 240;
    public static int BITRATE_QVGA = 2000000;

    public static int WIDTH_720P = 1280;
    public static int HEIGHT_720P = 720;
    public static int BITRATE_720P = 6000000;

    private static String TAG = "VideoResampler";
   private static bool WORK_AROUND_BUGS = false; // avoid fatal codec bugs
   private static bool VERBOSE = true; // lots of logging

   // parameters for the encoder
   public static int FPS_30 = 30; // 30fps
    public static int FPS_15 = 15; // 15fps
    public static int IFRAME_INTERVAL_10 = 10; // 10 seconds between I-frames

    // size of a frame, in pixels
    private int mWidth = WIDTH_720P;
    private int mHeight = HEIGHT_720P;

    // bit rate, in bits per second
    private int mBitRate = BITRATE_720P;

    private int mFrameRate = FPS_30;

    private int mIFrameInterval = IFRAME_INTERVAL_10;

    // private Uri mInputUri;
    private Uri mOutputUri;

    Surface mInputSurface;// = CreatePersistentInputSurface();

    Surface mOutputSurface;

    MediaCodec mEncoder = null;

    MediaMuxer mMuxer = null;
    int mTrackIndex = -1;
    bool mMuxerStarted = false;

    // MediaExtractor mExtractor = null;

    // MediaFormat mExtractFormat = null;

    // int mExtractIndex = 0;

    List<SamplerClip> mClips = new List<SamplerClip>();

    // int mStartTime = -1;

    // int mEndTime = -1;

    long mLastSampleTime = 0;

    long mEncoderPresentationTimeUs = 0;

    public VideoResampler()
    {

    }

    /*
     * public void setInput( Uri intputUri ) { mInputUri = intputUri; }
     */

    public void addSamplerClip(SamplerClip clip)
    {
        mClips.Add(clip);
    }

    public void setOutput(Uri outputUri)
    {
        mOutputUri = outputUri;
    }

    public void setOutputResolution(int width, int height)
    {
        if ((width % 16) != 0 || (height % 16) != 0)
        {
            System.Console.WriteLine(TAG, "WARNING: width or height not multiple of 16");
        }
        mWidth = width;
        mHeight = height;
    }

    public void setOutputBitRate(int bitRate)
    {
        mBitRate = bitRate;
    }

    public void setOutputFrameRate(int frameRate)
    {
        mFrameRate = frameRate;
    }

    public void setOutputIFrameInterval(int IFrameInterval)
    {
        mIFrameInterval = IFrameInterval;
    }

    /*
     * public void setStartTime( int startTime ) { mStartTime = startTime; }
     * 
     * public void setEndTime( int endTime ) { mEndTime = endTime; }
     */
    public void Start()
    {
        VideoEditWrapper wrapper = new VideoEditWrapper();
    //Thread th = new Thread(wrapper, "codec test");
    //th.start();
    //  th.join();
    //  if (wrapper.mThrowable != null ) {
    //     throw wrapper.mThrowable;
    //  }
   }

   /**
    * Wraps resampleVideo, running it in a new thread. Required because of the way 
    * SurfaceTexture.OnFrameAvailableListener works when the current thread has a Looper configured.
    */
   private class VideoEditWrapper : Java.Lang.Runnable
    {
//      private Throwable mThrowable;

//@Override
//      public void run()
//{
//    try
//    {
//        resampleVideo();
//    }
//    catch (Throwable th)
//    {
//        mThrowable = th;
//    }
//}
   }

    private void SetupEncoder()
    {

        MediaFormat outputFormat = MediaFormat.CreateVideoFormat(MediaFormat.MimetypeVideoAvc, mWidth, mHeight);

        outputFormat.SetInteger(MediaFormat.KeyColorFormat, (int)Android.Media.MediaCodecCapabilities.Formatsurface);
        outputFormat.SetInteger(MediaFormat.KeyBitRate, mBitRate);

        outputFormat.SetInteger(MediaFormat.KeyFrameRate, mFrameRate);
        outputFormat.SetInteger(MediaFormat.KeyIFrameInterval, mIFrameInterval);

        mEncoder = MediaCodec.CreateEncoderByType(MediaFormat.MimetypeVideoAvc);
        mEncoder.Configure(outputFormat, null, null, Android.Media.MediaCodecConfigFlags.Encode);


        mInputSurface = mEncoder.CreateInputSurface();

        //mInputSurface.makeCurrent(); @removed
        mEncoder.Start();
    }

    private void SetupMuxer()
    {

        try
        {
            mMuxer = new MediaMuxer(mOutputUri.ToString(), MuxerOutputType.Mpeg4);
        }
        catch (IOException ioe)
        {
            //throw new RuntimeException("MediaMuxer creation failed", ioe);
        }
    }

    private void resampleVideo()
    {

        SetupEncoder();
        SetupMuxer();

        foreach (SamplerClip clip in mClips)
        {
            feedClipToEncoder(clip);
        }

        mEncoder.SignalEndOfInputStream();

        ReleaseOutputResources();
    }

    private void feedClipToEncoder(SamplerClip clip)
    {

        mLastSampleTime = 0;

        MediaCodec decoder = null;

        MediaExtractor extractor = SetupExtractorForClip(clip);

        if (extractor == null)
        {
            return;
        }

        int trackIndex = GetVideoTrackIndex(extractor);
        extractor.SelectTrack(trackIndex);

        MediaFormat clipFormat = extractor.GetTrackFormat(trackIndex);

        if (clip.getStartTime() != -1)
        {
            extractor.SeekTo(clip.getStartTime() * 1000, MediaExtractor.SeekToPreviousSync);
            clip.setStartTime(extractor.SampleTime / 1000);
        }

        try
        {
            decoder = MediaCodec.CreateDecoderByType(MediaFormat.KeyMime);
            mOutputSurface = decoder.Out

            decoder.Configure(clipFormat, mOutputSurface.GetSurface(), null, 0);
            decoder.Start();

            resampleVideo(extractor, decoder, clip);

        }
        finally
        {

            if (mOutputSurface != null)
            {
                mOutputSurface.Release();
            }
            if (decoder != null)
            {
                decoder.Stop();
                decoder.Release();
            }

            if (extractor != null)
            {
                extractor.Release();
                extractor = null;
            }
        }
    }

private MediaExtractor SetupExtractorForClip(SamplerClip clip)
{
    MediaExtractor extractor = new MediaExtractor();
    try
    {
        extractor.SetDataSource(clip.getUri().Path);
    }
    catch (IOException e)
    {
            System.Console.WriteLine(e);
        return null;
    }

    return extractor;
}

private int GetVideoTrackIndex(MediaExtractor extractor)
{

    for (int trackIndex = 0; trackIndex < extractor.TrackCount; trackIndex++)
    {
        MediaFormat format = extractor.GetTrackFormat(trackIndex);

        String mime = format.GetString(MediaFormat.KeyMime);
        if (mime != null)
        {
            if (mime == "video/avc")
            {
                return trackIndex;
            }
        }
    }

    return -1;
}

private void ReleaseOutputResources()
{

    if (mInputSurface != null)
    {
        mInputSurface.Release();
    }

    if (mEncoder != null)
    {
        mEncoder.Stop();
        mEncoder.Release();
    }

    if (mMuxer != null)
    {
        mMuxer.Stop();
        mMuxer.Release();
        mMuxer = null;
    }
}

    private void resampleVideo(MediaExtractor extractor, MediaCodec decoder, SamplerClip clip)
    {
        Java.Nio.ByteBuffer decoderInputBuffer0 = decoder.GetInputBuffer(0);
        Java.Nio.ByteBuffer decoderInputBuffer1 = decoder.GetInputBuffer(1);
        var decoderInputBuffers = new List<Java.Nio.ByteBuffer>();
        decoderInputBuffers.Add(decoderInputBuffer0);
        decoderInputBuffers.Add(decoderInputBuffer1);
        var encoderOutputBuffers = new List<Java.Nio.ByteBuffer>();
        Java.Nio.ByteBuffer encoderOutputBuffer0 = mEncoder.GetOutputBuffer(0);
        Java.Nio.ByteBuffer encoderOutputBuffer1 = mEncoder.GetOutputBuffer(1);
        encoderOutputBuffers.Add(encoderOutputBuffer0);
        encoderOutputBuffers.Add(encoderOutputBuffer1);
        MediaCodec.BufferInfo info = new MediaCodec.BufferInfo();
        int inputChunk = 0;
        int outputCount = 0;

        long endTime = clip.getEndTime();

        if (endTime == -1)
        {
            endTime = clip.getVideoDuration();
        }

        bool outputDoneNextTimeWeCheck = false;

        bool outputDone = false;
        bool inputDone = false;
        bool decoderDone = false;

        while (!outputDone)
        {
            if (VERBOSE)
                System.Console.WriteLine(TAG, "edit loop");
            // Feed more data to the decoder.
            if (!inputDone)
            {
                int inputBufIndex = decoder.DequeueInputBuffer(TIMEOUT_USEC);
                if (inputBufIndex >= 0)
                {
                    if (extractor.SampleTime / 1000 >= endTime)
                    {
                        // End of stream -- send empty frame with EOS flag set.
                        decoder.QueueInputBuffer(inputBufIndex, 0, 0, 0L, MediaCodecBufferFlags.EndOfStream);
                        inputDone = true;
                        if (VERBOSE)
                            System.Console.WriteLine(TAG, "sent input EOS (with zero-length frame)");
                    }
                    else
                    {
                        // Copy a chunk of input to the decoder. The first chunk should have
                        // the BUFFER_FLAG_CODEC_CONFIG flag set.
                        Java.Nio.ByteBuffer inputBuf = decoderInputBuffers[inputBufIndex];
                        inputBuffer.Clear();

                        int sampleSize = extractor.ReadSampleData(inputBuf, 0);
                        if (sampleSize < 0)
                        {
                            System.Console.WriteLine(TAG, "InputBuffer BUFFER_FLAG_END_OF_STREAM");
                            decoder.QueueInputBuffer(inputBufIndex, 0, 0, 0, MediaCodecBufferFlags.EndOfStream);
                        }
                        else
                        {
                            System.Console.WriteLine(TAG, "InputBuffer ADVANCING");
                            decoder.QueueInputBuffer(inputBufIndex, 0, sampleSize, extractor.SampleTime, 0);
                            extractor.Advance();
                        }

                        inputChunk++;
                    }
                }
                else
                {
                    if (VERBOSE)
                        System.Console.WriteLine(TAG, "input buffer not available");
                }
            }

            // Assume output is available. Loop until both assumptions are false.
            bool decoderOutputAvailable = !decoderDone;
            bool encoderOutputAvailable = true;
            while (decoderOutputAvailable || encoderOutputAvailable)
            {
                // Start by draining any pending output from the encoder. It's important to
                // do this before we try to stuff any more data in.
                int encoderStatus = mEncoder.DequeueOutputBuffer(info, TIMEOUT_USEC);
                if (encoderStatus == (int)MediaCodecInfoState.TryAgainLater)
                {
                    // no output available yet
                    if (VERBOSE)
                        System.Console.WriteLine(TAG, "no output from encoder available");
                    encoderOutputAvailable = false;
                }
                else if (encoderStatus == (int)MediaCodecInfoState.OutputBuffersChanged)
                {
                    encoderOutputBuffers = mEncoder.GetOutputBuffer(1);
                    if (VERBOSE)
                        System.Console.WriteLine(TAG, "encoder output buffers changed");
                }
                else if (encoderStatus == (int)MediaCodecInfoState.OutputFormatChanged)
                {

                    MediaFormat newFormat = mEncoder.OutputFormat;

                    mTrackIndex = mMuxer.AddTrack(newFormat);
                    mMuxer.Start();
                    mMuxerStarted = true;
                    if (VERBOSE)
                        System.Console.WriteLine(TAG, "encoder output format changed: " + newFormat);
                }
                else if (encoderStatus < 0)
                {
                    // fail( "unexpected result from encoder.DequeueOutputBuffer: " + encoderStatus );
                }
                else
                { // encoderStatus >= 0
                    Java.Nio.ByteBuffer encodedData = encoderOutputBuffers[encoderStatus];
                    if (encodedData == null)
                    {
                        // fail( "encoderOutputBuffer " + encoderStatus + " was null" );
                    }
                    // Write the data to the output "file".
                    if (info.Size != 0)
                    {
                        encodedData.Position(info.Offset);
                        encodedData.Limit(info.Offset + info.Size);
                        outputCount++;

                        mMuxer.WriteSampleData(mTrackIndex, encodedData, info);

                        if (VERBOSE)
                            System.Console.WriteLine(TAG, "encoder output " + info.Size + " bytes");
                    }
                    outputDone = (info.Flags & MediaCodecBufferFlags.EndOfStream) != 0;

                    mEncoder.ReleaseOutputBuffer(encoderStatus, false);
                }

                if (outputDoneNextTimeWeCheck)
                {
                    outputDone = true;
                }

                if (encoderStatus != (int)MediaCodecInfoState.TryAgainLater)
                {
                    // Continue attempts to drain output.
                    continue;
                }
                // Encoder is drained, check to see if we've got a new frame of output from
                // the decoder. (The output is going to a Surface, rather than a ByteBuffer,
                // but we still get information through BufferInfo.)
                if (!decoderDone)
                {
                    int decoderStatus = decoder.DequeueOutputBuffer(info, TIMEOUT_USEC);
                    if (decoderStatus == (int)MediaCodecInfoState.TryAgainLater)
                    {
                        // no output available yet
                        if (VERBOSE)
                            System.Console.WriteLine(TAG, "no output from decoder available");
                        decoderOutputAvailable = false;
                    }
                    else if (decoderStatus == (int)MediaCodecInfoState.OutputBuffersChanged)
                    {
                        // decoderOutputBuffers = decoder.getOutputBuffers();
                        if (VERBOSE)
                            System.Console.WriteLine(TAG, "decoder output buffers changed (we don't care)");
                    }
                    else if (decoderStatus == (int)MediaCodecInfoState.OutputFormatChanged)
                    {
                        // expected before first buffer of data
                        MediaFormat newFormat = decoder.OutputFormat;
                        if (VERBOSE)
                            System.Console.WriteLine(TAG, "decoder output format changed: " + newFormat);
                    }
                    else if (decoderStatus < 0)
                    {
                        // fail( "unexpected result from decoder.DequeueOutputBuffer: " + decoderStatus );
                    }
                    else
                    { // decoderStatus >= 0
                        if (VERBOSE)
                            System.Console.WriteLine(TAG, "surface decoder given buffer " + decoderStatus + " (size=" + info.size + ")");
                        // The ByteBuffers are null references, but we still get a nonzero
                        // size for the decoded data.
                        bool doRender = (info.Size != 0);
                        // As soon as we call releaseOutputBuffer, the buffer will be forwarded
                        // to SurfaceTexture to convert to a texture. The API doesn't
                        // guarantee that the texture will be available before the call
                        // returns, so we need to wait for the onFrameAvailable callback to
                        // fire. If we don't wait, we risk rendering from the previous frame.
                        decoder.ReleaseOutputBuffer(decoderStatus, doRender);
                        if (doRender)
                        {
                            // This waits for the image and renders it after it arrives.
                            if (VERBOSE)
                                System.Console.WriteLine(TAG, "awaiting frame");

                            mOutputSurface.AwaitNewImage();
                            mOutputSurface.DrawImage();
                            // Send it to the encoder.

                            long nSecs = info.PresentationTimeUs * 1000;

                            if (clip.getStartTime() != -1)
                            {
                                nSecs = (info.PresentationTimeUs - (clip.getStartTime() * 1000)) * 1000;
                            }

                            System.Console.WriteLine("this", "Setting presentation time " + nSecs / (1000 * 1000));
                            nSecs = Math.Max(0, nSecs);

                            mEncoderPresentationTimeUs += (nSecs - mLastSampleTime);

                            mLastSampleTime = nSecs;

                            mInputSurface.SetPresentationTime(mEncoderPresentationTimeUs);
                            if (VERBOSE)
                                System.Console.WriteLine(TAG, "swapBuffers");
                            mInputSurface.SwapBuffers();
                        }
                        if ((info.Flags & MediaCodecBufferFlags.EndOfStream) != 0)
                        {
                            // mEncoder.signalEndOfInputStream();
                            outputDoneNextTimeWeCheck = true;
                        }
                    }
                }
            }
        }
        if (inputChunk != outputCount)
        {
            // throw new RuntimeException( "frame lost: " + inputChunk + " in, " + outputCount + " out" );
        }
    }

    public class SamplerClip
    {
        Android.Net.Uri mUri;

        long mStartTime = -1;
        long mEndTime = -1;

        int mVideoDuration;

        public SamplerClip(Android.Net.Uri uri)
        {
            mUri = uri;

            mVideoDuration = MediaHelper.GetDuration(uri);
        }

        public void setStartTime(long startTime)
        {
            mStartTime = startTime;
        }

        public void setEndTime(int endTime)
        {
            mEndTime = endTime;
        }

        public Android.Net.Uri getUri()
        {
            return mUri;
        }

        public long getStartTime()
        {
            return mStartTime;
        }

        public long getEndTime()
        {
            return mEndTime;
        }

        public int getVideoDuration()
        {
            return mVideoDuration;
        }
    }
}