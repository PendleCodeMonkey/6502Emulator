using PendleCodeMonkey.MOS6502EmulatorLib;
using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using Xunit;

namespace PendleCodeMonkey.MOS6502Emulator.Tests
{
	public class CPUTests
	{
		[Fact]
		public void NewCPU_ShouldNotBeNull()
		{
			CPU cpu = new CPU();

			Assert.NotNull(cpu);
		}

		[Fact]
		public void NewCPU_ShouldHaveStatusRegister()
		{
			CPU cpu = new CPU();

			Assert.NotNull(cpu.SR);
		}

		[Fact]
		public void Reset_ShouldResetState()
		{
			CPU cpu = new CPU
			{
				A = 123,
				X = 234,
				Y = 210,
				PC = 0x0200
			};
			cpu.SR.Flags = ProcessorFlags.Carry | ProcessorFlags.Zero;

			cpu.Reset();

			Assert.Equal(0x00, cpu.A);
			Assert.Equal(0x00, cpu.X);
			Assert.Equal(0x00, cpu.Y);
			Assert.Equal(0x0000, cpu.PC);
			Assert.Equal((ProcessorFlags)0, cpu.SR.Flags);
		}

		[Fact]
		public void SetState_ShouldInitializeState()
		{
			CPU cpu = new CPU();

			cpu.SetState(A: 123, X: 234, Y: 210, flags: ProcessorFlags.Negative);

			Assert.Equal(123, cpu.A);
			Assert.Equal(234, cpu.X);
			Assert.Equal(210, cpu.Y);
			Assert.Equal(0x0000, cpu.PC);
			Assert.Equal(ProcessorFlags.Negative, cpu.SR.Flags);
		}

		[Fact]
		public void GetState_ShouldGetState()
		{
			CPU cpu = new CPU
			{
				A = 123,
				X = 234,
				Y = 210,
				PC = 0x0200
			};
			cpu.SR.Flags = ProcessorFlags.Carry | ProcessorFlags.Zero;

			var (A, X, Y, PC, Flags) = cpu.GetState();

			Assert.Equal(123, A);
			Assert.Equal(234, X);
			Assert.Equal(210, Y);
			Assert.Equal(0x0200, PC);
			Assert.Equal(ProcessorFlags.Carry | ProcessorFlags.Zero, Flags);
		}

		[Fact]
		public void IncrementPC_ShouldIncrementProgramCounter()
		{
			CPU cpu = new CPU
			{
				PC = 0x0200
			};

			cpu.IncrementPC();

			Assert.Equal(0x0201, cpu.PC);
		}

		[Fact]
		public void AddPositiveOffsetToPC_ShouldIncreaseProgramCounter()
		{
			CPU cpu = new CPU
			{
				PC = 0x0200
			};

			cpu.AddOffsetToPC(0x60);

			Assert.Equal(0x0260, cpu.PC);
		}

		[Fact]
		public void AddNegativeOffsetToPC_ShouldDecreaseProgramCounter()
		{
			CPU cpu = new CPU
			{
				PC = 0x0200
			};

			cpu.AddOffsetToPC(-0x40);

			Assert.Equal(0x01C0, cpu.PC);
		}

	}
}
