using System;
using System.Collections.Generic;
using System.IO;
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
            string origin = "bits=64;history=10;stack=0x1024;";
            string[] con = new string[5];
            string config = System.IO.File.ReadAllText("consoleasm.config");
            config = String.Concat(config.Where(c => !Char.IsWhiteSpace(c)));
            if(config.Equals(origin)){return;};
            con = config.Split(';','=');
            
            registers.bits = byte.Parse(con[1]);
            //TODO:
            // Liczba zapisywanych instrukcji w tył = con[3];
            asmstack.stacksize = Convert.ToInt32(con[5], 16);
        }


        public static Dictionary<uint,lineofcode> code = new Dictionary<uint,lineofcode>();
        //Storing code with address(uint)
        public static asmstack stack = new asmstack();
        //Symulation of the stack this is just Stack structure but with implemented stack frame 

        public static registers Register = new registers();

        

        static void Main(string[] args)
        {
            uint instructioncounter = 0x500;
            init();   
            Console.WriteLine("ConsoleAsm 1.0.0\n[C# implementation] https://github.com/DeVianney\nType help, copyright, credits or license for more information.\n");
            lineofcode ri = new lineofcode();
            while(true){
            string codeline =  Console.ReadLine();
            if(codeline.StartsWith("$")){
                Command(codeline);
                continue;
            }
            //TODO:
            // Bzdurne komendy nie powinny powodować crasha programu
            ri = ReadInstructions(codeline);
            if(ri.asminstruction == NOT_A_INSTRUCTION){Console.ResetColor();continue;}
            Parse(ri);
            //instructioncounter = History(ri,instructioncounter);


            
            }
            
        }


        private static void Parse(lineofcode ri){
            // powinniśmy wiedzieć jakie rejestry + jaka instrukcja czyli funkcja do wywołania
            // ZLE SIE PREZENTUJE
            // SPRAWDZENIE JAKA TO INSTRUKCJA 
                WithInstruction(ri.asminstruction);

                if(ri.instruction0[0] == '['){
                    var reg = Registers.NAI;
                    reg = WithRegister(ri.instruction0);
                    if(reg == Registers.NAI){
            
                   }
                   else{
                       
                   }
                    
                }
                else if(Char.IsDigit(ri.instruction0[0])){
                    
                }
                else if(ri.instruction0 == "NAI"){
                    
                }
                else{
                     if(WithRegister(ri.instruction0) == Registers.NAI){
            
                   }
                   else{
                       
                   }
                }
                       
                if(ri.instruction1[0] == '['){
                   var reg = Registers.NAI;
                    reg = WithRegister(ri.instruction0);
                    if(reg == Registers.NAI){
            
                   }
                   else{
                       
                   }
                }
                 else if(Char.IsDigit(ri.instruction1[0])){
                    
                }
                 else if(ri.instruction1 == "NAI"){

                 }
                else{
                   if(WithRegister(ri.instruction1) == Registers.NAI){
                   }
                   else{

                   }
                }  

            

        }
        private static lineofcode ReadInstructions(string codeline){
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
                
                if(codeline[i] == 0x20)// 0x20 is a hex value of " "
                {
                    space = true;
                    if(codeline[i + 1] != 0x20){
                        line.asminstruction = sb.ToString().Trim();
                        sb.Clear();
                    }
                }

                if (codeline[i] == 0x2c)// 0x2C is a hex value of ","
                {
                    if(space == false){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR 324: There is no space before instructions. => '{codeline}'");
                        System.Environment.Exit(3);
                    }
                    if(codeline[i - 1 ] == 0x20){
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR 323: Space before ',' => '{codeline}'");
                        System.Environment.Exit(3);
                    }
                    isthree = true;
                    line.instruction0 = sb.ToString().Trim();
                    sb.Clear();
                    state = i + 1;
                    break;
                    
                }
                
                 sb.Append(codeline[i]);
                if(codeline.Length == i+1 && isthree == false){
                  line.instruction0 = sb.ToString().Trim();

                }
                
            }

            int len = codeline.Length - state;
            for (int i = 0; i < len; i++)
            {
                sb.Append(codeline[state]);
                state++;
                
            }
            if(space == false){
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR 325: Invalid instruction");
            Console.ResetColor();
            line.asminstruction = NOT_A_INSTRUCTION;
            return line;
            }
            if(isthree){
            line.instruction1 = sb.ToString().Trim().ToLower();
            line.asminstruction.ToLower();
            line.instruction0.ToLower();
            line.instruction1.ToLower();
            }
            else{
            line.asminstruction.ToLower();
            line.instruction0.ToLower();
            line.instruction1 = NOT_A_INSTRUCTION;
            }

            return line;
            

        }
        private static Instructions WithInstruction(string ri){
            return Instructions.ADD;

        }
        private static Registers WithRegister(string ri){
            //SPRAWDZA TYLKO JEDEN REJESTR
            System.Console.WriteLine(ri);
            int from = 0;
            int to   = 0;
            //  64 bit 0 - 7 32 bit 8 - 16
            string[] reg = new string[36] {"rax","rcx", "rdx","rbx", "rsp",
            "rbp", "rsi", "rdi", "eax", "ecx", "edx", "ebx", "esp", "ebp",
            "esi", "edi","ax", "ah", "al", "cx", "ch", "cl", "dx", "dh",
            "dl", "bx", "bh", "bl", "bp", "bpl", "spl", "si","sil", "di","dil","NAI"};
            char freg = ri[0];
            System.Console.WriteLine(ri[0]);
            //TODO:
            //Problem z zwracaniem odpowiedniego rejestru 
                switch (freg)
                {
                    case 'r':
                        from = 0;
                        to = 8;
                        break;
                    case 'e':
                        from = 8;
                        to = 16;
                        break;
                    case 'a':
                        from = 16;
                        to = 19;
    
                        break;
                    case 'c':
                        from = 19;
                        to = 22;
    
                        break;
                    case 'd':
                        from = 22;
                        to = 25;
    
                        break;
                    case 'b':
                       from = 25;
                        to = 30;
                        break;
                    case 's':
                        from = 30;
                        to = 35;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR 325: Invalid instruction => '{ri}'");
                        Console.ResetColor();
                        to = 36;
                        break;
            }
            System.Console.WriteLine($"from:{from},to:{to} FROMTO:{to - from}");
            if(to == 36){
                return Registers.NAI;
            }
            to = to - from; 
            for (int i = 0; i < to; i++)
            {
                System.Console.WriteLine($"{i},{reg[from]}");
                if(reg[from].Equals(ri)){
                    return (Registers)from;
                }
                from++;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR 325: Invalid instruction => '{ri}'");
            return Registers.RAX;
        }
        private static void Command(string command){
            switch (command)
            {
                case "$val":
                    GetValue(command);
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
        private static void GetValue(string command){

        }
        private static void ShowStack(){
            
        }
        
        private static void ShowHeap(){
            
        }
        
        private static uint History(lineofcode tosave,uint instructioncounter){
        code.Add(instructioncounter + 1,tosave);
        return instructioncounter;


        }
    }
}
