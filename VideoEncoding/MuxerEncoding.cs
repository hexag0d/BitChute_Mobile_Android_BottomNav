using System;
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
using BitChute.Classes;
using Java.Nio;
using MediaCodecHelper;
//using static BitChute.ViewModels.VideoEncoderVM;
using static Android.Media.MediaCodec;

namespace BitChute.VideoEncoding
{
    public class MuxerEncoding
    {
        public delegate void MuxerEventDelegate(MuxerEventArgs _args);
        public event MuxerEventDelegate Progress;
        public MuxerEncoding()
        {

        }
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
            await Task.Run(() =>
            {
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
                MediaMetadataRetriever retrieverSrc = new MediaMetadataRetriever();
                retrieverSrc.SetDataSource(inputPath);
                string degreesString = retrieverSrc.ExtractMetadata(MetadataKey.VideoRotation);
                if (degreesString != null) // unused ATM but will be useful for stabilized videoview in streaming
                {
                    int degrees = int.Parse(degreesString);
                    if (degrees >= 0) {  /* muxer.SetOrientationHint(degrees); */  } //muxer won't accept this param once started
                }
                if (startMs > 0) { extractor.SeekTo(startMs * 1000, MediaExtractorSeekTo.ClosestSync); }
                int offset = 0;
                if (bufferInfo == null) { bufferInfo = new MediaCodec.BufferInfo(); }
                ByteBuffer dstBuf = ByteBuffer.Allocate(bufferSize);
                long us = endMs * 1000; //define end microseconds, using a tiny var name to reduce memory usage, since this will get used a bunch
                long uo = us + ptOffset; // add the presentationtime offset in microseconds; I'm not exactly sure why the video encoder starts with a high PT @TODO figure out why?
                int cf = 0;
                try
                {
                    FileToMp4.AudioEncodingInProgress = true; 
                    while (true) // @TODO I think we should trim all of these var names down as much as possible; would that reduce memory usage?
                    {            // this loop will run many times so we need to be conscious of memory and CPU
                        bufferInfo.Offset = offset;
                        bufferInfo.Size = extractor.ReadSampleData(dstBuf, offset);
                        if (bufferInfo.Size < 0) { bufferInfo.Size = 0; break; }
                        else
                        {
                            cf++;
                            bufferInfo.PresentationTimeUs = (extractor.SampleTime + ptOffset); // I had to add this offset to get the audio lined up with visuals
                                                                                               //anything in this loop using presentationtime needs the ptOffset (uo) for proper calculation
                            if (endMs > 0 && extractor.SampleTime >= us) { break; } //out of while
                            else
                            {
                                bufferInfo.Flags = ConvertMediaExtractorSampleFlagsToMediaCodecBufferFlags(extractor.SampleFlags);
                                if (trackIndexOverride == -1) { muxer.WriteSampleData(FileToMp4.LatestAudioTrackIndex, dstBuf, bufferInfo); }
                                else { muxer.WriteSampleData(trackIndexOverride, dstBuf, bufferInfo); }
                                if (cf >= 30) //only send the muxer eventargs once every 30 frames to reduce CPU load
                                {
                                    this.Progress.Invoke(new MuxerEventArgs(extractor.SampleTime, us)); 
                                    cf = 0;
                                }
                            }
                            extractor.Advance();
                        }
                    }
                }
                catch (Java.Lang.IllegalStateException e)
                {
                    this.Progress.Invoke(new MuxerEventArgs(extractor.SampleTime, us, null, true, true));
                    Console.WriteLine("The source video file is malformed");
                }
                catch (Java.Lang.Exception ex)
                {
                    this.Progress.Invoke(new MuxerEventArgs(extractor.SampleTime, us, null, true, true));
                    Console.WriteLine(ex.Message);
                }
                if (AppSettings.Logging.SendToConsole) System.Console.WriteLine($"DrainEncoder audio finished @ {bufferInfo.PresentationTimeUs}");
            });
            FileToMp4.AudioEncodingInProgress = false;
                try
                {
                    if (!FileToMp4.VideoEncodingInProgress)
                    {
                        muxer.Stop();
                        muxer.Release();
                        muxer = null;
                    }
                }
                catch (Java.Lang.Exception ex) { Log.Debug("MuxingEncoder", ex.Message); }
                if (outputPath != null)
                {
                    var success = System.IO.File.Exists(outputPath);
                    if (success)
                    {
                        this.Progress.Invoke(new MuxerEventArgs(endMs * 1000, endMs, outputPath, true));
                        return outputPath;
                    }
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