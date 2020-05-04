using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AIMAPI.Classes.Parsers
{
    public class LoginParser
    {
        private string _version;
        private string _timestamp;
        private string _token;
        private bool _success = false;

        public bool Success
        {
            get { return _success; }
        }

        public string Version
        {
            get { return _version; }
        }

        public string TimeStamp
        {
            get { return _timestamp; }
        }

        public string Token
        {
            get { return _token; }
        }


        public LoginParser(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("api_response");            

            foreach (XmlNode node in nodes)
            {                                
                _version = node.SelectSingleNode("version").InnerText;
                _timestamp = node.SelectSingleNode("timestamp").InnerText;

                string success = node.SelectSingleNode("success").InnerText;

                if (success == "0")
                    _success = false;
                else
                    _success = true;

                _token = node.SelectSingleNode("token").InnerText;
             
            }
        }
    }
}
