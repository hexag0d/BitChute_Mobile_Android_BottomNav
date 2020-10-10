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
using BitChute;
using Java.Nio;
using MediaCodecHelper;
//using static BitChute.ViewModels.VideoEncoderVM;
using static Android.Media.MediaCodec;

namespace BitChute.VideoEncoding
{
    public class MuxerEncoding : Service
    {
        public delegate void MuxerEventDelegate(MuxerEventArgs _args);
        public event MuxerEventDelegate Progress;
        public MuxerEncoding()
        {

        }

        public static int GetVideoLength(string filepath = null, Android.Net.Uri inputUri = null)
        {
            MediaMetadataRetriever mmr = new MediaMetadataRetriever();
            if (inputUri != null) { mmr.SetDataSource(Android.App.Application.Context, inputUri); }
            else if (filepath != null) { mmr.SetDataSource(filepath); }
            string durationStr = mmr.ExtractMetadata(MediaMetadataRetriever.MetadataKeyDuration);
            int millSecond = Convert.ToInt32(durationStr);
            return millSecond;
        }

        public static MediaFormat GetAudioTrackFormat(string filepath, Android.Net.Uri inputUri = null)
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

        /// <summary>
        /// if both inputPath string and inputUri are not null, this
        /// method will use the Uri.  Else, set one or the other
        /// 
        /// They cannot both be null
        /// </summary>
        /// <param name="startMs">the start ms for trimming</param>
        /// <param name="endMs">the final ms for trimming</param>
        /// <param name="inputPath">optional input path string</param>
        /// <param name="muxer">the muxer to use for writing bytes</param>
        /// <param name="trackIndexOverride">the track index for muxer to write to</param>
        /// <param name="bufferInfo">an input bufferinfo to get properties from</param>
        /// <param name="outputPath">the output path for method to check after finished encoding</param>
        /// <param name="ptOffset">the presentation time offset for audio, used in syncing audio and video</param>
        /// <param name="inputUri">optional inputUri to read from</param>
        /// <returns></returns>
        public async Task<string> HybridMuxingTrimmer(int startMs, int endMs, string inputPath, MediaMuxer muxer, int trackIndexOverride = -1, BufferInfo bufferInfo = null, string outputPath = null, long ptOffset = 0, Android.Net.Uri inputUri = null)
        {
            var tio = trackIndexOverride; // trying to reduce resource usage by shortening these var names when they're used in long running loops
            await Task.Run(() =>
            {
                if (outputPath == null) { outputPath = FileToMp4.LatestOutputPath; }
                MediaExtractor ext = new MediaExtractor();
                if (inputUri != null) { ext.SetDataSource(Android.App.Application.Context, inputUri, null); }
                else { ext.SetDataSource(inputPath); }
                int trackCount = ext.TrackCount;
                Dictionary<int, int> indexDict = new Dictionary<int, int>(trackCount);
                int bufferSize = -1;
                for (int i = 0; i < trackCount; i++)
                {
                    MediaFormat format = ext.GetTrackFormat(i);
                    string mime = format.GetString(MediaFormat.KeyMime);
                    bool selectCurrentTrack = false;
                    if (mime.StartsWith("audio/")) { selectCurrentTrack = true; }
                    else if (mime.StartsWith("video/")) { selectCurrentTrack = false; } /*rerouted to gl video encoder*/
                    if (selectCurrentTrack)
                    {
                        ext.SelectTrack(i);
                        if (tio != -1) { indexDict.Add(i, i); }
                        if (format.ContainsKey(MediaFormat.KeyMaxInputSize))
                        {
                            int newSize = format.GetInteger(MediaFormat.KeyMaxInputSize);
                            bufferSize = newSize > bufferSize ? newSize : bufferSize;
                        }
                    }
                }
                MediaMetadataRetriever retrieverSrc = new MediaMetadataRetriever();
                if (!System.String.IsNullOrWhiteSpace(inputPath)) { retrieverSrc.SetDataSource(inputPath); }
                else { retrieverSrc.SetDataSource(Android.App.Application.Context, inputUri); }
                string degreesString = retrieverSrc.ExtractMetadata(MetadataKey.VideoRotation);
                if (degreesString != null) // unused ATM but will be useful for stabilized videoview in streaming
                {
                    int degrees = int.Parse(degreesString);
                    if (degrees >= 0) {  /* muxer.SetOrientationHint(degrees); */  } //muxer won't accept this param once started
                }
                if (startMs > 0) { ext.SeekTo(startMs * 1000, MediaExtractorSeekTo.ClosestSync); }
                int offset = 0;
                if (bufferInfo == null) { bufferInfo = new MediaCodec.BufferInfo(); }
                ByteBuffer dstBuf = ByteBuffer.Allocate(bufferSize);
                long us = endMs * 1000; 
                long uo = us + ptOffset;
                int cf = 0;
                try
                {
                    FileToMp4.AudioEncodingInProgress = true; 
                    while (true) 
                    {            
                        bufferInfo.Offset = offset;
                        bufferInfo.Size = ext.ReadSampleData(dstBuf, offset);
                        if (bufferInfo.Size < 0) { bufferInfo.Size = 0; break; }
                        else
                        {
                            cf++;
                            bufferInfo.PresentationTimeUs = ext.SampleTime + ptOffset; // I had to add this offset to get the audio lined up with visuals
                                                                                               //anything in this loop using presentationtime needs the ptOffset (uo) for proper calculation
                            if (endMs > 0 && ext.SampleTime >= us) { break; } //out of while
                            else
                            {
                                bufferInfo.Flags = MFlags2MCodecBuff(ext.SampleFlags);
                                if (tio == -1) { muxer.WriteSampleData(FileToMp4.LatestAudioTrackIndex, dstBuf, bufferInfo); }
                                else { muxer.WriteSampleData(tio, dstBuf, bufferInfo); }
                                if (cf >= 120) //only send the muxer eventargs once every 120 frames to reduce CPU load
                                {
                                    Notify(ext.SampleTime, us); 
                                    cf = 0;
                                }
                            }
                            ext.Advance();
                        }
                    }
                }
                catch (Java.Lang.IllegalStateException e)
                {
                    this.Progress.Invoke(new MuxerEventArgs(ext.SampleTime, us, null, true, true));
                    Console.WriteLine("The source video file is malformed");
                }
                catch (Java.Lang.Exception ex)
                {
                    this.Progress.Invoke(new MuxerEventArgs(ext.SampleTime, us, null, true, true));
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

        private async void Notify(long st, long us)
        {
            await Task.Factory.StartNew(() => { this.Progress.Invoke(new MuxerEventArgs(st, us)); });
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

        private MediaCodecBufferFlags MFlags2MCodecBuff(MediaExtractorSampleFlags mfg)
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

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}