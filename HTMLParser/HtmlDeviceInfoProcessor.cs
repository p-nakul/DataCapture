using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCapture.Models;
using HtmlAgilityPack;

namespace DataCapture.HTMLParser
{
    internal class HtmlDeviceInfoProcessor: HtmlProcessor
    {
        private Dictionary<string, string> keyValues = new Dictionary<string, string>();
        private List<HtmlSection> Sections { get; set; } = new();
        public HtmlDeviceInfoProcessor(HtmlDocument doc) : base(doc) { }

        string[] excludedKeys = { "System Shared Libraries", "Supported Locales", "Supported OpenGL Extensions", "System Available Features", "Recent Successful Android Device Configuration Service Connections", "Recent Failed Android Device Configuration Service Connections" };

        public override HtmlFile ExtractData(HtmlFile hf)
        {
            keyValues.Clear();
            Sections.Clear();
            try
            {
                ExtractKeyValues();
                ExtractListData();
            }
            catch (Exception ex) { 
            }


            
            hf.KeyValues = keyValues;
            hf.Sections = Sections;
            // Extract specific data logic for Type B
            return hf;
        }

        private void ExtractKeyValues()
        {
            // Find all <h3> elements with class "category-title"
            var headings = Document.DocumentNode.SelectNodes("//h3[@class='category-title']");

            if (headings != null)
            {
                foreach (var heading in headings)
                {
                    Console.WriteLine($"Category: {heading.InnerText.Trim()}");

                    // Extract the following text nodes
                    var extractedTexts = new List<string>();
                    var node = heading.NextSibling;

                    while (node != null)
                    {
                        if (node.NodeType == HtmlNodeType.Text)
                        {
                            string text = node.InnerText.Trim();
                            if (!string.IsNullOrEmpty(text))
                            {
                                string[] key_values = text.Split(':');
                                string value = key_values.Length > 1 ? string.Join(":", key_values, 1, key_values.Length - 1) : "";
                                
                                if (!value.Trim().Equals(""))
                                {
                                    if (!excludedKeys.Contains(key_values[0]))
                                    {
                                        
                                        keyValues[key_values[0]] = value;
                                    }
                                }

                            }
                        }
                        else if (node.Name == "br")
                        {
                            node = node.NextSibling; // Skip <br> and continue
                            continue;
                        }
                        else
                        {
                            break; // Stop when a new non-text element appears
                        }

                        node = node.NextSibling;
                    }

                    Console.WriteLine("Extracted Data:");
                    foreach (var text in extractedTexts)
                    {
                        Console.WriteLine($"- {text}");
                    }
                    Console.WriteLine("-------------------");
                }
            }
            else
            {
                Console.WriteLine("No category titles found.");
            }
        }
        

        private void ExtractListData()
        {
            //var sections = new List<HtmlSection>();

            // Select all <h3> elements (without class)
            foreach (var h3 in Document.DocumentNode.SelectNodes("//h3") ?? new HtmlNodeCollection(null))
            {
                var section = new HtmlSection { Header = h3.InnerText.Trim() };

                // Find the table that is the next sibling of <h3>
                var table = h3.SelectSingleNode("following-sibling::table");
                if (table != null)
                {
                    foreach (var row in table.SelectNodes(".//tr") ?? new HtmlNodeCollection(null))
                    {
                        var rowData = new List<string>();
                        foreach (var cell in row.SelectNodes("./th|./td") ?? new HtmlNodeCollection(null))
                        {
                            rowData.Add(cell.InnerText.Trim());
                        }
                        section.Table.Add(rowData);
                    }
                }

                Sections.Add(section);
            }


        }
    
    
    }
}
