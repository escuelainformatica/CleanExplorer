using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanExplorer
{
    public class Element
    {
        public string Key { set; get; }
        public string SubKey { set; get; }
        public string Name { set; get; }
        public int Value { set; get; }

        public Element()
        {

        }

        public Element(string key, string subKey, string name, int value)
        {
            Key = key;
            SubKey = subKey;
            Name = name;
            Value = value;
        }
    }
}
