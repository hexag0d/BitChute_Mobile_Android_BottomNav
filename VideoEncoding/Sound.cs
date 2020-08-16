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
using Java.Nio;

namespace BitChute.VideoEncoding
{
    public class Sound
    {
        public class Extraction : MediaCodec.Callback
        {
            static MediaExtractor me;
            static MediaCodec leftChannelCodec;
            static MediaCodec rightChannelCodec;

            public static void SetByteBufferOutputStream()
            {
            }

            public Extraction()
            {
                
            }

            public void Convert(string inputPath, string outputPath)
            {
                me = new MediaExtractor();
                var fileBeforConversion = inputPath;
                var fileAfterConversion = outputPath;
                me.SetDataSource(inputPath);
                var mediaCodecs = GetAudioCodecByFilePath(inputPath);
                try  {  leftChannelCodec = mediaCodecs.Result[0]; }
                catch  { }
                leftChannelCodec?.SetCallback(this);
            }

            public static async Task<ByteBuffer> GetAudioByteBuffer(MediaCodec source)
            {
                ByteBuffer bb = null;
                await Task.Run(() => bb = source.GetInputBuffer(source.DequeueInputBuffer(-1)));
                return bb;
            }

            public static void SetDecoderInputBufferAvail(MediaCodec codec)
            {
            }

            public override void OnError(MediaCodec codec, MediaCodec.CodecException e)
            {
            }

            public static bool ExtractorInitialized;
            public static string StaticPath;

            public static async Task<bool> InitializeExtractor(string inputPath)
            { 
                if (inputPath == null) { inputPath = MediaCodecHelper.FileToMp4.FileToReEncodePath; }
                me = new MediaExtractor();
                await me.SetDataSourceAsync(inputPath);
                me.SelectTrack(GetAudioCodecByFilePath(inputPath).Result.First().Key);
                return true;
            }

            public static bool AudioMuxInProgress;

            public override void OnInputBufferAvailable(MediaCodec codec, int index)
            {
                MatchCodec = codec;
                Console.WriteLine("InputBufferAvailable");
                var decoderInputBuffer = codec.GetInputBuffer(index);
                var size = me.ReadSampleData(decoderInputBuffer, 0);
                if (size < 0)
                {
                    codec.QueueInputBuffer(
                            index,
                            0,
                            0,
                            0,
                            MediaCodec.BufferFlagEndOfStream);

                    Log.Debug("AudioExtract", "audio extractor: EOS");
                }
                else
                {
                    codec.QueueInputBuffer(
                            index,
                            0,
                            size,
                            me.SampleTime,
                            0);
                    me.Advance();
                }
            }

            public static MediaCodec MatchCodec;
            
            public override void OnOutputBufferAvailable(MediaCodec codec, int index, MediaCodec.BufferInfo info)
            {
                
                var buffer = codec.GetOutputBuffer(index);
                //var b = buffer.Limit(c);
                //var a = buffer.Position();
                buffer.Position(info.Offset);
                buffer.Limit(info.Offset + info.Size);
                //buffer.Get(a);
                //buffer.Position(c);
                MediaCodecHelper.FileToMp4.DrainAudioEncoder(false, buffer, info);
                codec.ReleaseOutputBuffer(index, false);
            }

            public override void OnOutputFormatChanged(MediaCodec codec, MediaFormat format)
            {
            }

            public static async Task<Dictionary<int, MediaCodec>> GetAudioCodecByFilePath(string filepath)
            {
                Dictionary<int, MediaCodec> codecs = new Dictionary<int, MediaCodec>();
                MediaExtractor extractor = new MediaExtractor();
                try { extractor.SetDataSource(filepath); }
                catch (Exception ex) { Console.WriteLine(ex.Message); return null; }
                await Task.Run(() =>
                {
                    for (int i = 0; i < extractor.TrackCount; i++)
                    {
                        MediaFormat format = extractor.GetTrackFormat(i);
                        string mime = format.GetString(MediaFormat.KeyMime);
                        if (mime.StartsWith(@"audio/"))
                        {
                            MediaCodec decoder = null;
                            extractor.SelectTrack(i);
                            decoder = MediaCodec.CreateDecoderByType(mime);
                            decoder.Configure(format, null, null, 0);
                            if (decoder != null)
                            {
                                codecs.Add(i, decoder);
                            }
                        }
                    }
                });
                return codecs;
            }
        }
    }
}