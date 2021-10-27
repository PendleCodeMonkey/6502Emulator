using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using System.Collections.Generic;
using System.Linq;

namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the static <see cref="InstructionInfo"/> class.
	/// </summary>
	public static class InstructionInfo
	{
		/// <summary>
		/// Gets a dictionary that maps enumerated OpCode values to tuples containing the instruction name and addressing mode.
		/// </summary>
		public static Dictionary<OpCodes, (string Name, AddressingModes AddrMode)> Instructions
		{ get; } = new Dictionary<OpCodes, (string Name, AddressingModes AddrMode)>
		{
			// LDA
			{ OpCodes.LDA_IMM, ("LDA", AddressingModes.Immediate) },
			{ OpCodes.LDA_ABS, ("LDA", AddressingModes.Absolute) },
			{ OpCodes.LDA_ABS_X, ("LDA", AddressingModes.AbsoluteX) },
			{ OpCodes.LDA_ABS_Y, ("LDA", AddressingModes.AbsoluteY) },
			{ OpCodes.LDA_ZPG, ("LDA", AddressingModes.ZeroPage) },
			{ OpCodes.LDA_ZPG_X, ("LDA", AddressingModes.ZeroPageX) },
			{ OpCodes.LDA_IND_X, ("LDA", AddressingModes.IndexedXIndirect) },
			{ OpCodes.LDA_IND_Y, ("LDA", AddressingModes.IndirectIndexedY) },

			// LDX
			{ OpCodes.LDX_IMM, ("LDX", AddressingModes.Immediate) },
			{ OpCodes.LDX_ABS, ("LDX", AddressingModes.Absolute) },
			{ OpCodes.LDX_ABS_Y, ("LDX", AddressingModes.AbsoluteY) },
			{ OpCodes.LDX_ZPG, ("LDX", AddressingModes.ZeroPage) },
			{ OpCodes.LDX_ZPG_Y, ("LDX", AddressingModes.ZeroPageY) },

			// LDY
			{ OpCodes.LDY_IMM, ("LDY", AddressingModes.Immediate) },
			{ OpCodes.LDY_ABS, ("LDY", AddressingModes.Absolute) },
			{ OpCodes.LDY_ABS_X, ("LDY", AddressingModes.AbsoluteX) },
			{ OpCodes.LDY_ZPG, ("LDY", AddressingModes.ZeroPage) },
			{ OpCodes.LDY_ZPG_X, ("LDY", AddressingModes.ZeroPageX) },

			// STA
			{ OpCodes.STA_ABS, ("STA", AddressingModes.Absolute) },
			{ OpCodes.STA_ABS_X, ("STA", AddressingModes.AbsoluteX) },
			{ OpCodes.STA_ABS_Y, ("STA", AddressingModes.AbsoluteY) },
			{ OpCodes.STA_ZPG, ("STA", AddressingModes.ZeroPage) },
			{ OpCodes.STA_ZPG_X, ("STA", AddressingModes.ZeroPageX) },
			{ OpCodes.STA_IND_X, ("STA", AddressingModes.IndexedXIndirect) },
			{ OpCodes.STA_IND_Y, ("STA", AddressingModes.IndirectIndexedY) },

			// STX
			{ OpCodes.STX_ABS, ("STX", AddressingModes.Absolute) },
			{ OpCodes.STX_ZPG, ("STX", AddressingModes.ZeroPage) },
			{ OpCodes.STX_ZPG_Y, ("STX", AddressingModes.ZeroPageY) },

			// STY
			{ OpCodes.STY_ABS, ("STY", AddressingModes.Absolute) },
			{ OpCodes.STY_ZPG, ("STY", AddressingModes.ZeroPage) },
			{ OpCodes.STY_ZPG_X, ("STY", AddressingModes.ZeroPageX) },

			// TAX
			{ OpCodes.TAX, ("TAX", AddressingModes.Implied) },

			// TAY
			{ OpCodes.TAY, ("TAY", AddressingModes.Implied) },

			// TSX
			{ OpCodes.TSX, ("TSX", AddressingModes.Implied) },

			// TXA
			{ OpCodes.TXA, ("TXA", AddressingModes.Implied) },

			// TXS
			{ OpCodes.TXS, ("TXS", AddressingModes.Implied) },

			// TYA
			{ OpCodes.TYA, ("TYA", AddressingModes.Implied) },


			// PHA
			{ OpCodes.PHA, ("PHA", AddressingModes.Implied) },

			// PHP
			{ OpCodes.PHP, ("PHP", AddressingModes.Implied) },

			// PLA
			{ OpCodes.PLA, ("PLA", AddressingModes.Implied) },

			// PLP
			{ OpCodes.PLP, ("PLP", AddressingModes.Implied) },

			// ASL
			{ OpCodes.ASL_ACC, ("ASL", AddressingModes.Accumulator) },
			{ OpCodes.ASL_ABS, ("ASL", AddressingModes.Absolute) },
			{ OpCodes.ASL_ABS_X, ("ASL", AddressingModes.AbsoluteX) },
			{ OpCodes.ASL_ZPG, ("ASL", AddressingModes.ZeroPage) },
			{ OpCodes.ASL_ZPG_X, ("ASL", AddressingModes.ZeroPageX) },

			// LSR
			{ OpCodes.LSR_ACC, ("LSR", AddressingModes.Accumulator) },
			{ OpCodes.LSR_ABS, ("LSR", AddressingModes.Absolute) },
			{ OpCodes.LSR_ABS_X, ("LSR", AddressingModes.AbsoluteX) },
			{ OpCodes.LSR_ZPG, ("LSR", AddressingModes.ZeroPage) },
			{ OpCodes.LSR_ZPG_X, ("LSR", AddressingModes.ZeroPageX) },

			// ROL
			{ OpCodes.ROL_ACC, ("ROL", AddressingModes.Accumulator) },
			{ OpCodes.ROL_ABS, ("ROL", AddressingModes.Absolute) },
			{ OpCodes.ROL_ABS_X, ("ROL", AddressingModes.AbsoluteX) },
			{ OpCodes.ROL_ZPG, ("ROL", AddressingModes.ZeroPage) },
			{ OpCodes.ROL_ZPG_X, ("ROL", AddressingModes.ZeroPageX) },

			// ROR
			{ OpCodes.ROR_ACC, ("ROR", AddressingModes.Accumulator) },
			{ OpCodes.ROR_ABS, ("ROR", AddressingModes.Absolute) },
			{ OpCodes.ROR_ABS_X, ("ROR", AddressingModes.AbsoluteX) },
			{ OpCodes.ROR_ZPG, ("ROR", AddressingModes.ZeroPage) },
			{ OpCodes.ROR_ZPG_X, ("ROR", AddressingModes.ZeroPageX) },

			// AND
			{ OpCodes.AND_IMM, ("AND", AddressingModes.Immediate) },
			{ OpCodes.AND_ABS, ("AND", AddressingModes.Absolute) },
			{ OpCodes.AND_ABS_X, ("AND", AddressingModes.AbsoluteX) },
			{ OpCodes.AND_ABS_Y, ("AND", AddressingModes.AbsoluteY) },
			{ OpCodes.AND_ZPG, ("AND", AddressingModes.ZeroPage) },
			{ OpCodes.AND_ZPG_X, ("AND", AddressingModes.ZeroPageX) },
			{ OpCodes.AND_IND_X, ("AND", AddressingModes.IndexedXIndirect) },
			{ OpCodes.AND_IND_Y, ("AND", AddressingModes.IndirectIndexedY) },

			// BIT
			{ OpCodes.BIT_ABS, ("BIT", AddressingModes.Absolute) },
			{ OpCodes.BIT_ZPG, ("BIT", AddressingModes.ZeroPage) },

			// EOR
			{ OpCodes.EOR_IMM, ("EOR", AddressingModes.Immediate) },
			{ OpCodes.EOR_ABS, ("EOR", AddressingModes.Absolute) },
			{ OpCodes.EOR_ABS_X, ("EOR", AddressingModes.AbsoluteX) },
			{ OpCodes.EOR_ABS_Y, ("EOR", AddressingModes.AbsoluteY) },
			{ OpCodes.EOR_ZPG, ("EOR", AddressingModes.ZeroPage) },
			{ OpCodes.EOR_ZPG_X, ("EOR", AddressingModes.ZeroPageX) },
			{ OpCodes.EOR_IND_X, ("EOR", AddressingModes.IndexedXIndirect) },
			{ OpCodes.EOR_IND_Y, ("EOR", AddressingModes.IndirectIndexedY) },

			// ORA
			{ OpCodes.ORA_IMM, ("ORA", AddressingModes.Immediate) },
			{ OpCodes.ORA_ABS, ("ORA", AddressingModes.Absolute) },
			{ OpCodes.ORA_ABS_X, ("ORA", AddressingModes.AbsoluteX) },
			{ OpCodes.ORA_ABS_Y, ("ORA", AddressingModes.AbsoluteY) },
			{ OpCodes.ORA_ZPG, ("ORA", AddressingModes.ZeroPage) },
			{ OpCodes.ORA_ZPG_X, ("ORA", AddressingModes.ZeroPageX) },
			{ OpCodes.ORA_IND_X, ("ORA", AddressingModes.IndexedXIndirect) },
			{ OpCodes.ORA_IND_Y, ("ORA", AddressingModes.IndirectIndexedY) },

			// ADC
			{ OpCodes.ADC_IMM, ("ADC", AddressingModes.Immediate) },
			{ OpCodes.ADC_ABS, ("ADC", AddressingModes.Absolute) },
			{ OpCodes.ADC_ABS_X, ("ADC", AddressingModes.AbsoluteX) },
			{ OpCodes.ADC_ABS_Y, ("ADC", AddressingModes.AbsoluteY) },
			{ OpCodes.ADC_ZPG, ("ADC", AddressingModes.ZeroPage) },
			{ OpCodes.ADC_ZPG_X, ("ADC", AddressingModes.ZeroPageX) },
			{ OpCodes.ADC_IND_X, ("ADC", AddressingModes.IndexedXIndirect) },
			{ OpCodes.ADC_IND_Y, ("ADC", AddressingModes.IndirectIndexedY) },

			// CMP
			{ OpCodes.CMP_IMM, ("CMP", AddressingModes.Immediate) },
			{ OpCodes.CMP_ABS, ("CMP", AddressingModes.Absolute) },
			{ OpCodes.CMP_ABS_X, ("CMP", AddressingModes.AbsoluteX) },
			{ OpCodes.CMP_ABS_Y, ("CMP", AddressingModes.AbsoluteY) },
			{ OpCodes.CMP_ZPG, ("CMP", AddressingModes.ZeroPage) },
			{ OpCodes.CMP_ZPG_X, ("CMP", AddressingModes.ZeroPageX) },
			{ OpCodes.CMP_IND_X, ("CMP", AddressingModes.IndexedXIndirect) },
			{ OpCodes.CMP_IND_Y, ("CMP", AddressingModes.IndirectIndexedY) },

			// CPX
			{ OpCodes.CPX_IMM, ("CPX", AddressingModes.Immediate) },
			{ OpCodes.CPX_ABS, ("CPX", AddressingModes.Absolute) },
			{ OpCodes.CPX_ZPG, ("CPX", AddressingModes.ZeroPage) },

			// CPY
			{ OpCodes.CPY_IMM, ("CPY", AddressingModes.Immediate) },
			{ OpCodes.CPY_ABS, ("CPY", AddressingModes.Absolute) },
			{ OpCodes.CPY_ZPG, ("CPY", AddressingModes.ZeroPage) },

			// SBC
			{ OpCodes.SBC_IMM, ("SBC", AddressingModes.Immediate) },
			{ OpCodes.SBC_ABS, ("SBC", AddressingModes.Absolute) },
			{ OpCodes.SBC_ABS_X, ("SBC", AddressingModes.AbsoluteX) },
			{ OpCodes.SBC_ABS_Y, ("SBC", AddressingModes.AbsoluteY) },
			{ OpCodes.SBC_ZPG, ("SBC", AddressingModes.ZeroPage) },
			{ OpCodes.SBC_ZPG_X, ("SBC", AddressingModes.ZeroPageX) },
			{ OpCodes.SBC_IND_X, ("SBC", AddressingModes.IndexedXIndirect) },
			{ OpCodes.SBC_IND_Y, ("SBC", AddressingModes.IndirectIndexedY) },

			// DEC
			{ OpCodes.DEC_ABS, ("DEC", AddressingModes.Absolute) },
			{ OpCodes.DEC_ABS_X, ("DEC", AddressingModes.AbsoluteX) },
			{ OpCodes.DEC_ZPG, ("DEC", AddressingModes.ZeroPage) },
			{ OpCodes.DEC_ZPG_X, ("DEC", AddressingModes.ZeroPageX) },

			// DEX
			{ OpCodes.DEX, ("DEX", AddressingModes.Implied) },

			// DEY
			{ OpCodes.DEY, ("DEY", AddressingModes.Implied) },

			// INC
			{ OpCodes.INC_ABS, ("INC", AddressingModes.Absolute) },
			{ OpCodes.INC_ABS_X, ("INC", AddressingModes.AbsoluteX) },
			{ OpCodes.INC_ZPG, ("INC", AddressingModes.ZeroPage) },
			{ OpCodes.INC_ZPG_X, ("INC", AddressingModes.ZeroPageX) },

			// INX
			{ OpCodes.INX, ("INX", AddressingModes.Implied) },

			// INY
			{ OpCodes.INY, ("INY", AddressingModes.Implied) },

			// BRK
			{ OpCodes.BRK, ("BRK", AddressingModes.Implied) },

			// JMP
			{ OpCodes.JMP_ABS, ("JMP", AddressingModes.Absolute) },
			{ OpCodes.JMP_IND, ("JMP", AddressingModes.Indirect) },

			// JSR
			{ OpCodes.JSR, ("JSR", AddressingModes.Absolute) },

			// RTI
			{ OpCodes.RTI, ("RTI", AddressingModes.Implied) },

			// RTS
			{ OpCodes.RTS, ("RTS", AddressingModes.Implied) },

			// BCC
			{ OpCodes.BCC, ("BCC", AddressingModes.Relative) },

			// BCS
			{ OpCodes.BCS, ("BCS", AddressingModes.Relative) },

			// BEQ
			{ OpCodes.BEQ, ("BEQ", AddressingModes.Relative) },

			// BMI
			{ OpCodes.BMI, ("BMI", AddressingModes.Relative) },

			// BNE
			{ OpCodes.BNE, ("BNE", AddressingModes.Relative) },

			// BPL
			{ OpCodes.BPL, ("BPL", AddressingModes.Relative) },

			// BVC
			{ OpCodes.BVC, ("BVC", AddressingModes.Relative) },

			// BVS
			{ OpCodes.BVS, ("BVS", AddressingModes.Relative) },

			// CLC
			{ OpCodes.CLC, ("CLC", AddressingModes.Implied) },

			// CLD
			{ OpCodes.CLD, ("CLD", AddressingModes.Implied) },

			// CLI
			{ OpCodes.CLI, ("CLI", AddressingModes.Implied) },

			// CLV
			{ OpCodes.CLV, ("CLV", AddressingModes.Implied) },

			// SEC
			{ OpCodes.SEC, ("SEC", AddressingModes.Implied) },

			// SED
			{ OpCodes.SED, ("SED", AddressingModes.Implied) },

			// SEI
			{ OpCodes.SEI, ("SEI", AddressingModes.Implied) },

			// NOP
			{ OpCodes.NOP, ("NOP", AddressingModes.Implied) }
		};

		/// <summary>
		/// Gets a dictionary that maps addressing mode values to strings containing text used
		/// to format the addressing mode output.
		/// </summary>
		public static Dictionary<AddressingModes, string> AddressingModeFormat
		{ get; } = new Dictionary<AddressingModes, string>
		{
			{ AddressingModes.Accumulator, " A" },
			{ AddressingModes.Absolute, " ${0:X4}" },
			{ AddressingModes.AbsoluteX, " ${0:X4},X" },
			{ AddressingModes.AbsoluteY, " ${0:X4},Y" },
			{ AddressingModes.Immediate, " #${0:X2}" },
			{ AddressingModes.Implied, "" },
			{ AddressingModes.Indirect, " (${0:X4})" },
			{ AddressingModes.IndexedXIndirect, " (${0:X2},X)" },
			{ AddressingModes.IndirectIndexedY, " (${0:X2}),Y" },
			{ AddressingModes.Relative, " ${0:X4}" },				// Note: Relative adddressing is output as an absolute address after offset has been applied.
			{ AddressingModes.ZeroPage, " ${0:X2}" },
			{ AddressingModes.ZeroPageX, " ${0:X2},X" },
			{ AddressingModes.ZeroPageY, " ${0:X2},Y" }
		};


		/// <summary>
		/// Find the opcode value corresponding to the specified instruction mnemonic and addressing mode combination.
		/// </summary>
		/// <param name="mnemonic">The 6502 instruction mnemonic.</param>
		/// <param name="addrMode">The addressing mode being used.</param>
		/// <returns>
		/// A byte containing the opcode value corresponding to the specified instruction mnemonic and
		/// addressing mode combination (or null if the mnemonic and addressing mode combination is invalid).
		/// </returns>
		public static byte? FindInstruction(string mnemonic, AddressingModes addrMode)
		{
			byte? retVal = null;

			var elem = Instructions.Where(x => x.Value.Name.Equals(mnemonic) && x.Value.AddrMode == addrMode).Select(x => x.Key);
			if (elem.Count() == 1)
			{
				retVal = (byte)elem.First();
			}

			return retVal;
		}
	}
}
