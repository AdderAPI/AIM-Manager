using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AIMAPI.Classes.Data;
using AIMAPI.Classes.General;

namespace AIMAPI.Classes.Parsers
{
    public class CUSBParser : PageParser
    {
        public List<CUSB> CUSB = new List<CUSB>();

        public CUSBParser(string xml, CUSBNaming cusbnaming) : base(xml, cusbnaming)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("api_response");

            foreach (XmlNode node in nodes)
            {
                if (_errorcode == string.Empty)
                {
                    XmlNode cusbs = node.SelectSingleNode(cusbnaming.ContainerNode);
                    if (cusbs != null)
                    {
                        foreach (XmlNode cusbnode in cusbs)
                        {
                            CUSB cusb = new CUSB();

                            cusb.Xml = cusbnode.InnerXml;
                            cusb.APIVersion = _version;

                            cusb.Id = GetNode(cusbnode, "mac");
                            cusb.Name = GetNode(cusbnode, "name");
                            cusb.IPAddress = GetNode(cusbnode, "ip");
                            cusb.MacAddress = GetNode(cusbnode, "mac");
                            cusb.ConnectedTo = GetNode(cusbnode, "connectedTo");
                            cusb.Online = Str2Bool(GetNode(cusbnode, "online"));

                            string type = GetNode(cusbnode, "type");
                            if (type == "rx") cusb.Type = DeviceType.RX; else cusb.Type = DeviceType.TX;  

                            CUSB.Add(cusb);
                        }
                    }
                }
            }
        }
    }
}
