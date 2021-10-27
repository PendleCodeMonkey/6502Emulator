# An emulator for the MOS 6502 processor in C# #

This repo contains the code for a MOS 6502 processor emulator library.

<br>

The 6502Emulator solution consists of the following projects:

- **6502EmulatorConsoleTestApp**: A simple console application that demonstrates the functionality of the library.
- **6502EmulatorLib**: The code for the library itself.
- **6502Emulator.Tests**: An extensive set of tests.

<br>

### Prerequisites

- [.NET Core 3.1 SDK](https://www.microsoft.com/net/download/core)
  
<br>

### Why was this created?

The simple answer is "just for fun" :-)  
There are already many 6502 emulators out there and, to be honest, this one doesn't bring anything new to the table. I just quite fancied writing an emulator for one of the processors I programmed for way back in the 1980's, and as the 6502 is about the simplest that I have used, it seemed like as good a place to start as any.  
Also, I used this project as a way to practice some Test Driven Development (TDD).
  
<br>

### What isn't supported?

This library does not support any of the undocumented 6502 instructions.  
It also currently does nothing for the BRK and RTI instructions.  
  
<br>

### Usage

The included **6502EmulatorConsoleTestApp** project demonstrates how to use the emulator. This application has a couple of simple 6502 code examples that it runs through the emulator.

From a developer's point of view, the emulator is used as follows:
1. Create an instance of the `Machine` class.
2. Load binary executable data into the machine by calling the `LoadExecutableData` method, supplying a byte array containing the binary data and the address at which the data should be loaded in memory.
3. Load any other binary data into the machine [if required] by calling the `LoadData` method, supplying a byte array containing the binary data and the address at which the data should be loaded in memory. The final parameter passed to `LoadData` should be `false` to avoid clearing all memory before loading the data (otherwise any previously loaded executable data will be lost).
4. Set the initial state of the machine (e.g. register values, flags, etc.) [if required] by calling the `SetState` method.
5. Call the `Execute` method to execute the loaded 6502 code.
6. Once execution has completed, the `GetState` method can be called to retrieve the final state of the machine (register values, flags, etc.).
7. The `Dump` method can be called to get a string detailing the final state of the machine (which can be useful for debugging purposes).

<br>

### Disassembler

The disassembler takes binary 6502 code and converts it to 6502 Assembly Language instructions. The included **6502EmulatorConsoleTestApp** project includes a simple demonstration of this.  

The disassembler functionality is used as follows:  
1. Create an instance of the `Machine` class and load executable data into it (as above).  
2. Create an instance of the `Disassembler` class, supplying the `Machine` class instance, the address in memory at which to start disassembly, and the length of the code (in bytes).  
3. Add non-executable data sections as required - these allow you to mark specific blocks of memory as containing data that is not executable (this tells the disassembler not to attempt to disassemble this data into [what would be invalid] 6502 instructions; instead, it will output them as byte values using a DB assembler directive).  
4. Call the `Disassemble` method to perform the disassembly. This method returns a collection of tuple values, each of which consists of the address of the instruction and a string containing the disassembled instruction; for example, (20000, "LDA #$50").  
5. Iterate through the collection of tuples, outputting each address and disassembled instruction string (see the **6502EmulatorConsoleTestApp** project for an example of this).

<br>


### Assembler

The assembler takes 6502 assembly language source code and generates 6502 binary code from it. The included **6502EmulatorConsoleTestApp** project includes a demonstration of this.  

The assembler functionality is used as follows:  
1. Create an instance of the `Assembler` class.  
2. Call the `Assemble` method, passing a list of strings containing the lines of 6502 assembly language source code (one string per line of source code). This method returns a tuple, the first value of which indicates if the assembler successfully processed the supplied source code, if so, the second tuple value is a collection of bytes consisting of the generated binary data.  
3. Once successfully assembled, the binary data can then be written out to a file or fed straight into the emulator to be executed (see the **6502EmulatorConsoleTestApp** project for an example of this).
4. If the assembler failed to process the source code then the `AsmErrors` property of the instance of the `Assembler` class can be accessed to obtain information about any errors that occurred during assembly.
  
<br>

### What next?

The following are features that are being considered for the future:  
1. Implement some form of interactive debugger (with features such as single stepping, breakpoint handling, etc.).
2. Add support for undocumented 6502 instructions.


<br>

### History

| Version | Details
|---:| ---
| 1.0.0 | Initial implementation of 6502 emulator.
| 1.1.0 | Added Disassembler support.
| 1.2.0 | Added Assembler functionality.


