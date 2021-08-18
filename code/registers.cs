using System.Collections;

namespace ConsoleAsm
{
    class registers
    {

        public static byte bits = 32;

        public static BitArray eflags = new BitArray(32);
        // 0 = CF
        // 1 = ? 
        // 2 = PF
        // 3 = ? 
        // 4 = AF
        // 5 = ?
        // 6 = ZF
        // 7 = SF
        // 8 = TF
        // 9 = IF
        // 10 = DF
        // 11 = OF
        // 12-13 - IOPL
        // 14 = NT
        // 15 = ?
        // 16 = RF
        // 17 = VM
        // 18 = AC
        // 19 = VIF
        // 20 = VIP
        // 21 = ID
        // 22-31 = ? 

        public registers()
        {
            eflags.SetAll(false);
            eflags.Set(1,true);
        }

        public static BitArray eax = new BitArray(bits);

        public static BitArray ecx = new BitArray(bits);

        public static BitArray edx = new BitArray(bits);

        public static BitArray ebx = new BitArray(bits);

        public static BitArray esp = new BitArray(bits);

        public static BitArray ebp = new BitArray(bits);

        public static BitArray esi = new BitArray(bits);

        public static BitArray edi = new BitArray(bits);

    }


}
