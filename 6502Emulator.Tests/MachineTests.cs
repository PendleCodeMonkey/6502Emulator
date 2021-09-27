using System;
using PendleCodeMonkey.MOS6502EmulatorLib;
using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MOS6502Emulator.Tests
{
	public class MachineTests
	{
		[Fact]
		public void NewMachine_ShouldNotBeNull()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine);
		}

		[Fact]
		public void NewMachine_ShouldHaveCPU()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.CPU);
		}

		[Fact]
		public void NewMachine_ShouldHaveMemory()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.Memory);
		}

		[Fact]
		public void NewMachine_ShouldHaveAStack()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.Stack);
		}

		[Fact]
		public void NewMachine_ShouldHaveExecutionHandler()
		{
			Machine machine = new Machine();

			Assert.NotNull(machine.ExecutionHandler);
		}

		[Fact]
		public void LoadData_ShouldSucceedWhenDataFitsInMemory()
		{
			Machine machine = new Machine();
			var success = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.True(success);
		}

		[Fact]
		public void LoadData_ShouldFailWhenDataExceedsMemoryLimit()
		{
			Machine machine = new Machine();
			var success = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0xFFFC);

			Assert.False(success);
		}

		[Fact]
		public void LoadExecutableData_ShouldSetPCToStartOfLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.Equal(0x2000, machine.CPU.PC);
		}

		[Fact]
		public void IsEndOfData_ShouldBeFalseWhenPCIsWithinLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.False(machine.IsEndOfData);
		}

		[Fact]
		public void IsEndOfData_ShouldBeTrueWhenPCIsPassedEndOfLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.PC = 0x2008;

			Assert.True(machine.IsEndOfData);
		}

		[Fact]
		public void ReadNextPCByte_ShouldReturnValueWhenPCIsWithinLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			var value = machine.ReadNextPCByte();

			Assert.Equal(1, value);
		}

		[Fact]
		public void ReadNextPCByte_ShouldThrowExceptionWhenPCIsPassedEndOfLoadedData()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);
			machine.CPU.PC = 0x2008;

			Assert.Throws<InvalidOperationException>(() => machine.ReadNextPCByte());
		}

		[Fact]
		public void SetState_ShouldInitializeState()
		{
			Machine machine = new Machine();

			machine.SetState(A: 123, X: 234, Y: 210, PC: 0x2000, flags: ProcessorFlags.Negative);

			Assert.Equal(123, machine.CPU.A);
			Assert.Equal(234, machine.CPU.X);
			Assert.Equal(210, machine.CPU.Y);
			Assert.Equal(0x2000, machine.CPU.PC);
			Assert.Equal(ProcessorFlags.Negative, machine.CPU.SR.Flags);
		}

		[Fact]
		public void GetState_ShouldGetState()
		{
			Machine machine = new Machine();
			machine.CPU.A = 123;
			machine.CPU.X = 234;
			machine.CPU.Y = 210;
			machine.CPU.PC = 0x0200;
			machine.Stack.S = 0x7F;
			machine.CPU.SR.Flags = ProcessorFlags.Carry | ProcessorFlags.Zero;

			var (A, X, Y, PC, S, Flags) = machine.GetState();

			Assert.Equal(123, A);
			Assert.Equal(234, X);
			Assert.Equal(210, Y);
			Assert.Equal(0x0200, PC);
			Assert.Equal(0x7F, S);
			Assert.Equal(ProcessorFlags.Carry | ProcessorFlags.Zero, Flags);
		}


		[Fact]
		public void DumpMemory_ShouldReturnCorrectMemoryBlock()
		{
			Machine machine = new Machine();
			var _ = machine.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			var dump = machine.DumpMemory(0x2002, 0x0010);

			Assert.Equal(0x0010, dump.Length);
			Assert.Equal(0x05, dump[2]);
		}

		[Fact]
		public void GetZeroPageAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40, 0x20, 0x30 }, 0x0000);

			byte addr = machine.GetZeroPageAddress();

			Assert.Equal(0x80, addr);
		}

		[Fact]
		public void GetZeroPageXAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40, 0x20, 0x30 }, 0x0000);
			machine.CPU.X = 6;

			byte addr = machine.GetZeroPageXAddress();

			Assert.Equal(0x86, addr);
		}

		[Fact]
		public void GetZeroPageYAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40, 0x20, 0x30 }, 0x0000);
			machine.CPU.Y = 0x1F;

			byte addr = machine.GetZeroPageYAddress();

			Assert.Equal(0x9F, addr);
		}

		[Fact]
		public void GetAbsoluteAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40, 0x20, 0x30 }, 0x0000);

			ushort addr = machine.GetAbsoluteAddress();

			Assert.Equal(0x4080, addr);
		}

		[Fact]
		public void GetAbsoluteXAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40, 0x20, 0x30 }, 0x0000);
			machine.CPU.X = 6;

			ushort addr = machine.GetAbsoluteXAddress();

			Assert.Equal(0x4086, addr);
		}

		[Fact]
		public void GetAbsoluteYAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40, 0x20, 0x30 }, 0x0000);
			machine.CPU.Y = 0x1F;

			ushort addr = machine.GetAbsoluteYAddress();

			Assert.Equal(0x409F, addr);
		}

		[Fact]
		public void GetIndirectAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x80, 0x40 }, 0x0000);
			_ = machine.LoadData(new byte[] { 0x20, 0x60, 0x20, 0x30 }, 0x4080, false);

			ushort addr = machine.GetIndirectAddress();

			Assert.Equal(0x6020, addr);
		}

		[Fact]
		public void GetIndexedIndirectAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x40, 0x00 }, 0x0000);
			_ = machine.LoadData(new byte[] { 0x20, 0x60, 0x20, 0x30, 0x40, 0x30, 0x60, 0xA0 }, 0x0040, false);
			machine.CPU.X = 4;

			ushort addr = machine.GetIndexedIndirectAddress();

			Assert.Equal(0x3040, addr);
		}

		[Fact]
		public void GetIndirectIndexedAddress_ShouldReturnCorrectAddress()
		{
			Machine machine = new Machine();
			var _ = machine.LoadExecutableData(new byte[] { 0x42, 0x00 }, 0x0000);
			_ = machine.LoadData(new byte[] { 0x20, 0x60, 0x20, 0x30, 0x40, 0x30, 0x60, 0xA0 }, 0x0040, false);
			machine.CPU.Y = 0x08;

			ushort addr = machine.GetIndirectIndexedAddress();

			Assert.Equal(0x3028, addr);
		}

	}
}
