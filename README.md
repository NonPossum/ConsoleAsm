# ConsoleAsm 1.0 

ConsoleAsm is an x86 Assembly Simulator written in c# so far  handling basic instructions, stack, stack frames, heap allocation with malloc, all 32 bit registers and functions provided to easily look at memory and registers.  
  
**Examples:** Loop based on eflag register state. Memory addressing with malloc and with stack frame.
<img  align="left" src="https://github.com/Ryuel/ConsoleAsm/blob/main/img/0.png">

**Warnings:** For 'reasons' from ebp always is taking base address and ret only marks when loop must end.

**Provided Functions**:  
`$val` - Show decimal value of registers or memory segment.  
`$hval` - Show hex value of registers or memory segment.  
`$flags`- Show value of eflags register.  
`$inst` - Show written instructions.  
`$heap` - Show values of all heap.  
`$stack`- Show values of all stack.  
`$dump` - Dump written instructions to file history.txt.  
`$leave`- Exit program.  


