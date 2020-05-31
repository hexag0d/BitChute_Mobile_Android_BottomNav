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
using Android.Widget;
using BitChute.Fragments;
using HtmlAgilityPack;
using StartServices.Servicesclass;

namespace BitChute.Classes
{
    public class ExtNotifications
    {
        public static TheFragment4 _fm5 = new TheFragment4();

        public static List<string> _notificationTextList = new List<string>();
        public static List<string> _notificationTypes = new List<string>();
        public static List<string> _notificationLinks = new List<string>();
        public static List<CustomNotification> _customNoteList = new List<CustomNotification>();
        public static List<CustomNotification> _previousNoteList = new List<CustomNotification>();
        private int currentListIndex;

        public class CustomNotification : IEquatable<CustomNotification>
        {
            public string _noteType { get; set; }
            public string _noteText { get; set; }
            public string _noteLink { get; set; }

            public bool Equals(CustomNotification other)
            {
                if (this._noteType == other._noteType && this._noteText == other._noteText
                    && this._noteLink == other._noteLink)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static Notification BuildPlayControlNotification()
        {
            var pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, 0,
                new Intent(Android.App.Application.Context, typeof(MainActivity)),
                PendingIntentFlags.UpdateCurrent);


            // Using RemoteViews to bind custom layouts into Notification
            RemoteViews views = new RemoteViews(Android.App.Application.Context.PackageName, Resource.Layout.PlaystateNotification);
            RemoteViews bigViews = new RemoteViews(Android.App.Application.Context.PackageName, Resource.Layout.PlaystateNotification);

            // showing default album image
            //views.SetViewVisibility(R.id.status_bar_icon, ViewStates.Visible);
            //views.setViewVisibility(R.id.status_bar_album_art, View.GONE);
            //bigViews.setImageViewBitmap(R.id.status_bar_album_art,
            //Constants.getDefaultAlbumArt(this));

            Intent notificationIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));
            //notificationIntent.setAction(Constants.ACTION.MAIN_ACTION);
            //notificationIntent.AddFlags(
            //| Intent.Flag);

            //PendingIntent pendingIntent = PendingIntent.getActivity(this, 0, notificationIntent, 0);

            //Intent previousIntent = new Intent(this, NotificationService.class);
            //    previousIntent.setAction(Constants.ACTION.PREV_ACTION);
            //PendingIntent ppreviousIntent = PendingIntent.GetService(this, 0, previousIntent, 0);
            Intent playIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            playIntent.SetAction(ExtStickyService.ActionPlay);
            PendingIntent pplayIntent = PendingIntent.GetService(Android.App.Application.Context, 0, playIntent, 0);
            Intent nextIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            nextIntent.SetAction(ExtStickyService.ActionNext);
            PendingIntent pnextIntent = PendingIntent.GetService(Android.App.Application.Context, 0, nextIntent, 0);
            Intent pauseIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            pauseIntent.SetAction(ExtStickyService.ActionPause);
            PendingIntent ppauseIntent = PendingIntent.GetService(Android.App.Application.Context, 0, pauseIntent, 0);

            views.SetOnClickPendingIntent(Resource.Id.notificationPlay, pplayIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPlay , pplayIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationPause, ppauseIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPause, ppauseIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationNext, pnextIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationNext, pnextIntent);

            //views.setImageViewResource(R.id.status_bar_play,
            //R.drawable.apollo_holo_dark_pause);
            //bigViews.setImageViewResource(R.id.status_bar_play,
            //R.drawable.apollo_holo_dark_pause);
            //views.setTextViewText(R.id.status_bar_track_name, "Song Title");
            //bigViews.setTextViewText(R.id.status_bar_track_name, "Song Title");
            //views.setTextViewText(R.id.status_bar_artist_name, "Artist Name");
            //bigViews.setTextViewText(R.id.status_bar_artist_name, "Artist Name");
            //bigViews.setTextViewText(R.id.status_bar_album_name, "Album Name");

            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID)
                //.SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                .SetContentTitle("BitChute streaming in background")
                .SetSmallIcon(Resource.Drawable.bitchute_notification)
                .SetPriority(Android.Support.V4.App.NotificationCompat.PriorityLow);

            var status = builder.Build();
            status.ContentView = views;
            status.BigContentView = bigViews;
            status.Flags = NotificationFlags.OngoingEvent;
            status.Icon = Resource.Drawable.bitchute_notification;
            status.ContentIntent = pendingIntent;
            return status;
        }
            
        public async Task<List<CustomNotification>> DecodeHtmlNotifications(string html)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_fm5 == null)
                    {
                        _fm5 = TheFragment4._fm5;
                    }
                     
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var check = doc;

                    _notificationTextList.Clear();
                    _notificationTypes.Clear();
                    _notificationLinks.Clear();

                    if (doc != null)
                    {
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//title"))
                        {
                            var _tagContents = node.InnerText;
                            if (_tagContents.Contains("Notifications"))
                            {
                                AppState.UserIsLoggedIn = true;
                            }
                            else
                            {
                                AppState.UserIsLoggedIn = false;
                                return;
                            }
                        }

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-target']"))
                        {
                            //&#39; <<< '   ... &amp; <<< & "&#x27; '
                            var _tagContents = node.InnerText;
                            _tagContents = _tagContents.Replace(@"&#39;", @"'")
                                .Replace(@"&#x27;", @"'")
                                .Replace(@"&amp;", @"&")
                                .Replace(@"&quot;", "\"");
                            _notificationTextList.Add(_tagContents);
                        }

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-detail']"))
                        {
                            var _tagContents = node.InnerText;
                            _notificationTypes.Add(_tagContents.Split('-')[0]);
                        }

                        //foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-unread']"))
                        //{
                        //    var _tagContents = node.InnerText;

                        //    if (!_previousNotificationTypeList.Contains(_tagContents))
                        //    {
                        //        _notificationTypes.Add(_tagContents);
                        //    }
                        //}

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a[@class='notification-view']"))
                        {
                            var _tagContents = "https://bitchute.com" + node.Attributes["href"].Value.ToString();

                            _notificationLinks.Add(_tagContents);

                        }
                        currentListIndex = 0;
                        _customNoteList.Clear();

                        foreach (var nt in _notificationTypes)
                        {
                            var note = new CustomNotification();

                            note._noteType = nt.ToString();
                            note._noteLink = _notificationLinks[currentListIndex].ToString();
                            note._noteText = _notificationTextList[currentListIndex].ToString();
                            _customNoteList.Add(note);
                            currentListIndex++;
                        }
                        _customNoteList.Reverse();
                    }
                    _fm5 = TheFragment4._fm5;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                TheFragment4._notificationHttpRequestInProgress = false;

                //_fm5.SendNotifications();
            });
            return _customNoteList;
        }
        
        public async Task<string> GetBitChuteChannelLinkFromDiscus(string html)
        {
            string profileLink = "";

            await Task.Run(() =>
            {

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                int _nCount = 0;

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    var _tagContents = "https://bitchute.com/profile" + link.Attributes["href"].Value.ToString();

                    if (_nCount == 0)
                    {
                        profileLink = _tagContents;
                    }


                    _nCount++;
                }
            });

            return profileLink;
        }
    }
}