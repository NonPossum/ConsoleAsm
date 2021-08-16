using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace ConsoleAsm
{

    class instructions
    {

        public instructions()
        {


        }
        public static void Jump(int index)
        {
            lineofcode ri = new lineofcode();

            uint uindex = (uint)index + 1;

            for (int i = 0; i < assembler.code.Count; i++)
            {
                ri = assembler.code[uindex];
                uindex = uindex + 1;

                if (ri.asminstruction == "ret")
                {
                    break;
                }

                assembler.Parse(ri);
                                
            }


        }


        //C functions
        public static void malloc()
        {
            int size = 0;
            for (int i = 0; i < 32; i++)
            { size |= (Convert.ToByte(registers.eax.Get(i)) << i); }

            int[] malloc = new int[size];
            Random rnd = new Random();
            int addr = rnd.Next(1000, 1000000);

            assembler.memory.Add(addr, malloc);



            BitArray adr = new BitArray(new int[] { addr });

            for (int i = 0; i < 32; i++)
            {
                registers.ebp[i] = adr[i];

            }


            BitArray value = new BitArray(new int[] { size });

            for (int i = 0; i < 32; i++)
            {
                registers.esp[i] = value[i];



            }
        }

        public static void free()
        {
            int addr = 0;
            for (int i = 0; i < 32; i++)
            { addr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

            assembler.memory.Remove(addr);
            registers.ebp.SetAll(false);
            registers.esp.SetAll(false);
        }

        public static void Call(string method)
        {
            switch (method)
            {
                case "malloc":
                    malloc();
                    break;
                case "free":
                    free();
                    break;
                default:
                    break;
            }
        }
        public static void Pop(BitArray arg0, int bits)
        {
            asmstack.Pop();
        }
        public static void Pop(int arg0, int bits)
        {
            asmstack.Pop();

        }
        public static void Push(BitArray arg0, int bits)
        {
            int reg = 0;
            for (int i = 0; i < bits; i++)
            { reg |= (Convert.ToByte(arg0.Get(i)) << i); }

            asmstack.Push(reg);
        }
        public static void Push(int arg0, int bits)
        {
            asmstack.Push(arg0);
        }
        public static void Not(BitArray arg0, int bits)
        {
            BitArray reg1 = new BitArray(new int[] { bits });

            for (int i = 0; i < bits; i++)
            {
                arg0[i] = !arg0[i];

            }

        }
        public static void Jmp(BitArray arg0, int bits)
        {
            int ax = 0;
            for (int i = 0; i < bits; i++)
            { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

            Jump(ax);
        }
        public static void Jmp(int arg0, int bits)
        {
            Jump(arg0);
        }
        public static void Mul(BitArray arg0, int bits)
        {
            int ax = 0;
            for (int i = 0; i < 16; i++)
            { ax |= (Convert.ToByte(registers.eax.Get(i)) << i); }


            int value = 0;

            for (int i = 0; i < 16; i++)
            { value |= (Convert.ToByte(arg0.Get(i)) << i); }

            ax *= value;

            BitArray bitax = new BitArray(new int[] { ax });

            for (int i = 0; i < 16; i++)
            {
                registers.eax[i] = bitax[i];

            }

        }
        public static void Mul(int arg0, int bits)
        {
            int ax = 0;
            for (int i = 0; i < 16; i++)
            { ax |= (Convert.ToByte(registers.eax.Get(i)) << i); }

            ax *= arg0;

            BitArray bitax = new BitArray(new int[] { ax });

            for (int i = 0; i < 16; i++)
            {
                registers.eax[i] = bitax[i]; 

            }
             


        }
        public static void Inc(BitArray arg0, int bits)
        {
            long value = 0;
            for (int i = 0; i < bits; i++)
                { value |= (Convert.ToByte(arg0.Get(i)) << i); }

            BitArray bit = new BitArray(BitConverter.GetBytes(value + 1));

            for (int i = 0; i < bits; i++)
            {
                arg0[i] = bit[i];

            }


        }
        public static void Div(BitArray arg0, int bits)
        {
            int reg = 0;
            for (int i = 0; i < bits; i++)
            { reg |= (Convert.ToByte(arg0.Get(i)) << i); }

            int remainder = 0;
            int edi = 0;

            for (int i = 0; i < 32; i++)
            { edi |= (Convert.ToByte(registers.edi.Get(i)) << i); }

            Math.DivRem(edi, reg, out remainder);

            int eax = 0;
            for (int i = 0; i < 32; i++)
            { eax |= (Convert.ToByte(registers.eax.Get(i)) << i); }

            eax = (int)(eax / reg);

            BitArray bitax = new BitArray(new int[] { eax });

            for (int i = 0; i < 32; i++)
            {
                registers.eax[i] = bitax[i];
            }

            BitArray remnd = new BitArray(new int[] { remainder });


            for (int i = 0; i < 32; i++)
            {
                registers.edi[i] = remnd[i];
            }


        }
        public static void Div(int arg0, int bits)
        {
            int remainder = 0;
            int edi = 0;

            for (int i = 0; i < 32; i++)
            { edi |= (Convert.ToByte(registers.edi.Get(i)) << i); }

            Math.DivRem(edi, arg0, out remainder);

            int eax = 0;
            for (int i = 0; i < 32; i++)
            { eax |= (Convert.ToByte(registers.eax.Get(i)) << i); }

            eax = (int)(eax / arg0);

            BitArray bitax = new BitArray(new int[] { eax });

            for (int i = 0; i < 32; i++)
            {
                registers.eax[i] = bitax[i];
            }

            BitArray remnd = new BitArray(new int[] { remainder });


            for (int i = 0; i < 32; i++)
            {
                registers.edi[i] = remnd[i];
            }




        }
        public static void Dec(BitArray arg0, int bits)
        {
            long value = 0;
            for (int i = 0; i < bits; i++)
            { value |= (Convert.ToByte(arg0.Get(i)) << i); }

            BitArray bit = new BitArray(BitConverter.GetBytes(value - 1));

            for (int i = 0; i < bits; i++)
            {
                arg0[i] = bit[i];

            }


        }

        public static void Ja(BitArray arg0, int bits)
        {

        }
        public static void Ja(int arg0, int bits)
        {

        }
        public static void Jae(BitArray arg0, int bits)
        {

        }
        public static void Jae(int arg0, int bits)
        {

        }
        public static void Jb(BitArray arg0, int bits)
        {

        }
        public static void Jb(int arg0, int bits)
        {

        }
        public static void Jc(BitArray arg0, int bits)
        {

        }
        public static void Jc(int arg0, int bits)
        {

        }
        public static void Je(BitArray arg0, int bits)
        {

        }
        public static void Je(int arg0, int bits)
        {

        }
        public static void Jg(BitArray arg0, int bits)
        {

        }
        public static void Jg(int arg0, int bits)
        {

        }
        public static void Jge(BitArray arg0, int bits)
        {

        }
        public static void Jge(int arg0, int bits)
        {

        }
        public static void Jl(BitArray arg0, int bits)
        {

        }
        public static void Jl(int arg0, int bits)
        {

        }
        public static void Jle(BitArray arg0, int bits)
        {

        }
        public static void Jle(int arg0, int bits)
        {

        }
        public static void Jna(BitArray arg0, int bits)
        {

        }
        public static void Jna(int arg0, int bits)
        {

        }
        public static void Jnae(BitArray arg0, int bits)
        {

        }
        public static void Jnae(int arg0, int bits)
        {

        }
        public static void Jnb(BitArray arg0, int bits)
        {

        }
        public static void Jnb(int arg0, int bits)
        {

        }
        public static void Jnbe(BitArray arg0, int bits)
        {

        }
        public static void Jnbe(int arg0, int bits)
        {

        }
        public static void Jnc(BitArray arg0, int bits)
        {

        }
        public static void Jnc(int arg0, int bits)
        {

        }
        public static void Jne(BitArray arg0, int bits)
        {

        }
        public static void Jne(int arg0, int bits)
        {

        }
        public static void Jng(BitArray arg0, int bits)
        {

        }
        public static void Jng(int arg0, int bits)
        {

        }
        public static void Jnge(BitArray arg0, int bits)
        {

        }
        public static void Jnge(int arg0, int bits)
        {

        }
        public static void Jno(BitArray arg0, int bits)
        {

        }
        public static void Jno(int arg0, int bits)
        {

        }
        public static void Jnle(BitArray arg0, int bits)
        {

        }
        public static void Jnle(int arg0, int bits)
        {

        }
        public static void Jnp(BitArray arg0, int bits)
        {

        }
        public static void Jnp(int arg0, int bits)
        {

        }
        public static void Jns(BitArray arg0, int bits)
        {

        }
        public static void Jns(int arg0, int bits)
        {

        }
        public static void Jnz(BitArray arg0, int bits)
        {

        }
        public static void Jnz(int arg0, int bits)
        {

        }
        public static void Jo(BitArray arg0, int bits)
        {

        }
        public static void Jo(int arg0, int bits)
        {

        }
        public static void Jp(BitArray arg0, int bits)
        {

        }
        public static void Jp(int arg0, int bits)
        {

        }
        public static void Jpe(BitArray arg0, int bits)
        {

        }
        public static void Jpe(int arg0, int bits)
        {

        }
        public static void Jpo(BitArray arg0, int bits)
        {

        }
        public static void Jpo(int arg0, int bits)
        {

        }
        public static void Js(BitArray arg0, int bits)
        {

        }
        public static void Js(int arg0, int bits)
        {

        }
        public static void Jz(BitArray arg0, int bits)
        {

        }
        public static void Jz(int arg0, int bits)
        {

        }

        public static void Add(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            int reg0 = 0;
            for (int i = 0; i < 32; i++)
            { reg0 |= (Convert.ToByte(arg0.Get(i)) << i); }

            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

            reg0 = reg0 + reg1;

            if (reg0 == 0)
            {
                registers.eflags.Set(6, false);//zf
            }
            else
            {
                registers.eflags.Set(6, true);//zf

            }
            if (reg0 < 0)
            {
                registers.eflags.Set(7, false);//sf

            }
            else
            {
                registers.eflags.Set(7, true);//sf
            }


            BitArray remnd = new BitArray(new int[] { reg0 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = remnd[i];

            }
        }

        public static void Add(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Add(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }


        public static void And(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg0[i] & arg1[i];

            }

        }

        public static void And(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void And(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void And(int arg0, int arg1, int bits0, int bits1)
        {

        }

        public static void Cmp(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            int reg0 = 0;
            for (int i = 0; i < 32; i++)
            { reg0 |= (Convert.ToByte(arg0.Get(i)) << i); }

            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

            reg0 = reg0 - reg1;

            if (reg0 == 0)
            {
                registers.eflags.Set(6, false);//zf
            }
            else
            {
                registers.eflags.Set(6, true);//zf

            }

            if (reg0 < 0)
            {
                registers.eflags.Set(7, false);//sf

            }
            else
            {
                registers.eflags.Set(7, true);//sf
            }

        }

        public static void Cmp(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Cmp(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Cmp(int arg0, int arg1, int bits0, int bits1)
        {

        }

        public static void Lds(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Lds(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Lds(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Lds(int arg0, int arg1, int bits0, int bits1)
        {

        }

        public static void Lea(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Lea(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Lea(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Lea(int arg0, int arg1, int bits0, int bits1)
        {

        }

        public static void Les(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Les(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Les(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Les(int arg0, int arg1, int bits0, int bits1)
        {

        }

        public static void Mov(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg1[i];

            }
        }
        public static void Mov(int arg0, int arg1, int bits0, int bits1)
        {
            int iadr = 0;
            for (int i = 0; i < 32; i++)
            { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

            assembler.memory[iadr][arg0] = arg1;
        }


        public static void Mov(BitArray arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int iadr = 0;
                for (int i = 0; i < 32; i++)
                { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

                int value = assembler.memory[iadr][arg1];

                BitArray newarg0 = new BitArray(new int[] { value });

                for (int i = 0; i < bits0; i++)
                {
                    arg0[i] = newarg0[i];

                }

            }
            else
            {
                BitArray newarg1 = new BitArray(new int[] { arg1 });

                for (int i = 0; i < bits0; i++)
                {
                    arg0[i] = newarg1[i];

                }

            }




        }
        public static void Mov(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int iadr = 0;
                for (int i = 0; i < 32; i++)
                { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

                int reg = 0;
                for (int i = 0; i < bits1; i++)
                { reg |= (Convert.ToByte(arg1.Get(i)) << i); }

                assembler.memory[iadr][arg0] = reg;

            }
            else
            {
                return;

            }

        }

        public static void Or(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Or(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Or(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Or(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Rcr(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Rcr(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Rcr(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Rcr(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Rcl(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Rcl(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Rcl(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Rcl(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Rol(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Rol(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Rol(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Rol(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Ror(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Ror(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Ror(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Ror(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Shl(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Shl(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Shl(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Shl(int arg0, int arg1, int bits0, int bits1)
        {

        }



        public static void Xor(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

            for (int i = 0; i < bits0; i++)
            {
                arg0[i] ^= arg1[i];
                if (i > bits1)
                {
                    break;

                }
            }

        }

        public static void Xor(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Xor(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Xor(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Sub(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Sub(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Sub(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Sub(int arg0, int arg1, int bits0, int bits1)
        {

        }


        public static void Test(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Test(BitArray arg0, int arg1, int bits0, int bits1)
        {

        }
        public static void Test(int arg0, BitArray arg1, int bits0, int bits1)
        {

        }

        public static void Test(int arg0, int arg1, int bits0, int bits1)
        {

        }

        public static long GetFromBitArray(BitArray bitArray)
        {
            var array = new byte[8];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt64(array, 0);
        }


    }
}