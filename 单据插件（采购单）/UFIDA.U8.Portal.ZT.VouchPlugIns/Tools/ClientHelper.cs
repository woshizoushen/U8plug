using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class ClientHelper
    {
        private static Dictionary<string, CookieState> _cookie = new Dictionary<string, CookieState>();

        private static CookieContainer GetCookie(string siteUrl)
        {
            string host = new Uri(siteUrl).Host;
            if (!_cookie.ContainsKey(host))
            {
                _cookie[host] = new CookieState
                {
                    Cookie = new CookieContainer(),
                    CookieTime = DateTime.Now
                };
            }
            return _cookie[host].Cookie;
        }

        public static bool CheckOnline(string siteUrl)
        {
            string host = new Uri(siteUrl).Host;
            if (!_cookie.ContainsKey(host))
            {
                return false;
            }
            DateTime cookieTime = _cookie[host].CookieTime;
            if (cookieTime.AddMinutes(5.0).CompareTo(DateTime.Now) < 0)
            {
                _cookie.Remove(host);
                return false;
            }
            return true;
        }

        public static HttpWebResponse HttpGet(string url, Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, Encoding dataEncoding = null)
        {
            return HttpRequest("GET", url, headers, parameters, dataEncoding);
        }

        public static HttpWebResponse HttpPost(string url, Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, Encoding dataEncoding = null)
        {
            return HttpRequest("POST", url, headers, parameters, dataEncoding);
        }

        public static HttpWebResponse HttpRequest(string method, string url, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null, Encoding dataEncoding = null)
        {
            HttpWebRequest httpWebRequest = CreateRequest(method, url, headers, parameters, dataEncoding);
            if ((method == "POST" || method == "PUT") && parameters != null && parameters.Count != 0)
            {
                byte[] array = FormatPostParameters(parameters, dataEncoding, httpWebRequest.ContentType);
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(array, 0, array.Length);
                    stream.Close();
                }
            }
            WebResponse webResponse = null;
            try
            {
                webResponse = httpWebRequest.GetResponse();
            }
            catch (WebException ex)
            {
                webResponse = (HttpWebResponse)ex.Response;
            }
            catch (Exception ex2)
            {
                throw ex2;
            }
            if (webResponse == null)
            {
                return httpWebRequest.GetResponse() as HttpWebResponse;
            }
            return (HttpWebResponse)webResponse;
        }

        public static string HttpData(string method, string url, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null, Encoding dataEncoding = null)
        {
            Encoding encoding = dataEncoding ?? Encoding.UTF8;
            HttpWebResponse httpWebResponse = HttpRequest(method, url, headers, parameters, encoding);
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), encoding);
            if (parameters != null)
            {
                JObject jObject = new JObject();
                jObject["code"] = (int)httpWebResponse.StatusCode;
                jObject["text"] = httpWebResponse.StatusCode.ToString();
                parameters.Add("ResponseStatus", JsonConvert.SerializeObject(jObject));
            }
            return streamReader.ReadToEnd();
        }

        private static HttpWebRequest CreateRequest(string method, string url, Dictionary<string, string> headers, Dictionary<string, string> parameters, Encoding paraEncoding)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (parameters != null && parameters.Count > 0 && paraEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest httpWebRequest = null;
            if (method.ToLower() == "get" && parameters != null && parameters.Count > 0)
            {
                url = FormatGetParametersToUrl(url, parameters, paraEncoding);
            }
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                if (headers != null && headers.ContainsKey("Tls"))
                {
                    if (headers["Tls"] == "1.1")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                    }
                    if (headers["Tls"] == "1.2")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;//| SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    }
                }
            }
            httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            if (headers != null && headers.ContainsKey("UserName"))
            {
                httpWebRequest.Credentials = new NetworkCredential
                {
                    UserName = headers["UserName"],
                    Password = headers["Password"]
                };
            }
            if (headers != null)
            {
                FormatRequestHeaders(headers, httpWebRequest);
            }
            httpWebRequest.Method = method;
            httpWebRequest.ContentType = httpWebRequest.ContentType ?? "application/json";
            if (headers != null && headers.ContainsKey("charset"))
            {
                HttpWebRequest httpWebRequest2 = httpWebRequest;
                httpWebRequest2.ContentType = httpWebRequest2.ContentType + ";charset=" + headers["charset"];
            }
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.ServicePoint.ConnectionLimit = int.MaxValue;
            httpWebRequest.CookieContainer = GetCookie(url);
            return httpWebRequest;
        }

        private static void FormatRequestHeaders(Dictionary<string, string> headers, HttpWebRequest request)
        {
            foreach (KeyValuePair<string, string> header in headers)
            {
                switch (header.Key.ToLower())
                {
                    case "connection":
                        request.KeepAlive = false;
                        break;
                    case "content-type":
                        request.ContentType = header.Value;
                        break;
                    case "transfer-enconding":
                        request.TransferEncoding = header.Value;
                        break;
                    case "user-agent":
                        request.UserAgent = header.Value;
                        break;
                    default:
                        //request.Headers.Add(header.Key, Uri.EscapeDataString(header.Value));
                        request.Headers.Add(header.Key, header.Value);
                        break;
                }
            }
        }

        public static string FormatGetParametersToUrl(string url, Dictionary<string, string> parameters, Encoding dataEncoding)
        {
            if (dataEncoding == null)
            {
                dataEncoding = Encoding.UTF8;
            }
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                string key = parameter.Key;
                string arg = HttpUtility.UrlEncode(parameter.Value, dataEncoding);
                if (!key.StartsWith("_"))
                {
                    list.Add(string.Format("{0}={1}", key, arg));
                }
            }
            url = url + ((url.IndexOf("?") < 0) ? "?" : "&") + string.Join("&", list.ToArray());
            return url;
        }

        public static byte[] FormatPostParameters(Dictionary<string, string> parameters, Encoding dataEncoding, string contentType)
        {
            if (dataEncoding == null)
            {
                dataEncoding = Encoding.UTF8;
            }
            string s = string.Empty;
            if (contentType.Contains("application/json"))
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (!parameter.Key.StartsWith("_"))
                    {
                        dictionary[parameter.Key] = JsonHelper.GetToken(parameter.Value, parameter.Value);
                    }
                }
                s = ((dictionary.Count != 1 || !dictionary.ContainsKey("data")) ? JsonConvert.SerializeObject(dictionary) : dictionary["data"].ToString());
            }
            else if (contentType.Contains("text/xml"))
            {
                foreach (KeyValuePair<string, string> parameter2 in parameters)
                {
                    if (!parameter2.Key.StartsWith("_"))
                    {
                        s = parameter2.Value.ToString();
                        break;
                    }
                }
            }
            else
            {
                List<string> list = new List<string>();
                foreach (KeyValuePair<string, string> parameter3 in parameters)
                {
                    string key = parameter3.Key;
                    string value = parameter3.Value;
                    if (!parameter3.Key.StartsWith("_"))
                    {
                        list.Add(string.Format("{0}={1}", key, EscapeString(value)));
                    }
                }
                s = string.Join("&", list.ToArray());
            }
            return dataEncoding.GetBytes(s);
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public static string EscapeString(string stringToEscape)
        {
            int num = 32700;
            int num2 = num;
            string text = string.Empty;
            for (int i = 0; i < stringToEscape.Length; i += num)
            {
                if (i + num2 > stringToEscape.Length)
                {
                    num2 = stringToEscape.Length - i;
                }
                text += Uri.EscapeDataString(stringToEscape.Substring(i, num2));
            }
            return text;
        }
    }
    public class CookieState
    {
        public DateTime CookieTime;

        public CookieContainer Cookie;
    }
}
