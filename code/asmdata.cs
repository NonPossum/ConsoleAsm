using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAsm
{
    class asmdata
    {
        public static Dictionary<string, char[]> data = new Dictionary<string, char[]>();

        public void AddValue(string arg,string value)
        {
            data.Add(arg, value.ToCharArray());
        }
        public char[] GetValue(string arg)
        {
            char[] value = null;
            if (data.TryGetValue(arg, out value))
            {
                return value;
            }
            return null;
        }

    }
}
