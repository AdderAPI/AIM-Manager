using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.Data
{
    [Flags]
    public enum ConnectionMode
    {
        Default = 0, 
        ViewOnly = 1,
        Shared = 2,
        Exclusive = 4, 
        Private = 8
    }
}
