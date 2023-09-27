using System.Collections.Generic;
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
    }
}