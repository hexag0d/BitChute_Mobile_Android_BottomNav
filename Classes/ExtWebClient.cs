using System;
using System.Net;

namespace BitChute.Classes
{
    /// <summary>
    /// Custom WebClient with overrides
    /// </summary>
    class ExtWebClient : WebClient
    {
        /// <summary>
        /// Overrides the GetWebRequest for an issue with the ssl cert
        /// verifies the thumbprint
        /// 
        /// DO NOT use this for the WebView.. it's just for the download module
        /// which wouldn't work without this override ATM ....   .
        /// 
        /// I would advise against attaching any cookies to this.
        /// Right now it's just a raw request
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);

            // for security purposes make sure that we're requesting from bitchute.com
            if (address.Host.EndsWith(".bitchute.com") && address.AbsolutePath.EndsWith(".mp4"))
            {
                //this is super dirty; add to a one off Callback with a hardcoded hash
                request.ServerCertificateValidationCallback += (sender, cert, chain, error) =>
                {
                    //if the cert request matches the bitchute thumbprint then it's valid
                    if (cert.GetCertHashString() == "8D1C059093E2F8800A3F27D8C98A2E5A78F52C6D")
                    {
                        //it matches so don't throw
                        return true;
                    }
                    //cert thumbprint doesn't match bitchute so don't make the request
                    return false;
                };
            }
            return request;
        }
    }
}