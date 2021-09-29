using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using System;
using System.Collections.Generic;

namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="OpcodeExecutionHandler"/> class.
	/// </summary>
	internal class OpcodeExecutionHandler
	{
		private Dictionary<OpCodes, Action> _handlers = new Dictionary<OpCodes, Action>();
		private int _numberOfJSRCalls = 0;
		private OpCodes[] _opcodeToEnumMap = new OpCodes[256];

		/// <summary>
		/// Initializes a new instance of the <see cref="OpcodeExecutionHandler"/> class.
		/// </summary>
		/// <param name="machine">The <see cref="Machine"/> instance for which this object is handling the execution of instructions.</param>
		public OpcodeExecutionHandler(Machine machine)
		{
			Machine = machine;

			PopulateOpcodeToEnumMap();
			InitOpcodeHandlers();
		}

		/// <summary>
		/// Gets or sets the <see cref="Machine"/> instance for which this <see cref="OpcodeExecutionHandler"/> instance
		/// is handling the execution of instructions
		/// </summary>
		private Machine Machine { get; set; }

		/// <summary>
		/// Execute the specified instruction.
		/// </summary>
		/// <param name="opCode">Enumerated value of the instruction to be executed.</param>
		public void Execute(OpCodes opCode)
		{
			if (_handlers.ContainsKey(opCode))
			{
				_handlers[opCode]?.Invoke();            // Call the handler method to execute the instruction.
			}
			else
			{
				throw new InvalidOperationException("Unhandled instruction: " + opCode.ToString());
			}
		}

		/// <summary>
		/// Execute the specified instruction.
		/// </summary>
		/// <param name="opcode">Byte value of the instruction to be executed.</param>
		public void Execute(byte opcode)
		{
			// Get enumerated value that corresponds to the specified opcode byte value and execute the instruction.
			Execute(_opcodeToEnumMap[opcode]);
		}

		/// <summary>
		/// Populate the array that maps byte opcode values to their corresponding <see cref="OpCodes"/> enumerated values.
		/// </summary>
		/// <remarks>
		/// This map allows fast access to the enumerated <see cref="OpCodes"/> values.
		/// </remarks>
		private void PopulateOpcodeToEnumMap()
		{
			foreach (var code in Enum.GetValues(typeof(OpCodes)))
			{
				_opcodeToEnumMap[(byte)code] = (OpCodes)code;
			}
		}

		/// <summary>
		/// Initialize the dictionary of Opcode handlers.
		/// </summary>
		private void InitOpcodeHandlers()
		{
			AddLoadAndStoreOpcodeHandlers();
			AddTransferOpcodeHandlers();
			AddStackOpcodeHandlers();
			AddShiftOpcodeHandlers();
			AddLogicOpcodeHandlers();
			AddArithmeticOpcodeHandlers();
			AddIncrementOpcodeHandlers();
			AddControlOpcodeHandlers();
			AddBranchOpcodeHandlers();
			AddFlagsOpcodeHandlers();
			AddMiscOpcodeHandlers();
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Load and Store instructions (LDA, LDX, LDY, STA, STX, STY)
		/// </summary>
		private void AddLoadAndStoreOpcodeHandlers()
		{
			_handlers.Add(OpCodes.LDA_IMM, OpLDA_Immediate);
			_handlers.Add(OpCodes.LDA_ABS, OpLDA_Absolute);
			_handlers.Add(OpCodes.LDA_ABS_X, OpLDA_AbsoluteX);
			_handlers.Add(OpCodes.LDA_ABS_Y, OpLDA_AbsoluteY);
			_handlers.Add(OpCodes.LDA_ZPG, OpLDA_ZeroPage);
			_handlers.Add(OpCodes.LDA_ZPG_X, OpLDA_ZeroPageX);
			_handlers.Add(OpCodes.LDA_IND_X, OpLDA_IndirectX);
			_handlers.Add(OpCodes.LDA_IND_Y, OpLDA_IndirectY);

			_handlers.Add(OpCodes.LDX_IMM, OpLDX_Immediate);
			_handlers.Add(OpCodes.LDX_ABS, OpLDX_Absolute);
			_handlers.Add(OpCodes.LDX_ABS_Y, OpLDX_AbsoluteY);
			_handlers.Add(OpCodes.LDX_ZPG, OpLDX_ZeroPage);
			_handlers.Add(OpCodes.LDX_ZPG_Y, OpLDX_ZeroPageY);

			_handlers.Add(OpCodes.LDY_IMM, OpLDY_Immediate);
			_handlers.Add(OpCodes.LDY_ABS, OpLDY_Absolute);
			_handlers.Add(OpCodes.LDY_ABS_X, OpLDY_AbsoluteX);
			_handlers.Add(OpCodes.LDY_ZPG, OpLDY_ZeroPage);
			_handlers.Add(OpCodes.LDY_ZPG_X, OpLDY_ZeroPageX);

			_handlers.Add(OpCodes.STA_ABS, OpSTA_Absolute);
			_handlers.Add(OpCodes.STA_ABS_X, OpSTA_AbsoluteX);
			_handlers.Add(OpCodes.STA_ABS_Y, OpSTA_AbsoluteY);
			_handlers.Add(OpCodes.STA_ZPG, OpSTA_ZeroPage);
			_handlers.Add(OpCodes.STA_ZPG_X, OpSTA_ZeroPageX);
			_handlers.Add(OpCodes.STA_IND_X, OpSTA_IndirectX);
			_handlers.Add(OpCodes.STA_IND_Y, OpSTA_IndirectY);

			_handlers.Add(OpCodes.STX_ABS, OpSTX_Absolute);
			_handlers.Add(OpCodes.STX_ZPG, OpSTX_ZeroPage);
			_handlers.Add(OpCodes.STX_ZPG_Y, OpSTX_ZeroPageY);

			_handlers.Add(OpCodes.STY_ABS, OpSTY_Absolute);
			_handlers.Add(OpCodes.STY_ZPG, OpSTY_ZeroPage);
			_handlers.Add(OpCodes.STY_ZPG_X, OpSTY_ZeroPageX);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Transfer instructions (TAX, TAY, TSX, TXA, TXS, TYA)
		/// </summary>
		private void AddTransferOpcodeHandlers()
		{
			_handlers.Add(OpCodes.TAX, OpTAX);
			_handlers.Add(OpCodes.TAY, OpTAY);
			_handlers.Add(OpCodes.TSX, OpTSX);
			_handlers.Add(OpCodes.TXA, OpTXA);
			_handlers.Add(OpCodes.TXS, OpTXS);
			_handlers.Add(OpCodes.TYA, OpTYA);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Stack instructions (PHA, PHP, PLA, PLP)
		/// </summary>
		private void AddStackOpcodeHandlers()
		{
			_handlers.Add(OpCodes.PHA, OpPHA);
			_handlers.Add(OpCodes.PHP, OpPHP);
			_handlers.Add(OpCodes.PLA, OpPLA);
			_handlers.Add(OpCodes.PLP, OpPLP);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Shift instructions (ASL, LSR, ROL, ROR)
		/// </summary>
		private void AddShiftOpcodeHandlers()
		{
			_handlers.Add(OpCodes.ASL_ACC, OpASL_Accumulator);
			_handlers.Add(OpCodes.ASL_ABS, OpASL_Absolute);
			_handlers.Add(OpCodes.ASL_ABS_X, OpASL_AbsoluteX);
			_handlers.Add(OpCodes.ASL_ZPG, OpASL_ZeroPage);
			_handlers.Add(OpCodes.ASL_ZPG_X, OpASL_ZeroPageX);

			_handlers.Add(OpCodes.LSR_ACC, OpLSR_Accumulator);
			_handlers.Add(OpCodes.LSR_ABS, OpLSR_Absolute);
			_handlers.Add(OpCodes.LSR_ABS_X, OpLSR_AbsoluteX);
			_handlers.Add(OpCodes.LSR_ZPG, OpLSR_ZeroPage);
			_handlers.Add(OpCodes.LSR_ZPG_X, OpLSR_ZeroPageX);

			_handlers.Add(OpCodes.ROL_ACC, OpROL_Accumulator);
			_handlers.Add(OpCodes.ROL_ABS, OpROL_Absolute);
			_handlers.Add(OpCodes.ROL_ABS_X, OpROL_AbsoluteX);
			_handlers.Add(OpCodes.ROL_ZPG, OpROL_ZeroPage);
			_handlers.Add(OpCodes.ROL_ZPG_X, OpROL_ZeroPageX);

			_handlers.Add(OpCodes.ROR_ACC, OpROR_Accumulator);
			_handlers.Add(OpCodes.ROR_ABS, OpROR_Absolute);
			_handlers.Add(OpCodes.ROR_ABS_X, OpROR_AbsoluteX);
			_handlers.Add(OpCodes.ROR_ZPG, OpROR_ZeroPage);
			_handlers.Add(OpCodes.ROR_ZPG_X, OpROR_ZeroPageX);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Logic instructions (AND, BIT, EOR, ORA)
		/// </summary>
		private void AddLogicOpcodeHandlers()
		{
			_handlers.Add(OpCodes.AND_IMM, OpAND_Immediate);
			_handlers.Add(OpCodes.AND_ABS, OpAND_Absolute);
			_handlers.Add(OpCodes.AND_ABS_X, OpAND_AbsoluteX);
			_handlers.Add(OpCodes.AND_ABS_Y, OpAND_AbsoluteY);
			_handlers.Add(OpCodes.AND_ZPG, OpAND_ZeroPage);
			_handlers.Add(OpCodes.AND_ZPG_X, OpAND_ZeroPageX);
			_handlers.Add(OpCodes.AND_IND_X, OpAND_IndirectX);
			_handlers.Add(OpCodes.AND_IND_Y, OpAND_IndirectY);

			_handlers.Add(OpCodes.BIT_ABS, OpBIT_Absolute);
			_handlers.Add(OpCodes.BIT_ZPG, OpBIT_ZeroPage);

			_handlers.Add(OpCodes.EOR_IMM, OpEOR_Immediate);
			_handlers.Add(OpCodes.EOR_ABS, OpEOR_Absolute);
			_handlers.Add(OpCodes.EOR_ABS_X, OpEOR_AbsoluteX);
			_handlers.Add(OpCodes.EOR_ABS_Y, OpEOR_AbsoluteY);
			_handlers.Add(OpCodes.EOR_ZPG, OpEOR_ZeroPage);
			_handlers.Add(OpCodes.EOR_ZPG_X, OpEOR_ZeroPageX);
			_handlers.Add(OpCodes.EOR_IND_X, OpEOR_IndirectX);
			_handlers.Add(OpCodes.EOR_IND_Y, OpEOR_IndirectY);

			_handlers.Add(OpCodes.ORA_IMM, OpORA_Immediate);
			_handlers.Add(OpCodes.ORA_ABS, OpORA_Absolute);
			_handlers.Add(OpCodes.ORA_ABS_X, OpORA_AbsoluteX);
			_handlers.Add(OpCodes.ORA_ABS_Y, OpORA_AbsoluteY);
			_handlers.Add(OpCodes.ORA_ZPG, OpORA_ZeroPage);
			_handlers.Add(OpCodes.ORA_ZPG_X, OpORA_ZeroPageX);
			_handlers.Add(OpCodes.ORA_IND_X, OpORA_IndirectX);
			_handlers.Add(OpCodes.ORA_IND_Y, OpORA_IndirectY);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Arithmetic instructions (ADC, CMP, CPX, CPY, SBC)
		/// </summary>
		private void AddArithmeticOpcodeHandlers()
		{
			_handlers.Add(OpCodes.ADC_IMM, OpADC_Immediate);
			_handlers.Add(OpCodes.ADC_ABS, OpADC_Absolute);
			_handlers.Add(OpCodes.ADC_ABS_X, OpADC_AbsoluteX);
			_handlers.Add(OpCodes.ADC_ABS_Y, OpADC_AbsoluteY);
			_handlers.Add(OpCodes.ADC_ZPG, OpADC_ZeroPage);
			_handlers.Add(OpCodes.ADC_ZPG_X, OpADC_ZeroPageX);
			_handlers.Add(OpCodes.ADC_IND_X, OpADC_IndirectX);
			_handlers.Add(OpCodes.ADC_IND_Y, OpADC_IndirectY);

			_handlers.Add(OpCodes.CMP_IMM, OpCMP_Immediate);
			_handlers.Add(OpCodes.CMP_ABS, OpCMP_Absolute);
			_handlers.Add(OpCodes.CMP_ABS_X, OpCMP_AbsoluteX);
			_handlers.Add(OpCodes.CMP_ABS_Y, OpCMP_AbsoluteY);
			_handlers.Add(OpCodes.CMP_ZPG, OpCMP_ZeroPage);
			_handlers.Add(OpCodes.CMP_ZPG_X, OpCMP_ZeroPageX);
			_handlers.Add(OpCodes.CMP_IND_X, OpCMP_IndirectX);
			_handlers.Add(OpCodes.CMP_IND_Y, OpCMP_IndirectY);

			_handlers.Add(OpCodes.CPX_IMM, OpCPX_Immediate);
			_handlers.Add(OpCodes.CPX_ABS, OpCPX_Absolute);
			_handlers.Add(OpCodes.CPX_ZPG, OpCPX_ZeroPage);

			_handlers.Add(OpCodes.CPY_IMM, OpCPY_Immediate);
			_handlers.Add(OpCodes.CPY_ABS, OpCPY_Absolute);
			_handlers.Add(OpCodes.CPY_ZPG, OpCPY_ZeroPage);

			_handlers.Add(OpCodes.SBC_IMM, OpSBC_Immediate);
			_handlers.Add(OpCodes.SBC_ABS, OpSBC_Absolute);
			_handlers.Add(OpCodes.SBC_ABS_X, OpSBC_AbsoluteX);
			_handlers.Add(OpCodes.SBC_ABS_Y, OpSBC_AbsoluteY);
			_handlers.Add(OpCodes.SBC_ZPG, OpSBC_ZeroPage);
			_handlers.Add(OpCodes.SBC_ZPG_X, OpSBC_ZeroPageX);
			_handlers.Add(OpCodes.SBC_IND_X, OpSBC_IndirectX);
			_handlers.Add(OpCodes.SBC_IND_Y, OpSBC_IndirectY);

		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Increment/Decrement instructions (DEC, DEX, DEY, INC, INX, INY)
		/// </summary>
		private void AddIncrementOpcodeHandlers()
		{
			_handlers.Add(OpCodes.DEC_ABS, OpDEC_Absolute);
			_handlers.Add(OpCodes.DEC_ABS_X, OpDEC_AbsoluteX);
			_handlers.Add(OpCodes.DEC_ZPG, OpDEC_ZeroPage);
			_handlers.Add(OpCodes.DEC_ZPG_X, OpDEC_ZeroPageX);

			_handlers.Add(OpCodes.DEX, OpDEX);
			_handlers.Add(OpCodes.DEY, OpDEY);

			_handlers.Add(OpCodes.INC_ABS, OpINC_Absolute);
			_handlers.Add(OpCodes.INC_ABS_X, OpINC_AbsoluteX);
			_handlers.Add(OpCodes.INC_ZPG, OpINC_ZeroPage);
			_handlers.Add(OpCodes.INC_ZPG_X, OpINC_ZeroPageX);

			_handlers.Add(OpCodes.INX, OpINX);
			_handlers.Add(OpCodes.INY, OpINY);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Control instructions (BRK, JMP, JSR, RTI, RTS)
		/// </summary>
		private void AddControlOpcodeHandlers()
		{
			_handlers.Add(OpCodes.BRK, OpBRK);

			_handlers.Add(OpCodes.JMP_ABS, OpJMP_Absolute);
			_handlers.Add(OpCodes.JMP_IND, OpJMP_Indirect);

			_handlers.Add(OpCodes.JSR, OpJSR);
			_handlers.Add(OpCodes.RTI, OpRTI);
			_handlers.Add(OpCodes.RTS, OpRTS);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Branch instructions (BCC, BCS, BEQ, BMI, BNE, BPL, BVC, BVS)
		/// </summary>
		private void AddBranchOpcodeHandlers()
		{
			_handlers.Add(OpCodes.BCC, OpBCC);
			_handlers.Add(OpCodes.BCS, OpBCS);
			_handlers.Add(OpCodes.BEQ, OpBEQ);
			_handlers.Add(OpCodes.BMI, OpBMI);
			_handlers.Add(OpCodes.BNE, OpBNE);
			_handlers.Add(OpCodes.BPL, OpBPL);
			_handlers.Add(OpCodes.BVC, OpBVC);
			_handlers.Add(OpCodes.BVS, OpBVS);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for the Flags instructions (CLC, CLD, CLI, CLV, SEC, SED, SEI)
		/// </summary>
		private void AddFlagsOpcodeHandlers()
		{
			_handlers.Add(OpCodes.CLC, OpCLC);
			_handlers.Add(OpCodes.CLD, OpCLD);
			_handlers.Add(OpCodes.CLI, OpCLI);
			_handlers.Add(OpCodes.CLV, OpCLV);
			_handlers.Add(OpCodes.SEC, OpSEC);
			_handlers.Add(OpCodes.SED, OpSED);
			_handlers.Add(OpCodes.SEI, OpSEI);
		}

		/// <summary>
		/// Add opcode handlers to the dictionary for other miscellaneous instructions (NOP)
		/// </summary>
		private void AddMiscOpcodeHandlers()
		{
			_handlers.Add(OpCodes.NOP, OpNOP);
		}


		// *****************************
		//
		// Opcode handler helper methods
		//
		// *****************************

		/// <summary>
		/// Set the state of the Negative and Zero flags dependent on the supplied value.
		/// </summary>
		/// <param name="value">The value to be checked to determine the state of the Negative and Zero flags.</param>
		private void SetNegativeAndZeroFlags(byte value)
		{
			Machine.CPU.SR.Negative = (value & 0x80) == 0x80;
			Machine.CPU.SR.Zero = value == 0;
		}

		/// <summary>
		/// Perform an arithmetic shift left of the specified value.
		/// </summary>
		/// <remarks>
		/// This method also sets the flags according to the result of the shift operation.
		/// </remarks>
		/// <param name="value">The value to be shifted.</param>
		/// <returns>The result of the shift operation.</returns>
		private byte ArithmeticShiftLeft(byte value)
		{
			ushort result = (ushort)(value << 1);
			SetNegativeAndZeroFlags((byte)(result & 0xFF));
			Machine.CPU.SR.Carry = (result & 0x0100) == 0x0100;
			return (byte)(result & 0xFF);
		}

		/// <summary>
		/// Perform a logical shift right of the specified value.
		/// </summary>
		/// <remarks>
		/// This method also sets the flags according to the result of the shift operation.
		/// </remarks>
		/// <param name="value">The value to be shifted.</param>
		/// <returns>The result of the shift operation.</returns>
		private byte LogicalShiftRight(byte value)
		{
			byte result = (byte)(value >> 1);
			Machine.CPU.SR.Zero = result == 0;
			Machine.CPU.SR.Negative = false;
			Machine.CPU.SR.Carry = (value & 0x01) == 0x01;
			return result;
		}

		/// <summary>
		/// Perform a rotate left of the specified value.
		/// </summary>
		/// <remarks>
		/// This method also sets the flags according to the result of the rotate operation.
		/// </remarks>
		/// <param name="value">The value to be rotated.</param>
		/// <returns>The result of the rotate operation.</returns>
		private byte RotateLeft(byte value)
		{
			ushort result = (ushort)(value << 1);
			if (Machine.CPU.SR.Carry)
			{
				result += 1;
			}
			SetNegativeAndZeroFlags((byte)(result & 0xFF));
			Machine.CPU.SR.Carry = (result & 0x0100) == 0x0100;
			return (byte)(result & 0xFF);
		}

		/// <summary>
		/// Perform a rotate right of the specified value.
		/// </summary>
		/// <remarks>
		/// This method also sets the flags according to the result of the rotate operation.
		/// </remarks>
		/// <param name="value">The value to be rotated.</param>
		/// <returns>The result of the rotate operation.</returns>
		private byte RotateRight(byte value)
		{
			byte result = (byte)(value >> 1);
			if (Machine.CPU.SR.Carry)
			{
				result += 0x80;
			}
			SetNegativeAndZeroFlags(result);
			Machine.CPU.SR.Carry = (value & 0x01) == 0x01;
			return result;
		}

		/// <summary>
		/// Adds the supplied value (with carry) to the Accumulator.
		/// </summary>
		/// <remarks>
		/// This method also sets the flags according to the result of the add operation.
		/// </remarks>
		/// <param name="value">The value to be added.</param>
		private void AddWithCarry(byte value)
		{
			// When in Decimal mode we need to perform BCD arithmetic instead of standard binary arithmetic.
			if (Machine.CPU.SR.Decimal)
			{
				DecimalAddWithCarry(value);
				return;
			}

			byte initA = Machine.CPU.A;
			ushort result = (ushort)(Machine.CPU.A + value);
			if (Machine.CPU.SR.Carry)
			{
				result++;
			}
			Machine.CPU.A = (byte)result;

			// Overflow flag is set when the sign of the values being added is the same but
			// differs from the sign of the sum.
			Machine.CPU.SR.Overflow = (~(initA ^ value) & (initA ^ Machine.CPU.A) & 0x80) == 0x80;
			SetNegativeAndZeroFlags(Machine.CPU.A);
			Machine.CPU.SR.Carry = result > 0x00FF;
		}

		/// <summary>
		/// Adds the supplied value (with carry) to the Accumulator in Decimal mode (i.e. using BCD arithmetic).
		/// </summary>
		/// <remarks>
		/// This method only affects the Carry flag, whose value is set according to the result of the add operation.
		/// On a 6502 processor, the state of the Negative, Zero, and Overflow flags are not consistent with
		/// the decimal result of an ADC instruction and are therefore not altered by this method.
		/// </remarks>
		/// <param name="value">The value to be added.</param>
		private void DecimalAddWithCarry(byte value)
		{
			bool carry = false;
			byte lo = (byte)((Machine.CPU.A & 0x0F) + (value & 0x0F) + (Machine.CPU.SR.Carry ? 1 : 0));
			ushort result = (ushort)((Machine.CPU.A & 0xF0) + (value & 0xF0) + (lo > 0x09 ? (0x10 + lo - 0x0A) : lo));
			if (result > 0x99)
			{
				carry = true;
				result -= 0xA0;
			}
			Machine.CPU.A = (byte)result;
			Machine.CPU.SR.Carry = carry;
		}

		/// <summary>
		/// Subtracts the supplied value from the Accumulator (with borrow).
		/// </summary>
		/// <remarks>
		/// For binary mode (i.e. not Decimal mode), subtraction is the same as performing an addition using the complemented value.
		/// This method sets the flags according to the result of the add operation.
		/// </remarks>
		/// <param name="value">The value to be subtracted.</param>
		private void SubtractWithBorrow(byte value)
		{
			// When in Decimal mode we need to perform BCD arithmetic instead of standard binary arithmetic.
			if (Machine.CPU.SR.Decimal)
			{
				DecimalSubtractWithBorrow(value);
				return;
			}

			AddWithCarry((byte)~value);
		}

		/// <summary>
		/// Subtracts the supplied value from the Accumulator (with borrow) in Decimal mode (i.e. using BCD arithmetic).
		/// </summary>
		/// <remarks>
		/// This method only affects the Carry flag, whose value is set according to the result of the subtract operation.
		/// On a 6502 processor, the state of the Negative, Zero, and Overflow flags are not consistent with
		/// the decimal result of a SBC instruction and are therefore not altered by this method.
		/// </remarks>
		/// <param name="value">The value to be subtracted.</param>
		private void DecimalSubtractWithBorrow(byte value)
		{
			bool carry = true;		// Resulting Carry is true by default for SBC instructions (as the negated Carry is handled as a Borrow flag).
			byte lo = (byte)((Machine.CPU.A & 0x0F) - (value & 0x0F) - (Machine.CPU.SR.Carry ? 0 : 1));
			byte hi = (byte)((Machine.CPU.A & 0xF0) - (value & 0xF0));

			if ((byte)(lo & 0x80) == 0x80)
			{
				lo = (byte)(lo + 0x0A);
				hi -= 0x10;
			}
			if (hi > 0x90)
			{
				hi = (byte)(hi + 0xA0);
				carry = false;
			}
			Machine.CPU.A = (byte)(hi + lo);
			Machine.CPU.SR.Carry = carry;
		}

		/// <summary>
		/// Compare the two specified value (one of which is the value of a register), setting the flags
		/// to indicate the result of the comparison.
		/// </summary>
		/// <param name="regValue">The value of a register.</param>
		/// <param name="value">The value to which the register value should be compared.</param>
		private void Compare(byte regValue, byte value)
		{
			byte result = (byte)(regValue - value);
			Machine.CPU.SR.Zero = (regValue == value);
			Machine.CPU.SR.Negative = (result & 0x80) == 0x80;
			Machine.CPU.SR.Carry = (regValue >= value);
		}


		// **********************
		//
		// Opcode handler methods
		//
		// **********************

		// Load and Store opcodes

		/// <summary>
		/// LDA #$nn
		/// </summary>
		private void OpLDA_Immediate()
		{
			Machine.CPU.A = Machine.ReadNextPCByte();
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA $nnnn
		/// </summary>
		private void OpLDA_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA $nnnn,X
		/// </summary>
		private void OpLDA_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA $nnnn,Y
		/// </summary>
		private void OpLDA_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA $nn
		/// </summary>
		private void OpLDA_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA $nn,X
		/// </summary>
		private void OpLDA_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA ($nn,X)
		/// </summary>
		private void OpLDA_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDA ($nn),Y
		/// </summary>
		private void OpLDA_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			Machine.CPU.A = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// LDX #$nn
		/// </summary>
		private void OpLDX_Immediate()
		{
			Machine.CPU.X = Machine.ReadNextPCByte();
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// LDX $nnnn
		/// </summary>
		private void OpLDX_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.X = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// LDX $nnnn,Y
		/// </summary>
		private void OpLDX_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Machine.CPU.X = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// LDX $nn
		/// </summary>
		private void OpLDX_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.CPU.X = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// LDX $nn,Y
		/// </summary>
		private void OpLDX_ZeroPageY()
		{
			var address = Machine.GetZeroPageYAddress();
			Machine.CPU.X = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// LDY #$nn
		/// </summary>
		private void OpLDY_Immediate()
		{
			Machine.CPU.Y = Machine.ReadNextPCByte();
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// LDY $nnnn
		/// </summary>
		private void OpLDY_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.Y = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// LDY $nnnn,X
		/// </summary>
		private void OpLDY_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.CPU.Y = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// LDY $nn
		/// </summary>
		private void OpLDY_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.CPU.Y = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// LDY $nn,X
		/// </summary>
		private void OpLDY_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.CPU.Y = Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// STA $nnnn
		/// </summary>
		private void OpSTA_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STA $nnnn,X
		/// </summary>
		private void OpSTA_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STA $nnnn,Y
		/// </summary>
		private void OpSTA_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STA $nn
		/// </summary>
		private void OpSTA_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STA $nn,X
		/// </summary>
		private void OpSTA_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STA ($nn,X)
		/// </summary>
		private void OpSTA_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STA ($nn),Y
		/// </summary>
		private void OpSTA_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			Machine.Memory.Write(address, Machine.CPU.A);
		}

		/// <summary>
		/// STX $nnnn
		/// </summary>
		private void OpSTX_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, Machine.CPU.X);
		}

		/// <summary>
		/// STX $nn
		/// </summary>
		private void OpSTX_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, Machine.CPU.X);
		}

		/// <summary>
		/// STX $nn,Y
		/// </summary>
		private void OpSTX_ZeroPageY()
		{
			var address = Machine.GetZeroPageYAddress();
			Machine.Memory.Write(address, Machine.CPU.X);
		}

		/// <summary>
		/// STY $nnnn
		/// </summary>
		private void OpSTY_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, Machine.CPU.Y);
		}

		/// <summary>
		/// STY $nn
		/// </summary>
		private void OpSTY_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, Machine.CPU.Y);
		}

		/// <summary>
		/// STY $nn,X
		/// </summary>
		private void OpSTY_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.Memory.Write(address, Machine.CPU.Y);
		}

		// Transfer opcodes

		/// <summary>
		/// TAX
		/// </summary>
		private void OpTAX()
		{
			Machine.CPU.X = Machine.CPU.A;
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// TAY
		/// </summary>
		private void OpTAY()
		{
			Machine.CPU.Y = Machine.CPU.A;
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// TSX
		/// </summary>
		private void OpTSX()
		{
			Machine.CPU.X = Machine.Stack.S;
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// TXA
		/// </summary>
		private void OpTXA()
		{
			Machine.CPU.A = Machine.CPU.X;
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// TXS
		/// </summary>
		private void OpTXS()
		{
			Machine.Stack.S = Machine.CPU.X;
			// Note: TXS instruction does not affect any flags.
		}

		/// <summary>
		/// TYA
		/// </summary>
		private void OpTYA()
		{
			Machine.CPU.A = Machine.CPU.Y;
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		// Stack opcodes

		/// <summary>
		/// PHA
		/// </summary>
		private void OpPHA()
		{
			Machine.Stack.Push(Machine.CPU.A);
		}

		/// <summary>
		/// PHP
		/// </summary>
		private void OpPHP()
		{
			byte value = (byte)Machine.CPU.SR.Flags;
			Machine.Stack.Push(value);
		}

		/// <summary>
		/// PLA
		/// </summary>
		private void OpPLA()
		{
			Machine.CPU.A = Machine.Stack.Pop();
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// PLP
		/// </summary>
		private void OpPLP()
		{
			Machine.CPU.SR.Flags = (ProcessorFlags)Machine.Stack.Pop();
		}

		// Shift opcodes


		/// <summary>
		/// ASL A
		/// </summary>
		private void OpASL_Accumulator()
		{
			Machine.CPU.A = ArithmeticShiftLeft(Machine.CPU.A);
		}

		/// <summary>
		/// ASL $nnnn
		/// </summary>
		private void OpASL_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, ArithmeticShiftLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ASL $nnnn,X
		/// </summary>
		private void OpASL_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.Memory.Write(address, ArithmeticShiftLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ASL $nn
		/// </summary>
		private void OpASL_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, ArithmeticShiftLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ASL $nn,X
		/// </summary>
		private void OpASL_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.Memory.Write(address, ArithmeticShiftLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// LSR A
		/// </summary>
		private void OpLSR_Accumulator()
		{
			Machine.CPU.A = LogicalShiftRight(Machine.CPU.A);
		}

		/// <summary>
		/// LSR $nnnn
		/// </summary>
		private void OpLSR_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, LogicalShiftRight(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// LSR $nnnn,X
		/// </summary>
		private void OpLSR_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.Memory.Write(address, LogicalShiftRight(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// LSR $nn
		/// </summary>
		private void OpLSR_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, LogicalShiftRight(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// LSR $nn,X
		/// </summary>
		private void OpLSR_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.Memory.Write(address, LogicalShiftRight(Machine.Memory.Read(address)));
		}


		/// <summary>
		/// ROL A
		/// </summary>
		private void OpROL_Accumulator()
		{
			Machine.CPU.A = RotateLeft(Machine.CPU.A);
		}

		/// <summary>
		/// ROL $nnnn
		/// </summary>
		private void OpROL_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, RotateLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ROL $nnnn,X
		/// </summary>
		private void OpROL_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.Memory.Write(address, RotateLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ROL $nn
		/// </summary>
		private void OpROL_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, RotateLeft(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ROL $nn,X
		/// </summary>
		private void OpROL_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.Memory.Write(address, RotateLeft(Machine.Memory.Read(address)));
		}


		/// <summary>
		/// ROR A
		/// </summary>
		private void OpROR_Accumulator()
		{
			Machine.CPU.A = RotateRight(Machine.CPU.A);
		}

		/// <summary>
		/// ROR $nnnn
		/// </summary>
		private void OpROR_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Memory.Write(address, RotateRight(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ROR $nnnn,X
		/// </summary>
		private void OpROR_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.Memory.Write(address, RotateRight(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ROR $nn
		/// </summary>
		private void OpROR_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.Memory.Write(address, RotateRight(Machine.Memory.Read(address)));
		}

		/// <summary>
		/// ROR $nn,X
		/// </summary>
		private void OpROR_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.Memory.Write(address, RotateRight(Machine.Memory.Read(address)));
		}


		// Logic opcodes

		/// <summary>
		/// AND #$nn
		/// </summary>
		private void OpAND_Immediate()
		{
			Machine.CPU.A &= Machine.ReadNextPCByte();
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND $nnnn
		/// </summary>
		private void OpAND_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND $nnnn,X
		/// </summary>
		private void OpAND_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND $nnnn,Y
		/// </summary>
		private void OpAND_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND $nn
		/// </summary>
		private void OpAND_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND $nn,X
		/// </summary>
		private void OpAND_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND ($nn,X)
		/// </summary>
		private void OpAND_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// AND ($nn),Y
		/// </summary>
		private void OpAND_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			Machine.CPU.A &= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// BIT $nnnn
		/// </summary>
		private void OpBIT_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			var value = Machine.Memory.Read(address);
			Machine.CPU.SR.Negative = (value & 0x80) == 0x80;
			Machine.CPU.SR.Overflow = (value & 0x40) == 0x40;
			Machine.CPU.SR.Zero = (Machine.CPU.A & value) == 0;
		}

		/// <summary>
		/// BIT $nn
		/// </summary>
		private void OpBIT_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			var value = Machine.Memory.Read(address);
			Machine.CPU.SR.Negative = (value & 0x80) == 0x80;
			Machine.CPU.SR.Overflow = (value & 0x40) == 0x40;
			Machine.CPU.SR.Zero = (Machine.CPU.A & value) == 0;
		}

		/// <summary>
		/// EOR #$nn
		/// </summary>
		private void OpEOR_Immediate()
		{
			Machine.CPU.A ^= Machine.ReadNextPCByte();
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR $nnnn
		/// </summary>
		private void OpEOR_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR $nnnn,X
		/// </summary>
		private void OpEOR_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR $nnnn,Y
		/// </summary>
		private void OpEOR_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR $nn
		/// </summary>
		private void OpEOR_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR $nn,X
		/// </summary>
		private void OpEOR_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR ($nn,X)
		/// </summary>
		private void OpEOR_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// EOR ($nn),Y
		/// </summary>
		private void OpEOR_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			Machine.CPU.A ^= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA #$nn
		/// </summary>
		private void OpORA_Immediate()
		{
			Machine.CPU.A |= Machine.ReadNextPCByte();
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA $nnnn
		/// </summary>
		private void OpORA_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA $nnnn,X
		/// </summary>
		private void OpORA_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA $nnnn,Y
		/// </summary>
		private void OpORA_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA $nn
		/// </summary>
		private void OpORA_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA $nn,X
		/// </summary>
		private void OpORA_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA ($nn,X)
		/// </summary>
		private void OpORA_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}

		/// <summary>
		/// ORA ($nn),Y
		/// </summary>
		private void OpORA_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			Machine.CPU.A |= Machine.Memory.Read(address);
			SetNegativeAndZeroFlags(Machine.CPU.A);
		}


		// Arithmetic opcodes


		/// <summary>
		/// ADC #$nn
		/// </summary>
		private void OpADC_Immediate()
		{
			AddWithCarry(Machine.ReadNextPCByte());
		}

		/// <summary>
		/// ADC $nnnn
		/// </summary>
		private void OpADC_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// ADC $nnnn,X
		/// </summary>
		private void OpADC_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// ADC $nnnn,Y
		/// </summary>
		private void OpADC_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// ADC $nn
		/// </summary>
		private void OpADC_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// ADC $nn,X
		/// </summary>
		private void OpADC_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// ADC ($nn,X)
		/// </summary>
		private void OpADC_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// ADC ($nn),Y
		/// </summary>
		private void OpADC_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			AddWithCarry(Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP #$nn
		/// </summary>
		private void OpCMP_Immediate()
		{
			Compare(Machine.CPU.A, Machine.ReadNextPCByte());
		}

		/// <summary>
		/// CMP $nnnn
		/// </summary>
		private void OpCMP_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP $nnnn,X
		/// </summary>
		private void OpCMP_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP $nnnn,Y
		/// </summary>
		private void OpCMP_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP $nn
		/// </summary>
		private void OpCMP_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP $nn,X
		/// </summary>
		private void OpCMP_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP ($nn,X)
		/// </summary>
		private void OpCMP_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CMP ($nn),Y
		/// </summary>
		private void OpCMP_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			Compare(Machine.CPU.A, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CPX #$nn
		/// </summary>
		private void OpCPX_Immediate()
		{
			Compare(Machine.CPU.X, Machine.ReadNextPCByte());
		}

		/// <summary>
		/// CPX $nnnn
		/// </summary>
		private void OpCPX_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Compare(Machine.CPU.X, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CPX $nn
		/// </summary>
		private void OpCPX_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Compare(Machine.CPU.X, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CPY #$nn
		/// </summary>
		private void OpCPY_Immediate()
		{
			Compare(Machine.CPU.Y, Machine.ReadNextPCByte());
		}

		/// <summary>
		/// CPY $nnnn
		/// </summary>
		private void OpCPY_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Compare(Machine.CPU.Y, Machine.Memory.Read(address));
		}

		/// <summary>
		/// CPY $nn
		/// </summary>
		private void OpCPY_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			Compare(Machine.CPU.Y, Machine.Memory.Read(address));
		}


		/// <summary>
		/// SBC #$nn
		/// </summary>
		private void OpSBC_Immediate()
		{
			SubtractWithBorrow(Machine.ReadNextPCByte());
		}

		/// <summary>
		/// SBC $nnnn
		/// </summary>
		private void OpSBC_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}

		/// <summary>
		/// SBC $nnnn,X
		/// </summary>
		private void OpSBC_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}

		/// <summary>
		/// SBC $nnnn,Y
		/// </summary>
		private void OpSBC_AbsoluteY()
		{
			var address = Machine.GetAbsoluteYAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}

		/// <summary>
		/// SBC $nn
		/// </summary>
		private void OpSBC_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}

		/// <summary>
		/// SBC $nn,X
		/// </summary>
		private void OpSBC_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}

		/// <summary>
		/// SBC ($nn,X)
		/// </summary>
		private void OpSBC_IndirectX()
		{
			var address = Machine.GetIndexedIndirectAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}

		/// <summary>
		/// SBC ($nn),Y
		/// </summary>
		private void OpSBC_IndirectY()
		{
			var address = Machine.GetIndirectIndexedAddress();
			SubtractWithBorrow(Machine.Memory.Read(address));
		}


		// Increment and Decrement opcodes

		/// <summary>
		/// DEC $nnnn
		/// </summary>
		private void OpDEC_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			byte value = Machine.Memory.Read(address);
			value--;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// DEC $nnnn,X
		/// </summary>
		private void OpDEC_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			byte value = Machine.Memory.Read(address);
			value--;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// DEC $nn
		/// </summary>
		private void OpDEC_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			byte value = Machine.Memory.Read(address);
			value--;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// DEC $nn,X
		/// </summary>
		private void OpDEC_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			byte value = Machine.Memory.Read(address);
			value--;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// DEX
		/// </summary>
		private void OpDEX()
		{
			Machine.CPU.X--;
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// DEY
		/// </summary>
		private void OpDEY()
		{
			Machine.CPU.Y--;
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}

		/// <summary>
		/// INC $nnnn
		/// </summary>
		private void OpINC_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			byte value = Machine.Memory.Read(address);
			value++;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// INC $nnnn,X
		/// </summary>
		private void OpINC_AbsoluteX()
		{
			var address = Machine.GetAbsoluteXAddress();
			byte value = Machine.Memory.Read(address);
			value++;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// INC $nn
		/// </summary>
		private void OpINC_ZeroPage()
		{
			var address = Machine.GetZeroPageAddress();
			byte value = Machine.Memory.Read(address);
			value++;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// INC $nn,X
		/// </summary>
		private void OpINC_ZeroPageX()
		{
			var address = Machine.GetZeroPageXAddress();
			byte value = Machine.Memory.Read(address);
			value++;
			Machine.Memory.Write(address, value);
			SetNegativeAndZeroFlags(value);
		}

		/// <summary>
		/// INX
		/// </summary>
		private void OpINX()
		{
			Machine.CPU.X++;
			SetNegativeAndZeroFlags(Machine.CPU.X);
		}

		/// <summary>
		/// INY
		/// </summary>
		private void OpINY()
		{
			Machine.CPU.Y++;
			SetNegativeAndZeroFlags(Machine.CPU.Y);
		}


		// Control opcodes

		/// <summary>
		/// BRK
		/// </summary>
		private void OpBRK()
		{
			// Currently unhandled.
		}

		/// <summary>
		/// JMP $nnnn
		/// </summary>
		private void OpJMP_Absolute()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.CPU.PC = address;
		}

		/// <summary>
		/// JMP ($nnnn)
		/// </summary>
		private void OpJMP_Indirect()
		{
			var address = Machine.GetIndirectAddress();
			Machine.CPU.PC = address;
		}

		/// <summary>
		/// JSR $nnnn
		/// </summary>
		private void OpJSR()
		{
			var address = Machine.GetAbsoluteAddress();
			Machine.Stack.Push((byte)(Machine.CPU.PC >> 8));
			Machine.Stack.Push((byte)Machine.CPU.PC);
			Machine.CPU.PC = address;
			_numberOfJSRCalls++;
		}

		/// <summary>
		/// RTI
		/// </summary>
		private void OpRTI()
		{
			// Currently unhandled.
		}

		/// <summary>
		/// RTS
		/// </summary>
		private void OpRTS()
		{
			// Pop the return address off the stack.
			byte low = Machine.Stack.Pop();
			byte high = Machine.Stack.Pop();
			ushort address = (ushort)((high << 8) + low);
			Machine.CPU.PC = address;

			// if no JSR instruction has been executed then this RTS marks the termination of the code execution.
			if (_numberOfJSRCalls == 0)
			{
				Machine.IsEndOfExecution = true;
				return;
			}

			_numberOfJSRCalls--;
		}


		// Branch opcodes

		/// <summary>
		/// BCC $nnnn
		/// </summary>
		private void OpBCC()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (!Machine.CPU.SR.Carry)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BCS $nnnn
		/// </summary>
		private void OpBCS()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (Machine.CPU.SR.Carry)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BEQ $nnnn
		/// </summary>
		private void OpBEQ()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (Machine.CPU.SR.Zero)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BMI $nnnn
		/// </summary>
		private void OpBMI()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (Machine.CPU.SR.Negative)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BNE $nnnn
		/// </summary>
		private void OpBNE()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (!Machine.CPU.SR.Zero)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BPL $nnnn
		/// </summary>
		private void OpBPL()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (!Machine.CPU.SR.Negative)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BVC $nnnn
		/// </summary>
		private void OpBVC()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (!Machine.CPU.SR.Overflow)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}

		/// <summary>
		/// BVS $nnnn
		/// </summary>
		private void OpBVS()
		{
			sbyte offset = (sbyte)Machine.ReadNextPCByte();
			if (Machine.CPU.SR.Overflow)
			{
				Machine.CPU.AddOffsetToPC(offset);
			}
		}


		// Flags opcodes

		/// <summary>
		/// CLC
		/// </summary>
		private void OpCLC()
		{
			Machine.CPU.SR.Carry = false;
		}

		/// <summary>
		/// CLD
		/// </summary>
		private void OpCLD()
		{
			Machine.CPU.SR.Decimal = false;
		}

		/// <summary>
		/// CLI
		/// </summary>
		private void OpCLI()
		{
			Machine.CPU.SR.Interrupt = false;
		}

		/// <summary>
		/// CLV
		/// </summary>
		private void OpCLV()
		{
			Machine.CPU.SR.Overflow = false;
		}

		/// <summary>
		/// SEC
		/// </summary>
		private void OpSEC()
		{
			Machine.CPU.SR.Carry = true;
		}

		/// <summary>
		/// SED
		/// </summary>
		private void OpSED()
		{
			Machine.CPU.SR.Decimal = true;
		}

		/// <summary>
		/// SEI
		/// </summary>
		private void OpSEI()
		{
			Machine.CPU.SR.Interrupt = true;
		}

		// Miscellaneous opcodes

		/// <summary>
		/// NOP
		/// </summary>
		private void OpNOP()
		{
			// No-op performs no operation (obviously)
		}
	}
}
