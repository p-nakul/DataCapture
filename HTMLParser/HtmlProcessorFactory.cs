using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DataCapture.HTMLParser
{
    internal class HtmlProcessorFactory
    {
        public static HtmlProcessor GetProcessor(string filePath)
        {
            var doc = new HtmlDocument();
            doc.Load(filePath);

            if (IsSubscriberInfoType(doc))
                return new HtmlSubscriberInfoProcessor(doc);
            else if (IsDeviceInfoType(doc))
                return new HtmlDeviceInfoProcessor(doc);
            else
                throw new Exception("Unknown HTML type");
        }

        private static bool IsSubscriberInfoType(HtmlDocument doc)
        {
            var h2Node = doc.DocumentNode.SelectSingleNode("//h2");
            if (h2Node != null)
            {
                string h2Text = h2Node.InnerText.Trim().ToLower();

                if (h2Text.Contains("GOOGLE SUBSCRIBER INFORMATION".ToLower()))
                    return true;

                return false;
            }
            
            return false;
        }

        private static bool IsDeviceInfoType(HtmlDocument doc)
        {
            var titleDiv = doc.DocumentNode.SelectSingleNode("//div[@class='title']");
            if (titleDiv != null)
            {
                var imgNode = titleDiv.SelectSingleNode(".//img");
                if (imgNode != null)
                {
                    string divText = titleDiv.InnerText.Trim().ToLower();

                    if(divText.IndexOf("ANDROID DEVICE CONFIGURATION SERVICE DATA".ToLower(), StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }

                    return false;
                }

                return false;
            }

            return false;
        }
    }
}
