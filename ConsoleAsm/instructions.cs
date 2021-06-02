using System;

namespace ConsoleAsm
{
    enum Instructions
    {
        SUB,ADD,MUL,DIV,MOV,AND,ORR,XOR,
        DB,DW,DD,PUSH,POP,JMP,CALL,JZ,JNE,CMP

    }
    class instructions<T0,T1>
    {
        public Instructions Asminstruction { get; set; }
        public T0 instruction0 { get; set; }
        public T1 instruction1 { get; set; }

        public instructions()
        {
        
            
        }

    }
}