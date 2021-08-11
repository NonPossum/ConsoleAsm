using System;
using System.Collections.Generic;

namespace ConsoleAsm
{
    class asmheap
    {

        public static Dictionary<int, int[]> heap = new Dictionary<int, int[]>();
        public int AddValue(int size)
        {
            Random num = new Random();
            int addr = num.Next(0x1000, 0x1000000);
            int[] value = new int[size];
            heap.Add(addr,value);
            return addr;
        }
        public int[] GetValue(int arg)
        {
            int[] value = null;
            if (heap.TryGetValue(arg, out value))
            {
                return value;
            }
            return null;
        }
        public void ReleaseValue(int arg)
        {
            heap.Remove(arg);
        }

    }
}