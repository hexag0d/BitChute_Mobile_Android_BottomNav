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
using BottomNavigationViewPager.Fragments;
using HtmlAgilityPack;

namespace BottomNavigationViewPager.Classes
{
    class ExtNotifications
    {
        public static TheFragment5 _fm5 = MainActivity._fm5;

        public static List<string> _notificationTextList = new List<string>();

        public static List<string> _notificationTypes = new List<string>();

        public static List<string> _notificationLinks = new List<string>();

        public static List<string> _previousNotificationTextList = new List<string>();

        public static List<CustomNotification> _customNoteList = new List<CustomNotification>();
        private int currentListIndex;

        public class CustomNotification
        {
            public string _noteType { get; set; }
            public string _noteText { get; set; }
            public string _noteLink { get; set; }
            public int _noteIndex { get; set; }
        }

        public static bool _notificationChanged = false;

        public async void DecodeHtmlNotifications(string html)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    _notificationTextList.Clear();
                    _notificationTypes.Clear();
                    _notificationLinks.Clear();

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-target']"))
                    {
                        var _tagContents = node.InnerText;

                        _notificationTextList.Add(_tagContents);
                    }

                    if (_notificationTextList == _previousNotificationTextList)
                    {
                        return;
                    }

                    //if (_notificationTextList == _previousNotificationList)
                    //{
                    //    
                    //    return _customNoteList;
                    //}

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-unread']"))
                    {
                        var _tagContents = node.InnerText;

                        _notificationTypes.Add(_tagContents);
                    }

                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//a[@class='notification-view']"))
                    {
                        var _tagContents = "https://bitchute.com" + node.Attributes["href"].Value.ToString();

                        _notificationLinks.Add(_tagContents);
                    }
                    currentListIndex = 0;

                    _customNoteList.Clear();

                    

                    foreach (var nt in _notificationTextList)
                    {
                        var note = new CustomNotification();

                        note._noteType = nt.ToString();
                        note._noteLink = _notificationLinks[currentListIndex].ToString();
                        note._noteText = _notificationTextList[currentListIndex].ToString();
                        note._noteIndex = currentListIndex;
                        _customNoteList.Add(note);
                        currentListIndex++;
                    }
                    _fm5.SendNotifications();
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                TheFragment5._notificationHttpRequestInProgress = false;

                _previousNotificationTextList = _notificationTextList;
            });
            //_fm5.SendNotifications();
        }
    }
}