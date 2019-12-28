using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;

namespace PieskiLib
{
    class WebRequest
    {
        internal string webpath;
        internal WebRequest(string webpath)
        {
            this.webpath = webpath;
        }

        internal string Download()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(webpath);
            request.Timeout = 36000;
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dld_stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dld_stream, Encoding.UTF8);
            string html = reader.ReadToEnd();

            dld_stream.Close();
            dld_stream.Dispose();
            reader.Close();
            reader.Dispose();
            return html;
        }
    }
}
