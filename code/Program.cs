using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Text;
using System.Linq;

namespace ConsoleAsm
{
    struct lineofcode
    {
        public string asminstruction;
        public string instruction0;

        public string instruction1;
    }
    class assembler
    {
        public const string NOT_A_INSTRUCTION = "NAI";

        public static void init()
        {
            Registers.Add("eax", Tuple.Create(registers.eax, 32));
            Registers.Add("ecx", Tuple.Create(registers.ecx, 32));
            Registers.Add("edx", Tuple.Create(registers.edx, 32));
            Registers.Add("ebx", Tuple.Create(registers.ebx, 32));
            Registers.Add("esp", Tuple.Create(registers.esp, 32));
            Registers.Add("ebp", Tuple.Create(registers.ebp, 32));
            Registers.Add("esi", Tuple.Create(registers.esi, 32));
            Registers.Add("edi", Tuple.Create(registers.edi, 32));

            Registers.Add("ax", Tuple.Create(registers.eax, 16));
            Registers.Add("cx", Tuple.Create(registers.ecx, 16));
            Registers.Add("dx", Tuple.Create(registers.edx, 16));
            Registers.Add("bx", Tuple.Create(registers.ebx, 16));
            Registers.Add("sp", Tuple.Create(registers.esp, 16));
            Registers.Add("bp", Tuple.Create(registers.ebp, 16));
            Registers.Add("si", Tuple.Create(registers.esi, 16));
            Registers.Add("di", Tuple.Create(registers.edi, 16));

            Registers.Add("ah", Tuple.Create(registers.eax, 8));
            Registers.Add("al", Tuple.Create(registers.eax, 8));
            Registers.Add("ch", Tuple.Create(registers.ecx, 8));
            Registers.Add("cl", Tuple.Create(registers.ecx, 8));
            Registers.Add("dh", Tuple.Create(registers.edx, 8));
            Registers.Add("dl", Tuple.Create(registers.edx, 8));
            Registers.Add("bh", Tuple.Create(registers.ebx, 8));
            Registers.Add("bl", Tuple.Create(registers.ebx, 8));
            Registers.Add("spl", Tuple.Create(registers.esp, 8));
            Registers.Add("bpl", Tuple.Create(registers.ebp, 8));
            Registers.Add("sil", Tuple.Create(registers.esi, 8));
            Registers.Add("dil", Tuple.Create(registers.edi, 8));

            Registers.Add(NOT_A_INSTRUCTION, Tuple.Create(registers.eax, 0));





            //later %include        'functions.asm'
            string origin = "bits=64;history=10;stack=0x1024;";
            string[] con = new string[5];
            string config = System.IO.File.ReadAllText("consoleasm.config");
            config = String.Concat(config.Where(c => !Char.IsWhiteSpace(c)));
            if (config.Equals(origin)) { return; };
            con = config.Split(';', '=');

            registers.bits = byte.Parse(con[1]);
            asmstack.stacksize = Convert.ToInt32(con[5], 16);


            
        }


        public static Dictionary<uint, lineofcode> code = new Dictionary<uint, lineofcode>();
        public static asmstack astack = new asmstack();
        public static asmheap heap = new asmheap();
        public static asmdata data = new asmdata();
        public static registers Register = new registers();


        public static Dictionary<string, Tuple<BitArray, int>> Registers = new Dictionary<string, Tuple<BitArray, int>>();


        public static Dictionary<string, string> Variables = new Dictionary<string, string>();
        // db dd etc.

        public static Dictionary<string, uint> Lables = new Dictionary<string, uint>();

        public static Dictionary<int, int[]> memory = new Dictionary<int, int[]>();

        public static uint instructioncounter = 0x000000;



