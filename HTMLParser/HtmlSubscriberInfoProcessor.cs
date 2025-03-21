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
            hf.Sections = extractHtmlSections();

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

                                    string value = key_values.Length > 1 ? string.Join(":", key_values, 1, key_values.Length - 1) : "";

                                    if (!value.Trim().Equals(""))
                                        keyValues[key_values[0]] = value;
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


        private List<HtmlSection> extractHtmlSections()
        {
            var sections = new List<HtmlSection>();
            var nodes = Document.DocumentNode.SelectNodes("//h3 | //table | //text()") ?? new HtmlNodeCollection(null);

            HtmlSection currentSection = null;

            foreach (var node in nodes)
            {
                if (node.Name == "h3")
                {
                    if (currentSection != null) sections.Add(currentSection);
                    currentSection = new HtmlSection { Header = node.InnerText.Trim() };
                }
                else if (node.Name == "table" && currentSection != null)
                {
                    currentSection.Table = ExtractTable(node);
                }
                else if (node.NodeType == HtmlNodeType.Text && currentSection != null)
                {
                    string text = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        currentSection.TextContent += (currentSection.TextContent.Length > 0 ? " " : "") + text;
                    }
                }
            }

            if (currentSection != null) sections.Add(currentSection);
            
            return sections;
        }


        private static List<List<string>> ExtractTable(HtmlNode tableNode)
        {
            var table = new List<List<string>>();
            foreach (var row in tableNode.SelectNodes(".//tr") ?? new HtmlNodeCollection(null))
            {
                var rowData = row.SelectNodes(".//th | .//td")?.Select(cell => cell.InnerText.Trim()).ToList() ?? new List<string>();
                table.Add(rowData);
            }
            return table;
        }
    }
}
