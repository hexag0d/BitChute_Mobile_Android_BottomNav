using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Nio;

namespace BitChute.VideoEncoding
{
    public class MuxerEncoding
    {
        public MuxerEncoding()
        {

        }

        public string Trim(int startMs, int endMs, string inputPath)
        {
            // Set up MediaExtractor to read from the source.
            MediaExtractor extractor = new MediaExtractor();
            extractor.SetDataSource(inputPath);
            int trackCount = extractor.TrackCount;
            // Set up MediaMuxer for the destination.
            MediaMuxer muxer;
            string outputPath = GetOutputPath(inputPath);
            muxer = new MediaMuxer(outputPath, MuxerOutputType.Mpeg4);
            // Set up the tracks and retrieve the max buffer size for selected
            // tracks.
            Dictionary<int, int> indexDict = new Dictionary<int, int>(trackCount);
            int bufferSize = -1;
            for (int i = 0; i < trackCount; i++)
            {
                MediaFormat format = extractor.GetTrackFormat(i);
                string mime = format.GetString(MediaFormat.KeyMime);
                bool selectCurrentTrack = false;
                if (mime.StartsWith("audio/"))
                {
                    selectCurrentTrack = true;
                }
                else if (mime.StartsWith("video/"))
                {
                    selectCurrentTrack = true;
                }
                if (selectCurrentTrack)
                {
                    extractor.SelectTrack(i);
                    int dstIndex = muxer.AddTrack(format);
                    indexDict.Add(i, dstIndex);
                    if (format.ContainsKey(MediaFormat.KeyMaxInputSize))
                    {
                        int newSize = format.GetInteger(MediaFormat.KeyMaxInputSize);
                        bufferSize = newSize > bufferSize ? newSize : bufferSize;
                    }
                }
            }
            if (bufferSize < 0)
            {
                bufferSize = 1337; //TODO: I don't know what to put here tbh, it will most likely be above 0 at this point anyways :)
            }
            // Set up the orientation and starting time for extractor.
            MediaMetadataRetriever retrieverSrc = new MediaMetadataRetriever();
            retrieverSrc.SetDataSource(inputPath);
            string degreesString = retrieverSrc.ExtractMetadata(MetadataKey.VideoRotation);
            if (degreesString != null)
            {
                int degrees = int.Parse(degreesString);
                if (degrees >= 0)
                {
                    muxer.SetOrientationHint(degrees);
                }
            }
            if (startMs > 0)
            {
                extractor.SeekTo(startMs * 1000, MediaExtractorSeekTo.ClosestSync);
            }
            // Copy the samples from MediaExtractor to MediaMuxer. We will loop
            // for copying each sample and stop when we get to the end of the source
            // file or exceed the end time of the trimming.
            int offset = 0;
            int trackIndex = -1;
            ByteBuffer dstBuf = ByteBuffer.Allocate(bufferSize);
            MediaCodec.BufferInfo bufferInfo = new MediaCodec.BufferInfo();
            try
            {
                muxer.Start();
                while (true)
                {
                    bufferInfo.Offset = offset;
                    bufferInfo.Size = extractor.ReadSampleData(dstBuf, offset);
                    if (bufferInfo.Size < 0)
                    {
                        bufferInfo.Size = 0;
                        break;
                    }
                    else
                    {
                        bufferInfo.PresentationTimeUs = extractor.SampleTime;
                        if (endMs > 0 && bufferInfo.PresentationTimeUs > (endMs * 1000))
                        {
                            Console.WriteLine("The current sample is over the trim end time.");
                            break;
                        }
                        else
                        {
                            bufferInfo.Flags = ConvertMediaExtractorSampleFlagsToMediaCodecBufferFlags(extractor.SampleFlags);
                            trackIndex = extractor.SampleTrackIndex;
                            muxer.WriteSampleData(indexDict[trackIndex], dstBuf, bufferInfo);
                            extractor.Advance();
                        }
                    }
                }
                muxer.Stop();

                //deleting the old file
                //JFile file = new JFile(srcPath);
                //file.Delete();
            }
            catch (Java.Lang.IllegalStateException e)
            {
                // Swallow the exception due to malformed source.
                Console.WriteLine("The source video file is malformed");
            }
            finally
            {
                muxer.Release();
            }
            return outputPath;
        }

        //Splits the string at the dot, separating the file name and the extension. then adding the "_trimmed" string between both
        private string GetOutputPath(string inputPath)
        {
            string[] parts = inputPath.Split('.');
            return $"{parts[0]}_trimmed{new System.Random().Next(0, 66666666)}.{parts[1]}";
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