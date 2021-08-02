using System;
using System.Collections;

namespace ConsoleAsm
{
    class registers
    {

        public static byte bits = 64;

        public static BitArray flagReg = new BitArray(32);
        // cf pf af zf sf of nt rf vip id...

        public static BitArray rax = new BitArray(bits);
        /*
        Values are returned from functions in this register. 
        scratch rax	eax	ax	ah and al
        */

        public static BitArray rcx = new BitArray(bits);
        /*
        Typical scratch register.  Some instructions also use it as a counter.	scratch
        rcx	ecx	cx	ch and cl
        */
        public static BitArray rdx = new BitArray(bits);
        /*
        Scratch register.	scratch
        rdx	edx	dx	dh and dl*/

        public static BitArray rbx = new BitArray(bits);
        /*
        Preserved register: don't use it without saving it!	preserved
        rbx	ebx	bx	bh and bl*/
        public static BitArray rsp = new BitArray(bits);
        /*
        The stack pointer.  Points to the top of the stack (details coming soon!)
        preserved	rsp	esp	sp	spl*/
        public static BitArray rbp = new BitArray(bits);
        /*
        Preserved register.  Sometimes used to store the old value of the stack pointer, or the "base".	preserved
        rbp	ebp	bp	bpl
        */
        public static BitArray rsi = new BitArray(bits);
        /*
        Scratch register. 
        rsi	esi	si	sil
        */
        public static BitArray rdi = new BitArray(bits);
        /*
        Scratch register.
        rdi	edi	di	dil
        */

        public void SetValue(BitArray reg, int bits)
        {

        }


    }


}
