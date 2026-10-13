using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginLiveMarkdown
{
    public static class Style
    {
        public enum Color : byte
        {
            Normal = 0,
            Header = 5,
            Register = 3,
            Net = 9,
            Paramater = 7,
            Keyword = 4,
            Identifier = 6,
            Number = 8
        }
    }
}
