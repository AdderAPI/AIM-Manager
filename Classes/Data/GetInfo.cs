using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.Data
{
    [Flags]
    public enum Fetch
    {
        All = 0,
        Receivers = 1,
        Transmitters = 2,
        Channels = 4,
        Presets = 8,
        CUSBLan = 10
    }    
}
