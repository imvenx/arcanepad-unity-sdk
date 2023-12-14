using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ArcanepadSDK.AUtils
{
    public class AUtils
    {
        // public static Dictionary<string, string> GetQueryParams()
        // {
        //     Dictionary<string, string> queryParams = new Dictionary<string, string>();
        //     string url = Application.absoluteURL;

        //     int queryStart = url.IndexOf('?');
        //     int fragmentStart = url.IndexOf('#');

        //     if (queryStart >= 0)
        //     {
        //         // If fragment exists, get the substring between '?' and '#',
        //         // otherwise, take from '?' till the end.
        //         string paramsStrFull = (fragmentStart > queryStart) ?
        //             url.Substring(queryStart + 1, fragmentStart - queryStart - 1) :
        //             url.Substring(queryStart + 1);

        //         string[] paramsStr = paramsStrFull.Split('&');
        //         foreach (string paramStr in paramsStr)
        //         {
        //             string[] paramKeyValue = paramStr.Split('=');
        //             if (paramKeyValue.Length == 2)
        //             {
        //                 queryParams.Add(paramKeyValue[0], paramKeyValue[1]);
        //             }
        //         }
        //     }

        //     return queryParams;
        // }

        public static Dictionary<string, string> GetQueryParams()
        {
            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            string url = Application.absoluteURL;

            int queryStart = url.IndexOf('?');
            int fragmentStart = url.IndexOf('#');

            if (queryStart >= 0)
            {
                int paramsEnd = fragmentStart > queryStart ? fragmentStart : url.Length;

                // If there's a path between the query and fragment, like "/Pad/",
                // you might want to end the parameters string before this path.
                // Example: For path components like "/Pad/", you can find the index of "/" after "?".
                int pathStart = url.IndexOf('/', queryStart);
                if (pathStart != -1 && pathStart < paramsEnd)
                {
                    paramsEnd = pathStart;
                }

                string paramsStrFull = url.Substring(queryStart + 1, paramsEnd - queryStart - 1);

                string[] paramsStr = paramsStrFull.Split('&');
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