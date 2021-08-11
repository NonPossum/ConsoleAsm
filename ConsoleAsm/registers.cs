using System;
using System.Collections;

namespace ConsoleAsm
{
    class registers
    {

        public static byte bits = 64;

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

        public static BitArray eax = new BitArray(bits);
        /*
        Values are returned from functions in this register. 
        scratch rax	eax	ax	ah and al
        */

        public static BitArray ecx = new BitArray(bits);
        /*
        Typical scratch register.  Some instructions also use it as a counter.	scratch
        rcx	ecx	cx	ch and cl
        */
        public static BitArray edx = new BitArray(bits);
        /*
        Scratch register.	scratch
        rdx	edx	dx	dh and dl*/

        public static BitArray ebx = new BitArray(bits);
        /*
        Preserved register: don't use it without saving it!	preserved
        rbx	ebx	bx	bh and bl*/
        public static BitArray esp = new BitArray(bits);
        /*
        The stack pointer.  Points to the top of the stack (details coming soon!)
        preserved	rsp	esp	sp	spl*/
        public static BitArray ebp = new BitArray(bits);
        /*
        Preserved register.  Sometimes used to store the old value of the stack pointer, or the "base".	preserved
        rbp	ebp	bp	bpl
        */
        public static BitArray esi = new BitArray(bits);
        /*
        Scratch register. 
        rsi	esi	si	sil
        */
        public static BitArray edi = new BitArray(bits);
        /*
        Scratch register.
        rdi	edi	di	dil
        */

        public void SetValue(BitArray reg, int bits)
        {

        }


    }


}
