namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Stack"/> class.
	/// </summary>
	class Stack
	{
		private const byte StackPage = 1;       // On a 6502 the stack occupies memory page 1 (i.e. addresses 0x0100 to 0x01FF)
		private readonly Memory _memory;

		/// <summary>
		/// Initializes a new instance of the <see cref="Stack"/> class.
		/// </summary>
		/// <param memory="A">The <see cref="Memory"/> instance that will host the stack.</param>
		public Stack(Memory memory)
		{
			_memory = memory;
			S = 0xFF;       // Initialize stack pointer.
		}

		/// <summary>
		/// Gets or sets the current value of the Stack Pointer.
		/// </summary>
		internal byte S { get; set; }

		/// <summary>
		/// Reset the stack to its default setting.
		/// </summary>
		public void Reset()
		{
			S = 0xFF;       // Reset stack pointer.
		}

		/// <summary>
		/// Push the specified value onto the stack.
		/// </summary>
		/// <param name="value">The value to be pushed onto the stack.</param>
		public void Push(byte value)
		{
			_memory.WriteOnPage(StackPage, S, value);
			S--;
		}

		/// <summary>
		/// Pop the value off the top of the stack.
		/// </summary>
		/// <returns>The value retrieved from the top of the stack.</returns>
		public byte Pop()
		{
			S++;
			return _memory.ReadFromPage(StackPage, S);
		}
	}
}
