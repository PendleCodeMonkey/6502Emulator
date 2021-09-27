using System;

namespace PendleCodeMonkey.MOS6502EmulatorLib.Enumerations
{

	/// <summary>
	/// Enumeration of the processor flags.
	/// </summary>
	[Flags]
	public enum ProcessorFlags : byte
	{
		Carry = 0x01,
		Zero = 0x02,
		Interrupt = 0x04,
		Decimal = 0x08,
		Break = 0x10,
		Overflow = 0x40,
		Negative = 0x80
	};

	/// <summary>
	/// Enumeration of the 6502 processor's 13 addressing modes.
	/// </summary>
	public enum AddressingModes : byte
	{
		Accumulator,
		Absolute,               // $nnnn
		AbsoluteX,              // $nnnn,X
		AbsoluteY,              // $nnnn,Y
		Immediate,              // #$nn
		Implied,
		Indirect,               // ($nnnn)
		IndexedXIndirect,       // ($nn,X)
		IndirectIndexedY,       // ($nn),Y
		Relative,               // $nnnn
		ZeroPage,               // $nn
		ZeroPageX,              // $nn,X
		ZeroPageY               // $nn,Y
	};

	/// <summary>
	/// Enumeration of the 6502 processor's instructions
	/// </summary>
	public enum OpCodes : byte
	{
		// **** Load and Store instructions ****

		// LDA
		LDA_IMM = 0xA9,
		LDA_ABS = 0xAD,
		LDA_ABS_X = 0xBD,
		LDA_ABS_Y = 0xB9,
		LDA_ZPG = 0xA5,
		LDA_ZPG_X = 0xB5,
		LDA_IND_X = 0xA1,
		LDA_IND_Y = 0xB1,

		// LDX
		LDX_IMM = 0xA2,
		LDX_ABS = 0xAE,
		LDX_ABS_Y = 0xBE,
		LDX_ZPG = 0xA6,
		LDX_ZPG_Y = 0xB6,

		// LDY
		LDY_IMM = 0xA0,
		LDY_ABS = 0xAC,
		LDY_ABS_X = 0xBC,
		LDY_ZPG = 0xA4,
		LDY_ZPG_X = 0xB4,

		// STA
		STA_ABS = 0x8D,
		STA_ABS_X = 0x9D,
		STA_ABS_Y = 0x99,
		STA_ZPG = 0x85,
		STA_ZPG_X = 0x95,
		STA_IND_X = 0x81,
		STA_IND_Y = 0x91,

		// STX
		STX_ABS = 0x8E,
		STX_ZPG = 0x86,
		STX_ZPG_Y = 0x96,

		// STY
		STY_ABS = 0x8C,
		STY_ZPG = 0x84,
		STY_ZPG_X = 0x94,

		// **** Transfer instructions ****

		TAX = 0xAA,
		TAY = 0xA8,
		TSX = 0xBA,
		TXA = 0x8A,
		TXS = 0x9A,
		TYA = 0x98,

		// **** Stack instructions ****

		PHA = 0x48,
		PHP = 0x08,
		PLA = 0x68,
		PLP = 0x28,

		// **** Shift instructions ****

		// ASL
		ASL_ACC = 0x0A,
		ASL_ABS = 0x0E,
		ASL_ABS_X = 0x1E,
		ASL_ZPG = 0x06,
		ASL_ZPG_X = 0x16,

		// LSR
		LSR_ACC = 0x4A,
		LSR_ABS = 0x4E,
		LSR_ABS_X = 0x5E,
		LSR_ZPG = 0x46,
		LSR_ZPG_X = 0x56,

		// ROL
		ROL_ACC = 0x2A,
		ROL_ABS = 0x2E,
		ROL_ABS_X = 0x3E,
		ROL_ZPG = 0x26,
		ROL_ZPG_X = 0x36,

		// ROR
		ROR_ACC = 0x6A,
		ROR_ABS = 0x6E,
		ROR_ABS_X = 0x7E,
		ROR_ZPG = 0x66,
		ROR_ZPG_X = 0x76,

