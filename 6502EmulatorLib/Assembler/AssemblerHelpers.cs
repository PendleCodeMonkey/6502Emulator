using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using System.Collections.Generic;
using static PendleCodeMonkey.MOS6502EmulatorLib.Assembler.AssemblerEnumerations;

namespace PendleCodeMonkey.MOS6502EmulatorLib.Assembler
{
	/// <summary>
	/// Implementation of the <see cref="Assembler"/> class.
	/// </summary>
	internal class AssemblerHelpers
	{
		internal Dictionary<string, AMFlag> AddressModeFlags = new Dictionary<string, AMFlag>();

		/// <summary>
		/// Generate a collection of addressing mode bit flags for each instruction mnemonic.
		/// </summary>
		internal void GenerateAddressModeFlags()
		{
			foreach (var inst in InstructionInfo.Instructions)
			{
				var name = inst.Value.Name;
				var addrMode = inst.Value.AddrMode;
				var amFlag = AddressingModeToAMFlag(addrMode);
				if (AddressModeFlags.ContainsKey(name))
				{
					AddressModeFlags[name] |= amFlag;
				}
				else
				{
					AddressModeFlags.Add(name, amFlag);
				}
			}
		}

		/// <summary>
		/// Convert an <see cref="AddressingModes"/> eumerated value to the corresponding
		/// <see cref="AMFlag"/> enumerated bit flag value.
		/// </summary>
		/// <param name="mode">Tye enumerated addressing mode value.</param>
		/// <returns>The corresponding enumerated bit flag value.</returns>
		internal AMFlag AddressingModeToAMFlag(AddressingModes mode)
		{
			return mode switch
			{
				AddressingModes.Accumulator => AMFlag.Acc,
				AddressingModes.Absolute => AMFlag.Abs,
				AddressingModes.AbsoluteX => AMFlag.AbsX,
				AddressingModes.AbsoluteY => AMFlag.AbsY,
				AddressingModes.Immediate => AMFlag.Imm,
				AddressingModes.Indirect => AMFlag.Ind,
				AddressingModes.IndexedXIndirect => AMFlag.IndexXInd,
				AddressingModes.IndirectIndexedY => AMFlag.IndIndexY,
				AddressingModes.Relative => AMFlag.Rel,
				AddressingModes.ZeroPage => AMFlag.Zero,
				AddressingModes.ZeroPageX => AMFlag.ZeroX,
				AddressingModes.ZeroPageY => AMFlag.ZeroY,
				_ => AMFlag.Impl,
			};
		}

		/// <summary>
		/// Convert an <see cref="AMFlag"/> enumerated bit flag value to the
		/// corresponding <see cref="AddressingModes"/> eumerated value.
		/// </summary>
		/// <param name="flag">The enumerated bit flag value.</param>
		/// <returns>The corresponding enumerated addressing mode value.</returns>
		internal AddressingModes AMFlagToAddressingMode(AMFlag flag)
		{
			return flag switch
			{
				AMFlag.Acc => AddressingModes.Accumulator,
				AMFlag.Abs => AddressingModes.Absolute,
				AMFlag.AbsX => AddressingModes.AbsoluteX,
				AMFlag.AbsY => AddressingModes.AbsoluteY,
				AMFlag.Imm => AddressingModes.Immediate,
				AMFlag.Ind => AddressingModes.Indirect,
				AMFlag.IndexXInd => AddressingModes.IndexedXIndirect,
				AMFlag.IndIndexY => AddressingModes.IndirectIndexedY,
				AMFlag.Rel => AddressingModes.Relative,
				AMFlag.Zero => AddressingModes.ZeroPage,
				AMFlag.ZeroX => AddressingModes.ZeroPageX,
				AMFlag.ZeroY => AddressingModes.ZeroPageY,
				_ => AddressingModes.Implied
			};
		}
	}
}
