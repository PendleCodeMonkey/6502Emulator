using PendleCodeMonkey.MOS6502EmulatorLib;
using System;
using System.Linq;
using Xunit;

namespace PendleCodeMonkey.MOS6502Emulator.Tests
{
	public class MemoryTests
	{
		[Fact]
		public void NewMemory_ShouldNotBeNull()
		{
			Memory memory = new Memory();

			Assert.NotNull(memory);
		}

		[Fact]
		public void NewMemory_ShouldBeCorrectlyAllocated()
		{
			Memory memory = new Memory();

			Assert.NotNull(memory.Data);
			Assert.Equal(0x10000, memory.Data.Length);
		}

		[Fact]
		public void LoadData_SucceedsWhenDataFitsInMemory()
		{
			Memory memory = new Memory();
			var success = memory.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0x2000);

			Assert.True(success);
		}

		[Fact]
		public void LoadData_FailsWhenDataExceedsMemoryLimit()
		{
			Memory memory = new Memory();
			var success = memory.LoadData(new byte[] { 1, 2, 3, 4, 5, 6 }, 0xFFFC);

			Assert.False(success);
		}

		[Fact]
		public void Clear_ShouldClearAllMemory()
		{
			Memory memory = new Memory();
			Span<byte> machineMemorySpan = memory.Data;
			machineMemorySpan.Fill(0xAA);

			memory.Clear();

			Assert.False(memory.Data.Where(x => x > 0).Any());
		}

		[Fact]
		public void DumpMemory_ShouldReturnRequestedMemoryBlock()
		{
			Memory memory = new Memory();
			_ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 0x2000);

			var dump = memory.DumpMemory(0x2001, 0x0004);

			Assert.Equal(4, dump.Length);
			Assert.Equal(0x02, dump[0]);
			Assert.Equal(0x03, dump[1]);
			Assert.Equal(0x04, dump[2]);
			Assert.Equal(0x05, dump[3]);
		}

		[Theory]
		[InlineData(0x00, 0x0000)]
		[InlineData(0x01, 0x0100)]
		[InlineData(0x10, 0x1000)]
		[InlineData(0xFF, 0xFF00)]
		public void PageOffset_ShouldReturnCorrectPageStartAddress(byte page, ushort expectedAddress)
		{
			Memory memory = new Memory();

			ushort pageAddress = memory.PageOffset(page);

			Assert.Equal(expectedAddress, pageAddress);
		}

		[Fact]
		public void ReadFromPage_ShouldReadFromCorrectPage()
		{
			Memory memory = new Memory();
			_ = memory.LoadData(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 }, 0x2000);

			byte value = memory.ReadFromPage(0x20, 0x04);

			Assert.Equal(0x05, value);
		}

		[Fact]
		public void WriteOnPage_ShouldWriteToCorrectPage()
		{
			Memory memory = new Memory();

			memory.WriteOnPage(0x20, 0x04, 0x05);

			Assert.Equal(0x05, memory.Data[0x2004]);
		}

	}
}
