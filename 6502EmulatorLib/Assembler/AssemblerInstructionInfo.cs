using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using System.Collections.Generic;

namespace PendleCodeMonkey.MOS6502EmulatorLib.Assembler
{
	/// <summary>
	/// Implementation of the <see cref="AssemblerInstructionInfo"/> class.
	/// </summary>
	class AssemblerInstructionInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblerInstructionInfo"/> class.
		/// </summary>
		/// <param name="lineNumber">The line number in the source code corresponding to this instruction.</param>
		/// <param name="address">The address of this instruction.</param>
		/// <param name="instruction">The <see cref="Instruction"/> object corresponding to this instruction.</param>
		/// <param name="operand">The operand string.</param>
		/// <param name="opResolved">Boolean value indicating whether the operand value has been resolved.</param>
		/// <param name="dataSegment">An instance of the <see cref="DataSegmentInfo"/> class (or null if
		///								this <see cref="AssemblerInstructionInfo"/> instance does not correspond to a data segment).</param>
		internal AssemblerInstructionInfo(int lineNumber, ushort address, Instruction instruction, string operand, bool opResolved, DataSegmentInfo dataSegment = null)
		{
			LineNumber = lineNumber;
			Address = address;
			Instruction = instruction;
			Operand = operand;
			DataSegment = dataSegment;
			BinaryData = null;
			OperandsResolved = opResolved;
		}

		/// <summary>
		/// Gets or sets the line number in the source code corresponding to this instruction.
		/// </summary>
		internal int LineNumber { get; set; }

		/// <summary>
		/// Gets or sets the address of this instruction.
		/// </summary>
		internal ushort Address { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="Instruction"/> object corresponding to this instruction.
		/// </summary>
		internal Instruction Instruction { get; set; }

		/// <summary>
		/// Gets or sets the first operand.
		/// </summary>
		internal string Operand { get; set; }

		/// <summary>
		/// Gets or sets the object used for storing information related to a data segment.
		/// </summary>
		internal DataSegmentInfo DataSegment { get; set; }

		/// <summary>
		/// Gets or sets the list of binary data generated for this <see cref="AssemblerInstructionInfo"/> instance.
		/// </summary>
		internal List<byte> BinaryData { get; set; }

		/// <summary>
		/// Gets or sets a flag indicating whether the operands for this instruction have been resolved.
		/// </summary>
		internal bool OperandsResolved { get; set; }

		/// <summary>
		/// Generate the binary data for this <see cref="AssemblerInstructionInfo"/> instance.
		/// </summary>
		/// <returns>The generated binary data as a collection of bytes.</returns>
		internal List<byte> GenerateBinaryData()
		{
			List<byte> binData = new List<byte>();

			// If this object corresponds to a data segment then just retrieve the binary data for it directly from
			// the DataSegmentInfo object.
			if (DataSegment != null)
			{
				binData = DataSegment.Data;
			}
			else
			{
				// This instance doesn't correspond to a data segment so handle it as a 6502 instruction.
				// Add the instruction opcode byte.
				binData.Add(Instruction.Opcode);

				// Add operand data required for this instruction (if any)
				switch (Instruction.AddrMode)
				{
					case AddressingModes.Immediate:
					case AddressingModes.IndexedXIndirect:
					case AddressingModes.IndirectIndexedY:
					case AddressingModes.ZeroPage:
					case AddressingModes.ZeroPageX:
					case AddressingModes.ZeroPageY:
						binData.Add(Instruction.ByteOperand);
						break;
					case AddressingModes.Absolute:
					case AddressingModes.AbsoluteX:
					case AddressingModes.AbsoluteY:
					case AddressingModes.Indirect:
						binData.Add((byte)(Instruction.WordOperand & 0xFF));
						binData.Add((byte)(Instruction.WordOperand >> 8 & 0xFF));
						break;
					case AddressingModes.Relative:
						binData.Add(Instruction.Displacement);
						break;
					default:
						// It's an Accumulator or Implied addressing mode so must be a single byte instruction therefore
						// nothing more needs adding to the binary data here.
						break;
				}
			}

			BinaryData = new List<byte>(binData);

			return binData;
		}
	}
}
