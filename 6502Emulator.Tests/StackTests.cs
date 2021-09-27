using PendleCodeMonkey.MOS6502EmulatorLib;
using Xunit;

namespace PendleCodeMonkey.MOS6502Emulator.Tests
{
	public class StackTests
	{
		[Fact]
		public void NewStack_ShouldPointToTopOfStack()
		{
			Stack stack = new Stack(new Memory());

			Assert.Equal(0xFF, stack.S);
		}

		[Fact]
		public void Reset_ShouldResetPointerToTopOfStack()
		{
			Stack stack = new Stack(new Memory())
			{
				S = 0x80
			};

			stack.Reset();

			Assert.Equal(0xFF, stack.S);
		}

		[Fact]
		public void Push_ShouldStoreValueAndDecrementStackPointer()
		{
			Memory mem = new Memory();
			Stack stack = new Stack(mem);

			stack.Push(0x40);

			Assert.Equal(0xFE, stack.S);
			Assert.Equal(0x40, mem.Data[0x01FF]);
		}

		[Fact]
		public void Pop_ShouldIncrementStackPointerAndRetrieveValue()
		{
			Memory mem = new Memory();
			Stack stack = new Stack(mem)
			{
				S = 0xFE
			};
			mem.Data[0x01FF] = 0x7F;

			byte value = stack.Pop();

			Assert.Equal(0xFF, stack.S);
			Assert.Equal(0x7F, value);
		}

	}
}
