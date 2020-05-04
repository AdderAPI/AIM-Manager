using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AIMAPI.Classes.Parsers
{
    public class SuccessParser
    {
        protected string _version = string.Empty;
        protected string _errorcode = string.Empty;
        protected string _errormsg = string.Empty;
        protected string _timestamp = string.Empty;
        protected bool _success = false;

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

        public string ErrorCode
        {
            get { return _errorcode; }
        }

        public string ErrorMessage
        {
            get { return _errormsg; }
        }

        public SuccessParser(string xml)
        {
            if (xml != string.Empty)
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

                    XmlNode error = node.SelectSingleNode("errors/error");
                    if (error != null)
                    {
                        _errorcode = error["code"].InnerText;
                        _errormsg = error["msg"].InnerText;
                    }
                }
            }
        }

        protected string GetNode(XmlNode node, string name)
        {
            if (node.SelectSingleNode(name) != null)
            {
                return node.SelectSingleNode(name).InnerText;
            }

            return "";
        }

        protected Boolean Str2Bool(String value)
        {
            if (value == "1") return true;
            return false;
        }

        protected int Str2Int(String value)
        {
            int x = 0;
            Int32.TryParse(value, out x);
            return x;
        }
    }
}
