using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AIMAPI.Classes.Data;

namespace AIMAPI.Classes.Parsers
{
    public class ChannelParser : PageParser
    {
        public List<Channel> Channels = new List<Channel>();

        public  ChannelParser(string xml) : base(xml, null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList nodes = doc.GetElementsByTagName("api_response");

            foreach (XmlNode node in nodes)
            {
                if (_errorcode == string.Empty)
                {
                    XmlNode channels = node.SelectSingleNode("channels");
                    if (channels != null)
                    {
                        foreach (XmlNode channelnode in channels)
                        {
                            Channel channel = new Channel();

                            channel.Xml = channelnode.InnerXml;
                            channel.APIVersion = _version;

                            channel.Id = GetNode(channelnode, "c_id");
                            channel.Name = GetNode(channelnode, "c_name");
                            
                            channel.Description = GetNode(channelnode, "c_description");
                            channel.Location = GetNode(channelnode, "c_location");
                            channel.Favourite = GetNode(channelnode, "c_favourite");

                            channel.ChannelOnline = Str2Bool(GetNode(channelnode, "channel_online"));
                            
                            channel.ViewButton = GetViewType(GetNode(channelnode, "view_button"));
                            channel.SharedButton = GetViewType(GetNode(channelnode, "shared_button"));
                            channel.ExclusiveButton = GetViewType(GetNode(channelnode, "exclusive_button"));                          

                            //V4                            
                            channel.Video1_TXId = GetNode(channelnode, "c_video1");
                            channel.Video1_Head = GetNode(channelnode, "c_video1_head");
                            channel.Video2_TXId = GetNode(channelnode, "c_video2");
                            channel.Video2_Head = GetNode(channelnode, "c_video2_head");

                            channel.Audio_TXId = GetNode(channelnode, "c_audio");
                            channel.USB_TXId = GetNode(channelnode, "c_usb");
                            channel.Serial_TXId = GetNode(channelnode, "c_serial ");
                            channel.Sensitive = Str2Bool(GetNode(channelnode, "c_serial "));
                            channel.ControlButton = GetViewType(GetNode(channelnode, "control_button"));

                            Channels.Add(channel);
                        }
                    }
                }
            }
        }

        private ViewType GetViewType(String value)
        {
            value = value.ToLower();
            ViewType result = ViewType.Disabled;
            if (value == "disabled") result = ViewType.Disabled;
            if (value == "enabled") result = ViewType.Enabled;
            if (value == "hidden") result = ViewType.Hidden;
            return result;
        }
    }
}
