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

namespace BitChute.Classes
{
    class Encoding
    {
        /// <summary>
        /// -metadata[:metadata_specifier] key=value (output,per-metadata)
        /// Set a metadata key/value pair
        /// An optional metadata_specifier may be given to set metadata on streams, chapters or programs.See -map_metadata documentation for details.
        /// This option overrides metadata set with -map_metadata.It is also possible to delete metadata by using an empty value.
        ///For example, for setting the title in the output file:
        ///ffmpeg -i in.avi -metadata title = "my title" out.flv
        ///To set the language of the first audio stream:
        ///ffmpeg -i INPUT -metadata:s:a:0 language= eng OUTPUT
        /// </summary>
        /// 
        /*
         {
    "acodec": "aac",
    "audio_bit_rate": 96000,
    "audio_stream_index": 1,
    "audio_timebase": {
        "den": 44100,
        "num": 1
    },
    "channel_layout": 3,
    "channels": 2,
    "display_ratio": {
        "den": 9,
        "num": 16
    },
    "duration": 5441.875,
    "file_size": "247476181",
    "fps": {
        "den": 1,
        "num": 30
    },
    "has_audio": true,
    "has_single_image": false,
    "has_video": true,
    "height": 480,
    "id": "YVB03SS9BE",
    "interlaced_frame": false,
    "media_type": "video",
    "metadata": {
        "compatible_brands": "isomiso2avc1mp41",
        "encoder": "Lavf56.40.101",
        "handler_name": "SoundHandler",
        "language": "und",
        "major_brand": "isom",
        "minor_version": "512"
    },
    "path": "C:/Users/hegsagawd/Videos/ Demolitions.mp4",
    "pixel_format": 0,
    "pixel_ratio": {
        "den": 1281,
        "num": 1280
    },
    "sample_rate": 44100,
    "top_field_first": true,
    "type": "FFmpegReader",
    "vcodec": "h264",
    "video_bit_rate": 45475,
    "video_length": "163256",
    "video_stream_index": 0,
    "video_timebase": {
        "den": 15360,
        "num": 1
    },
    "width": 854
}
             */

        public class VideoConverter
        {
            Action<int, int> ProgressAction;
            Action<string> ProgressLog;

            public VideoConverter()
            {
                
            }

            public static async void StartVideoEncode()
            {
                VideoConverter vc = new VideoConverter();
                //vc.ConvertFileAsync( MainActivity.GetMainContext() , null, GetLogger(), null);
                MediaCodecTest(null);
            }

            public static void LogToConsole(string y)
            {
                
            }

            public static Action<string> GetLogger ()
            {
                Action<string> AddBook = LogToConsole; 
                AddBook += x => System.Console.WriteLine("EncoderStatus :{0}", x); 
                return LogToConsole;
            }

            public static void MediaCodecTest(MediaCodec mc)
            {
                VideoDownloader.GetExternalPermissions();
                var documentsPath = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
                var filePath = documentsPath + "5678.mp4";
                MediaMuxer muxer = new MediaMuxer((Android.OS.Environment.ExternalStorageDirectory.Path
                      + "/download/" + "_encoderTest" + new Random().Next(0, 666666) + ".mp4" ), Android.Media.MuxerOutputType.Mpeg4);

                Android.Media.MediaMetadataRetriever m = new Android.Media.MediaMetadataRetriever();
                m.SetDataSource(filePath);
                MediaExtractor me = new MediaExtractor();
                me.SetDataSourceAsync(filePath);
                //MediaFormat audioFormat = new MediaFormat();
                MediaFormat videoFormat = new MediaFormat();
                videoFormat = me.GetTrackFormat(1);
                //audioFormat = me.GetTrackFormat(1);
                //int audioTrackIndex = muxer.AddTrack(audioFormat);
                int videoTrackIndex = muxer.AddTrack(videoFormat);

                Java.Nio.ByteBuffer inputBuffer = Java.Nio.ByteBuffer.Allocate(45665);
                bool finished = false;
                BufferInfo bufferInfo = new BufferInfo();
                bufferInfo.S
                bufferInfo.Flags = MediaCodecBufferFlags.CodecConfig;

                
                
                muxer.Start();
                
                while (!finished)
                {
                    // getInputBuffer() will fill the inputBuffer with one frame of encoded
                    // sample from either MediaCodec or MediaExtractor, set isAudioSample to
                    // true when the sample is audio data, set up all the fields of bufferInfo,
                    // and return true if there are no more samples.

                    //finished = inputBuffer.GetInputBuffer(inputBuffer, isAudioSample, bufferInfo);
                    finished = inputBuffer.HasRemaining;
                    if (!finished)
                    {
                        //int currentTrackIndex = isAudioSample ? audioTrackIndex : videoTrackIndex;
                        var currentTrackIndex = 1;
                        muxer.WriteSampleData(currentTrackIndex, inputBuffer, bufferInfo);
                    }
                };
                muxer.Stop();
                muxer.Release();
            }

