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
using BitChute.Services;

namespace BitChute.Classes
{
    public class ExtNotifications
    {
        //internal static ExtNotifications ExtNotifications { get => _extNotifications; set => _extNotifications = value; }

        public static List<string> NotificationTextList = new List<string>();
        public static List<string> NotificationTypes = new List<string>();
        public static List<string> NotificationLinks = new List<string>();
        public static List<CustomNotification> CustomNoteList = new List<CustomNotification>();
        public static List<CustomNotification> PreviousNoteList = new List<CustomNotification>();
        private int _currentListIndex;

        private static List<CustomNotification> _sentNotificationList = new List<CustomNotification>();
        private static Android.Support.V4.App.NotificationManagerCompat _notificationManager;

        /// <summary>
        /// returns/should be set to true when there's already a notification http request
        /// in progress
        /// </summary>
        public static bool NotificationHttpRequestInProgress = false;

        private static int _count = 0;

        public class CustomNotification : IEquatable<CustomNotification>
        {
            public string NoteType { get; set; }
            public string NoteText { get; set; }
            public string NoteLink { get; set; }

            public bool Equals(CustomNotification other)
            {
                if (this.NoteType == other.NoteType && this.NoteText == other.NoteText
                    && this.NoteLink == other.NoteLink) { return true; }
                else  {return false; }
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
                    if (notificationList.Count == 0) { return; }
                    int notePos = 0;

                    foreach (var note in notificationList)
                    {
                        var resultIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));
                        var valuesForActivity = new Bundle();
                        valuesForActivity.PutInt(MainActivity.COUNT_KEY, _count);
                        valuesForActivity.PutString("URL", note.NoteLink);
                        resultIntent.SetAction(ExtSticky.ActionLoadUrl);
                        resultIntent.PutExtras(valuesForActivity);
                        var resultPendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, MainActivity.NOTIFICATION_ID, resultIntent, PendingIntentFlags.UpdateCurrent);
                        resultIntent.AddFlags(ActivityFlags.SingleTop);
                        var alarmAttributes = new Android.Media.AudioAttributes.Builder()
                                .SetContentType(Android.Media.AudioContentType.Sonification)
                                .SetUsage(Android.Media.AudioUsageKind.Notification).Build();

                        if (!_sentNotificationList.Contains(note) && notePos == 0)
                        {
                            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID + 1)
                                    .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                                    .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
                                    .SetContentTitle(note.NoteText) 
                                    .SetNumber(1)       
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
                        ExtSticky.NotificationsHaveBeenSent = true;
                    }
                }
                catch { }
            });
        }


        public static async void SendMainActivityNotifications(List<CustomNotification> notificationList)
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
                        ExtSticky.NotificationsHaveBeenSent = true;
                    }
                }
                catch { }
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



            Intent notificationIntent = new Intent(Android.App.Application.Context, typeof(MainActivity));

            Intent previousIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            previousIntent.SetAction(ExtSticky.ActionPrevious);
            PendingIntent ppreviousIntent = PendingIntent.GetService(Android.App.Application.Context, 0, previousIntent, 0);
            Intent playIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            playIntent.SetAction(ExtSticky.ActionPlay);
            PendingIntent pplayIntent = PendingIntent.GetService(Android.App.Application.Context, 0, playIntent, 0);
            Intent nextIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            nextIntent.SetAction(ExtSticky.ActionNext);
            PendingIntent pnextIntent = PendingIntent.GetService(Android.App.Application.Context, 0, nextIntent, 0);
            Intent pauseIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            pauseIntent.SetAction(ExtSticky.ActionPause);
            PendingIntent ppauseIntent = PendingIntent.GetService(Android.App.Application.Context, 0, pauseIntent, 0);
            Intent playNoteInBkgrdIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            playNoteInBkgrdIntent.SetAction(ExtSticky.ActionBkgrdNote);
            PendingIntent pplayNoteInBkgrdIntent = PendingIntent.GetService(Android.App.Application.Context, 0, playNoteInBkgrdIntent, 0);
            Intent playNoteResumeIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            playNoteResumeIntent.SetAction(ExtSticky.ActionResumeNote);
            PendingIntent pplayNoteResumeIntent = PendingIntent.GetService(Android.App.Application.Context, 0, playNoteResumeIntent, 0);


            views.SetOnClickPendingIntent(Resource.Id.notificationPlay, pplayIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPlay , pplayIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationPause, ppauseIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPause, ppauseIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationNext, pnextIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationNext, pnextIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationPrevious, ppreviousIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPrevious, ppreviousIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPlaysInBkgrdOnRb, pplayNoteInBkgrdIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationPlaysInBkgrdOnRb, pplayNoteInBkgrdIntent);
            bigViews.SetOnClickPendingIntent(Resource.Id.notificationPlaysInBkgrdOffRb, pplayNoteResumeIntent);
            views.SetOnClickPendingIntent(Resource.Id.notificationPlaysInBkgrdOffRb, pplayNoteResumeIntent);

            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Android.App.Application.Context, MainActivity.CHANNEL_ID)
                //.SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
                .SetContentTitle("BitChute streaming in background")
                .SetSmallIcon(Resource.Drawable.bitchute_notification2)
                .SetOngoing(true)
                .SetPriority(Android.Support.V4.App.NotificationCompat.PriorityMax);

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
            Intent newIntent = new Intent(Android.App.Application.Context, typeof(ExtSticky));
            newIntent.AddFlags(original.Flags);
            newIntent.PutExtras(original.Extras);
            
            newIntent.SetAction(ExtSticky.ActionLoadUrl);
            return newIntent;
        }

        public async Task<List<CustomNotification>> DecodeHtmlNotifications(string html)
        {
            await Task.Run(() =>
            {
                try
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    NotificationTextList.Clear();
                    NotificationTypes.Clear();
                    NotificationLinks.Clear();

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
                            NotificationTextList.Add(_tagContents);
                        }

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-detail']"))
                        {
                            var _tagContents = node.InnerText;
                            NotificationTypes.Add(_tagContents.Split('-')[0]);
                        }

                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a[@class='notification-view']"))
                        {
                            var _tagContents = "https://bitchute.com" + node.Attributes["href"].Value.ToString();

                            NotificationLinks.Add(_tagContents);

                        }
                        _currentListIndex = 0;
                        CustomNoteList.Clear();

                        foreach (var nt in NotificationTypes)
                        {
                            var note = new CustomNotification();

                            note.NoteType = nt.ToString();
                            note.NoteLink = NotificationLinks[_currentListIndex].ToString();
                            note.NoteText = NotificationTextList[_currentListIndex].ToString();
                            CustomNoteList.Add(note);
                            _currentListIndex++;
                        }
                        CustomNoteList.Reverse();
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.Message);
                }
                ExtNotifications.NotificationHttpRequestInProgress = false;
            });
            return CustomNoteList;
        }
    }
}