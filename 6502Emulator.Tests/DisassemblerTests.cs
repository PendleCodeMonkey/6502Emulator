using PendleCodeMonkey.MOS6502EmulatorLib;
using System;
using Xunit;

namespace PendleCodeMonkey.MOS6502Emulator.Tests
{
	public class DisassemblerTests
	{

		[Fact]
		public void NewDisassembler_ShouldNotBeNull()
		{
			Disassembler disassembler = new Disassembler(new Machine(), 0, 1);

			Assert.NotNull(disassembler);
		}

		[Fact]
		public void NewDisassemblerWithNullMachine_ShouldThrowException()
		{
			Disassembler disassembler;
			Assert.Throws<ArgumentNullException>(() => disassembler = new Disassembler(null, 0, 1));
		}

		[Fact]
		public void IsEndOfData_ShouldBeFalseWhenCurrentAddressWithinData()
		{
			Disassembler disassembler = new Disassembler(new Machine(), 0x2000, 6);

			Assert.False(disassembler.IsEndOfData);
		}

		[Fact]
		public void IsEndOfData_ShouldBeTrueWhenCurrentAddressIsPassedEndOfData()
		{
			Disassembler disassembler = new Disassembler(new Machine(), 0x2000, 6)
			{
				CurrentAddress = 0x2006
			};

			Assert.True(disassembler.IsEndOfData);
		}

		[Fact]
		public void AddNonExecutableSection_ShouldCreateNonExecutableSection()
		{
			Disassembler disassembler = new Disassembler(new Machine(), 0x2000, 0x0100);

			disassembler.AddNonExecutableSection(0x2010, 0x0040);

			Assert.Single(disassembler.NonExecutableSections);
			Assert.Equal(0x2010, disassembler.NonExecutableSections[0].Address);
			Assert.Equal(0x0040, disassembler.NonExecutableSections[0].Length);
		}

		[Fact]
		public void RemoveNonExecutableSection_ShouldRemoveANonExecutableSection()
		{
			Disassembler disassembler = new Disassembler(new Machine(), 0x2000, 0x0100);
			disassembler.AddNonExecutableSection(0x2010, 0x0040);
			disassembler.AddNonExecutableSection(0x2060, 0x0020);

			var removed = disassembler.RemoveNonExecutableSection(0);

			Assert.True(removed);
			Assert.Single(disassembler.NonExecutableSections);
			Assert.Equal(0x2060, disassembler.NonExecutableSections[0].Address);
			Assert.Equal(0x0020, disassembler.NonExecutableSections[0].Length);
		}

		[Fact]
		public void RemoveNonExecutableSection_ShouldReturnFalseIfDoesNotExist()
		{
			Disassembler disassembler = new Disassembler(new Machine(), 0x2000, 0x0100);
			disassembler.AddNonExecutableSection(0x2010, 0x0040);
			disassembler.AddNonExecutableSection(0x2060, 0x0020);

			var removed = disassembler.RemoveNonExecutableSection(3);		// 3 is out of range as only 2 sections have been added.

			Assert.False(removed);
			Assert.Equal(2, disassembler.NonExecutableSections.Count);
		}

