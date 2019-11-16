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
using BottomNavigationViewPager.Fragments;
using HtmlAgilityPack;

namespace BottomNavigationViewPager.Classes
{
    public class ExtNotifications
    {
        public static TheFragment5 _fm5 = new TheFragment5();

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
         
        public async Task<List<CustomNotification>> DecodeHtmlNotifications(string html)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_fm5 == null)
                    {
                        _fm5 = TheFragment5._fm5;
                    }
                     
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var check = doc;

                    _notificationTextList.Clear();
                    _notificationTypes.Clear();
                    _notificationLinks.Clear();

                    if (doc != null)
                    {
                        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='notification-target']"))
                        {
                            //&#39; <<< '   ... &amp; <<< &
                            var _tagContents = node.InnerText;
                            _tagContents = _tagContents.Replace(@"&#39;", @"'").Replace(@"&amp;", @"&").Replace(@"&quot;", "\"");
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
                    _fm5 = TheFragment5._fm5;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                TheFragment5._notificationHttpRequestInProgress = false;

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