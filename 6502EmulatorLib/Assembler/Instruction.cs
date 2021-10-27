using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MOS6502EmulatorLib.Assembler
{
	/// <summary>
	/// Implementation of the <see cref="Instruction"/> class.
	/// </summary>
	class Instruction
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Instruction"/> class.
		/// </summary>
		/// <param name="opcode">The 8-bit opcode value for the instruction.</param>
		/// <param name="prefix">The enumerated prefix value for the instruction.</param>
		/// <param name="info">An <see cref="InstructionInfo"/> instance giving info about the instruction.</param>
		/// <param name="byteOperand">The value of the 8-bit operand (if any).</param>
		/// <param name="wordOperand">The value of the 16-bit operand (if any).</param>
		/// <param name="displacement">The value of the 8-bit displacement (if any).</param>
		internal Instruction(byte opcode, string mnemonic, AddressingModes addrMode, byte byteOperand, ushort wordOperand, byte displacement)
		{
			Opcode = opcode;
			Mnemonic = mnemonic;
			AddrMode = addrMode;
			ByteOperand = byteOperand;
			WordOperand = wordOperand;
			Displacement = displacement;
		}


		/// <summary>
		/// The 8-bit opcode value for this instruction.
		/// </summary>
		internal byte Opcode { get; set; }

		/// <summary>
		/// The instruction mnemonic string.
		/// </summary>
		internal string Mnemonic { get; set; }

		/// <summary>
		/// An <see cref="AddressingModes"/> enumerated value of the addressing mode used for the instruction.
		/// </summary>
		internal AddressingModes AddrMode { get; set; }

		/// <summary>
		/// The value of the 8-bit operand specified for this instruction (if any).
		/// </summary>
		internal byte ByteOperand { get; set; }

		/// <summary>
		/// The value of the 16-bit operand specified for this instruction (if any).
		/// </summary>
		internal ushort WordOperand { get; set; }

		/// <summary>
		/// The value of the 8-bit displacement specified for this instruction (if any).
		/// </summary>
		internal byte Displacement { get; set; }

	}
}
