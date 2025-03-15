using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DataCapture.Models;
using HtmlAgilityPack;

namespace DataCapture.HTMLParser
{
    internal class HtmlSubscriberInfoProcessor : HtmlProcessor
    {
        public HtmlSubscriberInfoProcessor(HtmlDocument doc) : base(doc) { }

        public override HtmlFile ExtractData(HtmlFile hf)
        {
            // extract first section
            //var h2Node = doc.DocumentNode.SelectSingleNode("//h2");
            hf.KeyValues = extractDivSections();

            return hf;
        }

        private Dictionary<string, string> extractDivSections()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            // Select all <div class="section"> elements
            var sectionDivs = Document.DocumentNode.SelectNodes("//div[@class='section']");

            try
            {
                if (sectionDivs != null)
                {
                    foreach (var section in sectionDivs)
                    {
                        var ulNode = section.SelectSingleNode(".//ul");
                        if (ulNode != null)
                        {
                            var liNodes = ulNode.SelectNodes(".//li");
                            if (liNodes != null)
                            {
                                foreach (var li in liNodes)
                                {
                                    string[] key_values = li.InnerText.Trim().Split(':');

                                    

                                    if (!key_values[1].Trim().Equals(""))
                                        keyValues[key_values[0]] = key_values[1];
                                }

                            }
                        }
                    }
                }
                else
                {
                    return keyValues;
                }

                return keyValues;
            }
            catch(Exception ex)
            {
                return keyValues;
            }
        }
    }
}