		[Theory]
		[InlineData(new byte[] { 0xA9, 0x10 }, "LDA #$10")]
		[InlineData(new byte[] { 0xAD, 0x10, 0x30 }, "LDA $3010")]
		[InlineData(new byte[] { 0xBD, 0x10, 0x30 }, "LDA $3010,X")]
		[InlineData(new byte[] { 0xB9, 0x10, 0x30 }, "LDA $3010,Y")]
		[InlineData(new byte[] { 0xA5, 0x10 }, "LDA $10")]
		[InlineData(new byte[] { 0xB5, 0x10 }, "LDA $10,X")]
		[InlineData(new byte[] { 0xA1, 0x10 }, "LDA ($10,X)")]
		[InlineData(new byte[] { 0xB1, 0x10 }, "LDA ($10),Y")]
		public void Disassemble_LDA_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xA2, 0x10 }, "LDX #$10")]
		[InlineData(new byte[] { 0xAE, 0x10, 0x30 }, "LDX $3010")]
		[InlineData(new byte[] { 0xBE, 0x10, 0x30 }, "LDX $3010,Y")]
		[InlineData(new byte[] { 0xA6, 0x10 }, "LDX $10")]
		[InlineData(new byte[] { 0xB6, 0x10 }, "LDX $10,Y")]
		public void Disassemble_LDX_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xA0, 0x10 }, "LDY #$10")]
		[InlineData(new byte[] { 0xAC, 0x10, 0x30 }, "LDY $3010")]
		[InlineData(new byte[] { 0xBC, 0x10, 0x30 }, "LDY $3010,X")]
		[InlineData(new byte[] { 0xA4, 0x10 }, "LDY $10")]
		[InlineData(new byte[] { 0xB4, 0x10 }, "LDY $10,X")]
		public void Disassemble_LDY_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x8D, 0x10, 0x30 }, "STA $3010")]
		[InlineData(new byte[] { 0x9D, 0x10, 0x30 }, "STA $3010,X")]
		[InlineData(new byte[] { 0x99, 0x10, 0x30 }, "STA $3010,Y")]
		[InlineData(new byte[] { 0x85, 0x10 }, "STA $10")]
		[InlineData(new byte[] { 0x95, 0x10 }, "STA $10,X")]
		[InlineData(new byte[] { 0x81, 0x10 }, "STA ($10,X)")]
		[InlineData(new byte[] { 0x91, 0x10 }, "STA ($10),Y")]
		public void Disassemble_STA_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x8E, 0x10, 0x30 }, "STX $3010")]
		[InlineData(new byte[] { 0x86, 0x10 }, "STX $10")]
		[InlineData(new byte[] { 0x96, 0x10 }, "STX $10,Y")]
		public void Disassemble_STX_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x8C, 0x10, 0x30 }, "STY $3010")]
		[InlineData(new byte[] { 0x84, 0x10 }, "STY $10")]
		[InlineData(new byte[] { 0x94, 0x10 }, "STY $10,X")]
		public void Disassemble_STY_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xAA }, "TAX")]
		[InlineData(new byte[] { 0xA8 }, "TAY")]
		[InlineData(new byte[] { 0xBA }, "TSX")]
		[InlineData(new byte[] { 0x8A }, "TXA")]
		[InlineData(new byte[] { 0x9A }, "TXS")]
		[InlineData(new byte[] { 0x98 }, "TYA")]
		public void Disassemble_TransferOpcodes_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x48 }, "PHA")]
		[InlineData(new byte[] { 0x08 }, "PHP")]
		[InlineData(new byte[] { 0x68 }, "PLA")]
		[InlineData(new byte[] { 0x28 }, "PLP")]
		public void Disassemble_StackOpcodes_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x0A }, "ASL A")]
		[InlineData(new byte[] { 0x0E, 0x10, 0x30 }, "ASL $3010")]
		[InlineData(new byte[] { 0x1E, 0x10, 0x30 }, "ASL $3010,X")]
		[InlineData(new byte[] { 0x06, 0x10 }, "ASL $10")]
		[InlineData(new byte[] { 0x16, 0x10 }, "ASL $10,X")]
		public void Disassemble_ASL_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x4A }, "LSR A")]
		[InlineData(new byte[] { 0x4E, 0x10, 0x30 }, "LSR $3010")]
		[InlineData(new byte[] { 0x5E, 0x10, 0x30 }, "LSR $3010,X")]
		[InlineData(new byte[] { 0x46, 0x10 }, "LSR $10")]
		[InlineData(new byte[] { 0x56, 0x10 }, "LSR $10,X")]
		public void Disassemble_LSR_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x2A }, "ROL A")]
		[InlineData(new byte[] { 0x2E, 0x10, 0x30 }, "ROL $3010")]
		[InlineData(new byte[] { 0x3E, 0x10, 0x30 }, "ROL $3010,X")]
		[InlineData(new byte[] { 0x26, 0x10 }, "ROL $10")]
		[InlineData(new byte[] { 0x36, 0x10 }, "ROL $10,X")]
		public void Disassemble_ROL_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x6A }, "ROR A")]
		[InlineData(new byte[] { 0x6E, 0x10, 0x30 }, "ROR $3010")]
		[InlineData(new byte[] { 0x7E, 0x10, 0x30 }, "ROR $3010,X")]
		[InlineData(new byte[] { 0x66, 0x10 }, "ROR $10")]
		[InlineData(new byte[] { 0x76, 0x10 }, "ROR $10,X")]
		public void Disassemble_ROR_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x29, 0x10 }, "AND #$10")]
		[InlineData(new byte[] { 0x2D, 0x10, 0x30 }, "AND $3010")]
		[InlineData(new byte[] { 0x3D, 0x10, 0x30 }, "AND $3010,X")]
		[InlineData(new byte[] { 0x39, 0x10, 0x30 }, "AND $3010,Y")]
		[InlineData(new byte[] { 0x25, 0x10 }, "AND $10")]
		[InlineData(new byte[] { 0x35, 0x10 }, "AND $10,X")]
		[InlineData(new byte[] { 0x21, 0x10 }, "AND ($10,X)")]
		[InlineData(new byte[] { 0x31, 0x10 }, "AND ($10),Y")]
		public void Disassemble_AND_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x2C, 0x10, 0x30 }, "BIT $3010")]
		[InlineData(new byte[] { 0x24, 0x10 }, "BIT $10")]
		public void Disassemble_BIT_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x49, 0x10 }, "EOR #$10")]
		[InlineData(new byte[] { 0x4D, 0x10, 0x30 }, "EOR $3010")]
		[InlineData(new byte[] { 0x5D, 0x10, 0x30 }, "EOR $3010,X")]
		[InlineData(new byte[] { 0x59, 0x10, 0x30 }, "EOR $3010,Y")]
		[InlineData(new byte[] { 0x45, 0x10 }, "EOR $10")]
		[InlineData(new byte[] { 0x55, 0x10 }, "EOR $10,X")]
		[InlineData(new byte[] { 0x41, 0x10 }, "EOR ($10,X)")]
		[InlineData(new byte[] { 0x51, 0x10 }, "EOR ($10),Y")]
		public void Disassemble_EOR_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x09, 0x10 }, "ORA #$10")]
		[InlineData(new byte[] { 0x0D, 0x10, 0x30 }, "ORA $3010")]
		[InlineData(new byte[] { 0x1D, 0x10, 0x30 }, "ORA $3010,X")]
		[InlineData(new byte[] { 0x19, 0x10, 0x30 }, "ORA $3010,Y")]
		[InlineData(new byte[] { 0x05, 0x10 }, "ORA $10")]
		[InlineData(new byte[] { 0x15, 0x10 }, "ORA $10,X")]
		[InlineData(new byte[] { 0x01, 0x10 }, "ORA ($10,X)")]
		[InlineData(new byte[] { 0x11, 0x10 }, "ORA ($10),Y")]
		public void Disassemble_ORA_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x69, 0x10 }, "ADC #$10")]
		[InlineData(new byte[] { 0x6D, 0x10, 0x30 }, "ADC $3010")]
		[InlineData(new byte[] { 0x7D, 0x10, 0x30 }, "ADC $3010,X")]
		[InlineData(new byte[] { 0x79, 0x10, 0x30 }, "ADC $3010,Y")]
		[InlineData(new byte[] { 0x65, 0x10 }, "ADC $10")]
		[InlineData(new byte[] { 0x75, 0x10 }, "ADC $10,X")]
		[InlineData(new byte[] { 0x61, 0x10 }, "ADC ($10,X)")]
		[InlineData(new byte[] { 0x71, 0x10 }, "ADC ($10),Y")]
		public void Disassemble_ADC_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xC9, 0x10 }, "CMP #$10")]
		[InlineData(new byte[] { 0xCD, 0x10, 0x30 }, "CMP $3010")]
		[InlineData(new byte[] { 0xDD, 0x10, 0x30 }, "CMP $3010,X")]
		[InlineData(new byte[] { 0xD9, 0x10, 0x30 }, "CMP $3010,Y")]
		[InlineData(new byte[] { 0xC5, 0x10 }, "CMP $10")]
		[InlineData(new byte[] { 0xD5, 0x10 }, "CMP $10,X")]
		[InlineData(new byte[] { 0xC1, 0x10 }, "CMP ($10,X)")]
		[InlineData(new byte[] { 0xD1, 0x10 }, "CMP ($10),Y")]
		public void Disassemble_CMP_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xE0, 0x10 }, "CPX #$10")]
		[InlineData(new byte[] { 0xEC, 0x10, 0x30 }, "CPX $3010")]
		[InlineData(new byte[] { 0xE4, 0x10 }, "CPX $10")]
		public void Disassemble_CPX_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xC0, 0x10 }, "CPY #$10")]
		[InlineData(new byte[] { 0xCC, 0x10, 0x30 }, "CPY $3010")]
		[InlineData(new byte[] { 0xC4, 0x10 }, "CPY $10")]
		public void Disassemble_CPY_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xE9, 0x10 }, "SBC #$10")]
		[InlineData(new byte[] { 0xED, 0x10, 0x30 }, "SBC $3010")]
		[InlineData(new byte[] { 0xFD, 0x10, 0x30 }, "SBC $3010,X")]
		[InlineData(new byte[] { 0xF9, 0x10, 0x30 }, "SBC $3010,Y")]
		[InlineData(new byte[] { 0xE5, 0x10 }, "SBC $10")]
		[InlineData(new byte[] { 0xF5, 0x10 }, "SBC $10,X")]
		[InlineData(new byte[] { 0xE1, 0x10 }, "SBC ($10,X)")]
		[InlineData(new byte[] { 0xF1, 0x10 }, "SBC ($10),Y")]
		public void Disassemble_SBC_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xCE, 0x10, 0x30 }, "DEC $3010")]
		[InlineData(new byte[] { 0xDE, 0x10, 0x30 }, "DEC $3010,X")]
		[InlineData(new byte[] { 0xC6, 0x10 }, "DEC $10")]
		[InlineData(new byte[] { 0xD6, 0x10 }, "DEC $10,X")]
		public void Disassemble_DEC_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xCA }, "DEX")]
		[InlineData(new byte[] { 0x88 }, "DEY")]
		public void Disassemble_DEX_DEY_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xEE, 0x10, 0x30 }, "INC $3010")]
		[InlineData(new byte[] { 0xFE, 0x10, 0x30 }, "INC $3010,X")]
		[InlineData(new byte[] { 0xE6, 0x10 }, "INC $10")]
		[InlineData(new byte[] { 0xF6, 0x10 }, "INC $10,X")]
		public void Disassemble_INC_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xE8 }, "INX")]
		[InlineData(new byte[] { 0xC8 }, "INY")]
		public void Disassemble_INX_INY_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x00 }, "BRK")]
		[InlineData(new byte[] { 0x4C, 0x10, 0x30 }, "JMP $3010")]
		[InlineData(new byte[] { 0x6C, 0x10, 0x30 }, "JMP ($3010)")]
		[InlineData(new byte[] { 0x20, 0x10, 0x30 }, "JSR $3010")]
		[InlineData(new byte[] { 0x40 }, "RTI")]
		[InlineData(new byte[] { 0x60 }, "RTS")]
		public void Disassemble_ControlOpcodes_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x90, 0x40 }, "BCC $0242")]
		[InlineData(new byte[] { 0xB0, 0x40 }, "BCS $0242")]
		[InlineData(new byte[] { 0xF0, 0x40 }, "BEQ $0242")]
		[InlineData(new byte[] { 0x30, 0x40 }, "BMI $0242")]
		[InlineData(new byte[] { 0xD0, 0x40 }, "BNE $0242")]
		[InlineData(new byte[] { 0x10, 0x40 }, "BPL $0242")]
		[InlineData(new byte[] { 0x50, 0x40 }, "BVC $0242")]
		[InlineData(new byte[] { 0x70, 0x40 }, "BVS $0242")]
		public void Disassemble_BranchOpcodes_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x18 }, "CLC")]
		[InlineData(new byte[] { 0xD8 }, "CLD")]
		[InlineData(new byte[] { 0x58 }, "CLI")]
		[InlineData(new byte[] { 0xB8 }, "CLV")]
		[InlineData(new byte[] { 0x38 }, "SEC")]
		[InlineData(new byte[] { 0xF8 }, "SED")]
		[InlineData(new byte[] { 0x78 }, "SEI")]
		public void Disassemble_FlagsOpcodes_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xEA }, "NOP")]
		public void Disassemble_NOP_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15 }, "DB 0x10, 0x11, 0x12, 0x13, 0x14, 0x15")]
		public void Disassemble_NonExecutableData_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);
			disassembler.AddNonExecutableSection(0x200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

		[Theory]
		[InlineData(new byte[] { 0xFF }, "???")]
		public void Disassemble_IllegalInstruction_ShouldYieldCorrectDisassemblyString(byte[] code, string expectedDisassembly)
		{
			// Arrange
			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0x0200);
			Disassembler disassembler = new Disassembler(machine, 0x0200, (ushort)code.Length);

			// Act
			var result = disassembler.Disassemble();

			// Assert
			Assert.Single(result);
			Assert.Equal(expectedDisassembly, result[0].Disassembly);
		}

	}
}
