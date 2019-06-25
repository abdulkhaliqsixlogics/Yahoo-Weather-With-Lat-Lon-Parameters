using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

class YWSample
{
    public static void Main()
    {
        const string cURL = "https://weather-ydn-yql.media.yahoo.com/forecastrss";
        const string cAppID = "your App-Id";
        const string cConsumerKey = "your cConsumerKey";
        const string cConsumerSecret = "your cConsumerSecret";
        const string cOAuthVersion = "1.0";
        const string cOAuthSignMethod = "HMAC-SHA1";
        string lat = Uri.EscapeDataString("37.372");
        string lon = Uri.EscapeDataString("-122.038");
        const string cUnitID = "u=f";           // Metric units
        const string cFormat = "json";

        string _get_timestamp()
        {
            TimeSpan lTS = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(lTS.TotalSeconds).ToString();
        }  // end _get_timestamp

        string _get_nonce()
        {
            return Convert.ToBase64String(
             new ASCIIEncoding().GetBytes(
              DateTime.Now.Ticks.ToString()
             )
            );
        }  // end _get_nonce


        string _get_auth()
        {
            string retVal;
            string lNonce = _get_nonce();
            string lTimes = _get_timestamp();
            string lCKey = string.Concat(cConsumerSecret, "&");
            string lSign = string.Format(  // note the sort order !!!
             "format={0}&" +
             "lat={1}&" +
             "lon={2}&" +
             "oauth_consumer_key={3}&" +
             "oauth_nonce={4}&" +
             "oauth_signature_method={5}&" +
             "oauth_timestamp={6}&" +
             "oauth_version={7}&" +
             "{8}",
             cFormat,
             lat,
             lon,
             cConsumerKey,
             lNonce,
             cOAuthSignMethod,
             lTimes,
             cOAuthVersion,
             cUnitID
            );

            lSign = string.Concat(
             "GET&", Uri.EscapeDataString(cURL), "&", Uri.EscapeDataString(lSign)
            );

            using (var lHasher = new HMACSHA1(Encoding.ASCII.GetBytes(lCKey)))
            {
                lSign = Convert.ToBase64String(
                 lHasher.ComputeHash(Encoding.ASCII.GetBytes(lSign))
                );
            }  // end using

            return "OAuth " +
                   "oauth_consumer_key=\"" + cConsumerKey + "\", " +
                   "oauth_nonce=\"" + lNonce + "\", " +
                   "oauth_timestamp=\"" + lTimes + "\", " +
                   "oauth_signature_method=\"" + cOAuthSignMethod + "\", " +
                   "oauth_signature=\"" + lSign + "\", " +
                   "oauth_version=\"" + cOAuthVersion + "\"";

        }  // end _get_auth


        string url = cURL + "?lat=" + lat + "&lon=" + lon + "&" + cUnitID + "&format=" + cFormat;
        using (var client = new WebClient())
        {
            string responseText = string.Empty;
            try
            {
                string headerString = _get_auth();

                WebClient webClient = new WebClient();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                webClient.Headers[HttpRequestHeader.Authorization] = headerString;
                webClient.Headers.Add("X-Yahoo-App-Id", cAppID);
                byte[] reponse = webClient.DownloadData(url);
                string lOut = Encoding.ASCII.GetString(reponse);
                Console.WriteLine(lOut);
                Console.ReadKey();
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    var responseStream = exception.Response.GetResponseStream();
                    
                }

            }
        }
    }
}  