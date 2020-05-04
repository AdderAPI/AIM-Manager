using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AIMAPI.Classes.General;

namespace AIMAPI.Classes.Parsers
{
    public class PageParser : SuccessParser
    {
        protected int _page = -1;
        protected int _resultsperpage = -1;
        protected int _itemcount = -1;

        public int Page
        {
            get { return _page; }
        }

        public int ResultsPerPage
        {
            get { return _resultsperpage; }
        }

        public int ItemCount
        {
            get { return _itemcount; }
        }

        public PageParser(string xml, CUSBNaming cusbnaming) : base (xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("api_response");            

            foreach (XmlNode node in nodes)
            {                      
                if (GetNode(node, "page") != string.Empty)
                {
                    _page = Str2Int(GetNode(node, "page"));
                    _resultsperpage = Str2Int(GetNode(node, "results_per_page"));

                    if (GetNode(node, "count_devices") != "")
                    {
                        _itemcount = Str2Int(GetNode(node, "count_devices"));
                    }

                    if (GetNode(node, "count_channels") != "")
                    {
                        _itemcount = Str2Int(GetNode(node, "count_channels"));
                    }  

                    if (GetNode(node, "count_presets") != "")
                    {
                        _itemcount = Str2Int(GetNode(node, "count_presets"));
                    }                    
                }
                else if (cusbnaming != null)
                {
                    if (GetNode(node, cusbnaming.CountNode) != string.Empty)
                    {
                        _itemcount = Str2Int(GetNode(node, cusbnaming.CountNode));
                        _page = 1;
                        _resultsperpage = 100000;
                    }
                }

            }
        }

    }
}
