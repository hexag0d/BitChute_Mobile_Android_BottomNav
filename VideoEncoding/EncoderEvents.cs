using System;

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
            if (finished) { _finished = true; }
            _encodedData = encoded;
            _totalData = total;
            _error = error;
            if (filepath != null) { _filepath = filepath; }
        }

        public long EncodedData { get { return _encodedData; } }
        public long TotalData { get { return _totalData; } }
        public bool Finished { get { return _finished; } }
        public bool Error { get { return _error; } }
        public string FilePath{get{if(!_error){return _filepath;}else{return "an error occured processing video data";}}}
    }
}