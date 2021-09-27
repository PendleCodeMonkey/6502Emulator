using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="StatusRegister"/> class.
	/// </summary>
	class StatusRegister
	{
		private ProcessorFlags _flags;

		/// <summary>
		/// Gets or sets a value indicating the current state of all of the processor flags.
		/// </summary>
		public ProcessorFlags Flags
		{
			get
			{
				return _flags;
			}
			set
			{
				_flags = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the Carry flag.
		/// </summary>
		public bool Carry
		{
			get => _flags.HasFlag(ProcessorFlags.Carry);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Carry;
				}
				else
				{
					_flags &= ~ProcessorFlags.Carry;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Zero flag.
		/// </summary>
		public bool Zero
		{
			get => _flags.HasFlag(ProcessorFlags.Zero);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Zero;
				}
				else
				{
					_flags &= ~ProcessorFlags.Zero;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Interrupt Disable flag.
		/// </summary>
		public bool Interrupt
		{
			get => _flags.HasFlag(ProcessorFlags.Interrupt);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Interrupt;
				}
				else
				{
					_flags &= ~ProcessorFlags.Interrupt;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Decimal flag.
		/// </summary>
		public bool Decimal
		{
			get => _flags.HasFlag(ProcessorFlags.Decimal);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Decimal;
				}
				else
				{
					_flags &= ~ProcessorFlags.Decimal;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Break flag.
		/// </summary>
		public bool Break
		{
			get => _flags.HasFlag(ProcessorFlags.Break);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Break;
				}
				else
				{
					_flags &= ~ProcessorFlags.Break;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Overflow flag.
		/// </summary>
		public bool Overflow
		{
			get => _flags.HasFlag(ProcessorFlags.Overflow);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Overflow;
				}
				else
				{
					_flags &= ~ProcessorFlags.Overflow;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the Negative flag.
		/// </summary>
		public bool Negative
		{
			get => _flags.HasFlag(ProcessorFlags.Negative);
			set
			{
				if (value)
				{
					_flags |= ProcessorFlags.Negative;
				}
				else
				{
					_flags &= ~ProcessorFlags.Negative;
				}
			}
		}
	}
}
