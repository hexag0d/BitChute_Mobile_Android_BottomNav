﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Nio;
using MediaCodecHelper;
//using static BitChute.ViewModels.VideoEncoderVM;
using static Android.Media.MediaCodec;

namespace BitChute.VideoEncoding
{
    public class MuxerEncoding
    {
        public MuxerEncoding() { }
        public static int GetVideoLength(string filepath)
        {
            MediaMetadataRetriever mmr = new MediaMetadataRetriever();
            mmr.SetDataSource(filepath);
            string durationStr = mmr.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDuration);
            int millSecond = Convert.ToInt32(durationStr);
            return millSecond;
        }

        public static MediaFormat GetAudioTrackFormat(string filepath)
        {
            MediaExtractor extractor = new MediaExtractor();
            extractor.SetDataSource(filepath);
            int trackCount = extractor.TrackCount;
            int bufferSize = -1;
            for (int i = 0; i < trackCount; i++)
            {
                MediaFormat format = extractor.GetTrackFormat(i);
                string mime = format.GetString(MediaFormat.KeyMime);
                bool selectCurrentTrack = false;
                if (mime.StartsWith("audio/")) { selectCurrentTrack = true; }
                else if (mime.StartsWith("video/")) { selectCurrentTrack = false; }
                if (selectCurrentTrack)
                {
                    extractor.SelectTrack(i);
                    if (format.ContainsKey(MediaFormat.KeyMaxInputSize))
                    {
                        int newSize = format.GetInteger(MediaFormat.KeyMaxInputSize);
                        bufferSize = newSize > bufferSize ? newSize : bufferSize;
                    }
                    return format;
                }
            }
            return null;
        }

        public async Task<string> HybridMuxingTrimmer(int startMs, int endMs, string inputPath, MediaMuxer muxer, int trackIndexOverride = -1, BufferInfo bufferInfo = null, string outputPath = null, long ptOffset = 0)
        {
            //OnAudioProcessingProgressChanged(0, endMs, "initializing audio processing");
            if (outputPath == null) { outputPath = FileToMp4.LatestOutputPath; }
            MediaExtractor extractor = new MediaExtractor();
            extractor.SetDataSource(inputPath);
            int trackCount = extractor.TrackCount;
            Dictionary<int, int> indexDict = new Dictionary<int, int>(trackCount);
            int bufferSize = -1;
            for (int i = 0; i < trackCount; i++)
            {
                MediaFormat format = extractor.GetTrackFormat(i);
                string mime = format.GetString(MediaFormat.KeyMime);
                bool selectCurrentTrack = false;
                if (mime.StartsWith("audio/")) { selectCurrentTrack = true; }
                else if (mime.StartsWith("video/")) { selectCurrentTrack = false; } /*rerouted to gl video encoder*/
                if (selectCurrentTrack)
                {
                    extractor.SelectTrack(i);
                    if (trackIndexOverride != -1) { indexDict.Add(i, i); }
                    if (format.ContainsKey(MediaFormat.KeyMaxInputSize))
                    {
                        int newSize = format.GetInteger(MediaFormat.KeyMaxInputSize);
                        bufferSize = newSize > bufferSize ? newSize : bufferSize;
                    }
                }
            }
            if (bufferSize < 0) { bufferSize = 1337; } //arbitrary value
            MediaMetadataRetriever retrieverSrc = new MediaMetadataRetriever();
            retrieverSrc.SetDataSource(inputPath);
            string degreesString = retrieverSrc.ExtractMetadata(MetadataKey.VideoRotation);
            if (degreesString != null)
            {
                int degrees = int.Parse(degreesString);
                if (degrees >= 0) {  /* muxer.SetOrientationHint(degrees); */  } //muxer won't accept this param once started
            }
            if (startMs > 0) { extractor.SeekTo(startMs * 1000, MediaExtractorSeekTo.ClosestSync); }
            int offset = 0;
            if (bufferInfo == null) { bufferInfo = new MediaCodec.BufferInfo(); }
            ByteBuffer dstBuf = ByteBuffer.Allocate(bufferSize);
            try
            {
                while (true)
                {
                    bufferInfo.Offset = offset;
                    bufferInfo.Size = extractor.ReadSampleData(dstBuf, offset);
                    if (bufferInfo.Size < 0) { bufferInfo.Size = 0; break; }
                    else
                    {
                        bufferInfo.PresentationTimeUs = (extractor.SampleTime + ptOffset);
                        if (false) { } //@DEBUG @TODO add back in the trimmer, right now I'm just trying to get the timestamps working
                        //if (endMs > 0 && bufferInfo.PresentationTimeUs > (endMs * 1000)) { Console.WriteLine("The current sample is over the trim end time."); break; }
                        else
                        {
                            bufferInfo.Flags = ConvertMediaExtractorSampleFlagsToMediaCodecBufferFlags(extractor.SampleFlags);
                            if (trackIndexOverride == -1) { muxer.WriteSampleData(FileToMp4.LatestAudioTrackIndex, dstBuf, bufferInfo); }
                            else { muxer.WriteSampleData(trackIndexOverride, dstBuf, bufferInfo); }
                        }
                        extractor.Advance();
                    }
                }
            }
            catch (Java.Lang.IllegalStateException e) { Console.WriteLine("The source video file is malformed"); }
            catch (Java.Lang.Exception ex) { Console.WriteLine(ex.Message); }
            try
            {
                muxer.Stop();
                muxer.Release();
                muxer = null;
            }
            catch (Java.Lang.Exception ex) { Log.Debug("MuxingEncoder", ex.Message); }
            if (outputPath != null)
            {
                var success = System.IO.File.Exists(outputPath);
                if (success) { return outputPath; }
            }
            return null; //nothing to look for
        }

        public static void OnAudioProcessingProgressChanged(long currentums, int totalMs, string text = null)
        {
            //OnVideoEncoderProgressChanged(((int)((decimal)(currentums / (totalMs * 1000))) * 100), text);
        }

        private string GetOutputPath(string inputPath)
        {
            string[] parts = inputPath.Split('.');
            return $"{parts[0]}_encoded{new System.Random().Next(0, 66666666)}.{parts[1]}";
        }

        private MediaCodecBufferFlags ConvertMediaExtractorSampleFlagsToMediaCodecBufferFlags(MediaExtractorSampleFlags mediaExtractorSampleFlag)
        {
            switch (mediaExtractorSampleFlag)
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