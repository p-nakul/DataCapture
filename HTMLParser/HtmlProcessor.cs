using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCapture.Models;
using HtmlAgilityPack;

namespace DataCapture.HTMLParser
{
    abstract class HtmlProcessor
    {
        protected HtmlDocument Document { get; }

        public HtmlProcessor(HtmlDocument doc)
        {
            Document = doc;
        }

        public abstract HtmlFile ExtractData(HtmlFile hf);
    }
}