        static void Main(string[] args)
        {
            init();
            Console.WriteLine("ConsoleAsm 1.0.0\n[C# implementation] https://github.com/Transyl\nType help, copyright, credits or license for more information.\n");
            lineofcode ri = new lineofcode(); 
            while (true)
            {
                string codeline = Console.ReadLine();
                if (codeline.StartsWith("$"))
                {
                    Command(codeline);
                    continue;
                }
                if (codeline.StartsWith("."))
                { 
                    Data(codeline);
                    continue;
                }
                if (codeline.EndsWith(":"))
                {
                    Label(codeline);
                    continue;
                }

                string[] specialInstruction = new string[4];
                specialInstruction[0] = "ret";
                specialInstruction[1] = "retn";
                specialInstruction[2] = "leave";
                specialInstruction[3] = "enter";


                for (int i = 0; i < specialInstruction.Length; i++)
                {
                    if (codeline == specialInstruction[i])
                    {
                        ExecuteInstruction<int>(codeline, 0, 0);
                        ri.asminstruction = codeline;
                        ri.instruction0 = NOT_A_INSTRUCTION;
                        ri.instruction1   = NOT_A_INSTRUCTION;
                        goto End;
                    }
                }

                ri = ReadInstructions(codeline);

                foreach (var item in Lables)
                {
                    if (ri.instruction0.Equals(item.Key))
                    {
                        ri.instruction0 = item.Value.ToString();
                    }
                }



                if (ri.asminstruction == NOT_A_INSTRUCTION) { Console.ResetColor(); continue; }
                Parse(ri);

                End:
                instructioncounter = instructioncounter + 1;
                code.Add(instructioncounter, ri);


            }

        }


        static BitArray reg0;
        static BitArray reg1;


