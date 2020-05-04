using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMAPI.Classes.General
{
    public class Useful
    {
        public static int Str2Int(String value)
        {
            int x = 0;
            Int32.TryParse(value, out x);
            return x;
        }
    }
}
