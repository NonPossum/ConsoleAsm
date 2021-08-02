using System;

namespace ConsoleAsm
{
    class asmstack
    {
        public static int stacksize = 0x1024;

        private static int Top = 0;
        public static int[] stack = new int[stacksize];
        public static (int Start, int End) stackframe = (0, 0);
        public void Pop()
        {
            int top = 0;
            if (stackframe.End == 0)
            {
                top = Top;
            }
            else
            {
                top = Top + stackframe.End;
            }
            if (top > 0)
            {
                stack[top--] = 0;
            }
            else if (top == 0)
            {
                stack[top] = 0;
            }
        }
        public void Push(int x)
        {
            int top = 0;
            if (stackframe.End == 0)
            {
                top = Top;
            }
            else
            {
                top = Top + stackframe.End;
            }
            if (top == stacksize)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Stack Overflow");
                Console.ResetColor();
            }
            else if (top == 0)
            {
                stack[top++] = x;
            }
            else
            {
                stack[top] = x;
            }

        }

        public void InitialStackFrame()
        {
            // mov ebp, esp
            stackframe.Start = Top;

        }

        public void DestroyStackFrame()
        {
            // ret
            Array.Clear(stack, stackframe.Start, stackframe.Start - stackframe.End);
            stackframe = (0, 0);
        }
        public static void SetStackFrameValue(int ebp, int value)
        {
            if (ebp == 0) { ebp = 1 + stackframe.Start; }
            else { ebp = ebp + stackframe.Start; };
            stack[ebp] = value;
            stackframe.End++;

        }
        public void GetValueWithStackFrame()
        {

        }
        public void StackSize()
        {

            int i = 0;
            for (; i < stacksize; i++)
            {
                //If value is equals 0 ??
                if (stack[i] == 0)
                {
                    break;
                }
            }
            Console.WriteLine(i);
        }

    }

}