		// **** Logic instructions ****

		// AND
		AND_IMM = 0x29,
		AND_ABS = 0x2D,
		AND_ABS_X = 0x3D,
		AND_ABS_Y = 0x39,
		AND_ZPG = 0x25,
		AND_ZPG_X = 0x35,
		AND_IND_X = 0x21,
		AND_IND_Y = 0x31,

		// BIT
		BIT_ABS = 0x2C,
		BIT_ZPG = 0x24,

		// EOR
		EOR_IMM = 0x49,
		EOR_ABS = 0x4D,
		EOR_ABS_X = 0x5D,
		EOR_ABS_Y = 0x59,
		EOR_ZPG = 0x45,
		EOR_ZPG_X = 0x55,
		EOR_IND_X = 0x41,
		EOR_IND_Y = 0x51,

		// ORA
		ORA_IMM = 0x09,
		ORA_ABS = 0x0D,
		ORA_ABS_X = 0x1D,
		ORA_ABS_Y = 0x19,
		ORA_ZPG = 0x05,
		ORA_ZPG_X = 0x15,
		ORA_IND_X = 0x01,
		ORA_IND_Y = 0x11,

		// **** Arithmetic instructions ****

		// ADC
		ADC_IMM = 0x69,
		ADC_ABS = 0x6D,
		ADC_ABS_X = 0x7D,
		ADC_ABS_Y = 0x79,
		ADC_ZPG = 0x65,
		ADC_ZPG_X = 0x75,
		ADC_IND_X = 0x61,
		ADC_IND_Y = 0x71,

		// CMP
		CMP_IMM = 0xC9,
		CMP_ABS = 0xCD,
		CMP_ABS_X = 0xDD,
		CMP_ABS_Y = 0xD9,
		CMP_ZPG = 0xC5,
		CMP_ZPG_X = 0xD5,
		CMP_IND_X = 0xC1,
		CMP_IND_Y = 0xD1,

		// CPX
		CPX_IMM = 0xE0,
		CPX_ABS = 0xEC,
		CPX_ZPG = 0xE4,

		// CPY
		CPY_IMM = 0xC0,
		CPY_ABS = 0xCC,
		CPY_ZPG = 0xC4,

		// SBC
		SBC_IMM = 0xE9,
		SBC_ABS = 0xED,
		SBC_ABS_X = 0xFD,
		SBC_ABS_Y = 0xF9,
		SBC_ZPG = 0xE5,
		SBC_ZPG_X = 0xF5,
		SBC_IND_X = 0xE1,
		SBC_IND_Y = 0xF1,

		// **** Increment and Decrement instructions ****

		// DEC
		DEC_ABS = 0xCE,
		DEC_ABS_X = 0xDE,
		DEC_ZPG = 0xC6,
		DEC_ZPG_X = 0xD6,

		DEX = 0xCA,
		DEY = 0x88,

		// INC
		INC_ABS = 0xEE,
		INC_ABS_X = 0xFE,
		INC_ZPG = 0xE6,
		INC_ZPG_X = 0xF6,

		INX = 0xE8,
		INY = 0xC8,

		// **** Control instructions ****

		BRK = 0x00,

		// JMP
		JMP_ABS = 0x4C,
		JMP_IND = 0x6C,

		JSR = 0x20,
		RTI = 0x40,
		RTS = 0x60,

		// **** Branch instructions ****

		BCC = 0x90,
		BCS = 0xB0,
		BEQ = 0xF0,
		BMI = 0x30,
		BNE = 0xD0,
		BPL = 0x10,
		BVC = 0x50,
		BVS = 0x70,

		// **** Flags instructions ****

		CLC = 0x18,
		CLD = 0xD8,
		CLI = 0x58,
		CLV = 0xB8,
		SEC = 0x38,
		SED = 0xF8,
		SEI = 0x78,

		// **** Miscellaneous instructions ****

		NOP = 0xEA
	};
}
