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

### What next?

The following are features that are being considered for the future:  
1. Implement a 6502 disassembler.
2. Implement some form of interactive debugger (with features such as single stepping, breakpoint handling, etc.).
3. Implement a 6502 assembler (because it's a tad frustrating having to supply 6502 executable data in binary format... something that is very reminiscent of the days when I used to type in machine code programs that they used to print in computer magazines in the 80's - urghhh!).
