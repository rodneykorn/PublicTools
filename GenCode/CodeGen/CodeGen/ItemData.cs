using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen
{
    class ItemData
    {
        public string Class { get; set; }
        public string Property { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool MultiLine { get; set; }
    }
}
