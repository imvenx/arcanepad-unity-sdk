using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ArcanepadSDK.AUtils
{
    public class AUtils
    {
        public static Dictionary<string, string> GetQueryParams()
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            string url = Application.absoluteURL;
            int queryStart = url.IndexOf('?');

            if (queryStart >= 0)
            {
                string[] paramsStr = url.Substring(queryStart + 1).Split('&');
                foreach (string paramStr in paramsStr)
                {
                    string[] paramKeyValue = paramStr.Split('=');
                    if (paramKeyValue.Length == 2)
                    {
                        queryParams.Add(paramKeyValue[0], paramKeyValue[1]);
                    }
                }
            }

            return queryParams;
        }

        [DllImport("__Internal")]
        public static extern bool IsIframe();

        public static string GetHost()
        {
            // string url = Application.absoluteURL;
            // var uri = new System.Uri(url);
            // return uri.Host;
            string url = Application.absoluteURL;
            var uri = new System.Uri(url);

            // Remove fragment identifier if present
            string absoluteUriWithoutFragment = uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Fragment, UriFormat.UriEscaped);

            var uriWithoutFragment = new System.Uri(absoluteUriWithoutFragment);

            return uriWithoutFragment.Host;
        }
    }
}