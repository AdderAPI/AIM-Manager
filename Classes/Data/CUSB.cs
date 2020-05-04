using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.Data
{
    public class CUSB : Base, IEquatable<CUSB>
    {
        public string MacAddress = string.Empty;
        public DeviceType Type = DeviceType.RX;

        public bool Online = false;
        public string IPAddress = string.Empty;

        public string ConnectedTo = string.Empty;

        public bool Equals(CUSB other)
        {
            if (other == null)
                return false;

            return
                MacAddress == other.MacAddress &&
                Name == other.Name &&
                Type == other.Type &&
                Online == other.Online &&
                IPAddress == other.IPAddress &&
                ConnectedTo == other.ConnectedTo;
        }
    }
}
