using System;
using System.Collections;

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
        private static void malloc()
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

        private static void free()
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

        private static void SetAF(BitArray reg)
        {
            if (reg.Get(3) && reg.Get(3))
            {
                registers.eflags.Set(4, true);//af

            }
            else
            {
                registers.eflags.Set(4, false);//af

            }
        }
        private static void SetZF(int value)
        {
            if (value == 0)
            {
                registers.eflags.Set(6, true);//zf
            }
            else
            {
                registers.eflags.Set(6, false);//zf

            }
        }
        private static void SetOF(int value,int bits)
        {
            switch (bits)
            {
                case 32:
                    if (value > Int32.MaxValue)
                    {
                        registers.eflags.Set(11, true);//of
                    }
                    break;
                case 16:
                    if (value > Int16.MaxValue)
                    {
                        registers.eflags.Set(11, true);//of
                    }
                    break;
                case 8:
                    if (value > char.MaxValue)
                    {
                        registers.eflags.Set(11, true);//of
                    }
                    break;
                default:
                    registers.eflags.Set(11, false);//of
                    break;
            }
        }
        private static void SetCF(int value, int bits)
        {
            switch (bits)
            {
                case 32:
                    if (value > Int32.MaxValue)
                    {
                        registers.eflags.Set(0, true);//cf
                    }
                    break;
                case 16:
                    if (value > Int16.MaxValue)
                    {
                        registers.eflags.Set(0, true);//cf
                    }
                    break;
                case 8:
                    if (value > char.MaxValue)
                    {
                        registers.eflags.Set(0, true);//cf
                    }
                    break;
                default:
                    registers.eflags.Set(0, false);//cf
                    break;
            }
        }

        private static void SetPF(int value)
        {
            if (value % 2 == 0)
            {
                registers.eflags.Set(2, true);//pf

            }
            else
            {
                registers.eflags.Set(2, false);//pf
            }
        }
        private static void SetSF(int value)
        {
            if (value < 0)
            {
                registers.eflags.Set(7, true);//sf

            }
            else
            {
                registers.eflags.Set(7, false);//sf
            }

        }

        private static void SetDF()
        {

        }


        private static void SetTF()
        {

        }

        private static void SetIF()
        {

        }
        private static void SetIOPL()
        {

        }
        private static void SetNT()
        {

        }

        private static int GetValAddr(int arg0)
        {
            int iadr = 0;
            for (int i = 0; i < 32; i++)
            { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

            return assembler.memory[iadr][arg0];
        }

        private static void SetValAddr(int value,int offset)
        {
            int iadr = 0;
            for (int i = 0; i < 32; i++)
            { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

            assembler.memory[iadr][offset] = value;
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

            if (bitax.Get(bits))
            {
                registers.eflags.Set(0, true);//cf
                registers.eflags.Set(11, true);//of
            }
            else
            {
                registers.eflags.Set(0, false);//cf
                registers.eflags.Set(11, false);//of
            }

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

            if (bitax.Get(bits))
            {
                registers.eflags.Set(0, true);//cf
                registers.eflags.Set(11, true);//of
            }
            else
            {
                registers.eflags.Set(0, false);//cf
                registers.eflags.Set(11, false);//of
            }
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

            int set = 0;
            for (int i = 0; i < bits; i++)
            { set |= (Convert.ToByte(bit.Get(i)) << i); }

            SetOF(set, bits);
            SetSF(set);
            SetZF(set);
            SetAF(bit);
            SetPF(set);

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
            int value = 0;
            for (int i = 0; i < bits; i++)
            { value |= (Convert.ToByte(arg0.Get(i)) << i); }

            BitArray bit = new BitArray(BitConverter.GetBytes(value - 1));

            int set = 0;
            for (int i = 0; i < bits; i++)
            { set |= (Convert.ToByte(bit.Get(i)) << i); }

            SetOF(set, bits);
            SetSF(set);
            SetZF(set);
            SetAF(bit);
            SetPF(set);


            for (int i = 0; i < bits; i++)
            {
                arg0[i] = bit[i];

            }
        }

        public static void Jc(BitArray arg0, int bits)
        {
            if (registers.eflags.Get(0))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }

        }
        public static void Jc(int arg0, int bits)
        {
            if (registers.eflags.Get(0))
            {
                Jump(arg0);
            }
        }
        public static void Je(BitArray arg0, int bits)
        {
            if (registers.eflags.Get(6))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Je(int arg0, int bits)
        {
            if (registers.eflags.Get(6))
            {
                Jump(arg0);
            }
        }
        public static void Jg(BitArray arg0, int bits)
        {
            if (!registers.eflags.Get(6) && registers.eflags.Get(7) == registers.eflags.Get(11))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jg(int arg0, int bits)
        {
            if (!registers.eflags.Get(6) && registers.eflags.Get(7) == registers.eflags.Get(11))
            {
                Jump(arg0);
            }
        }
        public static void Jge(BitArray arg0, int bits)
        {
            if (registers.eflags.Get(7) == registers.eflags.Get(11))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jge(int arg0, int bits)
        {
            if (registers.eflags.Get(7) == registers.eflags.Get(11))
            {
                Jump(arg0);
            }
        }
        public static void Jl(BitArray arg0, int bits)
        {
            if (registers.eflags.Get(6) != registers.eflags.Get(11))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jl(int arg0, int bits)
        {
            if (registers.eflags.Get(6) != registers.eflags.Get(11))
            {
                Jump(arg0);
            }
        }
        public static void Jle(BitArray arg0, int bits)
        {
            if (registers.eflags.Get(6) || registers.eflags.Get(6) != registers.eflags.Get(11))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jle(int arg0, int bits)
        {
            if (registers.eflags.Get(6) || registers.eflags.Get(6) != registers.eflags.Get(11))
            {
                Jump(arg0);
            }
          
        }

        public static void Jne(BitArray arg0, int bits)
        {
            if (!registers.eflags.Get(6))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jne(int arg0, int bits)
        {
            if (!registers.eflags.Get(6))
            {
                Jump(arg0);
            }
        }
        public static void Jno(BitArray arg0, int bits)
        {
            if (!registers.eflags.Get(11))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jno(int arg0, int bits)
        {
            if (!registers.eflags.Get(11))
            {
                Jump(arg0);
            }
        }

        public static void Jo(BitArray arg0, int bits)
        {
            if (registers.eflags.Get(11))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jo(int arg0, int bits)
        {
            if (registers.eflags.Get(11))
            {
                Jump(arg0);
            }
        }

        public static void Jz(BitArray arg0, int bits)
        {
            if (!registers.eflags.Get(6))
            {
                int ax = 0;
                for (int i = 0; i < bits; i++)
                { ax |= (Convert.ToByte(arg0.Get(i)) << i); }

                Jump(ax);
            }
        }
        public static void Jz(int arg0, int bits)
        {
            if (!registers.eflags.Get(6))
            {
                Jump(arg0);
            }
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

            SetZF(reg0);
            SetSF(reg0);
            SetPF(reg0);
            SetCF(reg0, bits1);
            SetOF(reg0, bits1);
            SetAF(arg0);
            


            BitArray remnd = new BitArray(new int[] { reg0 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = remnd[i]; 

            }
        }

        public static void Add(BitArray arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg1);

                int regadr0 = 0;
                for (int i = 0; i < 32; i++)
                { regadr0 |= (Convert.ToByte(arg0.Get(i)) << i); }


                regadr0 = regadr0 + value;

                SetZF(regadr0);
                SetSF(regadr0);
                SetPF(regadr0);
                SetCF(regadr0, bits1);
                SetOF(regadr0, bits1);
                SetAF(arg0);

                BitArray remndadr = new BitArray(new int[] { regadr0 });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    arg0[i] = remndadr[i];


                }
                return;
            }
            int reg0 = 0;
            for (int i = 0; i < 32; i++)
            { reg0 |= (Convert.ToByte(arg0.Get(i)) << i); }


            reg0 = reg0 + arg1;

            SetZF(reg0);
            SetSF(reg0);
            SetPF(reg0);
            SetCF(reg0, bits1);
            SetOF(reg0, bits1);
            SetAF(arg0);



            BitArray remnd = new BitArray(new int[] { reg0 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = remnd[i];

            }

        }
        public static void Add(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {

                int value = GetValAddr(arg0);

                int regadr1 = 0;
                for (int i = 0; i < bits1; i++)
                { regadr1 |= (Convert.ToByte(arg1.Get(i)) << i); }

                value = value + regadr1;


                assembler.memory[value][arg0] = value;

                SetZF(value);
                SetSF(value);
                SetPF(value);
                SetCF(value, bits1);
                SetOF(value, bits1);
                SetAF(arg1);

                return;
            }
            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }


            reg1 = reg1 + arg0;

            SetZF(reg1);
            SetSF(reg1);
            SetPF(reg1);
            SetCF(reg1, bits1);
            SetOF(reg1, bits1);
            SetAF(arg1);

        }
        public static void Add(int arg0, int arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {

                int value = GetValAddr(arg0);


                value = value + arg1;


                assembler.memory[value][arg0] = value;

                SetZF(value);
                SetSF(value);
                SetPF(value);
                SetCF(value, bits1);
                SetOF(value, bits1);
                BitArray remndadr = new BitArray(new int[] { value });
                SetAF(remndadr);

                return;
            }

            arg1 = arg1 + arg0;

            SetZF(arg1);
            SetSF(arg1);
            SetPF(arg1);
            SetCF(arg1, bits1);
            SetOF(arg1, bits1);
            BitArray remnd = new BitArray(new int[] { arg1 });
            SetAF(remnd);

        }


        public static int And(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg0[i] & arg1[i];

            }


            int reg1 = 0;
            for (int i = 0; i < bits1; i++)
            { reg1 |= (Convert.ToByte(arg0.Get(i)) << i); }

            registers.eflags.Set(0, false);
            registers.eflags.Set(11, false);
            SetSF(reg1);
            SetZF(reg1);
            SetPF(reg1);

            return reg1;
        }

        public static int And(BitArray arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg1);

                BitArray remndadr = new BitArray(new int[] { value });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    arg0[i] = arg0[i] & remndadr[i];

                }
                int re0 = 0;
                for (int i = 0; i < bits1; i++)
                { re0 |= (Convert.ToByte(arg0.Get(i)) << i); }

                registers.eflags.Set(0, false);
                registers.eflags.Set(11, false);
                SetSF(re0);
                SetZF(re0);
                SetPF(re0);

                return re0;
            }

            BitArray remnd = new BitArray(new int[] { arg1 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg0[i] & remnd[i];

            }
            int re1 = 0;
            for (int i = 0; i < bits1; i++)
            { re1 |= (Convert.ToByte(arg0.Get(i)) << i); }

            registers.eflags.Set(0, false);
            registers.eflags.Set(11, false);
            SetSF(re1);
            SetZF(re1);
            SetPF(re1);

            return re1;

        }
        public static int And(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int value = GetValAddr(arg0);

                BitArray remndadr = new BitArray(new int[] { value });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    remndadr[i] = remndadr[i] & arg1[i];

                }

                int reg1 = 0;
                for (int i = 0; i < bits1; i++)
                { reg1 |= (Convert.ToByte(remndadr.Get(i)) << i); }

                SetValAddr(reg1, arg0);

                return reg1;

            }
            return 1;


        }

        public static int And(int arg0, int arg1, int bits0, int bits1)
        {

            if (bits0 == 0xAD)
            {
                int value = GetValAddr(arg0);

                BitArray remndadr = new BitArray(new int[] { value });
                BitArray arg = new BitArray(new int[] { arg1 });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    remndadr[i] = remndadr[i] & arg[i];

                }

                int reg1 = 0;
                for (int i = 0; i < bits1; i++)
                { reg1 |= (Convert.ToByte(remndadr.Get(i)) << i); }

                SetValAddr(reg1, arg0);


                return reg1;
            }
            return 1;

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

            SetZF(reg0);
            SetSF(reg0);

        }

        public static void Cmp(BitArray arg0, int arg1, int bits0, int bits1)
        {
            int reg0 = 0;
            for (int i = 0; i < 32; i++)
            { reg0 |= (Convert.ToByte(arg0.Get(i)) << i); }

            reg0 = reg0 - arg1;

            SetZF(reg0);
            SetSF(reg0);

        }
        public static void Cmp(int arg0, BitArray arg1, int bits0, int bits1)
        {

            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

            arg0 = arg0 - reg1;

            SetZF(arg0);
            SetSF(arg0);
        }

        public static void Cmp(int arg0, int arg1, int bits0, int bits1)
        {
            arg0 = arg0 - arg1;

            SetZF(arg0);
            SetSF(arg0);
        }


        public static void Lea(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg1[i];

            }
        }
       


        public static void Lea(BitArray arg0, int arg1, int bits0, int bits1)
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
            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg0[i] || arg1[i];

            }

            int orn = 0;
            for (int i = 0; i < bits1; i++)
            { orn |= (Convert.ToByte(arg0.Get(i)) << i); }


            registers.eflags.Set(0, false);
            registers.eflags.Set(11, false);
            SetSF(orn);
            SetZF(orn);
            SetPF(orn);

        }

        public static void Or(BitArray arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg1);

                BitArray remndadr = new BitArray(new int[] { value });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    arg0[i] = arg0[i] || remndadr[i];

                }

                int orn = 0;
                for (int i = 0; i < bits1; i++)
                { orn |= (Convert.ToByte(arg0.Get(i)) << i); }


                registers.eflags.Set(0, false);
                registers.eflags.Set(11, false);
                SetSF(orn);
                SetZF(orn);
                SetPF(orn);

                return;
            }

            BitArray remnd = new BitArray(new int[] { arg1 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = arg0[i] || remnd[i];

            }
        }
        public static void Or(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int value = GetValAddr(arg0);

                BitArray remndadr = new BitArray(new int[] { value });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    remndadr[i] = remndadr[i] || arg1[i];

                }

                int reg1 = 0;
                for (int i = 0; i < bits1; i++)
                { reg1 |= (Convert.ToByte(remndadr.Get(i)) << i); }

                SetValAddr(value, reg1);

            }
        }

        public static void Or(int arg0, int arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int value = GetValAddr(arg0);

                BitArray remndadr = new BitArray(new int[] { value });
                BitArray arg = new BitArray(new int[] { arg1 });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    remndadr[i] = remndadr[i] || arg[i];

                }

                int reg1 = 0;
                for (int i = 0; i < bits1; i++)
                { reg1 |= (Convert.ToByte(remndadr.Get(i)) << i); }


                SetValAddr(value, reg1);

            }
        }


       


        public static void Shl(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            int reg1 = 0;
            for (int i = 0; i < bits1; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

            arg0.LeftShift(reg1);

            int res = 0;
            for (int i = 0; i < bits1; i++)
            { res |= (Convert.ToByte(arg0.Get(i)) << i); }


            SetOF(res, bits0);
            SetSF(res);
            SetZF(res);
            SetPF(res);


        }

        public static void Shl(BitArray arg0, int arg1, int bits0, int bits1)
        {
            arg0.LeftShift(arg1);

            int res = 0;
            for (int i = 0; i < bits1; i++)
            { res |= (Convert.ToByte(arg0.Get(i)) << i); }


            SetOF(res, bits0);
            SetSF(res);
            SetZF(res);
            SetPF(res);
        }
        public static void Shl(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int reg1 = 0;
                for (int i = 0; i < bits1; i++)
                { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

                int value = GetValAddr(arg0);

                value = value << reg1;

                SetValAddr(value, arg0);


            }


        }

        public static void Shl(int arg0, int arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int value = GetValAddr(arg0);

                value = value << arg1;

                SetValAddr(value, arg0);

            }


            
        }
        public static void Shr(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            int reg1 = 0;
            for (int i = 0; i < bits1; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

            arg0.RightShift(reg1);

            int res = 0;
            for (int i = 0; i < bits1; i++)
            { res |= (Convert.ToByte(arg0.Get(i)) << i); }


            SetOF(res, bits0);
            SetSF(res);
            SetZF(res);
            SetPF(res);

        }

        public static void Shr(BitArray arg0, int arg1, int bits0, int bits1)
        {
            arg0.RightShift(arg1);

            int res = 0;
            for (int i = 0; i < bits1; i++)
            { res |= (Convert.ToByte(arg0.Get(i)) << i); }


            SetOF(res, bits0);
            SetSF(res);
            SetZF(res);
            SetPF(res);
        }
        public static void Shr(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int reg1 = 0;
                for (int i = 0; i < bits1; i++)
                { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

                int value = GetValAddr(arg0);

                value = value >> reg1;

                SetValAddr(value, arg0);

            }
        }

        public static void Shr(int arg0, int arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {
                int value = GetValAddr(arg0);

                value = value >> arg1;

                SetValAddr(value, arg0);


            }
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


            int xrn = 0;
            for (int i = 0; i < bits1; i++)
            { xrn |= (Convert.ToByte(arg0.Get(i)) << i); }


            registers.eflags.Set(0, false);
            registers.eflags.Set(11, false);
            SetSF(xrn);
            SetZF(xrn);
            SetPF(xrn);

        }

        public static void Xor(BitArray arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg1);
                BitArray remndadr = new BitArray(new int[] { value });

                for (int i = 0; i < bits0; i++)
                {
                    arg0[i] ^= remndadr[i];
                    if (i > bits1)
                    {
                        break;

                    }
                }
                int xrn0 = 0;
                for (int i = 0; i < bits1; i++)
                { xrn0 |= (Convert.ToByte(arg0.Get(i)) << i); }


                registers.eflags.Set(0, false);
                registers.eflags.Set(11, false);
                SetSF(xrn0);
                SetZF(xrn0);
                SetPF(xrn0);

                return;
            }
            BitArray remnd = new BitArray(new int[] { arg1 });

            for (int i = 0; i < bits0; i++)
            {
                arg0[i] ^= remnd[i];
                if (i > bits1)
                {
                    break;

                }
            }
            int xrn = 0;
            for (int i = 0; i < bits1; i++)
            { xrn |= (Convert.ToByte(arg0.Get(i)) << i); }


            registers.eflags.Set(0, false);
            registers.eflags.Set(11, false);
            SetSF(xrn);
            SetZF(xrn);
            SetPF(xrn);


        }
        public static void Xor(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg0);
                BitArray remndadr = new BitArray(new int[] { value });

                for (int i = 0; i < bits0; i++)
                {
                    arg1[i] ^= remndadr[i];
                    if (i > bits1)
                    {
                        break;

                    }
                }
                int iadr = 0;
                for (int i = 0; i < bits1; i++)
                { iadr |= (Convert.ToByte(arg1.Get(i)) << i); }

                SetValAddr(value, iadr);

                return;
            }
            BitArray remnd = new BitArray(new int[] { arg0 });

            for (int i = 0; i < bits0; i++)
            {
                arg1[i] ^= remnd[i];
                if (i > bits1)
                {
                    break;

                }
            }

        }

        public static void Xor(int arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg0);

                arg1 ^= arg1;

                SetValAddr(value,arg1);

            }
        }


        public static void Sub(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            int reg0 = 0;
            for (int i = 0; i < 32; i++)
            { reg0 |= (Convert.ToByte(arg0.Get(i)) << i); }

            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }

            reg0 = reg0 - reg1;

            SetZF(reg0);
            SetSF(reg0);
            SetPF(reg0);
            SetCF(reg0, bits1);
            SetOF(reg0, bits1);
            SetAF(arg0);



            BitArray remnd = new BitArray(new int[] { reg0 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = remnd[i];

            }
        }

        public static void Sub(BitArray arg0, int arg1, int bits0, int bits1)
        {
            if (bits1 == 0xAD)
            {
                int value = GetValAddr(arg1);

                int regadr0 = 0;
                for (int i = 0; i < 32; i++)
                { regadr0 |= (Convert.ToByte(arg0.Get(i)) << i); }


                regadr0 = regadr0 - value;

                SetZF(regadr0);
                SetSF(regadr0);
                SetPF(regadr0);
                SetCF(regadr0, bits1);
                SetOF(regadr0, bits1);
                SetAF(arg0);

                BitArray remndadr = new BitArray(new int[] { regadr0 });

                for (int i = 0; i < bits0 && i < bits1; i++)
                {
                    arg0[i] = remndadr[i];


                }
                return;
            }
            int reg0 = 0;
            for (int i = 0; i < 32; i++)
            { reg0 |= (Convert.ToByte(arg0.Get(i)) << i); }


            reg0 = reg0 - arg1;

            SetZF(reg0);
            SetSF(reg0);
            SetPF(reg0);
            SetCF(reg0, bits1);
            SetOF(reg0, bits1);
            SetAF(arg0);



            BitArray remnd = new BitArray(new int[] { reg0 });

            for (int i = 0; i < bits0 && i < bits1; i++)
            {
                arg0[i] = remnd[i];

            }

        }
        public static void Sub(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {

                int value = GetValAddr(arg0);

                int regadr1 = 0;
                for (int i = 0; i < bits1; i++)
                { regadr1 |= (Convert.ToByte(arg1.Get(i)) << i); }

                value = value - regadr1;


                assembler.memory[value][arg0] = value;

                SetZF(value);
                SetSF(value);
                SetPF(value);
                SetCF(value, bits1);
                SetOF(value, bits1);
                SetAF(arg1);

                return;
            }
            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg1.Get(i)) << i); }


            reg1 = reg1 - arg0;

            SetZF(reg1);
            SetSF(reg1);
            SetPF(reg1);
            SetCF(reg1, bits1);
            SetOF(reg1, bits1);
            SetAF(arg1);

        }
        public static void Sub(int arg0, int arg1, int bits0, int bits1)
        {
            if (bits0 == 0xAD)
            {

                int value = GetValAddr(arg0);


                value = value - arg1;


                assembler.memory[value][arg0] = value;

                SetZF(value);
                SetSF(value);
                SetPF(value);
                SetCF(value, bits1);
                SetOF(value, bits1);
                BitArray remndadr = new BitArray(new int[] { value });
                SetAF(remndadr);

                return;
            }

            arg1 = arg1 - arg0;

            SetZF(arg1);
            SetSF(arg1);
            SetPF(arg1);
            SetCF(arg1, bits1);
            SetOF(arg1, bits1);
            BitArray remnd = new BitArray(new int[] { arg1 });
            SetAF(remnd);

        }


        public static void Test(BitArray arg0, BitArray arg1, int bits0, int bits1)
        {
            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg0.Get(i)) << i); }

            if (And(arg0, arg1, bits0, bits1) == reg1)
            {
                SetZF(reg1);
                SetSF(reg1);
                SetPF(reg1);
            }
        }

        public static void Test(BitArray arg0, int arg1, int bits0, int bits1)
        {
            int reg1 = 0;
            for (int i = 0; i < 32; i++)
            { reg1 |= (Convert.ToByte(arg0.Get(i)) << i); }

            if (And(arg0, arg1, bits0, bits1) == reg1)
            {
                SetZF(reg1);
                SetSF(reg1);
                SetPF(reg1);
            }
        }
        public static void Test(int arg0, BitArray arg1, int bits0, int bits1)
        {
            if (And(arg0, arg1, bits0, bits1) == arg0)
            {
                SetZF(arg0);
                SetSF(arg0);
                SetPF(arg0);
            }
        }

        public static void Test(int arg0, int arg1, int bits0, int bits1)
        {
            if( And(arg0, arg1, bits0, bits1) == arg0)
            {
                SetZF(arg0);
                SetSF(arg0);
                SetPF(arg0);
            }
        }

        public static long GetFromBitArray(BitArray bitArray)
        {
            var array = new byte[8];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt64(array, 0);
        }


    }
}