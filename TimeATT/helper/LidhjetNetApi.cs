using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace TimeATT
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    class LidhjetNetApi
    {
        public string endpoint { get; set; }
        public httpVerb httpMethod { get; set; }


        public int lidhuApi(ListBox lbPMLog, string host, string user, string pass)
        {
            lbPMLog.Items.Add(DateTime.Now + ": " + "Login Api");
            return 1;
        }

        public LidhjetNetApi()
        {
            endpoint = string.Empty;
            httpMethod = httpVerb.GET;
        }

        public string GjejApiInfo()
        {
            string res = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = httpMethod.ToString();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new ApplicationException("Error!!! " + response.StatusCode.ToString());
                }
                using (Stream responseStream = response.GetResponseStream())
                {
                    if(responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            res = reader.ReadToEnd();
                        }
                    }
                }
                return res;
            }
        }
    }
}
