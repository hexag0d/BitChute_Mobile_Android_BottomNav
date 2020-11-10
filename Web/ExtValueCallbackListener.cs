using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using BitChute.Fragments;
using Java.Lang;
using static BitChute.Services.MainPlaybackSticky;

namespace BitChute.Web
{
    public class ExtWebValueCallbackListener : Java.Lang.Object, IValueCallback
    {
        private static Dictionary<int, ExtWebValueCallbackListener> _extValueCallbackDictionary = new Dictionary<int, ExtWebValueCallbackListener>();
        public static ExtWebValueCallbackListener GetValueCallbackById(int id =-1, ExtWebValueCallbackListener valueCb = null)
        {
            if (id == -1 && valueCb == null)
            {
                if (_extValueCallbackDictionary.Count > 0)
                _extValueCallbackDictionary.First();
            }
            if (!_extValueCallbackDictionary.ContainsKey(id) && valueCb != null)
            {
                _extValueCallbackDictionary.Add(id, valueCb);
                return _extValueCallbackDictionary[id];
            }
            else if (_extValueCallbackDictionary.ContainsKey(id) && valueCb == null)
            {
                return _extValueCallbackDictionary[id];
            }
            else
            {
                _extValueCallbackDictionary.Remove(id);
                _extValueCallbackDictionary.Add(id, valueCb);
                return _extValueCallbackDictionary[id];
            }
        }

        public int CallbackId { get; set; }
        public int RequestObjectId { get; set; }
        public object ResultObject { get; set; }
        public Java.Lang.Object ResultObjectJava { get; set; }
        

        public ExtWebValueCallbackListener(int requestByObjectId) {
            this.CallbackId = new System.Random().Next(999999999);
            this.RequestObjectId = requestByObjectId;
            GetValueCallbackById(this.CallbackId, this);
        }

        static bool checkingPlayer = false;

        List<ExtWebValueCallbackListener> listeners = new List<ExtWebValueCallbackListener>();
        static List<ServiceWebView> _serviceWebViewPlayingList;
        static int _nullObjectCount = 0;


        public static async Task<List<ServiceWebView>> GetCurrentlyPlayingWebView()
        {
            if (!checkingPlayer)
            {
                checkingPlayer = true;
                _serviceWebViewPlayingList = new List<ServiceWebView>();
            }
            else
            {
                return null;
            }

            foreach (var view in PlaystateManagement.WebViewIdDictionary.Values)
            {
                SetCallbackValue(view, RequestStrings.IsPlyrPlaying);
            }
            while (PlaystateManagement.WebViewIdDictionary.Count > _extValueCallbackDictionary.Count)
            {
                await Task.Delay(10);
            }
            foreach (var listener in _extValueCallbackDictionary)
            {
                Task.Factory.StartNew(() =>
                {
                    listener.Value.StartAwaitCallbackValueLoop(listener.Value);
                });
            }
            await Task.Delay(50);
            Task.Factory.StartNew(() =>
            {
                while (_nullObjectCount < _extValueCallbackDictionary.Count || !(_serviceWebViewPlayingList.Count > 0))
                {
                    Task.Delay(100);
                    if (_serviceWebViewPlayingList.Count > 0)
                    {
                        checkingPlayer = false;
                        break;
                    }
                }
            });
            return _serviceWebViewPlayingList;
            checkingPlayer = false;
            return _serviceWebViewPlayingList;
        }

        public async void StartAwaitCallbackValueLoop(ExtWebValueCallbackListener listener)
        {
            bool loopStarted = false;
            while (((listener.ResultObject?.ToString() == "null" || listener.ResultObject?.ToString() == "true" 
                || listener.ResultObject?.ToString() == "false") || listener.ResultObject == null) || loopStarted == false)
            {
                loopStarted = true;
                await Task.Delay(100);
                if (listener.ResultObject != null && ((ObjectType)listener.ResultObject) != ObjectType.None)
                {
                    listener.ResultObjectJava = (Java.Lang.Object)listener.ResultObject;
                    var cast = listener.ResultObject.ToString();
                    if (cast == "true")
                    {
                        _serviceWebViewPlayingList.Add(PlaystateManagement.GetWebViewPlayerById(listener.RequestObjectId, -1));
                        return;
                    }
                }

                if (listener.ResultObject != null)
                {
                    if ((ObjectType)listener.ResultObject == ObjectType.None)
                    {
                        _nullObjectCount++;
                        return;
                    }
                }
            }
        }

        public ExtWebValueCallbackListener GetCallbackListener(int requestObjectId)
        {
            ExtWebValueCallbackListener listener = new ExtWebValueCallbackListener(requestObjectId);
                GetValueCallbackById(this.CallbackId, this);
            return listener;
        }

        void IValueCallback.OnReceiveValue(Java.Lang.Object value)
        {
            ResultObject = value;
            if (ResultObject.ToString() == "null")
            {
                ResultObject = ObjectType.None;
            }
        }

        public void SetReceiveValue(ServiceWebView wv, string javascript)
        {
            ViewHelpers.DoActionOnUiThread(() => wv.EvaluateJavascript(javascript, this));
        }

        public static void SetCallbackValue(ServiceWebView wv, string javascript)
        {
            new ExtWebValueCallbackListener(wv.Id).SetReceiveValue(wv, javascript);
        }

        public class RequestStrings
        {
            public static string IsPlyrPlaying = "plyr.playing";
        }

        public enum ObjectType
        {
            None,
            Bool,
            String,
            Object,
            Other
        };
    }
}