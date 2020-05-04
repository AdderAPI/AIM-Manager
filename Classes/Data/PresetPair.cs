using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.Data
{
    public class PresetPair
    {
        private string _rxid = string.Empty;
        private string _chid = string.Empty;

        public string Pair
        {
            get { return _chid + "-" + _rxid; }
        }

        public string ReceiverId
        {
            get { return _rxid; }
            set { _rxid = value; }
        }

        public string ChannelId
        {
            get { return _chid; }
            set { _chid = value; }
        }

        public PresetPair(string receiverid, string channelid)
        {
            _rxid = receiverid;
            _chid = channelid;
        }
    }
}
