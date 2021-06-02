using System;
using System.Collections;

namespace ConsoleAsm
{
    enum Registers
    {
        RAX,RCX,RDX, RBX,RSP,RBP,RSI,RDI,
        EAX,ECX,EDX,EBX,ESP,EBP,ESI,EDI,
        AX,AH,AL,CX,CH,CL,DX,DH,DL,BX,BH,BL,BP,BPL,SPL,SI,SIL,DI,DIL,NAI

    }
    class registers
    {
        public static byte bits = 64; 

        private static BitArray rax = new BitArray(bits);
        /*
        Values are returned from functions in this register. 
        scratch rax	eax	ax	ah and al
        */

        private static BitArray rcx = new BitArray(bits);
        /*
        Typical scratch register.  Some instructions also use it as a counter.	scratch
        rcx	ecx	cx	ch and cl
        */
        private static BitArray rdx = new BitArray(bits);
        /*
        Scratch register.	scratch
        rdx	edx	dx	dh and dl*/

        private static BitArray rbx = new BitArray(bits);
        /*
        Preserved register: don't use it without saving it!	preserved
        rbx	ebx	bx	bh and bl*/
        private static BitArray rsp = new BitArray(bits);
        /*
        The stack pointer.  Points to the top of the stack (details coming soon!)
        preserved	rsp	esp	sp	spl*/
        private static BitArray rbp = new BitArray(bits);
        /*
        Preserved register.  Sometimes used to store the old value of the stack pointer, or the "base".	preserved
        rbp	ebp	bp	bpl
        */
        private static BitArray rsi = new BitArray(bits);
        /*
        Scratch register. 
        rsi	esi	si	sil
        */
        private static BitArray rdi = new BitArray(bits);
        /*
        Scratch register.
        rdi	edi	di	dil
        */

        public void SetValue64(int value,Registers reg){

        }
         public void SetValue32(int value,Registers reg){

        }
         public void SetValue16(int value,Registers reg){

        }
         public void SetValue8(int value,Registers reg){

        }
/*
        public void Xor(Registers reg){

        }

        public void Mov(Registers reg){

        }
        public void Add(Registers reg){

        }
        public void Sub(Registers reg){

        }
        public void Mul(Registers reg){

        }
        */
        

    }
    

}
