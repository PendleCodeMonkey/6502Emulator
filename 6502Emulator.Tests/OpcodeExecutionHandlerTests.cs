using PendleCodeMonkey.MOS6502EmulatorLib;
using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MOS6502Emulator.Tests
{
	public class OpcodeExecutionHandlerTests
	{
		//
		// Tests for each 6502 instruction.
		//
		// These all follow this pattern:
		// 1) Arrange:
		//		a) Create a 6502 Machine
		//		b) Load executable data into the machine (i.e. just the binary data required for the single instruction being tested)
		//		c) Load other data into memory as required (when testing instructions that use addressing modes that involve accessing memory)
		//		d) Initialize the state of the machine as required (e.g. initializing register values, flags, etc.)
		// 2) Act:
		//		Get the machine to execute the instruction.
		// 3) Assert:
		//		Assert the results of executing the instruction (i.e. checking register values, flags, memory contents, etc.)
		//



		// *********************
		//			LDA
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDA_Immediate(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA9, data };        // LDA #data
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDA_Absolute(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xAD, 0x00, 0x04 };  // LDA $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400 and don't clear memory first (or we'd delete the executable code)

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDA_AbsoluteX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xBD, 0x00, 0x04 };  // LDA $0400,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[xReg], machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDA_AbsoluteY(byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xB9, 0x00, 0x04 };  // LDA $0400,Y
			Machine machine = new Machine();
			machine.SetState(Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[yReg], machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDA_ZeroPage(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA5, 0x10 };  // LDA $10
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0010, false);       // load data into memory at zero page address 0x10

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDA_ZeroPageX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xB5, 0x10 };  // LDA $10,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[xReg], machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, 2, ProcessorFlags.Zero)]
		[InlineData(1, 4, (ProcessorFlags)0)]
		[InlineData(2, 6, ProcessorFlags.Negative)]
		public void LDA_IndirectX(byte index, byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA1, 0x10 };  // LDA ($10,X)
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[index], machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDA_IndirectY(byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xB1, 0x10 };  // LDA ($10),Y
			Machine machine = new Machine();
			machine.SetState(Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[yReg], machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			LDX
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDX_Immediate(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA2, data };        // LDX #data
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDX_Absolute(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xAE, 0x00, 0x04 };  // LDX $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400 and don't clear memory first (or we'd delete the executable code)

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDX_AbsoluteY(byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xBE, 0x00, 0x04 };  // LDX $0400,Y
			Machine machine = new Machine();
			machine.SetState(Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[yReg], machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDX_ZeroPage(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA6, 0x10 };  // LDX $10
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0010, false);       // load data into memory at zero page address 0x10

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDX_ZeroPageY(byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xB6, 0x10 };  // LDX $10,Y
			Machine machine = new Machine();
			machine.SetState(Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[yReg], machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			LDY
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDY_Immediate(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA0, data };        // LDY #data
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDY_Absolute(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xAC, 0x00, 0x04 };  // LDY $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400 and don't clear memory first (or we'd delete the executable code)

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDY_AbsoluteX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xBC, 0x00, 0x04 };  // LDY $0400,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[xReg], machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void LDY_ZeroPage(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA4, 0x10 };  // LDY $10
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0010, false);       // load data into memory at zero page address 0x10

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void LDY_ZeroPageX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xB4, 0x10 };  // LDY $10,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, 0x80 };
			machine.LoadData(binData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(binData[xReg], machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			STA
		// *********************

		[Fact]
		public void STA_Absolute()
		{
			// Arrange
			byte[] code = new byte[] { 0x8D, 0x00, 0x04 };  // STA $0x0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: 123);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 2);
			Assert.Equal(123, dumpMem[0]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that the flags have not been altered.
		}

		[Fact]
		public void STA_AbsoluteX()
		{
			// Arrange
			byte[] code = new byte[] { 0x9D, 0x00, 0x04 };  // STA $0x0400,X
			Machine machine = new Machine();
			machine.SetState(A: 123, X: 2);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 4);
			Assert.Equal(0, dumpMem[0]);
			Assert.Equal(123, dumpMem[2]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STA_AbsoluteY()
		{
			// Arrange
			byte[] code = new byte[] { 0x99, 0x00, 0x04 };  // STA $0x0400,Y
			Machine machine = new Machine();
			machine.SetState(A: 123, Y: 2);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 4);
			Assert.Equal(0, dumpMem[0]);
			Assert.Equal(123, dumpMem[2]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STA_ZeroPage()
		{
			// Arrange
			byte[] code = new byte[] { 0x85, 0x20 };  // STA $0x20
			Machine machine = new Machine();
			machine.SetState(A: 123);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0020, 2);
			Assert.Equal(123, dumpMem[0]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STA_ZeroPageX()
		{
			// Arrange
			byte[] code = new byte[] { 0x95, 0x20 };  // STA $0x20,X
			byte xReg = 4;
			Machine machine = new Machine();
			machine.SetState(A: 123, X: xReg);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0020, 6);
			Assert.Equal(123, dumpMem[xReg]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STA_IndirectX()
		{
			// Arrange
			byte[] code = new byte[] { 0x81, 0x10 };  // STA ($10,X)
			byte xReg = 4;
			Machine machine = new Machine();
			machine.SetState(A: 123, X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04 };
			machine.LoadData(addrData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 4);
			Assert.Equal(123, dumpMem[1]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STA_IndirectY()
		{
			// Arrange
			byte[] code = new byte[] { 0x91, 0x10 };  // STA ($10),Y
			byte yReg = 4;
			Machine machine = new Machine();
			machine.SetState(A: 123, Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 6);
			Assert.Equal(123, dumpMem[yReg]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		// *********************
		//			STX
		// *********************

		[Fact]
		public void STX_Absolute()
		{
			// Arrange
			byte[] code = new byte[] { 0x8E, 0x00, 0x04 };  // STX $0x0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(X: 0x55);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 2);
			Assert.Equal(0x55, dumpMem[0]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that the flags have not been altered.
		}

		[Fact]
		public void STX_ZeroPage()
		{
			// Arrange
			byte[] code = new byte[] { 0x86, 0x20 };  // STX $0x20
			Machine machine = new Machine();
			machine.SetState(X: 0x55);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0020, 2);
			Assert.Equal(0x55, dumpMem[0]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STX_ZeroPageY()
		{
			// Arrange
			byte[] code = new byte[] { 0x96, 0x20 };  // STX $0x20,Y
			byte yReg = 4;
			Machine machine = new Machine();
			machine.SetState(X: 0x55, Y: yReg);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0020, 6);
			Assert.Equal(0x55, dumpMem[yReg]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}


		// *********************
		//			STY
		// *********************

		[Fact]
		public void STY_Absolute()
		{
			// Arrange
			byte[] code = new byte[] { 0x8C, 0x00, 0x04 };  // STY $0x0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(Y: 0xAA);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0400, 2);
			Assert.Equal(0xAA, dumpMem[0]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that the flags have not been altered.
		}

		[Fact]
		public void STY_ZeroPage()
		{
			// Arrange
			byte[] code = new byte[] { 0x84, 0x20 };  // STY $0x20
			Machine machine = new Machine();
			machine.SetState(Y: 0xAA);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0020, 2);
			Assert.Equal(0xAA, dumpMem[0]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}

		[Fact]
		public void STY_ZeroPageX()
		{
			// Arrange
			byte[] code = new byte[] { 0x94, 0x20 };  // STY $0x20,X
			byte xReg = 4;
			Machine machine = new Machine();
			machine.SetState(X: xReg, Y: 0xAA);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0020, 6);
			Assert.Equal(0xAA, dumpMem[xReg]);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);
		}


		// *********************
		//			TAX
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void TAX(byte accumulator, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xAA };        // TAX
			Machine machine = new Machine();
			machine.SetState(A: accumulator);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(accumulator, machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			TAY
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void TAY(byte accumulator, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xA8 };        // TAY
			Machine machine = new Machine();
			machine.SetState(A: accumulator);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(accumulator, machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			TSX
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void TSX(byte sp, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xBA };        // TSX
			Machine machine = new Machine();
			machine.SetState(S: sp);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(sp, machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			TXA
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void TXA(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x8A };        // TXA
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(xReg, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			TXS
		// *********************

		[Fact]
		public void TXS()
		{
			// Arrange
			byte[] code = new byte[] { 0x9A };        // TXS
			byte xReg = 0x30;
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(xReg, machine.Stack.S);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that TXS instruction does not affect the flags.
		}

		// *********************
		//			TYA
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void TYA(byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x98 };        // TYA
			Machine machine = new Machine();
			machine.SetState(Y: yReg);
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(yReg, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			PHA
		// *********************

		[Fact]
		public void PHA()
		{
			// Arrange
			byte[] code = new byte[] { 0x48 };        // PHA
			byte accumulator = 0x30;
			Machine machine = new Machine();
			machine.SetState(A: accumulator);
			byte initialS = machine.Stack.S;
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0100, 0x0100);           // Get the entire stack memory dump.
			Assert.Equal((byte)(initialS - 1), machine.Stack.S);        // Assert that the stack pointer has been decremented.
			Assert.Equal(accumulator, dumpMem[initialS]);               // Assert that the accumulator value occupies the correct location on the stack.
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that PHA instruction does not affect the flags.
		}

		// *********************
		//			PHP
		// *********************

		[Fact]
		public void PHP()
		{
			// Arrange
			byte[] code = new byte[] { 0x08 };        // PHP
			ProcessorFlags pf = ProcessorFlags.Carry | ProcessorFlags.Negative;
			Machine machine = new Machine();
			machine.SetState(flags: pf);
			byte initialS = machine.Stack.S;
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var dumpMem = machine.DumpMemory(0x0100, 0x0100);           // Get the entire stack memory dump.
			Assert.Equal((byte)(initialS - 1), machine.Stack.S);        // Assert that the stack pointer has been decremented.
			Assert.Equal((byte)pf, dumpMem[initialS]);                  // Assert that the ProcessorFlags value occupies the correct location on the stack.
			Assert.Equal(pf, machine.CPU.SR.Flags);                     // Assert that PHP instruction does not affect the flags (i.e is still Carry | Negative).
		}

		// *********************
		//			PLA
		// *********************

		[Theory]
		[InlineData(0, ProcessorFlags.Zero)]
		[InlineData(1, (ProcessorFlags)0)]
		[InlineData(0x80, ProcessorFlags.Negative)]
		public void PLA(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x68 };  // PLA
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			byte initialS = machine.Stack.S;
			machine.LoadData(binData, (ushort)(0x0100 + initialS), false);           // Load data value into memory location at the top of the stack
			initialS--;
			machine.SetState(S: initialS);            // Initialize stack pointer to be 1 byte before end of stack.

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(data, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
			Assert.Equal((byte)(initialS + 1), machine.Stack.S);        // Assert that the stack pointer has been incremented.
		}

		// *********************
		//			PLP
		// *********************

		[Theory]
		[InlineData(ProcessorFlags.Zero)]
		[InlineData((ProcessorFlags)0)]
		[InlineData(ProcessorFlags.Negative)]
		public void PLP(ProcessorFlags flags)
		{
			// Arrange
			byte[] code = new byte[] { 0x28 };  // PLP
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { (byte)flags };
			byte initialS = machine.Stack.S;
			machine.LoadData(binData, (ushort)(0x0100 + initialS), false);           // Load flags value into memory location at the top of the stack
			initialS--;
			machine.SetState(S: initialS);            // Initialize stack pointer to be 1 byte before end of stack.

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(flags, machine.CPU.SR.Flags);
			Assert.Equal((byte)(initialS + 1), machine.Stack.S);        // Assert that the stack pointer has been incremented.
		}

		// *********************
		//			ASL
		// *********************

		[Theory]
		[InlineData(0x01, 0x02, (ProcessorFlags)0)]
		[InlineData(0x82, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x40, 0x80, ProcessorFlags.Negative)]
		public void ASL_Accumulator(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x0A };  // ASL A
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: value);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x02, (ProcessorFlags)0)]
		[InlineData(0x82, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x40, 0x80, ProcessorFlags.Negative)]
		public void ASL_Absolute(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x0E, 0x00, 0x04 };  // ASL $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x02, (ProcessorFlags)0)]
		[InlineData(0x82, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x40, 0x80, ProcessorFlags.Negative)]
		public void ASL_AbsoluteX(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x1E, 0x00, 0x04 };  // ASL $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			byte xReg = 3;
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x02, (ProcessorFlags)0)]
		[InlineData(0x82, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x40, 0x80, ProcessorFlags.Negative)]
		public void ASL_ZeroPage(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x06, 0x40 };  // ASL $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x02, (ProcessorFlags)0)]
		[InlineData(0x82, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x40, 0x80, ProcessorFlags.Negative)]
		public void ASL_ZeroPageX(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x16, 0x40 };  // ASL $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			LSR
		// *********************

		[Theory]
		[InlineData(0x01, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x82, 0x41, (ProcessorFlags)0)]
		[InlineData(0x41, 0x20, ProcessorFlags.Carry)]
		public void LSR_Accumulator(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x4A };  // LSR A
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: value);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x82, 0x41, (ProcessorFlags)0)]
		[InlineData(0x41, 0x20, ProcessorFlags.Carry)]
		public void LSR_Absolute(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x4E, 0x00, 0x04 };  // LSR $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x82, 0x41, (ProcessorFlags)0)]
		[InlineData(0x41, 0x20, ProcessorFlags.Carry)]
		public void LSR_AbsoluteX(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x5E, 0x00, 0x04 };  // LSR $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			byte xReg = 3;
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x82, 0x41, (ProcessorFlags)0)]
		[InlineData(0x41, 0x20, ProcessorFlags.Carry)]
		public void LSR_ZeroPage(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x46, 0x40 };  // LSR $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x82, 0x41, (ProcessorFlags)0)]
		[InlineData(0x41, 0x20, ProcessorFlags.Carry)]
		public void LSR_ZeroPageX(byte value, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x56, 0x40 };  // LSR $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			ROL
		// *********************

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x02, (ProcessorFlags)0)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x03, (ProcessorFlags)0)]
		[InlineData(0x82, (ProcessorFlags)0, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x80, ProcessorFlags.Carry, 0x01, ProcessorFlags.Carry)]
		[InlineData(0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative)]
		[InlineData(0x40, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative)]
		[InlineData(0xC0, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void ROL_Accumulator(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x2A };  // ROL A
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: value, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x02, (ProcessorFlags)0)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x03, (ProcessorFlags)0)]
		[InlineData(0x82, (ProcessorFlags)0, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x80, ProcessorFlags.Carry, 0x01, ProcessorFlags.Carry)]
		[InlineData(0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative)]
		[InlineData(0x40, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative)]
		[InlineData(0xC0, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void ROL_Absolute(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x2E, 0x00, 0x04 };  // ROL $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x02, (ProcessorFlags)0)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x03, (ProcessorFlags)0)]
		[InlineData(0x82, (ProcessorFlags)0, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x80, ProcessorFlags.Carry, 0x01, ProcessorFlags.Carry)]
		[InlineData(0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative)]
		[InlineData(0x40, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative)]
		[InlineData(0xC0, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void ROL_AbsoluteX(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x3E, 0x00, 0x04 };  // ROL $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			byte xReg = 3;
			machine.SetState(X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x02, (ProcessorFlags)0)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x03, (ProcessorFlags)0)]
		[InlineData(0x82, (ProcessorFlags)0, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x80, ProcessorFlags.Carry, 0x01, ProcessorFlags.Carry)]
		[InlineData(0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative)]
		[InlineData(0x40, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative)]
		[InlineData(0xC0, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void ROL_ZeroPage(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x26, 0x40 };  // ROL $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x02, (ProcessorFlags)0)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x03, (ProcessorFlags)0)]
		[InlineData(0x82, (ProcessorFlags)0, 0x04, ProcessorFlags.Carry)]
		[InlineData(0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero)]
		[InlineData(0x80, ProcessorFlags.Carry, 0x01, ProcessorFlags.Carry)]
		[InlineData(0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative)]
		[InlineData(0x40, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative)]
		[InlineData(0xC0, ProcessorFlags.Carry, 0x81, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void ROL_ZeroPageX(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x36, 0x40 };  // ROL $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			ROR
		// *********************

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x80, ProcessorFlags.Carry | ProcessorFlags.Negative)]
		[InlineData(0x82, (ProcessorFlags)0, 0x41, (ProcessorFlags)0)]
		[InlineData(0x00, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero)]
		[InlineData(0x00, ProcessorFlags.Carry, 0x80, ProcessorFlags.Negative)]
		public void ROR_Accumulator(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x6A };  // ROR A
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: value, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x80, ProcessorFlags.Carry | ProcessorFlags.Negative)]
		[InlineData(0x82, (ProcessorFlags)0, 0x41, (ProcessorFlags)0)]
		[InlineData(0x00, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero)]
		[InlineData(0x00, ProcessorFlags.Carry, 0x80, ProcessorFlags.Negative)]
		public void ROR_Absolute(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x6E, 0x00, 0x04 };  // ROR $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x80, ProcessorFlags.Carry | ProcessorFlags.Negative)]
		[InlineData(0x82, (ProcessorFlags)0, 0x41, (ProcessorFlags)0)]
		[InlineData(0x00, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero)]
		[InlineData(0x00, ProcessorFlags.Carry, 0x80, ProcessorFlags.Negative)]
		public void ROR_AbsoluteX(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x7E, 0x00, 0x04 };  // ROR $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			byte xReg = 3;
			machine.SetState(X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x80, ProcessorFlags.Carry | ProcessorFlags.Negative)]
		[InlineData(0x82, (ProcessorFlags)0, 0x41, (ProcessorFlags)0)]
		[InlineData(0x00, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero)]
		[InlineData(0x00, ProcessorFlags.Carry, 0x80, ProcessorFlags.Negative)]
		public void ROR_ZeroPage(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x66, 0x40 };  // ROR $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 2);
			Assert.Equal(expectedResult, memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x01, ProcessorFlags.Carry, 0x80, ProcessorFlags.Carry | ProcessorFlags.Negative)]
		[InlineData(0x82, (ProcessorFlags)0, 0x41, (ProcessorFlags)0)]
		[InlineData(0x00, (ProcessorFlags)0, 0x00, ProcessorFlags.Zero)]
		[InlineData(0x00, ProcessorFlags.Carry, 0x80, ProcessorFlags.Negative)]
		public void ROR_ZeroPageX(byte value, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x76, 0x40 };  // ROR $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x00, 0x00, 0x00, value };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0040, 4);
			Assert.Equal(expectedResult, memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			AND
		// *********************

		[Theory]
		[InlineData(0x40, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 0x40, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void AND_Immediate(byte accumulator, byte andData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x29, andData };        // AND #andData
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 0x40, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void AND_Absolute(byte accumulator, byte andData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x2D, 0x00, 0x04 };  // AND $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { andData };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 0x40, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void AND_AbsoluteX(byte accumulator, byte andData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x3D, 0x00, 0x04 };  // AND $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, andData };
			machine.LoadData(binData, 0x0400, false);
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 0x40, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void AND_AbsoluteY(byte accumulator, byte andData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x39, 0x00, 0x04 };  // AND $0400,Y
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, andData };
			machine.LoadData(binData, 0x0400, false);
			byte yReg = 3;
			machine.SetState(A: accumulator, Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 0x40, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void AND_ZeroPage(byte accumulator, byte andData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x25, 0x40 };  // AND $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { andData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 0x40, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void AND_ZeroPageX(byte accumulator, byte andData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x35, 0x40 };  // AND $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, andData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 2, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 4, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 6, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 8, 0x00, ProcessorFlags.Zero)]
		public void AND_IndirectX(byte accumulator, byte xReg, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x21, 0x10 };  // AND ($10,X)
			Machine machine = new Machine();
			machine.SetState(A: accumulator, X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04, 0x03, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x80, 0x40, 0xFF };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 1, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 2, 0x80, ProcessorFlags.Negative)]
		[InlineData(0xC0, 3, 0x40, (ProcessorFlags)0)]
		[InlineData(0x00, 4, 0x00, ProcessorFlags.Zero)]
		public void AND_IndirectY(byte accumulator, byte yReg, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x31, 0x10 };  // AND ($10),Y
			Machine machine = new Machine();
			machine.SetState(A: accumulator, Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x80, 0x40, 0xFF };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			BIT
		// *********************

		[Theory]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, ProcessorFlags.Zero)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero, ProcessorFlags.Overflow)]
		[InlineData(0x80, 0xFF, ProcessorFlags.Zero, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x01, 0x07, ProcessorFlags.Zero | ProcessorFlags.Negative | ProcessorFlags.Overflow, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, (ProcessorFlags)0, ProcessorFlags.Zero | ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		public void BIT_Absolute(byte accumulator, byte data, ProcessorFlags initFlags, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x2C, 0x00, 0x04 };  // BIT $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, ProcessorFlags.Zero)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero, ProcessorFlags.Overflow)]
		[InlineData(0x80, 0xFF, ProcessorFlags.Zero, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x01, 0x07, ProcessorFlags.Zero | ProcessorFlags.Negative | ProcessorFlags.Overflow, (ProcessorFlags)0)]
		[InlineData(0x00, 0xFF, (ProcessorFlags)0, ProcessorFlags.Zero | ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		public void BIT_ZeroPage(byte accumulator, byte data, ProcessorFlags initFlags, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x24, 0x40 };  // BIT $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			EOR
		// *********************

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 0x80, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void EOR_Immediate(byte accumulator, byte eorData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x49, eorData };        // EOR #eorData
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 0x80, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void EOR_Absolute(byte accumulator, byte eorData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x4D, 0x00, 0x04 };  // EOR $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { eorData };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 0x80, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void EOR_AbsoluteX(byte accumulator, byte eorData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x5D, 0x00, 0x04 };  // EOR $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, eorData };
			machine.LoadData(binData, 0x0400, false);
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 0x80, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void EOR_AbsoluteY(byte accumulator, byte eorData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x59, 0x00, 0x04 };  // EOR $0400,Y
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, eorData };
			machine.LoadData(binData, 0x0400, false);
			byte yReg = 3;
			machine.SetState(A: accumulator, Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 0x80, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void EOR_ZeroPage(byte accumulator, byte eorData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x45, 0x40 };  // EOR $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { eorData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 0x80, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 0xFF, 0x00, ProcessorFlags.Zero)]
		public void EOR_ZeroPageX(byte accumulator, byte eorData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x55, 0x40 };  // EOR $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, eorData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 2, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 4, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 6, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 8, 0x00, ProcessorFlags.Zero)]
		public void EOR_IndirectX(byte accumulator, byte xReg, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x41, 0x10 };  // EOR ($10,X)
			Machine machine = new Machine();
			machine.SetState(A: accumulator, X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04, 0x03, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x80, 0x80, 0xFF };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 1, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 2, 0x40, (ProcessorFlags)0)]
		[InlineData(0x0F, 3, 0x8F, ProcessorFlags.Negative)]
		[InlineData(0xFF, 4, 0x00, ProcessorFlags.Zero)]
		public void EOR_IndirectY(byte accumulator, byte yReg, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x51, 0x10 };  // EOR ($10),Y
			Machine machine = new Machine();
			machine.SetState(A: accumulator, Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x80, 0x80, 0xFF };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			ORA
		// *********************

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 0xF0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 0x0F, 0x7F, (ProcessorFlags)0)]
		public void ORA_Immediate(byte accumulator, byte orData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x09, orData };        // ORA #eorData
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 0xF0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 0x0F, 0x7F, (ProcessorFlags)0)]
		public void ORA_Absolute(byte accumulator, byte orData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x0D, 0x00, 0x04 };  // ORA $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { orData };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 0xF0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 0x0F, 0x7F, (ProcessorFlags)0)]
		public void ORA_AbsoluteX(byte accumulator, byte orData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x1D, 0x00, 0x04 };  // ORA $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, orData };
			machine.LoadData(binData, 0x0400, false);
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 0xF0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 0x0F, 0x7F, (ProcessorFlags)0)]
		public void ORA_AbsoluteY(byte accumulator, byte orData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x19, 0x00, 0x04 };  // ORA $0400,Y
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, orData };
			machine.LoadData(binData, 0x0400, false);
			byte yReg = 3;
			machine.SetState(A: accumulator, Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 0xF0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 0x0F, 0x7F, (ProcessorFlags)0)]
		public void ORA_ZeroPage(byte accumulator, byte orData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x05, 0x40 };  // ORA $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { orData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 0x80, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 0xF0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 0x0F, 0x7F, (ProcessorFlags)0)]
		public void ORA_ZeroPageX(byte accumulator, byte orData, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x15, 0x40 };  // ORA $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, orData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 2, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 4, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 6, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 8, 0x7F, (ProcessorFlags)0)]
		public void ORA_IndirectX(byte accumulator, byte xReg, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x01, 0x10 };  // ORA ($10,X)
			Machine machine = new Machine();
			machine.SetState(A: accumulator, X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04, 0x03, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x80, 0xF0, 0x0F };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 1, 0x00, ProcessorFlags.Zero)]
		[InlineData(0xC0, 2, 0xC0, ProcessorFlags.Negative)]
		[InlineData(0x0F, 3, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x7F, 4, 0x7F, (ProcessorFlags)0)]
		public void ORA_IndirectY(byte accumulator, byte yReg, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x11, 0x10 };  // ORA ($10),Y
			Machine machine = new Machine();
			machine.SetState(A: accumulator, Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x80, 0xF0, 0x0F };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			ADC
		// *********************

		[Theory]
		[InlineData(0x40, 0x00, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 0x00, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x60, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 0x80, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		[InlineData(0x58, 0x46, ProcessorFlags.Decimal | ProcessorFlags.Carry, 0x05, ProcessorFlags.Carry | ProcessorFlags.Decimal)]
		[InlineData(0x12, 0x34, ProcessorFlags.Decimal, 0x46, ProcessorFlags.Decimal)]
		[InlineData(0x15, 0x26, ProcessorFlags.Decimal, 0x41, ProcessorFlags.Decimal)]
		[InlineData(0x81, 0x92, ProcessorFlags.Decimal, 0x73, ProcessorFlags.Carry | ProcessorFlags.Decimal)]
		public void ADC_Immediate(byte accumulator, byte add, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x69, add };        // ADC #add
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 0x00, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x60, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 0x80, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_Absolute(byte accumulator, byte add, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x6D, 0x00, 0x04 };  // ADC $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { add };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 0x00, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x60, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 0x80, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_AbsoluteX(byte accumulator, byte add, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x7D, 0x00, 0x04 };  // ADC $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, add };
			machine.LoadData(binData, 0x0400, false);
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 0x00, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x60, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 0x80, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_AbsoluteY(byte accumulator, byte add, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x79, 0x00, 0x04 };  // ADC $0400,Y
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, add };
			machine.LoadData(binData, 0x0400, false);
			byte yReg = 3;
			machine.SetState(A: accumulator, Y: yReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 0x00, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x60, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 0x80, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_ZeroPage(byte accumulator, byte add, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x65, 0x40 };  // ADC $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { add };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 0x00, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 0x00, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 0x80, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 0x60, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 0x80, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_ZeroPageX(byte accumulator, byte add, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x75, 0x40 };  // ADC $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, add };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 2, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 4, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 6, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 8, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 10, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 12, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_IndirectX(byte accumulator, byte xReg, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x61, 0x10 };  // ADC ($10,X)
			Machine machine = new Machine();
			machine.SetState(A: accumulator, X: xReg, flags: initFlags);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04, 0x03, 0x04, 0x04, 0x04, 0x05, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x00, 0x80, 0x40, 0x60, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x40, 1, (ProcessorFlags)0, 0x40, (ProcessorFlags)0)]
		[InlineData(0x40, 2, ProcessorFlags.Carry, 0x41, (ProcessorFlags)0)]
		[InlineData(0x80, 3, (ProcessorFlags)0, 0x00, ProcessorFlags.Carry | ProcessorFlags.Zero | ProcessorFlags.Overflow)]
		[InlineData(0x40, 4, (ProcessorFlags)0, 0x80, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x40, 5, ProcessorFlags.Carry, 0xA1, ProcessorFlags.Negative | ProcessorFlags.Overflow)]
		[InlineData(0x90, 6, ProcessorFlags.Carry, 0x11, ProcessorFlags.Carry | ProcessorFlags.Overflow)]
		public void ADC_IndirectY(byte accumulator, byte yReg, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0x71, 0x10 };  // ADC ($10),Y
			Machine machine = new Machine();
			machine.SetState(A: accumulator, Y: yReg, flags: initFlags);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x00, 0x80, 0x40, 0x60, 0x80 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			CMP
		// *********************

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_Immediate(byte accumulator, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC9, cmpData };        // CMP #cmpData
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_Absolute(byte accumulator, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xCD, 0x00, 0x04 };  // CMP $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { cmpData };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_AbsoluteX(byte accumulator, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xDD, 0x00, 0x04 };  // CMP $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, cmpData };
			machine.LoadData(binData, 0x0400, false);
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_AbsoluteY(byte accumulator, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xD9, 0x00, 0x04 };  // CMP $0400,Y
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, cmpData };
			machine.LoadData(binData, 0x0400, false);
			byte yReg = 3;
			machine.SetState(A: accumulator, Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_ZeroPage(byte accumulator, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC5, 0x40 };  // CMP $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { cmpData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_ZeroPageX(byte accumulator, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xD5, 0x40 };  // CMP $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, cmpData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 2, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 4, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 6, ProcessorFlags.Carry)]
		[InlineData(0x10, 8, ProcessorFlags.Negative)]
		[InlineData(0xE1, 10, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_IndirectX(byte accumulator, byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC1, 0x10 };  // AND ($10,X)
			Machine machine = new Machine();
			machine.SetState(A: accumulator, X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04, 0x03, 0x04, 0x04, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x40, 0x80, 0x30, 0x20 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 1, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 2, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 3, ProcessorFlags.Carry)]
		[InlineData(0x10, 4, ProcessorFlags.Negative)]
		[InlineData(0xE1, 5, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CMP_IndirectY(byte accumulator, byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xD1, 0x10 };  // CMP ($10),Y
			Machine machine = new Machine();
			machine.SetState(A: accumulator, Y: yReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x40, 0x80, 0x30, 0x20 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			CPX
		// *********************

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CPX_Immediate(byte xReg, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE0, cmpData };        // CPX #cmpData
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CPX_Absolute(byte xReg, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xEC, 0x00, 0x04 };  // CPX $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { cmpData };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CPX_ZeroPage(byte xReg, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE4, 0x40 };  // CPX $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { cmpData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			CPY
		// *********************

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CPY_Immediate(byte yReg, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC0, cmpData };        // CPY #cmpData
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CPY_Absolute(byte yReg, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xCC, 0x00, 0x04 };  // CPY $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { cmpData };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0xC0, 0x80, ProcessorFlags.Carry)]
		[InlineData(0x10, 0x30, ProcessorFlags.Negative)]
		[InlineData(0xE1, 0x20, ProcessorFlags.Negative | ProcessorFlags.Carry)]
		public void CPY_ZeroPage(byte yReg, byte cmpData, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC4, 0x40 };  // CPY $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { cmpData };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			SBC
		// *********************

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 0xB0, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 0x70, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		[InlineData(0x46, 0x12, ProcessorFlags.Decimal | ProcessorFlags.Carry, 0x34, ProcessorFlags.Carry | ProcessorFlags.Decimal)]
		[InlineData(0x40, 0x13, ProcessorFlags.Decimal | ProcessorFlags.Carry, 0x27, ProcessorFlags.Carry | ProcessorFlags.Decimal)]
		[InlineData(0x32, 0x02, ProcessorFlags.Decimal, 0x29, ProcessorFlags.Carry | ProcessorFlags.Decimal)]
		[InlineData(0x12, 0x21, ProcessorFlags.Decimal | ProcessorFlags.Carry, 0x91, ProcessorFlags.Decimal)]
		[InlineData(0x21, 0x34, ProcessorFlags.Decimal | ProcessorFlags.Carry, 0x87, ProcessorFlags.Decimal)]
		public void SBC_Immediate(byte accumulator, byte sub, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE9, sub };        // SBC #sub
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 0xB0, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 0x70, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_Absolute(byte accumulator, byte sub, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xED, 0x00, 0x04 };  // SBC $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { sub };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 0xB0, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 0x70, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_AbsoluteX(byte accumulator, byte sub, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xFD, 0x00, 0x04 };  // SBC $0400,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, sub };
			machine.LoadData(binData, 0x0400, false);
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 0xB0, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 0x70, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_AbsoluteY(byte accumulator, byte sub, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xF9, 0x00, 0x04 };  // SBC $0400,Y
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, sub };
			machine.LoadData(binData, 0x0400, false);
			byte yReg = 3;
			machine.SetState(A: accumulator, Y: yReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 0xB0, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 0x70, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_ZeroPage(byte accumulator, byte sub, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE5, 0x40 };  // SBC $40
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { sub };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			machine.SetState(A: accumulator, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 0x00, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 0x20, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 0x40, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 0xB0, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 0x70, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_ZeroPageX(byte accumulator, byte sub, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xF5, 0x40 };  // SBC $40,X
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x01, sub };
			machine.LoadData(binData, 0x0040, false);       // load data into memory at 0x0040
			byte xReg = 3;
			machine.SetState(A: accumulator, X: xReg, flags: initFlags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 2, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 4, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 6, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 8, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 10, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 12, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 14, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_IndirectX(byte accumulator, byte xReg, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE1, 0x10 };  // SBC ($10,X)
			Machine machine = new Machine();
			machine.SetState(A: accumulator, X: xReg, flags: initFlags);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0xFF, 0xFF, 0x00, 0x04, 0x01, 0x04, 0x02, 0x04, 0x03, 0x04, 0x04, 0x04, 0x05, 0x04, 0x06, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0x00, 0x20, 0x40, 0x20, 0x40, 0xB0, 0x70 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x00, 1, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 2, (ProcessorFlags)0, 0x1F, ProcessorFlags.Carry)]
		[InlineData(0x40, 3, ProcessorFlags.Carry, 0x00, ProcessorFlags.Zero | ProcessorFlags.Carry)]
		[InlineData(0x40, 4, ProcessorFlags.Carry, 0x20, ProcessorFlags.Carry)]
		[InlineData(0x40, 5, (ProcessorFlags)0, 0xFF, ProcessorFlags.Negative)]
		[InlineData(0x50, 6, ProcessorFlags.Carry, 0xA0, ProcessorFlags.Overflow | ProcessorFlags.Negative)]
		[InlineData(0xD0, 7, ProcessorFlags.Carry, 0x60, ProcessorFlags.Overflow | ProcessorFlags.Carry)]
		public void SBC_IndirectY(byte accumulator, byte yReg, ProcessorFlags initFlags, byte expectedResult, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xF1, 0x10 };  // SBC ($10),Y
			Machine machine = new Machine();
			machine.SetState(A: accumulator, Y: yReg, flags: initFlags);
			machine.LoadExecutableData(code, 0x0200);
			byte[] addrData = new byte[] { 0x00, 0x04 };
			machine.LoadData(addrData, 0x0010, false);
			byte[] binData = new byte[] { 0xFF, 0x00, 0x20, 0x40, 0x20, 0x40, 0xB0, 0x70 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedResult, machine.CPU.A);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			DEC
		// *********************

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0, ProcessorFlags.Negative)]
		public void DEC_Absolute(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xCE, 0x00, 0x04 };  // DEC $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400 and don't clear memory first.

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 2);
			Assert.Equal((byte)(data - 1), memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void DEC_AbsoluteX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xDE, 0x00, 0x04 };  // DEC $0400,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x01, 0x40, 0x00 };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 4);
			Assert.Equal((byte)(binData[xReg] - 1), memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0x01, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x00, ProcessorFlags.Negative)]
		public void DEC_ZeroPage(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC6, 0x10 };  // DEC $10
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0010, false);       // load data into memory at zero page address 0x10

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0010, 2);
			Assert.Equal((byte)(data - 1), memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void DEC_ZeroPageX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xD6, 0x10 };  // DEC $10,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0x01, 0x40, 0x00 };
			machine.LoadData(binData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0010, 4);
			Assert.Equal((byte)(binData[xReg] - 1), memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			DEX
		// *********************

		[Theory]
		[InlineData(0x01, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x00, ProcessorFlags.Negative)]
		public void DEX(byte xReg, ProcessorFlags expectedFlags)
		{
			byte[] code = new byte[] { 0xCA };      // DEX
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(X: xReg);

			machine.ExecuteInstruction();

			Assert.Equal((byte)(xReg - 1), machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			DEY
		// *********************

		[Theory]
		[InlineData(0x01, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x00, ProcessorFlags.Negative)]
		public void DEY(byte yReg, ProcessorFlags expectedFlags)
		{
			byte[] code = new byte[] { 0x88 };      // DEY
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(Y: yReg);

			machine.ExecuteInstruction();

			Assert.Equal((byte)(yReg - 1), machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			INC
		// *********************

		[Theory]
		[InlineData(0xFF, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x7F, ProcessorFlags.Negative)]
		public void INC_Absolute(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xEE, 0x00, 0x04 };  // INC $0400
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0400, false);       // load data into memory at 0x0400 and don't clear memory first.

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 2);
			Assert.Equal((byte)(data + 1), memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void INC_AbsoluteX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xFE, 0x00, 0x04 };  // INC $0400,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0xFF, 0x40, 0x7F };
			machine.LoadData(binData, 0x0400, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0400, 4);
			Assert.Equal((byte)(binData[xReg] + 1), memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(0xFF, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x7F, ProcessorFlags.Negative)]
		public void INC_ZeroPage(byte data, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE6, 0x10 };  // INC $10
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { data };
			machine.LoadData(binData, 0x0010, false);       // load data into memory at zero page address 0x10

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0010, 2);
			Assert.Equal((byte)(data + 1), memDump[0]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		[Theory]
		[InlineData(1, ProcessorFlags.Zero)]
		[InlineData(2, (ProcessorFlags)0)]
		[InlineData(3, ProcessorFlags.Negative)]
		public void INC_ZeroPageX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xF6, 0x10 };  // INC $10,X
			Machine machine = new Machine();
			machine.SetState(X: xReg);
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0xFF, 0xFF, 0x40, 0x7F };
			machine.LoadData(binData, 0x0010, false);

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0010, 4);
			Assert.Equal((byte)(binData[xReg] + 1), memDump[xReg]);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			INX
		// *********************

		[Theory]
		[InlineData(0xFF, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x7F, ProcessorFlags.Negative)]
		public void INX(byte xReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xE8 };      // INX
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(X: xReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((byte)(xReg + 1), machine.CPU.X);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}

		// *********************
		//			INY
		// *********************

		[Theory]
		[InlineData(0xFF, ProcessorFlags.Zero)]
		[InlineData(0x40, (ProcessorFlags)0)]
		[InlineData(0x7F, ProcessorFlags.Negative)]
		public void INY(byte yReg, ProcessorFlags expectedFlags)
		{
			// Arrange
			byte[] code = new byte[] { 0xC8 };      // INY
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(Y: yReg);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((byte)(yReg + 1), machine.CPU.Y);
			Assert.Equal(expectedFlags, machine.CPU.SR.Flags);
		}


		// *********************
		//			BRK
		// *********************

		[Fact]
		public void BRK()
		{
			// No implementation to be tested.
		}

		// *********************
		//			JMP
		// *********************

		[Fact]
		public void JMP_Absolute()
		{
			// Arrange
			byte[] code = new byte[] { 0x4C, 0x56, 0x04 };      // JMP $0456
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(0x0456, machine.CPU.PC);       // Assert that Program Counter is now $0456.
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that flags are unaffected by JMP instruction.
		}

		[Fact]
		public void JMP_Indirect()
		{
			// Arrange
			byte[] code = new byte[] { 0x6C, 0x00, 0x04 };      // JMP ($0400)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] binData = new byte[] { 0x78, 0x06 };
			machine.LoadData(binData, 0x0400, false);       // load data (16-bit value 0x0678) into memory at 0x0400

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(0x0678, machine.CPU.PC);       // Assert that Program Counter is now $00678 (the value stored in ($0400)).
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that flags are unaffected by JMP instruction.
		}

		// *********************
		//			JSR
		// *********************

		[Fact]
		public void JSR()
		{
			// Arrange
			byte[] code = new byte[] { 0x20, 0x56, 0x04 };      // JSR $0456
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte initialS = machine.Stack.S;

			// Act
			machine.ExecuteInstruction();

			// Assert
			var memDump = machine.DumpMemory(0x0100, 0x0100);       // Get stack memory dump
																	// Assert that 0x0203 (the address of the instruction immediately following the JSR) has been pushed onto the stack.
			Assert.Equal(0x02, memDump[0xFF]);
			Assert.Equal(0x03, memDump[0xFE]);
			Assert.Equal((byte)(initialS - 2), machine.Stack.S);    // Assert that stack pointer has been decremented by 2.
			Assert.Equal(0x0456, machine.CPU.PC);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that flags are unaffected by JSR instruction.
		}

		// *********************
		//			RTI
		// *********************

		[Fact]
		public void RTI()
		{
			// No implementation to be tested.
		}

		// *********************
		//			RTS
		// *********************

		[Fact]
		public void RTS()
		{
			// Arrange
			byte[] code = new byte[] { 0x60 };      // RTS
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			byte[] stackData = new byte[] { 0x34, 0x02 };      // Put address $0234 onto top of stack.
			machine.LoadData(stackData, 0x01FE, false);
			byte initialS = (byte)(machine.Stack.S - 2);
			machine.SetState(S: initialS);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((byte)(initialS + 2), machine.Stack.S);        // Assert that stack pointer has been incremented by 2.
			Assert.Equal(0x0234, machine.CPU.PC);
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that flags are unaffected by RTS instruction.
		}


		// *********************
		//			BCC
		// *********************

		[Theory]
		[InlineData((ProcessorFlags)0, 0x0242)]
		[InlineData(ProcessorFlags.Carry, 0x0202)]
		public void BCC(ProcessorFlags flags, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0x90, 0x40 };      // BCC (with offset of 0x40)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BCS
		// *********************

		[Theory]
		[InlineData((ProcessorFlags)0, 0x0202)]
		[InlineData(ProcessorFlags.Carry, 0x0242)]
		public void BCS(ProcessorFlags flags, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0xB0, 0x40 };      // BCS (with offset of 0x40)
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BEQ
		// *********************

		[Theory]
		[InlineData((ProcessorFlags)0, 0x40, 0x0202)]
		[InlineData(ProcessorFlags.Zero, 0x40, 0x0242)]
		[InlineData(ProcessorFlags.Zero, 0xF0, 0x01F2)]
		public void BEQ(ProcessorFlags flags, byte offset, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0xF0, offset };      // BEQ
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BMI
		// *********************

		[Theory]
		[InlineData((ProcessorFlags)0, 0x40, 0x0202)]
		[InlineData(ProcessorFlags.Negative, 0x40, 0x0242)]
		[InlineData(ProcessorFlags.Negative, 0xF0, 0x01F2)]
		public void BMI(ProcessorFlags flags, byte offset, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0x30, offset };      // BMI
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BNE
		// *********************

		[Theory]
		[InlineData(ProcessorFlags.Zero, 0x40, 0x0202)]
		[InlineData((ProcessorFlags)0, 0x40, 0x0242)]
		[InlineData((ProcessorFlags)0, 0xF0, 0x01F2)]
		public void BNE(ProcessorFlags flags, byte offset, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0xD0, offset };      // BNE
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BPL
		// *********************

		[Theory]
		[InlineData(ProcessorFlags.Negative, 0x40, 0x0202)]
		[InlineData((ProcessorFlags)0, 0x40, 0x0242)]
		[InlineData((ProcessorFlags)0, 0xF0, 0x01F2)]
		public void BPL(ProcessorFlags flags, byte offset, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0x10, offset };      // BPL
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BVC
		// *********************

		[Theory]
		[InlineData(ProcessorFlags.Overflow, 0x40, 0x0202)]
		[InlineData((ProcessorFlags)0, 0x40, 0x0242)]
		[InlineData((ProcessorFlags)0, 0xF0, 0x01F2)]
		public void BVC(ProcessorFlags flags, byte offset, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0x50, offset };      // BVC
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			BVS
		// *********************

		[Theory]
		[InlineData((ProcessorFlags)0, 0x40, 0x0202)]
		[InlineData(ProcessorFlags.Overflow, 0x40, 0x0242)]
		[InlineData(ProcessorFlags.Overflow, 0xF0, 0x01F2)]
		public void BVS(ProcessorFlags flags, byte offset, ushort expectedPC)
		{
			// Arrange
			byte[] code = new byte[] { 0x70, offset };      // BVS
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: flags);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(expectedPC, machine.CPU.PC);
			Assert.Equal(flags, machine.CPU.SR.Flags);
		}

		// *********************
		//			CLC
		// *********************

		[Fact]
		public void CLC()
		{
			// Arrange
			byte[] code = new byte[] { 0x18 };      // CLC
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: ProcessorFlags.Carry);    // Set Carry flag.

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that Carry flag has been cleared.
		}


		// *********************
		//			CLD
		// *********************

		[Fact]
		public void CLD()
		{
			// Arrange
			byte[] code = new byte[] { 0xD8 };      // CLD
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: ProcessorFlags.Decimal);    // Set Decimal flag.

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that Decimal flag has been cleared.
		}


		// *********************
		//			CLI
		// *********************

		[Fact]
		public void CLI()
		{
			// Arrange
			byte[] code = new byte[] { 0x58 };      // CLI
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: ProcessorFlags.Interrupt);    // Set Interrupt Disable flag.

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that Interrupt Disable flag has been cleared.
		}


		// *********************
		//			CLV
		// *********************

		[Fact]
		public void CLV()
		{
			// Arrange
			byte[] code = new byte[] { 0xB8 };      // CLV
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			machine.SetState(flags: ProcessorFlags.Overflow);    // Set Overflow flag.

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that Overflow flag has been cleared.
		}


		// *********************
		//			SEC
		// *********************

		[Fact]
		public void SEC()
		{
			// Arrange
			byte[] code = new byte[] { 0x38 };      // SEC
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(ProcessorFlags.Carry, machine.CPU.SR.Flags);      // Assert that Carry flag has been set.
		}

		// *********************
		//			SED
		// *********************

		[Fact]
		public void SED()
		{
			// Arrange
			byte[] code = new byte[] { 0xF8 };      // SED
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(ProcessorFlags.Decimal, machine.CPU.SR.Flags);      // Assert that Decimal flag has been set.
		}

		// *********************
		//			SEI
		// *********************

		[Fact]
		public void SEI()
		{
			// Arrange
			byte[] code = new byte[] { 0x78 };      // SEI
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal(ProcessorFlags.Interrupt, machine.CPU.SR.Flags);      // Assert that Interrupt Disable flag has been set.
		}

		// *********************
		//			NOP
		// *********************

		[Fact]
		public void NOP()
		{
			// Arrange
			byte[] code = new byte[] { 0xEA };      // NOP
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);

			// Act
			machine.ExecuteInstruction();

			// Assert
			Assert.Equal((ProcessorFlags)0, machine.CPU.SR.Flags);      // Assert that flags have not been affected.
		}

	}
}
