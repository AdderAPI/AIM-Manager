using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.Data
{
    public class Channel : Base, IEquatable<Channel>
    {
        public string Location = "";
        public string Favourite = "";
        public bool ChannelOnline = false;

        public ViewType ViewButton = ViewType.Disabled;
        public ViewType SharedButton = ViewType.Disabled;
        public ViewType ExclusiveButton = ViewType.Disabled;
        public ViewType ControlButton = ViewType.Disabled;        // V4

        // V4        
        public string Video1_TXId = "";
        public string Video1_Head = "";
        public string Video2_TXId = "";
        public string Video2_Head = "";
        public string Audio_TXId = "";
        public string USB_TXId = "";
        public string Serial_TXId = "";
        public bool Sensitive = false;


        public bool Equals(Channel other)
        {
            if (other == null)
                return false;

            return
                Id == other.Id &&
                Name == other.Name &&
                Description == other.Description &&
                Location == other.Location &&
                Favourite == other.Favourite &&
                ViewButton == other.ViewButton &&
                SharedButton == other.SharedButton &&
                ExclusiveButton == other.ExclusiveButton &&

                //V4
                ControlButton == other.ControlButton &&
                ChannelOnline == other.ChannelOnline &&
                Video1_TXId == other.Video1_TXId &&
                Video1_Head == other.Video1_Head &&
                Video2_TXId == other.Video2_TXId &&
                Video2_Head == other.Video2_Head &&
                Sensitive == other.Sensitive;                
        }
    }
}
