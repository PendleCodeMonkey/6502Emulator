namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="CPU"/> class.
	/// </summary>
	internal class CPU
	{
		/// <summary>
		/// Gets or sets the value of the Accumulator.
		/// </summary>
		internal byte A { get; set; }

		/// <summary>
		/// Gets or sets the value of the X register.
		/// </summary>
		internal byte X { get; set; }

		/// <summary>
		/// Gets or sets the value of the Y register.
		/// </summary>
		internal byte Y { get; set; }


		/// <summary>
		/// Gets or sets the value of the Program Counter.
		/// </summary>
		internal ushort PC { get; set; }


		/// <summary>
		/// Gets or sets the <see cref="StatusRegister"/> instance (that maintains the flag settings).
		/// </summary>
		internal StatusRegister SR { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CPU"/> class.
		/// </summary>
		public CPU()
		{
			Reset();
		}

		/// <summary>
		/// Resets the CPU to its default settings.
		/// </summary>
		public void Reset()
		{
			A = 0;
			X = 0;
			Y = 0;
			PC = 0;
			SR = new StatusRegister();
		}

		/// <summary>
		/// Sets the values for elements of the CPU's state.
		/// </summary>
		/// <param name="A">Accumulator value (optional).</param>
		/// <param name="X">X Register value (optional).</param>
		/// <param name="Y">Y Register value (optional).</param>
		/// <param name="PC">Program Counter value (optional).</param>
		/// <param name="flags">Flags setting (optional).</param>
		public void SetState(byte? A = null, byte? X = null, byte? Y = null, ushort? PC = null, Enumerations.ProcessorFlags? flags = null)
		{
			if (A.HasValue)
			{
				this.A = A.Value;
			}
			if (X.HasValue)
			{
				this.X = X.Value;
			}
			if (Y.HasValue)
			{
				this.Y = Y.Value;
			}
			if (PC.HasValue)
			{
				this.PC = PC.Value;
			}
			if (flags.HasValue)
			{
				SR.Flags = flags.Value;
			}
		}

		/// <summary>
		/// Gets the current state of the CPU settings.
		/// </summary>
		/// <returns>A tuple containing the values of the Accumulator, X Register, Y Register, Program Counter, and Flags).</returns>
		public (byte A, byte X, byte Y, ushort PC, Enumerations.ProcessorFlags Flags) GetState()
		{
			return (A, X, Y, PC, SR.Flags);
		}

		/// <summary>
		/// Increment the value of the Program Counter.
		/// </summary>
		public void IncrementPC()
		{
			PC++;
		}

		/// <summary>
		///	Add the specified offset value to the Program Counter (offset can be negative).
		/// </summary>
		/// <param name="offset">The offset value to be added to the Program Counter (in the range -128 to 127).</param>
		public void AddOffsetToPC(sbyte offset)
		{
			PC = (ushort)(PC + offset);
		}
	}
}
