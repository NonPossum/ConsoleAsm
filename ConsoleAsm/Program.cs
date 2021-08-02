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
            //INIT CATCHE WITH REGISTERS 
            Registers.Add("rax", Tuple.Create(registers.rax, 64));
            Registers.Add("rcx", Tuple.Create(registers.rcx, 64));
            Registers.Add("rdx", Tuple.Create(registers.rdx, 64));
            Registers.Add("rbx", Tuple.Create(registers.rbx, 64));
            Registers.Add("rsp", Tuple.Create(registers.rsp, 64));
            Registers.Add("rbp", Tuple.Create(registers.rbp, 64));
            Registers.Add("rsi", Tuple.Create(registers.rsi, 64));
            Registers.Add("rdi", Tuple.Create(registers.rdi, 64));

            Registers.Add("eax", Tuple.Create(registers.rax, 32));
            Registers.Add("ecx", Tuple.Create(registers.rcx, 32));
            Registers.Add("edx", Tuple.Create(registers.rdx, 32));
            Registers.Add("ebx", Tuple.Create(registers.rbx, 32));
            Registers.Add("esp", Tuple.Create(registers.rsp, 32));
            Registers.Add("ebp", Tuple.Create(registers.rbp, 32));
            Registers.Add("esi", Tuple.Create(registers.rsi, 32));
            Registers.Add("edi", Tuple.Create(registers.rdi, 32));

            Registers.Add("ax", Tuple.Create(registers.rax, 16));
            Registers.Add("cx", Tuple.Create(registers.rcx, 16));
            Registers.Add("dx", Tuple.Create(registers.rdx, 16));
            Registers.Add("bx", Tuple.Create(registers.rbx, 16));
            Registers.Add("sp", Tuple.Create(registers.rsp, 16));
            Registers.Add("bp", Tuple.Create(registers.rbp, 16));
            Registers.Add("si", Tuple.Create(registers.rsi, 16));
            Registers.Add("di", Tuple.Create(registers.rdi, 16));

            Registers.Add("ah", Tuple.Create(registers.rax, 8));
            Registers.Add("al", Tuple.Create(registers.rax, 8));
            Registers.Add("ch", Tuple.Create(registers.rcx, 8));
            Registers.Add("cl", Tuple.Create(registers.rcx, 8));
            Registers.Add("dh", Tuple.Create(registers.rdx, 8));
            Registers.Add("dl", Tuple.Create(registers.rdx, 8));
            Registers.Add("bh", Tuple.Create(registers.rbx, 8));
            Registers.Add("bl", Tuple.Create(registers.rbx, 8));
            Registers.Add("spl", Tuple.Create(registers.rsp, 8));
            Registers.Add("bpl", Tuple.Create(registers.rbp, 8));
            Registers.Add("sil", Tuple.Create(registers.rsi, 8));
            Registers.Add("dil", Tuple.Create(registers.rdi, 8));

            Registers.Add(NOT_A_INSTRUCTION, Tuple.Create(registers.rax, 0));





            //later %include        'functions.asm'
            string origin = "bits=64;history=10;stack=0x1024;";
            string[] con = new string[5];
            string config = System.IO.File.ReadAllText("consoleasm.config");
            config = String.Concat(config.Where(c => !Char.IsWhiteSpace(c)));
            if (config.Equals(origin)) { return; };
            con = config.Split(';', '=');

            registers.bits = byte.Parse(con[1]);

            //TODO:
            // Liczba zapisywanych instrukcji w tył = con[3];
            asmstack.stacksize = Convert.ToInt32(con[5], 16);


            
        }


        public static Dictionary<uint, lineofcode> code = new Dictionary<uint, lineofcode>();
        public static asmstack stack = new asmstack();
        public static asmheap heap = new asmheap();
        public static asmdata data = new asmdata();
        public static registers Register = new registers();


        public static Dictionary<string, Tuple<BitArray, int>> Registers = new Dictionary<string, Tuple<BitArray, int>>();


        public static Dictionary<string, string> Variables = new Dictionary<string, string>();
        // db dd etc.




        static void Main(string[] args)
        {
            uint instructioncounter = 0x500;
            init();
            Console.WriteLine("ConsoleAsm 1.0.0\n[C# implementation] https://github.com/DeVianney\nType help, copyright, credits or license for more information.\n");
            lineofcode ri = new lineofcode();
            while (true)
            {
                string codeline = Console.ReadLine();
                //.multiplyLoop: dodać obsługę pętli
                //_start: dodać obsługę funckji
                if (codeline.StartsWith("$"))
                {
                    Command(codeline);
                    continue;
                }
                if (codeline.StartsWith("."))
                { // dodawanie lable jmp etc.
                    Data(codeline);
                    continue;
                }
                if (codeline.EndsWith(":"))
                {
                    Label(codeline);
                    continue;
                }

                //TODO:
                // dodać przekazywanie wielkości => word ptr [ebp+var_418]
                // LEA ESI, [EBX + 8*EAX + 4] dodać obsługę takich potworków
                // Bzdurne komendy nie powinny powodować crasha programu
                // przechwytywanie ASCI '' or ""
                ri = ReadInstructions(codeline);
                if (ri.asminstruction == NOT_A_INSTRUCTION) { Console.ResetColor(); continue; }
                Parse(ri);
                //instructioncounter = History(ri,instructioncounter);



            }

        }


        static BitArray reg0;
        static BitArray reg1;


        private static int Parse(lineofcode ri)
        {
            // powinniśmy wiedzieć jakie rejestry + jaka instrukcja czyli funkcja do wywołania
            // ZLE SIE PREZENTUJE
            // SPRAWDZENIE JAKA TO INSTRUKCJA
            int counter = 0;


            int numeric0 = 0;
            int numeric1 = 0;
            int bits0 = 0;
            int bits1 = 0;


            // if dla 1 argumentu
            if (!ri.instruction0.Equals(NOT_A_INSTRUCTION))
            {
                counter++;
                if (ri.instruction0[0] == '[')
                {

                    if (Char.IsDigit(ri.instruction0[1]))
                    {
                        numeric0 =  GetValueFromAddress(int.Parse(ri.instruction0));

                    }
                    else if (ri.instruction0.Contains('+') || ri.instruction0.Contains('-'))
                    {
                        numeric0 = CalcAddr();

                    }
                    else
                    {
                        (reg0, bits0) = WithRegister(ri.instruction0);
                        if (bits0 == 0)
                        {
                            return 3;

                        }

                    }


                   
                    if (ri.instruction1.Equals(NOT_A_INSTRUCTION))
                    {
                        ExecuteInstruction<int>(ri.asminstruction, numeric0, bits0);
                    }


                }
                else if (Char.IsDigit(ri.instruction0[0]))
                {
                    numeric0 = int.Parse(ri.instruction0);
                    if (ri.instruction1.Equals(NOT_A_INSTRUCTION))
                    {
                        ExecuteInstruction<int>(ri.asminstruction, numeric0, bits0);
                    }
                }
                else if (ri.instruction0[0] == '"' || ri.instruction0 == "'")
                {
                    numeric0 = GetValueFromASCII(ri.instruction0);
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


            // if dla 2 argumentu
            if (!ri.instruction1.Equals(NOT_A_INSTRUCTION))
            {
                counter++;
                if (ri.instruction1[0] == '[')
                {
                    //address checking

                    (reg1, bits1) = WithRegister(ri.instruction1);
                    if (bits1 == 0)
                    {
                        return 3;

                    }
                    numeric1 = GetValueFromAddress(int.Parse(ri.instruction1));
                    if (reg0 == null)
                    {
                        ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);
                    }
                    else
                    {
                        ExecuteInstruction<int, BitArray>(ri.asminstruction, numeric0, reg0, bits0, bits1);
                    }
                }
                else if (Char.IsDigit(ri.instruction1[0]))
                {
                    numeric1 = int.Parse(ri.instruction1);
                    if (reg0 == null)
                    {
                        ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);
                    }
                    else
                    {
                        ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);
                    }
                }
                else if (ri.instruction1[0] == '"' || ri.instruction1 == "'")
                {
                    numeric1 = GetValueFromASCII(ri.instruction0);
                    if (reg0 == null)
                    {
                        ExecuteInstruction<int, int>(ri.asminstruction, numeric0, numeric1, bits0, bits1);
                    }
                    else
                    {
                        ExecuteInstruction<int, BitArray>(ri.asminstruction, numeric1, reg0, bits0, bits1);
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
                        ExecuteInstruction<BitArray, int>(ri.asminstruction, reg0, numeric1, bits0, bits1);
                    }


                }
            }

            if (counter == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"ERROR 325: Invalid instruction => '{ri}'");
                Console.ResetColor();
            }

            // czy można podzielić szukanie instrukcji na 2 i 1 argumentowe
            // wywoływanie w withInstruction

            // add eax,ebx
            // mov eax,34
            // push ebp 
            // xor edi,0x34
            //
            //

            reg0 = null;
            reg1 = null;
            return 0;


        }
        private static lineofcode ReadInstructions(string codeline)
        {
            // Zczytuje pierwszy do spacji a następnie zczytuje do przecinka 
            // xor eax, eax numer instrucji
            //  asdf 
            //  asd 
            //  pop eax
            //  asd asd 
            // zwracanie NAI 
            // ZLE SIE PREZENTUJE
            bool isthree = false;

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
                        System.Environment.Exit(3);
                    }
                    if (codeline[i - 1] == 0x20)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine($"ERROR 323: Space before ',' => '{codeline}'");
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
                Console.Error.WriteLine("ERROR 325: Invalid instruction");
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
                case "lds":
                    instructions.Lds(Arg0, Arg1, bits0, bits1);
                    break;
                case "lea":
                    instructions.Lea(Arg0, Arg1, bits0, bits1);
                    break;
                case "les":
                    instructions.Les(Arg0, Arg1, bits0, bits1);
                    break;
                case "mov":
                    instructions.Mov(Arg0, Arg1, bits0, bits1);
                    break;
                case "or":
                    instructions.Or(Arg0, Arg1, bits0, bits1);
                    break;
                case "rcr":
                    instructions.Rcr(Arg0, Arg1, bits0, bits1);
                    break;
                case "rcl":
                    instructions.Rcl(Arg0, Arg1, bits0, bits1);
                    break;
                case "rol":
                    instructions.Rol(Arg0, Arg1, bits0, bits1);
                    break;
                case "ror":
                    instructions.Ror(Arg0, Arg1, bits0, bits1);
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

        // wykonaj funkcje która obsługuje instruckcje 
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

                case "call":
                    instructions.Call(arg, bits);
                    break;
                case "pop":
                    instructions.Pop(arg, bits);
                    break;
                case "push":
                    instructions.Push(arg, bits);
                    break;
                case "not":
                    instructions.Not(arg, bits);
                    break;
                case "jmp":
                    instructions.Jmp(arg, bits);
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
                case "enter":
                    instructions.Enter(arg, bits);
                    break;
                case "leave":
                    instructions.Leave(arg, bits);
                    break;
                case "ret":
                    instructions.Ret(arg, bits);
                    break;
                case "retn":
                    instructions.Retn(arg, bits);
                    break;
                case "idiv":
                    instructions.Idiv(arg, bits);
                    break;
                case "imul":
                    instructions.Imul(arg, bits);
                    break;


            }
        jmp:
            switch (ri)
            {
                case "ja":
                    instructions.Ja(arg, bits);
                    break;
                case "jae":
                    instructions.Jae(arg, bits);
                    break;
                case "jb":
                    instructions.Jb(arg, bits);
                    break;
                case "jc":
                    instructions.Jc(arg, bits);
                    break;
                case "je":
                    instructions.Je(arg, bits);
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
                case "jna":
                    instructions.Jna(arg, bits);
                    break;
                case "jnae":
                    instructions.Jnae(arg, bits);
                    break;
                case "jnb":
                    instructions.Jnb(arg, bits);
                    break;
                case "jnbe":
                    instructions.Jnbe(arg, bits);
                    break;
                case "jnc":
                    instructions.Jnc(arg, bits);
                    break;
                case "jne":
                    instructions.Jne(arg, bits);
                    break;
                case "jng":
                    instructions.Jng(arg, bits);
                    break;
                case "jnge":
                    instructions.Jnge(arg, bits);
                    break;
                case "jno":
                    instructions.Jno(arg, bits);
                    break;
                case "jnle":
                    instructions.Jnle(arg, bits);
                    break;
                case "jnp":
                    instructions.Jnp(arg, bits);
                    break;
                case "jns":
                    instructions.Jns(arg, bits);
                    break;
                case "jnz":
                    instructions.Jnz(arg, bits);
                    break;
                case "jo":
                    instructions.Jo(arg, bits);
                    break;
                case "jp":
                    instructions.Jp(arg, bits);
                    break;
                case "jpe":
                    instructions.Jpe(arg, bits);
                    break;
                case "jpo":
                    instructions.Jpo(arg, bits);
                    break;
                case "js":
                    instructions.Js(arg, bits);
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
                Console.Error.WriteLine("ERROR 325: Invalid instruction");
                Console.ResetColor();
                return Registers[NOT_A_INSTRUCTION];
            }

            

        }
        public static int GetValueFromAddress(int addr)
        {
            //adres raw, adres z rejsestru, adres
            int[] value = heap.GetValue(addr);
            if (!value.Equals(0xFF))
            {
                return value[0];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine($"ERROR 326: Invalid Address => '{addr}'");
                Console.ResetColor();
                return 0;
            }

        }
        public static int GetValueFromASCII(string ASCII)
        {
            char val = ASCII[1];
            return (int)Char.GetNumericValue(val);
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
                    else { System.Console.WriteLine("usage: $val <register>"); }
                    break;
                case "$stack":
                    ShowStack();
                    break;
                case "$heap":
                    ShowHeap();
                    break;
                case "$exit":
                    Environment.Exit(0);
                    break;
            }
        }
        private static void Label(string arg)
        {
            lineofcode label = new lineofcode();
            label.asminstruction = arg;
            label.instruction0 = NOT_A_INSTRUCTION;
            label.instruction1 = NOT_A_INSTRUCTION;
            code.Add(0, label);
            /* zczytać z code następnie jeśli natrafi
               się na label wykonać wszystkie dalsze instrukcje
               aż do natrafienia na kolejny label
            foreach (var item in code)
            {
                
                
            }
            */
        }
        private static void Data(string arg)
        {
            //.data name, 4
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

        }
        private static void GetValue(string arg)
        {
            (BitArray reg, int bits) = WithRegister(arg);
            int regValue = 0;
            for (int i = 0; i < bits; i++)
            { regValue |= (Convert.ToByte(reg.Get(i)) << i); }
            System.Console.WriteLine(regValue);
            // sprawdzić czy działa 
        }
        private static int CalcAddr()
        {
            //      word ptr [ebp+var_418]
            //(mov) [esp+28h+var_18]


            return 3;

        }
        private static void ShowStack()
        {

        }

        private static void ShowHeap()
        {
            int pc = 0;
            string fill = "##################";
            string txt = $@"
              --------HEAP--------
            .-----.          .-----. 
            | .---'          `---. | 
            | |{fill}| | 
            | |                  | | 
            | |                  | | 
            | |                  | | 
            | |                  | | 
            | |                  | | 
            | |                  | | 
            | |                  | | 
            | |                  | | 
            | '---.          .---` | 
            '-----'          `-----`
            {pc}                      ";



        }

        private static uint History(lineofcode tosave, uint instructioncounter)
        {
            code.Add(instructioncounter + 1, tosave);
            return instructioncounter;


        }
        private static void ExecuteInstructions()
        {
            //Wykonywanie instrukcji wszystkich z 'code'
        }
    }
}
