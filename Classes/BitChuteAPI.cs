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
using BitChute;
using Newtonsoft.Json.Linq;
using static BottomNavigationViewPager.Models.SubscriptionModel;

namespace BottomNavigationViewPager.Classes
{
    public class BitChuteAPI
    {
        private string _response;

        public string MakeAPICall(string details)
        {
            //get details then make api call
            _response = "";
            return _response;
        }

        /// <summary>
        /// should probably only return the top 25 w/full video info 
        /// and the rest just the creator list
        /// </summary>
        /// <returns></returns>
        public List<CreatorPackage> GetSubscriptionsFromAPI(bool entireList)
        {
            List<CreatorPackage> creatorPackageList = new List<CreatorPackage>();

            //string json = "{'results':[{'SwiftCode':'','City':'','BankName':'Deutsche    Bank','Bankkey':'10020030','Bankcountry':'DE'},{'SwiftCode':'','City':'10891    Berlin','BankName':'Commerzbank Berlin (West)','Bankkey':'10040000','Bankcountry':'DE'}]}";

            string json = MakeAPICall("apiFormattedStringHere");

            var resultObjects = AllChildren(JObject.Parse(json))
                .First(c => c.Type == JTokenType.Array && c.Path.Contains("results"))
                .Children<JObject>();

            foreach (JObject result in resultObjects)
            {
                CreatorPackage cp = new CreatorPackage();
                cp = result.ToObject<CreatorPackage>();

                creatorPackageList.Add(cp);
                foreach (JProperty property in result.Properties())
                {
                    
                }
            }

            return creatorPackageList;
        }

        // recursively yield all children of json
        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }
    }
}