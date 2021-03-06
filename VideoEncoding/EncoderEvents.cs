﻿using System;

namespace BitChute.VideoEncoding
{
    public class MuxerEventArgs : EventArgs
    {
        private string _filepath;
        private bool _finished;
        private long _time;
        private long _length;
        private bool _error;
        public MuxerEventArgs(long time, long length, string filepath = null, bool finished = false, bool error = false)
        {
            _filepath = filepath;
            _finished = finished;
            _time = time;
            _length = length;
            _error = error;
        }

        public string FilePath {get{if(!_error){return _filepath;}else{return "an error occured processing audio";}}}
        public bool Finished { get { return _finished; } }
        public long Time { get { return _time; } }
        public long Length { get { return _length; } }
        public bool Error { get { return _error; } }
    }

    public class EncoderEventArgs : EventArgs
    {
        private long _encodedData;
        private long _totalData;
        private bool _finished;
        private bool _error;
        private string _filepath;
        public EncoderEventArgs(long encoded, long total, bool finished = false, bool error = false, string filepath = null)
        {
            _finished = finished;
            _encodedData = encoded;
            _totalData = total;
            _error = error;
            if (filepath != null) { _filepath = filepath; }
        }

        public long EncodedData { get { return _encodedData; } }
        public long TotalData { get { return _totalData; } }
        public bool Finished { get { return _finished; } }
        public bool Error { get { return _error; } }
        public string FilePath{get{if(!_error){return _filepath;}else{ return "error occured during video encoding"; }}}
    }
    
    public class MinEventArgs 
    {
        public class EncoderMinArgs : EventArgs
        {
            public EncoderMinArgs(long encoded, long total, bool finished = false, bool error = false, string filepath = null)
            {
                _finished = finished;
                _encodedData = encoded;
                _totalData = total;
                _error = error;
                if (filepath != null) { _filepath = filepath; }
            }
        }

        private static long _encodedData;
        private static long _totalData;
        private static bool _finished;
        private static bool _error;
        private static string _filepath;


        public static long EncodedData { get { return _encodedData; } }
        public static long TotalData { get { return _totalData; } }
        public static bool Finished { get { return _finished; } }
        public static bool Error { get { return _error; } }
        public static string FilePath { get { if (!_error) { return _filepath; } else { return ""; } } }
    }
}