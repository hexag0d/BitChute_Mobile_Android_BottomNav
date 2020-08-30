using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BitChute.VideoEncoding
{
    public class MuxerEventArgs : EventArgs
    {
        private string _data;
        private bool _finished;
        private long _time;
        private long _length;
        public MuxerEventArgs(long time, long length, string data = null, bool finished = false)
        {
            _data = data;
            _finished = finished;
            _time = time;
            _length = length;
        } 
        
        public string Data { get { return _data; } }
        public bool Finished { get { return _finished; } }
        public long Time { get { return _time; } }
        public long Length { get { return _length; } }
    }

    public class EncoderEventArgs : EventArgs
    {
        private long _encodedData;
        private long _totalData;
        private bool _finished;
        public EncoderEventArgs(long encoded, long total, bool finished = false)
        {
            if (finished) { _finished = true; }
            _encodedData = encoded;
            _totalData = total;
        }

        public long EncodedData { get { return _encodedData; } }
        public long TotalData { get { return _totalData; } }
        public bool Finished { get { return _finished; } }
    }
}