# ConsoleAsm Alpha 

ConsoleAsm is an x86 Assembly Simulator written in c# so far  handling basic instructions, stack, stack frames, heap allocation with malloc, all 32 bit registers and functions provided to easily look at memory and registers.  
**Examples:**  
Loop base on eflags state:

<img  align="left" src="https://github.com/DeVianney/ConsoleAsm/blob/main/img/0.png" width="417 " height="295">
Memory addressing with malloc:
<img  align="right" src="https://github.com/DeVianney/ConsoleAsm/blob/main/img/1.png" width="199" height="352">
Memory addressing with stack frame:
<img src="https://github.com/DeVianney/ConsoleAsm/blob/main/img/2.png" width="188" height="224">



Provided Functions:  
`$val` - Show decimal value of registers of memory segment.  
`$hval` - Show hex value of registers of memory segment.  
`$flags`- Show value of eflags register.  
`$inst` - Show writed instructions.  
`$heap` - Show value of all heap.  
`$stack`- Show value of all stack.  
`$dump` - Dump writed instructions to file history.txt.  
`$leave`- Exit program.  