            /**
            * This method must be called from UI thread.
            ***/
            public async System.Threading.Tasks.Task<File> ConvertFileAsync(Context context,
                File inputFile,Action<string> logger = null, Action<int, int> onProgress = null)
            {
                VideoDownloader.GetExternalPermissions();
                var documentsPath = Android.OS.Environment.ExternalStorageDirectory.Path + "/download/";
                var filePath = documentsPath + "5678.mp4";
                inputFile = new File(filePath);
                File ouputFile = new File(Android.OS.Environment.ExternalStorageDirectory.Path 
                    + "/download/" + "_encoderTest" + new Random().Next(0, 666666666));

                List<string> cmd = new List<string>();
                cmd.Add("-y");
                cmd.Add("-i");
                cmd.Add(inputFile.CanonicalPath);

                Android.Media.MediaMetadataRetriever m = new Android.Media.MediaMetadataRetriever();
                m.SetDataSource(inputFile.CanonicalPath);

                string rotate = m.ExtractMetadata(Android.Media.MetadataKey.VideoRotation);

                int r = 0;

                if (!string.IsNullOrWhiteSpace(rotate))
                {
                    r = int.Parse(rotate);
                }

                cmd.Add("-b:v");
                cmd.Add("1M");

                cmd.Add("-b:a");
                cmd.Add("128k");

                switch (r)
                {
                    case 270:
                        cmd.Add("-vf scale=-1:480,transpose=cclock");
                        break;
                    case 180:
                        cmd.Add("-vf scale=-1:480,transpose=cclock,transpose=cclock");
                        break;
                    case 90:
                        cmd.Add("-vf scale=480:-1,transpose=clock");
                        break;
                    case 0:
                        cmd.Add("-vf scale=-1:480");
                        break;
                    default:

                        break;
                }

                cmd.Add("-f");
                cmd.Add("mpeg");

                cmd.Add(ouputFile.CanonicalPath);

                string cmdParams = string.Join(" ", cmd);

                int total = 0;
                int current = 0;
                

                //Xamarin.MP4Transcoder.Transcoder.For720pFormat.s
                //await Xamarin.MP4Transcoder.Transcoder.For720pFormat().ConvertAsync(inputFile, ouputFile, f => {
                //    onProgress?.Invoke((int)(f * (double)100), 100);

                //});
                return ouputFile;

                await FFMpeg.Xamarin.FFMpegLibrary.Run(
                    context,
                    cmdParams
                    , (s) => {
                        //logger?.Invoke(s);
                        ViewHelpers.Main.UpdateView(ViewHelpers.VideoEncoder.EncodingStatusTextView.Text, s);
                        int n = Extract(s, "Duration:", ",");
                        if (n != -1)
                        {
                            total = n;
                        }
                        n = Extract(s, "time=", " bitrate=");
                        if (n != -1)
                        {
                            current = n;
                            onProgress?.Invoke(current, total);
                        }
                    });
                FileBrowser.SaveFileToStorage(ouputFile);

                return ouputFile;
            }

            int Extract(String text, String start, String end)
            {
                int i = text.IndexOf(start);
                if (i != -1)
                {
                    text = text.Substring(i + start.Length);
                    i = text.IndexOf(end);
                    if (i != -1)
                    {
                        text = text.Substring(0, i);
                        return parseTime(text);
                    }
                }
                return -1;
            }

            public static int parseTime(String time)
            {
                time = time.Trim();
                String[] tokens = time.Split(':');
                int hours = int.Parse(tokens[0]);
                int minutes = int.Parse(tokens[1]);
                float seconds = float.Parse(tokens[2]);
                int s = (int)seconds * 100;
                return hours * 360000 + minutes * 60100 + s;
            }
        }
    }
}