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
        public HtmlDeviceInfoProcessor(HtmlDocument doc) : base(doc) { }

        public override HtmlFile ExtractData(HtmlFile hf)
        {
            Console.WriteLine("Processing HTML Type B...");
            // Extract specific data logic for Type B
            return null;
        }
    }
}
