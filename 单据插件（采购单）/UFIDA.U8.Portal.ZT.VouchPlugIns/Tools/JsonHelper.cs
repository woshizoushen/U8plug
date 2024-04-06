using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UFIDA.U8.Portal.ZT.VouchPlugIns.Tools
{
    public class JsonHelper
    {
        public static JToken GetToken(string tokenData, JToken defaultToken)
        {
            tokenData = (tokenData ?? "").ToString().TrimStart(" \f\n\r\t\v".ToCharArray());
            if (tokenData.StartsWith("[") && tokenData.EndsWith("]"))
            {
                return JArray.Parse(tokenData);
            }
            if (tokenData.StartsWith("{") && tokenData.EndsWith("}"))
            {
                return JObject.Parse(tokenData);
            }
            if (tokenData.StartsWith("<"))
            {
                return GetJObject(tokenData, "xml");
            }
            return defaultToken;
        }

        public static JObject GetJObject(object dataSource, string dataType = "", bool formatValue = false)
        {
            if (dataSource is JObject)
            {
                return dataSource as JObject;
            }
            JObject jObject = new JObject();
            string text = dataType.ToLower();
            try
            {
                string text2 = (dataSource ?? "").ToString().TrimStart(" \f\n\r\t\v".ToCharArray());
                string text3 = text2.PadRight(10, ' ').Substring(0, 10);
                if (text == "jarray" || (text3.StartsWith("[") && text2.EndsWith("]")))
                {
                    jObject["data"] = JArray.Parse(text2);
                    return jObject;
                }
                if (text == "jobject" || (text3.StartsWith("{") && text2.EndsWith("}")))
                {
                    jObject = JObject.Parse(text2);
                    return jObject;
                }
                if (text == "xml" || text3.StartsWith("<?xml") || text3.StartsWith("<") || text3.Contains("xmlns:"))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlElement xmlElement = xmlDocument.CreateElement("xmlObject");
                    xmlElement.InnerXml = text2;
                    string text4 = JsonConvert.SerializeXmlNode(xmlElement);
                    text4 = text4.Replace("\"@", "\"");
                    jObject = JObject.Parse(text4);
                    jObject.Remove("?xml");
                    return jObject["xmlObject"] as JObject;
                }
                if (dataSource is JValue)
                {
                    jObject = new JObject(new JProperty("data", text2));
                    return jObject;
                }
                if (dataSource is string)
                {
                    jObject["undefined"] = text2;
                    return jObject;
                }
            }
            catch
            {
            }
            return jObject;
        }
    }
}
