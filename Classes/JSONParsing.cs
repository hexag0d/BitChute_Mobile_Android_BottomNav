using Android.App;
using Android.Content.Res;
using MvvmCross;
using Newtonsoft.Json;
using System.IO;

namespace BottomNavigationViewPager.Classes
{
    public class JSONParsing : Activity
    {
        public class Samples
        {
            //public string 
            
        }

        public static string GetObjects()
        {
            // Read the contents of our asset
            string content;
            AssetManager assets = MainActivity.GetAssetManager();
            using (StreamReader sr = new StreamReader(assets.Open("articlesExample.json")))
            {
                content = sr.ReadToEnd();
            }

            return content;
        }
    }
}