using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMAPI.Classes.Data;

namespace AIMAPI.Classes.General
{
    public class DeviceInfo
    {
        private DeviceType _devicetype = DeviceType.RX;
        private string _version = "";
        private string _variant = "";
        private string _model = "";
        private int _videoheads = 0;

        public DeviceType Type
        {
            get { return _devicetype; }
        }

        public string Version
        {
            get { return _version; }
        }

        public string Variant
        {
            get { return _variant; }
        }

        public string Model
        {
            get { return _model; }
        }

        public int VideoHeads
        {
            get { return _videoheads; }
        }

        public DeviceInfo(DeviceType type, string version, string variant)
        {
            if (version == "2")
            {
                if (type == DeviceType.TX)
                {
                    switch (variant.ToLower())
                    {
                        case "": _model = "ALIF2000T"; _videoheads = 2; break;
                        case "b": _model = "ALIF2002T"; _videoheads = 2; break;
                        case "s": _model = "ALIF1002T"; _videoheads = 1; break;
                        case "t": _model = "ALIF2020T"; _videoheads = 2; break;
                        case "v": _model = "ALIF2112T"; _videoheads = 2; break;
                        default: _model = "Unknown"; _videoheads = 1; break;                            
                    }
                }
                if (type == DeviceType.RX)
                {
                    switch (variant.ToLower())
                    {
                        case "": _model = "ALIF2000R"; _videoheads = 2; break;
                        case "s": _model = "ALIF1002R"; _videoheads = 1; break;
                        case "t": _model = "ALIF2020R"; _videoheads = 2; break;
                        default: _model = "Unknown"; _videoheads = 1; break;
                    }
                }
            }

            else if (version == "")
            {
                if (type == DeviceType.TX)
                {
                    _model = "ALIF1000T";
                    _videoheads = 1;
                }

                if (type == DeviceType.RX)
                {
                    _model = "ALIF1000R";
                    _videoheads = 1;
                }
            }
        }

        public override string ToString()
        {
            return _model;
        }
    }
}