        public static int Parse(lineofcode ri)
        {

            // STACK FRAME 
            if (ri.asminstruction.CompareTo("push") == 0 && ri.instruction0.CompareTo("ebp") == 0)
            {
                ExecuteInstruction<int>("push ebp", 0, 0);
                return 0;
            }
            else if (ri.asminstruction.CompareTo("mov") == 0 && ri.instruction0.CompareTo("ebp") == 0  && ri.instruction1.CompareTo("esp") == 0)
            {
                ExecuteInstruction<int>("mov ebp,esp", 0, 0);
                return 0;
            }
            else if (ri.asminstruction.CompareTo("sub") == 0 && ri.instruction0.CompareTo("esp") == 0)
            {
                if (Char.IsDigit(ri.instruction1[0]))
                {
                    ExecuteInstruction<int>("sub esp,", int.Parse(ri.instruction1), 0);
                }
                else
                {
                    (BitArray a ,int b)  = WithRegister(ri.instruction1);

                    int reg = 0;
                    for (int i = 0; i < 16; i++)
                    { reg |= (Convert.ToByte(a.Get(i)) << i); }

                    ExecuteInstruction<int>("sub esp,",reg, 0);
                }

                return 0;
            }
            if (ri.asminstruction.CompareTo("call") == 0)
            {
                instructions.Call(ri.instruction0);
                return 0;
            }



            int counter = 0;
            int numeric0 = 0;
            int numeric1 = 0;
            int bits0 = 0;
            int bits1 = 0;
            int addr0 = 0;
            int addr1 = 0;



            // arg 0
            if (!ri.instruction0.Equals(NOT_A_INSTRUCTION))
            {
                counter++;
                if (ri.instruction0[0] == '[')
                {
                    addr0 = 1;
                    string RI = ri.instruction0[1..^1];
                    if (Char.IsDigit(RI[0]))
                    {
                        if (RI.Length > 1)
                        {
                            if (RI[1] == 'x')
                            {
                                RI = RI[2..];
      
                                int iadr = int.Parse(RI,System.Globalization.NumberStyles.HexNumber);

                                numeric0 = assembler.memory[iadr][0];

                            }
                            else
                            {
                                int iadr = int.Parse(RI);
                                numeric0 = assembler.memory[iadr][0];

                            }
                        }
                        else
                        {
                            int iadr = int.Parse(RI);
                            numeric0 = assembler.memory[iadr][0];

                        }

                    }
                    else if (RI[1..^1].Contains('+') || RI[1..^1].Contains('-') || RI[1..^1].Contains('*') || RI[1..^1].Contains('/'))
                    {
                        int value = CalcAddr(RI);
                        int iadr = 0;
                        for (int i = 0; i < 32; i++)
                        { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }
                        int index = 0;
                        index = value - iadr;


                        numeric0 = index;

                    }
                    else
                    {
                        (reg1, bits1) = WithRegister(RI);
                        int regvalue = 0;
                        for (int i = 0; i < bits0; i++)
                        { regvalue |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

                        numeric0 = assembler.memory[regvalue][0];

                    }


                   
                    if (ri.instruction1.Equals(NOT_A_INSTRUCTION))
                    {
                        bits0 = 0xFF;
                        ExecuteInstruction<int>(ri.asminstruction, numeric0, bits0);
                    }


                }

                else if (Char.IsDigit(ri.instruction0[0]))
                {
                     if (ri.instruction0.Length > 1)
                     {

                        if (ri.instruction0[1] == 'x')
                        {
                            ri.instruction0 = ri.instruction0[2..];
                            numeric0 = int.Parse(ri.instruction0, System.Globalization.NumberStyles.HexNumber);

                        }
                        else
                        {
                            numeric0 = int.Parse(ri.instruction0);

                        }

                    }
                    else
                    {
                        numeric0 = int.Parse(ri.instruction0);

                    }


                    if (ri.instruction1.Equals(NOT_A_INSTRUCTION))
                    {
                        ExecuteInstruction<int>(ri.asminstruction, numeric0, bits0);
                    }
                }

                else if (ri.instruction0[0] == '"' || ri.instruction0 == "'")
                {
                    numeric0 = GetValueFromASCII(ri.instruction0[1]);
                    if (ri.instruction1.Equals(NOT_A_INSTRUCTION))
                    {
                        ExecuteInstruction<int>(ri.asminstruction, numeric0, bits0);
                    }
                }
                else
                {
                    (reg0, bits0) = WithRegister(ri.instruction0);
                    if (bits0 == 0)
                    {
                        return 3;

                    }
                    if (ri.instruction1.Equals(NOT_A_INSTRUCTION))
                    {
                        ExecuteInstruction<BitArray>(ri.asminstruction, reg0, bits0);
                    }


                }
            }


            // arg 1
            if (!ri.instruction1.Equals(NOT_A_INSTRUCTION))
            {
                counter++;
                if (ri.instruction1[0] == '[')
                {
                    addr1 = 1;
                    string RI = ri.instruction1[1..^1];
                    if (Char.IsDigit(RI[1]))
                    {
                        if (RI[2] == 'x')
                        {
                            RI = RI[2..];

                            int iadr = int.Parse(RI, System.Globalization.NumberStyles.HexNumber);

                            numeric1 = assembler.memory[iadr][0];

                        }
                        else
                        {
                            int iadr = int.Parse(RI);
                            numeric1 = assembler.memory[iadr][0];

                        }
                    }
                    else if (RI[1..^1].Contains('+') || RI[1..^1].Contains('-') || RI[1..^1].Contains('*') || RI[1..^1].Contains('/'))
                    {
                        int value = CalcAddr(RI);
                        int iadr = 0;
                        for (int i = 0; i < 32; i++)
                        { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }
                        int index = value - iadr;

                        numeric1 = assembler.memory[iadr][index];

                    }
                    else
                    {
                        (reg1, bits1) = WithRegister(RI);
                        int regvalue = 0;
                        for (int i = 0; i < bits0; i++)
                        { regvalue |= (Convert.ToByte(reg1.Get(i)) << i); }

                        numeric1 = assembler.memory[regvalue][0];


                    }

                    if (reg0 == null)
                    {
                        bits0 = 0xAD;
                        ExecuteInstruction<int, BitArray>(ri.asminstruction, numeric0, reg1, bits0, bits1);
                        return 0;
                    }

                    else if(reg1 == null)
                    {
                        bits1 = 0xAD;
                        ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);
                        return 0;
                    }

                    else
                    {
                        if (addr1 == 0)
                        {
                            ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);

                        }
                        else
                        {
                            bits1 = 0xAD;
                            if (bits0 == 1)
                            {
                                bits0 = 0xAD;
                                ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);
                                return 0;

                            }
                            ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);

                        }
                        return 0;
                    }
                }
                else if (Char.IsDigit(ri.instruction1[0]))
                {
                    if (ri.instruction1.Length > 1)
                    {
                        if (ri.instruction1[1] == 'x')
                        {

                            ri.instruction1 = ri.instruction1[2..];
                            numeric1 = int.Parse(ri.instruction1, System.Globalization.NumberStyles.HexNumber);

                        }
                        else
                        {
                            numeric1 = int.Parse(ri.instruction1);

                        }
                    }
                    else
                    {
                        numeric1 = int.Parse(ri.instruction1);

                    }


                    if (reg0 == null)
                    {
                        if (addr1 == 0)
                        {
                            ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);

                        }
                        else
                        {
                            bits1 = 0xAD;
                            if (bits0 == 1)
                            {
                                bits0 = 0xAD;
                                ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);
                                return 0;

                            }
                            ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);

                        }

                    }
                    else
                    {
                        if (addr1 == 0)
                        {
                            ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);

                        }
                        else
                        {
                            bits1 = 0xAD;
                            ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);

                        }

                    }
                }
                else if (ri.instruction1[0] == '"' || ri.instruction1 == "'")
                {
                    numeric1 = GetValueFromASCII(ri.instruction1[1]);
                    if (reg0 == null)
                    {
                        if (addr1 == 0)
                        {
                            ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);

                        }
                        else
                        {
                            bits1 = 0xAD;
                            if (bits0 == 1)
                            {
                                bits0 = 0xAD;
                                ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);
                                return 0;

                            }
                            ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);



                        }

                    }
                    else
                    {
                        if (addr1 == 0)
                        {
                            ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);

                        }
                        else
                        {
                            bits1 = 0xAD;
                            ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);

                        }
                    }
                }
                else
                {
                    (reg1, bits1) = WithRegister(ri.instruction1);
                    if (bits1 == 0)
                    {
                        return 3;

                    }
                    if (numeric0 == 0)
                    {
                        ExecuteInstruction<BitArray, BitArray>(ri.asminstruction, reg0, reg1, bits0, bits1);
                    }
                    else
                    {
                        bits0 = 0xAD;
                        ExecuteInstruction<int, BitArray>(ri.asminstruction, numeric0, reg1, bits0, bits1);
                    }


                }
            }
            if (counter == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"PARSE ERROR 325: Invalid instruction => '{ri}'");
                Console.ResetColor();
            }

            reg0 = null;
            reg1 = null;
            return 0;


        }
        private static lineofcode ReadInstructions(string codeline)
        {
            bool isthree = false;
            codeline = codeline.Trim();
            lineofcode line = new lineofcode();
            StringBuilder sb = new StringBuilder();
            int state = 0;
            bool space = false;


            for (int i = 0; i < codeline.Length; i++)
            {

                if (codeline[i] == 0x20)// 0x20 is a hex value of " "
                {
                    space = true;
                    if (codeline[i + 1] != 0x20)
                    {
                        line.asminstruction = sb.ToString().Trim();
                        sb.Clear();
                    }
                }

                if (codeline[i] == 0x2c)// 0x2C is a hex value of ","
                {
                    if (space == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"ERROR 324: There is no space before instructions. => '{codeline}'");
                        Console.ResetColor();
                        System.Environment.Exit(3);
                    }
                    if (codeline[i - 1] == 0x20)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"ERROR 323: Space before ',' => '{codeline}'");
                        Console.ResetColor();
                        System.Environment.Exit(3);
                    }
                    isthree = true;
                    line.instruction0 = sb.ToString().Trim();
                    sb.Clear();
                    state = i + 1;
                    break;

                }

                sb.Append(codeline[i]);
                if (codeline.Length == i + 1 && isthree == false)
                {
                    line.instruction0 = sb.ToString().Trim();

                }

            }

            int len = codeline.Length - state;
            for (int i = 0; i < len; i++)
            {
                sb.Append(codeline[state]);
                state++;

            }
            if (space == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("READ ERROR 325: Invalid instruction");
                Console.ResetColor();
                line.asminstruction = NOT_A_INSTRUCTION;
                return line;
            }
            if (isthree)
            {
                line.instruction1 = sb.ToString().Trim().ToLower();
                line.asminstruction.ToLower();
                line.instruction0.ToLower();
                line.instruction1.ToLower();
            }
            else
            {
                line.asminstruction.ToLower();
                line.instruction0.ToLower();
                line.instruction1 = NOT_A_INSTRUCTION;
            }

            return line;


        }

        private static void ExecuteInstruction<T, U>(string ri, T arg0, U arg1, int bits0, int bits1)
        {

            dynamic Arg0;

            if (arg0 is BitArray)
            {
                Arg0 = (BitArray)(object)arg0;
            }
            else
            {
                Arg0 = (int)(object)arg0;
            }

            dynamic Arg1;

            if (arg1 is BitArray)
            {
                Arg1 = (BitArray)(object)arg1;
            }
            else
            {
                Arg1 = (int)(object)arg1;
            }



            switch (ri)
            {

                case "add":
                    instructions.Add(Arg0, Arg1, bits0, bits1);
                    break;
                case "and":
                    instructions.And(Arg0, Arg1, bits0, bits1);
                    break;
                case "cmp":
                    instructions.Cmp(Arg0, Arg1, bits0, bits1);
                    break;
                case "lea":
                    instructions.Lea(Arg0, Arg1, bits0, bits1);
                    break;
                case "mov":
                    instructions.Mov(Arg0, Arg1, bits0, bits1);
                    break;
                case "or":
                    instructions.Or(Arg0, Arg1, bits0, bits1);
                    break;
                case "shl":
                    instructions.Shl(Arg0, Arg1, bits0, bits1);
                    break;
                case "xor":
                    instructions.Xor(Arg0, Arg1, bits0, bits1);
                    break;
                case "sub":
                    instructions.Sub(Arg0, Arg1, bits0, bits1);
                    break;
                case "test":
                    instructions.Test(Arg0, Arg1, bits0, bits1);
                    break;

            }

        }
        private static void ExecuteInstruction<T>(string ri, T arg0, int bits)
        {

            dynamic arg;

            if (arg0 is BitArray)
            {
                arg = (BitArray)(object)arg0;
            }
            else
            {
                arg = (int)(object)arg0;
            }

            if (ri[0] == 'j')
            {
                goto jmp;
            }


            switch (ri)
            {

                case "pop":
                    instructions.Pop(arg, bits);
                    break;
                case "push":
                    instructions.Push(arg, bits);
                    break;
                case "not":
                    instructions.Not(arg, bits);
                    break;
                case "mul":
                    instructions.Mul(arg, bits);
                    break;
                case "inc":
                    instructions.Inc(arg, bits);
                    break;
                case "div":
                    instructions.Div(arg, bits);
                    break;
                case "dec":
                    instructions.Dec(arg, bits);
                    break;
                case "push ebp":
                    asmstack.PushEbp();
                    break;
                case "mov ebp,esp":
                    asmstack.MovEbpEsp();
                    break;
                case "sub esp,":
                    asmstack.SubEsp(arg);
                    break;

            }
        jmp:
            switch (ri)
            {
                case "jmp":
                    instructions.Jmp(arg, bits);
                    break;
                case "jc":
                    instructions.Jc(arg, bits);
                    break;
                case "je":
                    instructions.Je(arg, bits);
                    break;
                case "jne":
                    instructions.Jne(arg, bits);
                    break;
                case "jg":
                    instructions.Jg(arg, bits);
                    break;
                case "jge":
                    instructions.Jge(arg, bits);
                    break;
                case "jl":
                    instructions.Jl(arg, bits);
                    break;
                case "jle":
                    instructions.Jle(arg, bits);
                    break;
                case "jno":
                    instructions.Jno(arg, bits);
                    break;
                case "jo":
                    instructions.Jo(arg, bits);
                    break;
                case "jz":
                    instructions.Jz(arg, bits);
                    break;
            }

        }


        private static Tuple<BitArray, int> WithRegister(string ri)
        {

            try
            {
                return Registers[ri];
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("REGISTER ERROR 325: Invalid instruction");
                Console.ResetColor();
                return Registers[NOT_A_INSTRUCTION];
            }

            

        }

        public static int GetValueFromASCII(char ASCII)
        {
            return (int)ASCII;
        }
        private static void Command(string arg)
        {
            string[] command = arg.Split(' ');

            switch (command[0])
            {
                case "$val":
                    if (command.Length > 1)
                    {
                        GetValue(command[1]);
                    }
                    else { Console.ForegroundColor = ConsoleColor.DarkYellow;  System.Console.WriteLine("usage: $val <register>"); Console.ResetColor(); }
                    break;
                case "$hval":
                    if (command.Length > 1)
                    {
                        GetValue(command[1],0);
                    }
                    else { Console.ForegroundColor = ConsoleColor.DarkYellow; System.Console.WriteLine("usage: $val <register>"); Console.ResetColor(); }
                    break;
                case "$flags":
                    ShowEflags();
                    break;
                case "$inst":
                    ShowInstruction();
                    break;
                case "$stack":
                    ShowStack();
                    break;
                case "$heap":
                    ShowHeap();
                    break;
                case "$dump":
                    DumpHistory();
                    break;
                case "$leave":
                    Environment.Exit(0);
                    break;
            }
        }
        private static void Label(string arg)
        {
            arg = arg.Remove(arg.Length - 1);

            lineofcode lable = new lineofcode();
            lable.asminstruction = arg;
            lable.instruction0 = NOT_A_INSTRUCTION;
            lable.instruction1 = NOT_A_INSTRUCTION;
            instructioncounter = instructioncounter + 1;


            Lables.Add(arg, instructioncounter);
            code.Add(instructioncounter, lable);

        }
        private static void Data(string arg)
        {
            string[] name = new string[3];
            
            foreach (var item in arg)
            {
                int i = 0;
                name[i] += item;
                if (item == ' ')
                {
                    i++;
                }

            }

            if (name[0] == ".data")
            {
                data.AddValue(name[1], name[2]);

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"DATA ERROR 325: Invalid instruction => '{name[0]}'");
                Console.ResetColor();
            }
            

        }
        private static void GetValue(string arg)
        {
            if (arg[0] == '[')
            {

                if (arg[1..^1].Contains('+') || arg[1..^1].Contains('-') || arg[1..^1].Contains('*') || arg[1..^1].Contains('/'))
                {
                    string sadr = arg[1..^1];
                    int iadr = 0;
                    for (int i = 0; i < 32; i++)
                    { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

                    int value = CalcAddr(sadr);
                    int index =  value - iadr;
                    Console.WriteLine(assembler.memory[iadr][index]);
                    return;
                }
                else if (Char.IsDigit(arg[1]))
                {
                    string sadr = arg[1..^1];
                    int iadr = int.Parse(sadr);

                    Console.WriteLine(assembler.memory[iadr][0]);

                    return;

                }

                else
                {

                  (BitArray r, int b) = WithRegister(arg[1..^1]);

                    int ptr = 0;
                    for (int i = 0; i < 32; i++)
                    { ptr |= (Convert.ToByte(r.Get(i)) << i); }


                    Console.WriteLine(assembler.memory[ptr][0]);
                    return;

                }



            }

            (BitArray reg, int bits) = WithRegister(arg);
            int regValue = 0;
            for (int i = 0; i < bits; i++)
            { regValue |= (Convert.ToByte(reg.Get(i)) << i); }

            Console.WriteLine(regValue);
        }
        private static void GetValue(string arg,int type)
        {
            if (arg[0] == '[')
            {

                if (arg[1..^1].Contains('+') || arg[1..^1].Contains('-') || arg[1..^1].Contains('*') || arg[1..^1].Contains('/'))
                {
                    string sadr = arg[1..^1];
                    int iadr = 0;
                    for (int i = 0; i < 32; i++)
                    { iadr |= (Convert.ToByte(registers.ebp.Get(i)) << i); }

                    int value = CalcAddr(arg);
                    int index = value - iadr;
                    Console.WriteLine("0x{0:X}",assembler.memory[iadr][index]);
                    return;
                }
                else if (Char.IsDigit(arg[1]))
                {
                    string sadr = arg[1..^1];
                    int iadr = int.Parse(sadr);

                    Console.WriteLine("0x{0:X}", assembler.memory[iadr][0]);

                    return;

                }

                else
                {

                    (BitArray r, int b) = WithRegister(arg[1..^1]);

                    int ptr = 0;
                    for (int i = 0; i < 32; i++)
                    { ptr |= (Convert.ToByte(r.Get(i)) << i); }

                    Console.WriteLine("0x{0:X}", assembler.memory[ptr][0]);
                    return;

                }



            }

            (BitArray reg, int bits) = WithRegister(arg);
            int regValue = 0;
            for (int i = 0; i < bits; i++)
            { regValue |= (Convert.ToByte(reg.Get(i)) << i); }

            Console.WriteLine("0x{0:X}", regValue);
        }
        
        private static int CalcAddr(string sadr)
        {
            string[] calc = new string[20];
            int l = 0;
            int last = 0;
            StringBuilder state = new StringBuilder();

            for (int i = 0; i < sadr.Length; i++)
            {
                if (Char.IsDigit(sadr[i]))
                {
                    if (last == 2 || last == 3)
                    {
                        calc[l] = state.ToString();
                        l = l + 1;
                        state.Clear();

                    }

                    last = 1;
                    state.Append(sadr[i]);
                    if (sadr.Length - 1 == i)
                    {
                        calc[l] = state.ToString();
                    }
                }
                else if (sadr[i] == '+')
                {
                    if (last == 1 || last == 2)
                    {
                        calc[l] = state.ToString();
                        l = l + 1;
                        state.Clear();

                    }
                    last = 3;
                    state.Append(sadr[i]);
                    if (sadr.Length - 1 == i)
                    {
                        calc[l] = state.ToString();
                    }

                }
                else if (sadr[i] == '-')
                {
                    if (last == 1 || last == 2)
                    {

                        calc[l] = state.ToString();
                        l = l + 1;
                        state.Clear();

                    }
                    last = 3;
                    state.Append(sadr[i]);
                    if (sadr.Length - 1 == i)
                    {
                        calc[l] = state.ToString();
                    }

                }
                else if (sadr[i] == '*')
                {
                    if (last == 1 || last == 2)
                    {
                        calc[l] = state.ToString();
                        l = l + 1;
                        state.Clear();

                    }
                    last = 3;
                    state.Append(sadr[i]);
                    if (sadr.Length - 1 == i)
                    {
                        calc[l] = state.ToString();
                    }

                }
                else if (sadr[i] == '/')
                {
                    if (last == 1 || last == 2)
                    {
                        calc[l] = state.ToString();
                        l = l + 1;
                        state.Clear();

                    }
                    last = 3;
                    state.Append(sadr[i]);
                    if (sadr.Length - 1 == i)
                    {
                        calc[l] = state.ToString();
                    }

                }
                else
                {
                    if (last == 1 || last == 3)
                    {
                        calc[l] = state.ToString();
                        l = l + 1;
                        state.Clear();

                    }
                    last = 2;
                    state.Append(sadr[i]);
                    if (sadr.Length - 1 == i)
                    {
                        calc[l] = state.ToString();
                    }

                }
            }

            int finalvalue = 0;
            (bool value, char artm) artmtk = (false, ' ');

            for (int i = 0; i < l + 1; i++)
            {


                if (Char.IsDigit(calc[i][0]))
                {
                    if (artmtk.value)
                    {
                        switch (artmtk.artm)
                        {
                            case '+':
                                finalvalue = finalvalue + int.Parse(calc[i]);
                                break;
                            case '-':
                                finalvalue = finalvalue - int.Parse(calc[i]);
                                break;
                            case '*':
                                finalvalue = finalvalue * int.Parse(calc[i]);
                                break;
                            case '/':
                                finalvalue = finalvalue / int.Parse(calc[i]);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        finalvalue = int.Parse(calc[i]);
                    }


                }
                else if (calc[i][0] == '+')
                {
                    artmtk = (true, '+');
                }
                else if (calc[i][0] == '-')
                {
                    artmtk = (true, '-');

                }
                else if (calc[i][0] == '*')
                {
                    artmtk = (true, '*');

                }
                else if (calc[i][0] == '/')
                {
                    artmtk = (true, '/');

                }
                else
                {
                    if (artmtk.value)
                    {
                        switch (artmtk.artm)
                        {
                            case '+':
                                (BitArray r0, int b0) = WithRegister(calc[i]);

                                int value0 = 0;

                                for (int j = 0; j < b0; j++)
                                { value0 |= (Convert.ToByte(r0.Get(j)) << j); }

                                finalvalue = finalvalue + value0;

                                break;
                            case '-':
                                (BitArray r1, int b1) = WithRegister(calc[i]);

                                int value1 = 0;

                                for (int j = 0; j < b1; j++)
                                { value1 |= (Convert.ToByte(r1.Get(j)) << j); }

                                finalvalue = finalvalue - value1;
                                break;
                            case '*':
                                (BitArray r2, int b2) = WithRegister(calc[i]);

                                int value2 = 0;

                                for (int j = 0; j < b2; j++)
                                { value2 |= (Convert.ToByte(r2.Get(j)) << j); }

                                finalvalue = finalvalue * value2;
                                break;
                            case '/':
                                (BitArray r3, int b3) = WithRegister(calc[i]);

                                int value3 = 0;

                                for (int j = 0; j < b3; j++)
                                { value3 |= (Convert.ToByte(r3.Get(j)) << j); }

                                finalvalue = finalvalue / value3;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        (BitArray r, int b) = WithRegister(calc[i]);

                        int value = 0;

                        for (int j = 0; j < b; j++)
                        { value |= (Convert.ToByte(r.Get(j)) << j); }

                        finalvalue = value;
                    }
                }

            }


            return finalvalue;

        }
        private static void ShowStack()
        {
           asmstack.Show();
        }
        private static void ShowEflags()
        {
            string[] flags = new string[22] { "CF", "?", "PF", "?","AF", "?", "ZF", "SF", "TF", "IF", "DF", "OF", "IOPL", "IOPL", "NT", "?", "RF", "VM", "AC", "VIF", "VIP", "ID" };
            Console.Write("[");
            for (int i = 0; i < 32; i++)
            {
                if (i < 22)
                {
                    Console.Write(flags[i]);
                    Console.Write(":");
                }
                Console.Write(Convert.ToInt32(registers.eflags.Get(i)));

                if (i == 31)
                {
                    break;
                }
                Console.Write(", ");

            }
            Console.Write("]\n");
        }
        private static void ShowInstruction()
        {
            foreach (var item in code)
            {
                Console.WriteLine($"On Offset: 0x{item.Key.ToString("X2")} Value: {item.Value.asminstruction} {item.Value.instruction0} {item.Value.instruction1}");
            }
        }

        private static void ShowHeap()
        {
            foreach (var item in assembler.memory)
            {
                Console.WriteLine("ADDR:{0}", item.Key);
                for (int i = 0; i < item.Value.Length; i++)
                {
                    Console.Write(item.Value[i]);
                }
            }
        }

        private static void DumpHistory()
        {
            StringBuilder sb = new StringBuilder("");

            Console.WriteLine("DUMPING...");

            foreach (var item in code)
            {
                sb.Append(item.Value.asminstruction);
                sb.Append(" ");
                sb.Append(item.Value.instruction0);
                sb.Append(",");
                sb.Append(item.Value.instruction1);
                sb.Append("\n");

            }
            using (StreamWriter file = new StreamWriter(@"history.txt"))
            {
                file.WriteLine(sb.ToString()); 
            }
            Console.WriteLine("END");
        }
    }
}
