using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleAsm
{
    class asmstack
    {
        public static int stacksize = 0x1024;
        private static int instr = 0;
        private static int stackframeaddr = 0;
        private static List<int[]> stack = new List<int[]>();

        public asmstack()
        {
            stack.Add(new int[stacksize]);
        }


        public static void Pop()
        {
            stack[0][instr] = 0;
        }
        public static void Push(int x)
        {
            stack[0][instr] = x;
            instr = instr + 1;

        }
        public static void PushEbp()
        {
            Push(stackframeaddr);
        }
        public static void MovEbpEsp()
        {
            stackframeaddr = stackframeaddr + 1;

        }
        public static void SubEsp(int value)
        {
            int[] newstackframe = new int[value];
            newstackframe[1] = 34;
            newstackframe[2] = 24;
            newstackframe[3] = 11;
            stack.Add(newstackframe);
            Random rnd = new Random();
            int addr = rnd.Next(1000, 1000000);

            assembler.memory.Add(addr, newstackframe);


            BitArray adr = new BitArray(new int[] { addr });

            for (int i = 0; i < 32; i++)
            {
                registers.ebp[i] = adr[i];

            }


            BitArray size = new BitArray(new int[] { value });

            for (int i = 0; i < 32; i++)
            {
                registers.esp[i] = size[i];

            }


        }
        public static int EbpAddressing(int value)
        {
            int ptr = 0;

            for (int i = 0; i < 32; i++)
            { ptr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }


            return (ptr + value);
        }
        public static void Leave()
        {
            stack.Remove(stack[stackframeaddr]);
        }
        public static void Show()
        {
            for (int i = 0; i < stack[0].Length; i++)
            {
                Console.WriteLine(stack[0][i]);
            }
        }

    }

}
