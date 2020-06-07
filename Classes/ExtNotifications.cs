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
        //internal static ExtNotifications ExtNotifications { get => _extNotifications; set => _extNotifications = value; }

        public static TheFragment4 _fm5 = new TheFragment4();

        public static List<string> NotificationTextList = new List<string>();
        public static List<string> NotificationTypes = new List<string>();
        public static List<string> NotificationLinks = new List<string>();
        public static List<CustomNotification> CustomNoteList = new List<CustomNotification>();
        public static List<CustomNotification> PreviousNoteList = new List<CustomNotification>();
        private int currentListIndex;

        private static List<CustomNotification> _sentNotificationList = new List<CustomNotification>();
        private static Android.Support.V4.App.NotificationManagerCompat _notificationManager;

        public static bool _notificationHttpRequestInProgress = false;

        private static int _count = 0;

        public class CustomNotification : IEquatable<CustomNotification>
        {
            public string NoteType { get; set; }
            public string NoteText { get; set; }
            public string NoteLink { get; set; }

            public bool Equals(CustomNotification other)
            {
                if (this.NoteType == other.NoteType && this.NoteText == other.NoteText
                    && this.NoteLink == other.NoteLink)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public static async void SendNotifications(List<CustomNotification> notificationList)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_notificationManager == null)
                    {
                        _notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(Android.App.Application.Context);
                    }

                    if (notificationList.Count == 0)
                    {
                        return;
                    }
                    int notePos = 0;

                    // When the user clicks the notification, MainActivity will start up.

                    foreach (var note in notificationList)
                    {
                        var resultIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));
                        var valuesForActivity = new Bundle();
                        valuesForActivity.PutInt(MainActivity.COUNT_KEY, _count);
                        valuesForActivity.PutString("URL", note.NoteLink);
                        resultIntent.PutExtras(valuesForActivity);
                        var resultPendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, MainActivity.NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);
                        resultIntent.AddFlags(ActivityFlags.SingleTop);

                        var alarmAttributes = new Android.Media.AudioAttributes.Builder()
                                .SetContentType(Android.Media.AudioContentType.Sonification)
                                .SetUsage(Android.Media.AudioUsageKind.Notification).Build();

                        if (!_sentNotificationList.Contains(note) && notePos == 0)
                        {
                            // Build the notification:
                            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID + 1)
                                    .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                    .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                                    .SetContentTitle(note.NoteText) // Set the title
                                    .SetNumber(1) // Display the count in the Content Info
                                                  //.SetLargeIcon(_notificationBMP) // This is the icon to display
                                    .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                                    .SetContentText(note.NoteType)
                                    .SetPriority(Android.Support.V4.App.NotificationCompat.PriorityMin);

                            MainActivity.NOTIFICATION_ID++;

                            // publish the notification:
                            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
                            _notificationManager.Notify(MainActivity.NOTIFICATION_ID, builder.Build());
                            _sentNotificationList.Add(note);
                            notePos++;
                        }
                        else if (!_sentNotificationList.Contains(note))
                        {
                            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID)
                                .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                                .SetContentTitle(note.NoteText) // Set the title
                                .SetNumber(1) // Display the count in the Content Info
                                              //.SetLargeIcon(_notificationBMP) // This is the icon to display
                                .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                                .SetContentText(note.NoteType)
                                .SetPriority(Android.Support.V4.App.NotificationCompat.PriorityLow);

                            MainActivity.NOTIFICATION_ID++;


                            // publish the notification:
                            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(_ctx);
                            _notificationManager.Notify(MainActivity.NOTIFICATION_ID, builder.Build());
                            _sentNotificationList.Add(note);
                            notePos++;
                        }

                        ExtStickyService.NotificationsHaveBeenSent = true;
                    }
                }
                catch
                {
                }
            });
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

            Intent previousIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            previousIntent.SetAction(ExtStickyService.ActionPrevious);
            PendingIntent ppreviousIntent = PendingIntent.GetService(Android.App.Application.Context, 0, previousIntent, 0);
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
            views.SetOnClickPendingIntent(Resource.Id.notificationPrevious, ppreviousIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPrevious, ppreviousIntent);

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
                .SetOngoing(true)
                .SetPriority(Android.Support.V4.App.NotificationCompat.PriorityHigh);

            var status = builder.Build();
            status.ContentView = views;
            status.BigContentView = bigViews;
            status.Flags = NotificationFlags.OngoingEvent;
            status.Icon = Resource.Drawable.bitchute_notification2;
            status.ContentIntent = pendingIntent;
            return status;
        }

        public static Notification BuildPlayControlNotificationTest()
        {
            var pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, 0,
                new Intent(Android.App.Application.Context, typeof(ExtStickyService)),
                PendingIntentFlags.UpdateCurrent);

            // Using RemoteViews to bind custom layouts into Notification
            RemoteViews views = new RemoteViews(Android.App.Application.Context.PackageName, Resource.Layout.PlaystateNotification);
            RemoteViews bigViews = new RemoteViews(Android.App.Application.Context.PackageName, Resource.Layout.PlaystateNotification);

            // showing default album image
            //views.SetViewVisibility(R.id.status_bar_icon, ViewStates.Visible);
            //views.setViewVisibility(R.id.status_bar_album_art, View.GONE);
            //bigViews.setImageViewBitmap(R.id.status_bar_album_art,
            //Constants.getDefaultAlbumArt(this));

            Intent notificationIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            //notificationIntent.setAction(Constants.ACTION.MAIN_ACTION);
            //notificationIntent.AddFlags(
            //| Intent.Flag);

            Intent playIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            playIntent.SetAction(ExtStickyService.ActionLoadUrl);
            PendingIntent pplayIntent = PendingIntent.GetService(Android.App.Application.Context, 0, playIntent, 0);

            views.SetOnClickPendingIntent(Resource.Layout.PlaystateNotification, pplayIntent);
            bigViews.SetOnClickPendingIntent(Resource.Layout.PlaystateNotification, pplayIntent);
            views.SetOnClickPendingIntent(Resource.Id.playControlNotification, pplayIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.playControlNotification, pplayIntent);
            views.SetOnClickPendingIntent(Resource.Id.playControlNotification, pplayIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationNext, pplayIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationNext, pplayIntent);

            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID)
                //.SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                .SetContentTitle("BitChute streaming test")
                .SetSmallIcon(Resource.Drawable.bitchute_notification)
                .SetOngoing(true)
                .SetPriority(Android.Support.V4.App.NotificationCompat.PriorityHigh);

            var status = builder.Build();
            
            status.ContentView = views;
            status.BigContentView = bigViews;
            status.Flags = NotificationFlags.OngoingEvent;
            status.Icon = Resource.Drawable.bitchute_notification2;
            status.ContentIntent = pendingIntent;

            return status;
        }

        /// <summary>
        /// takes an intent and turns it into a notification that won't bring the app to forefront.
        /// this is useful for when a user wants to play a new video from the notification
        /// menu instead of going back in the app
        /// </summary>
        /// <param name="original">the intent you want to strip</param>
        /// <returns>background intent</returns>
        public static Intent SwapToBackgroundNotification(Intent original)
        {
            Intent newIntent = new Intent(Android.App.Application.Context, typeof(ExtStickyService));
            newIntent.AddFlags(original.Flags);
            newIntent.PutExtras(original.Extras);
            newIntent.SetAction(ExtStickyService.ActionLoadUrl);
            return newIntent;
        }

        public async Task<List<CustomNotification>> DecodeHtmlNotifications(string html)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_fm5 == null)
                    {
                        _fm5 = TheFragment4.Fm4;
                    }
                     
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    NotificationTextList.Clear();
                    NotificationTypes.Clear();
                    NotificationLinks.Clear();

                    if (doc != null)
                    {
                        var check = doc.DocumentNode;
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
                            NotificationTextList.Add(_tagContents);
                        }

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-detail']"))
                        {
                            var _tagContents = node.InnerText;
                            NotificationTypes.Add(_tagContents.Split('-')[0]);
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

                            NotificationLinks.Add(_tagContents);

                        }
                        currentListIndex = 0;
                        CustomNoteList.Clear();

                        foreach (var nt in NotificationTypes)
                        {
                            var note = new CustomNotification();

                            note.NoteType = nt.ToString();
                            note.NoteLink = NotificationLinks[currentListIndex].ToString();
                            note.NoteText = NotificationTextList[currentListIndex].ToString();
                            CustomNoteList.Add(note);
                            currentListIndex++;
                        }
                        CustomNoteList.Reverse();
                    }
                    _fm5 = TheFragment4.Fm4;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                ExtNotifications._notificationHttpRequestInProgress = false;

                //_fm5.SendNotifications();
            });
            return CustomNoteList;
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