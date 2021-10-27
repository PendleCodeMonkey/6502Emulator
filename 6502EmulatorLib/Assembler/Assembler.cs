using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static PendleCodeMonkey.MOS6502EmulatorLib.Assembler.AssemblerEnumerations;

namespace PendleCodeMonkey.MOS6502EmulatorLib.Assembler
{
	/// <summary>
	/// Implementation of the <see cref="Assembler"/> class.
	/// </summary>
	public class Assembler
	{
		private readonly string _operators = "+-*/%";

		private int _currentAddress = 0;
		private int _currentLineNumber = 0;
		private bool _fatalErrorEncountered = false;

		private readonly AssemblerHelpers _aii = null;

		private readonly Dictionary<string, string> _equates = new Dictionary<string, string>();
		private readonly Dictionary<string, int> _labels = new Dictionary<string, int>();
		private readonly List<AssemblerInstructionInfo> _instructions = new List<AssemblerInstructionInfo>();
		private readonly List<string> _reservedWords = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Assembler"/> class.
		/// </summary>
		public Assembler()
		{
			Reset();
			_aii = new AssemblerHelpers();
			_aii.GenerateAddressModeFlags();
			GenerateReservedWordList();
		}

		/// <summary>
		/// Gets a collection of tuples containing details of errors that occurred during assembly.
		/// </summary>
		public List<(int LineNumber, Errors Error, string AdditionalInfo)> AsmErrors { get; private set; }

		/// <summary>
		/// Gets a collection of tuples containing information about data segments created during
		/// assembler operation (i.e. start address and length in bytes of each data segment).
		/// This information can then be fed into the disassembler to prevent it attempting to
		/// disassemble non-executable binary data.
		/// </summary>
		public List<(ushort startAddress, ushort size)> DataSegments { get; private set; }

		/// <summary>
		/// Run the source code (supplied as a list of strings) through the 6502 assembler.
		/// </summary>
		/// <remarks>
		/// This is the main entry point for operation of the assembler.
		/// </remarks>
		/// <param name="code">The 6502 assembly language source code as a collection of strings (one
		/// string per line of source code).</param>
		/// <returns>
		/// A tuple consisting of the following fields:
		///		Success - <c>true</c> if the assembler completed successfully; otherwise <c>false</c>.
		///		BinaryData - The generated binary data as a collection of bytes (only if successful).
		///	</returns>
		public (bool Success, List<byte> BinaryData) Assemble(List<string> code)
		{
			int lineNumber = 1;

			Reset();

			// First pass.
			// Parses directives and instructions from the supplied source code. Generates binary code with
			// placeholders where the operand values will be placed during the second pass.
			foreach (var line in code)
			{
				ParseLine(line, lineNumber);

				// If we have encountered a fatal error then there's little point continuing.
				if (_fatalErrorEncountered)
				{
					break;
				}

				// Increment the line number counter.
				lineNumber++;
			}

			// Second pass.
			// Evaluate the actual values of the operands (as this could not fully be done during the first pass) - this fills in
			// the placeholders that were created during the first pass (which can now be done as the values of all
			// equates and labels, etc. should have been fully determined during the first pass).
			// Of course, we only perform the second pass if we have not encountered a fatal error during the first pass.
			if (!_fatalErrorEncountered)
			{
				EvaluateOperands();

				// Look for errors (specifically, any unresolved operands)
				foreach (var inst in _instructions)
				{
					if (!inst.OperandsResolved)
					{
						LogError(Errors.UnresolvedOperandValue, inst.Operand, inst.LineNumber);
					}
				}
			}

			bool success = false;
			List<byte> binaryData = new List<byte>();
			if (AsmErrors.Count == 0)
			{
				// No errors have been logged so we should be OK to generate the final binary data.
				// We call on each AssemberInstructionInfo object to generate its own binary data, adding it
				// to the final binary blob.
				foreach (var instruction in _instructions)
				{
					binaryData.AddRange(instruction.BinaryData);
				}
				success = true;
			}
			else
			{
				// Sort the logged errors by line number.
				AsmErrors = AsmErrors.OrderBy(x => x.LineNumber).ToList();
			}

			return (success, binaryData);
		}

		/// <summary>
		/// Reset the assembler internal data to its default settings.
		/// </summary>
		private void Reset()
		{
			_equates.Clear();
			_labels.Clear();
			_instructions.Clear();
			_fatalErrorEncountered = false;
			AsmErrors = new List<(int LineNumber, Errors Error, string AdditionalInfo)>();
			DataSegments = new List<(ushort startAddress, ushort size)>();
		}

		/// <summary>
		/// Evaluate the values of the operand.
		/// </summary>
		/// <remarks>
		/// This method performs virtually all of the operations required for the second pass of the
		/// assembler (i.e. calculating the final values of operands after all EQUs and label values
		/// have been determined by the first pass).
		/// </remarks>
		private void EvaluateOperands()
		{
			foreach (var instruction in _instructions)
			{
				_currentAddress = instruction.Address;
				_currentLineNumber = instruction.LineNumber;

				// If the AssemblerInstructionInfo object corresponds to data segment then
				// determine that segments values.
				if (instruction.DataSegment != null)
				{

					if (instruction.DataSegment.Type == DataSegmentType.Byte)
					{
						DefineByteSegment(null, instruction);
					}
					else if (instruction.DataSegment.Type == DataSegmentType.Word)
					{
						DefineWordSegment(null, instruction);
					}
				}
				else
				{
					// Not a data segment so evaluate the operand for the 6502 instruction.
					EvaluateOperand(instruction);
				}

				// Regenerate the binary data (to include the actual operand values instead of the placeholders from the first pass)
				instruction.GenerateBinaryData();
			}
		}

		/// <summary>
		/// Evaluate a single 6502 instruction operand.
		/// </summary>
		/// <param name="aii">The <see cref="AssemblerInstructionInfo"/> instance corresponding to the 6502 instruction.</param>
		private void EvaluateOperand(AssemblerInstructionInfo aii)
		{
			byte? byteOp = null;
			ushort? wordOp = null;
			byte? disp = null;
			string operand = aii.Operand;
			switch (aii.Instruction.AddrMode)
			{
				case AddressingModes.Immediate:
					{
						var (evaluated, value) = Evaluate(operand[1..]);
						if (evaluated)
						{
							aii.OperandsResolved = true;
							if (value >= -128 && value <= 255)
							{
								byteOp = (byte)value;
							}
							else
							{
								LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
							}
						}
					}
					break;
				case AddressingModes.IndexedXIndirect:
					{
						int pos = operand.ToUpper().IndexOf(",X");
						if (pos >= 0)
						{
							var (evaluated, value) = Evaluate(operand[1..pos]);
							if (evaluated)
							{
								aii.OperandsResolved = true;
								if (value >= -128 && value <= 255)
								{
									byteOp = (byte)value;
								}
								else
								{
									LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
								}
							}
						}
					}
					break;
				case AddressingModes.IndirectIndexedY:
					{
						var (evaluated, value) = Evaluate(operand[1..^1]);
						if (evaluated)
						{
							aii.OperandsResolved = true;
							if (value >= -128 && value <= 255)
							{
								byteOp = (byte)value;
							}
							else
							{
								LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
							}
						}
					}
					break;
				case AddressingModes.Indirect:
					{
						var (evaluated, value) = Evaluate(operand[1..^1]);
						if (evaluated)
						{
							aii.OperandsResolved = true;
							if (value >= 0 && value <= 65535)
							{
								wordOp = (ushort)(value & 0xFFFF);
							}
							else
							{
								LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
							}
						}
					}
					break;
				case AddressingModes.Absolute:
				case AddressingModes.AbsoluteX:
				case AddressingModes.AbsoluteY:
					{
						var (evaluated, value) = Evaluate(operand);
						if (evaluated)
						{
							aii.OperandsResolved = true;
							if (value >= -32768 && value <= 65535)
							{
								wordOp = (ushort)(value & 0xFFFF);
							}
							else
							{
								LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
							}
						}
					}
					break;
				case AddressingModes.Relative:
					{
						var (evaluated, value) = Evaluate(operand);
						if (evaluated)
						{
							aii.OperandsResolved = true;
							if (value >= 0 && value <= 65535)
							{
								wordOp = (ushort)(value & 0xFFFF);
								var displacement = wordOp.Value - (aii.Address + aii.BinaryData.Count);
								if (displacement < -128 || displacement > 127)
								{
									LogError(Errors.DisplacementOutOfRange, operand, aii.LineNumber);
								}
								else
								{
									disp = (byte)displacement;
								}
							}
							else
							{
								LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
							}
						}
					}
					break;
				case AddressingModes.ZeroPage:
				case AddressingModes.ZeroPageX:
				case AddressingModes.ZeroPageY:
					{
						var (evaluated, value) = Evaluate(operand);
						if (evaluated)
						{
							aii.OperandsResolved = true;
							if (value >= -128 && value <= 255)
							{
								byteOp = (byte)value;
							}
							else
							{
								LogError(Errors.OperandValueOutOfRange, operand, aii.LineNumber);
							}
						}
					}
					break;
			}

			if (byteOp.HasValue)
			{
				aii.Instruction.ByteOperand = byteOp.Value;
			}
			if (wordOp.HasValue)
			{
				aii.Instruction.WordOperand = wordOp.Value;
			}
			if (disp.HasValue)
			{
				aii.Instruction.Displacement = disp.Value;
			}
		}

		/// <summary>
		/// Parse a single line of Z80 assembly language source code.
		/// </summary>
		/// <param name="line">A string containing a single line of Z80 assembly language source code.</param>
		/// <param name="lineNumber">The number of this line in the source code.</param>
		private void ParseLine(string line, int lineNumber)
		{
			_currentLineNumber = lineNumber;

			if (string.IsNullOrEmpty(line) || line[0] == ';')
			{
				// An empty line or a commented out line, so ignore it.
				return;
			}

			// Use a regular expression to tokenize the line of text.
			Regex regex = new Regex(@"(?<tkn>^[\w$_:]*)|((?<tkn>[\w$_.()-]+\s[+/*-]\s[\w$_.()-]+)|(?<tkn>[""'(](.*?)(?<!\\)[""')]|(?<tkn>[\w'#$&%_=+*/.()-]+))(\s)*)|(?<cmt>;.*)", RegexOptions.None);

			var tokens = (from Match m in regex.Matches(line)
						  where m.Groups["tkn"].Success
						  select m.Groups["tkn"].Value).ToList();

			var comment = (from Match m in regex.Matches(line)
						   where m.Groups["cmt"].Success
						   select m.Groups["cmt"].Value).ToList();

			if (tokens.Count > 1)
			{
				// Check for special case of character constant immediately followed by a token starting with a '+' (i.e. a potential
				// arithmetic operation on a character constant) - to cover strings such as "'A'+$80".
				int tokenNum = 1;
				while (tokenNum < tokens.Count)
				{
					if (tokens[tokenNum].Length == 3 && tokens[tokenNum][0] == '\'' && tokens[tokenNum][2] == '\'')
					{
						if (tokenNum < tokens.Count - 1 && tokens[tokenNum + 1].Length > 1 && tokens[tokenNum + 1].StartsWith('+'))
						{
							// Join the character constant token with the arithmetic operation token and remove the
							// artihmetic operation token (i.e. merge the two tokens into a single token)
							tokens[tokenNum] += tokens[tokenNum + 1];
							tokens.RemoveAt(tokenNum + 1);
						}
					}
					tokenNum++;
				}
			}

			// Perform some processing of potential arithmetic operations.
			if (tokens.Count > 2)
			{
				int index = 2;
				while (index < tokens.Count)
				{
					int operatorIndex = -1;
					int tokNum = index;
					for (; tokNum < tokens.Count; tokNum++)
					{
						if (tokens[tokNum].Length == 1 && _operators.Contains(tokens[tokNum]))
						{
							operatorIndex = tokNum;
							break;
						}
					}

					if (operatorIndex >= 0)
					{
						while (tokens.Count > operatorIndex)
						{
							if (tokens[operatorIndex].Length == 1 && _operators.Contains(tokens[operatorIndex]))
							{
								tokens[operatorIndex - 1] += tokens[operatorIndex] + tokens[operatorIndex + 1];

								tokens.RemoveAt(operatorIndex + 1);
								tokens.RemoveAt(operatorIndex);

							}
							else
							{
								break;
							}
						}
					}
					index = tokNum + 1;
				}
			}

			if (tokens.Count >= 1)
			{
				string label = null;

				// If the first token ends with a colon then treat it as a label.
				if (tokens[0].EndsWith(':'))
				{
					label = tokens[0].TrimEnd(':');
				}

				// Determine if this line is an EQU
				if (tokens.Count > 2)
				{
					if (tokens[1].ToUpper().Equals("EQU") || tokens[1].Equals("="))
					{
						string equName = label ?? tokens[0];
						if (_reservedWords.Contains(equName))
						{
							LogError(Errors.EQUNameCannotBeReservedWord, equName);
						}
						else
						{
							if (!_equates.ContainsKey(equName))
							{
								_equates.Add(equName, tokens[2]);
							}
							else
							{
								LogError(Errors.CannotRedefineEquValue, equName);
							}
						}

						// We've handled the EQU and therefore finished with this line of source code, so just return.
						return;
					}
				}

				// If this line contains a label (that is not an EQU) then handle it as a label for the current address.
				if (label != null)
				{
					if (_reservedWords.Contains(label))
					{
						LogError(Errors.LabelNameCannotBeReservedWord, label);
					}
					else
					{
						if (!_labels.ContainsKey(label))
						{
							_labels.Add(label, _currentAddress);
						}
						else
						{
							LogError(Errors.CannotHaveDuplicateLabelNames, label);
						}
					}

					// We remove token[0] as it is a label that has now been handled.
					tokens.RemoveAt(0);
				}
			}

			if (tokens.Count > 0 && string.IsNullOrWhiteSpace(tokens[0]))
			{
				// We remove token[0] as it only contains whitespace (and can therefore be ignored).
				tokens.RemoveAt(0);
			}

			if (tokens.Count == 0)
			{
				// No more tokens on this line so there is nothing more to be done.
				return;
			}

			// Try to handle the first token as an assembler directive or as a Z80 instruction.
			string cmd = tokens[0].ToUpper();
			switch (cmd)
			{
				case "ORG":
					if (tokens.Count > 1)
					{
						var orgAddress = GetAsNumber(tokens[1]);
						if (orgAddress.HasValue)
						{
							if (orgAddress.Value >= 0 && orgAddress.Value <= 0xFFFF)
							{
								_currentAddress = orgAddress.Value;
							}
							else
							{
								LogError(Errors.OrgAddressOutOfValidRange, tokens[1]);
							}
						}
						else
						{
							LogError(Errors.InvalidOrgAddress, tokens[1]);
						}
					}
					break;
				case "DB":
				case "DEFB":
				case "DM":
				case "DEFM":
					DefineByteSegment(tokens, null);
					break;
				case "DW":
				case "DEFW":
					DefineWordSegment(tokens, null);
					break;
				case "DS":
				case "DEFS":
					DefineSpaceSegment(tokens);
					break;

				default:
					// If not recognised as an assembler directive then try to handle as a 6502 instruction with operands
					var (inst, operand1, resolved) = GetOpcode(tokens);
					if (inst != null)
					{
						var asmInstrInfo = new AssemblerInstructionInfo(lineNumber, (ushort)_currentAddress, inst, operand1, resolved);
						var binData = asmInstrInfo.GenerateBinaryData();
						_instructions.Add(asmInstrInfo);
						_currentAddress += binData.Count;
					}
					else
					{
						LogError(Errors.InvalidInstruction, line.Trim());
					}
					break;
			}

			// Check if the current assembly address has gone beyond the range of memory, flagging a fatal
			// error if it does.
			if (_currentAddress > 0xFFFF)
			{
				LogError(Errors.CurrentAddressOutOfRange, _currentAddress.ToString());
				_fatalErrorEncountered = true;
			}
		}

		/// <summary>
		/// Attempt to retrieve details for a 6502 instruction.
		/// </summary>
		/// <param name="tokens">Collection of strings containing the tokens making up the instruction.</param>
		/// <returns>
		/// A tuple consisting of the following elements:
		///		inst - An <see cref="Instruction"/> object containing details of the instruction (or null if not a valid instruction)
		///		operand1 - A string containing the first operand.
		///		op1Type - Enumerated type of the first operand.
		///		operand2 - A string containing the second operand.
		///		op2Type - Enumerated type of the second operand.
		/// </returns>
		private (Instruction inst, string operand, bool resolved) GetOpcode(List<string> tokens)
		{
			bool operandsResolved = false;
			string operand = null;

			if (tokens == null || tokens.Count == 0)
			{
				return default;
			}

			string instruction = tokens[0].ToUpper();

			Instruction foundInst = null;

			var opInfo = AnalyzeOperands(instruction, tokens);

			AddressingModes addrMode = AddressingModes.Implied;

			if (_aii.AddressModeFlags.ContainsKey(instruction))
			{
				// Apply bitwise AND to restrict the addressing modes to ones that are valid for the instruction.
				AMFlag maskedAM = opInfo.addrModeFlag & _aii.AddressModeFlags[instruction];

				// Count the number of set bits in the masked addressing mode flag
				int bitCount = BitCount((ushort)maskedAM);

				// if bitcount is zero then we have invalid addressing mode operands.
				// if bitcount is 1 then we have narrowed the addressing mode down to a single possibility, so use that one.
				// if bitcount > 1 then we have multiple possibilities for the addressing mode.

				if (bitCount == 0)
				{
					return default;
				}
				else if (bitCount == 1)
				{
					addrMode = _aii.AMFlagToAddressingMode(maskedAM);
				}
				else
				{
					if (!instruction.Equals("JSR") && !instruction.Equals("JMP") && opInfo.byteOperand.HasValue)
					{
						maskedAM &= (AMFlag.Zero | AMFlag.ZeroX | AMFlag.ZeroY);
					}
					else
					{
						maskedAM &= (AMFlag.Abs | AMFlag.AbsX | AMFlag.AbsY);
					}
					bitCount = BitCount((ushort)maskedAM);
					if (bitCount == 1)
					{
						addrMode = _aii.AMFlagToAddressingMode(maskedAM);
					}
				}

				if ((instruction.Equals("JSR") || instruction.Equals("JMP")) && !opInfo.wordOperand.HasValue && opInfo.byteOperand.HasValue)
				{
					// JSR and JMP require a 16-bit operand, so if we only have an 8-bit operand then use it as the 16-bit value.
					opInfo.wordOperand = opInfo.byteOperand.Value;
				}

				byte? opcode = InstructionInfo.FindInstruction(instruction, addrMode);
				if (opcode.HasValue)
				{
					operandsResolved = addrMode != AddressingModes.Relative && (opInfo.byteOperand.HasValue || opInfo.wordOperand.HasValue ||
											addrMode == AddressingModes.Implied || addrMode == AddressingModes.Accumulator);
					foundInst = new Instruction(opcode.Value, instruction, addrMode, opInfo.byteOperand ?? 0, opInfo.wordOperand ?? 0, 0);
				}
			}

			if (tokens.Count > 1)
			{
				operand = tokens[1];
			}

			return (foundInst, operand, operandsResolved);
		}

		/// <summary>
		/// Analyze the operands specified for a single 6502 instruction in order to attempt to
		/// determine the addressing mode being used.
		/// </summary>
		/// <param name="instruction">The 6502 instruction mnemonic.</param>
		/// <param name="tokens">A collection of token strings containing the operands.</param>
		/// <returns>A tupole consisting of the following fields:
		///		addrModeFlag - An enumerated bit mask giving details of the possible addressing modes based on the format of the operands.
		///		byteOperand - The 8-bit operand value (if one could be evaluated); otherwise null.
		///		wordOperand - The 16-bit operand value (if one could be evaluated); otherwise null.
		///	</returns>
		private (AMFlag addrModeFlag, byte? byteOperand, ushort? wordOperand) AnalyzeOperands(string instruction, List<string> tokens)
		{
			string operand1 = null;
			string operand2 = null;
			if (tokens.Count > 1)
			{
				operand1 = tokens[1];
			}
			if (tokens.Count > 2)
			{
				operand2 = tokens[2];
			}

			AMFlag amFlag = 0;
			byte? byteOp = null;
			ushort? wordOp = null;

			if (!string.IsNullOrEmpty(operand1))
			{
				if (operand1.ToUpper().Equals("A"))
				{
					// Accumulator addressing mode
					amFlag |= AMFlag.Acc;
				}
				else if (operand1.StartsWith('#'))
				{
					// Immediate addressing mode
					amFlag |= AMFlag.Imm;
					var (evaluated, value) = Evaluate(operand1[1..]);
					if (evaluated)
					{
						if (value >= -128 && value <= 255)
						{
							byteOp = (byte)value;
						}
					}
				}
				else if (operand1.StartsWith('(') && operand1.EndsWith(')'))
				{
					int pos = operand1.ToUpper().IndexOf(",X");
					if (pos >= 0)
					{
						// Looks like X-Indexed Zero Page Indirect
						amFlag |= AMFlag.IndexXInd;
						var (evaluated, value) = Evaluate(operand1[1..pos]);
						if (evaluated)
						{
							if (value >= -128 && value <= 255)
							{
								byteOp = (byte)value;
							}
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(operand2))
						{
							if (operand2.ToUpper().Equals("Y"))
							{
								// Looks like Zero Page Indirect Y-Indexed
								amFlag |= AMFlag.IndIndexY;
								var (evaluated, value) = Evaluate(operand1[1..^1]);
								if (evaluated)
								{
									if (value >= -128 && value <= 255)
									{
										byteOp = (byte)value;
									}
								}
							}
						}
						else
						{
							// No second operand so it looks like Indirect addressing mode.
							amFlag |= AMFlag.Ind;
							var (evaluated, value) = Evaluate(operand1[1..^1]);
							if (evaluated)
							{
								if (value >= 0 && value <= 65535)
								{
									wordOp = (ushort)(value & 0xFFFF);
								}
							}
						}
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(operand2))
					{
						if (operand2.ToUpper().Equals("X"))
						{
							// Looks like X-Indexed Absolute or X-Indexed Zero Page
							amFlag |= AMFlag.AbsX | AMFlag.ZeroX;
						}
						if (operand2.ToUpper().Equals("Y"))
						{
							// Looks like Y-Indexed Absolute or Y-Indexed Zero Page
							amFlag |= AMFlag.AbsY | AMFlag.ZeroY;
						}
					}
					else
					{
						// No second operand so could be Absolute, Zero Page, or Relative
						amFlag |= AMFlag.Abs | AMFlag.Zero | AMFlag.Rel;
					}

					// Try to evaluate the first operand
					var (evaluated, value) = Evaluate(operand1);
					if (evaluated)
					{
						if (value >= -128 && value <= 255)
						{
							byteOp = (byte)value;
							amFlag &= (AMFlag.Zero | AMFlag.ZeroX | AMFlag.ZeroY | AMFlag.Rel |
								AMFlag.Abs | AMFlag.AbsX | AMFlag.AbsY);
						}
						else if (value >= -32768 && value <= 65535)
						{
							wordOp = (ushort)(value & 0xFFFF);
							amFlag &= (AMFlag.Abs | AMFlag.AbsX | AMFlag.AbsY | AMFlag.Rel);
						}
					}
					else
					{
						// If the operand could not be evaluated then default to absolute or relative addressing mode
						// (that it, using a 16-bit operand - i.e. NOT Zero-page)
						amFlag &= (AMFlag.Abs | AMFlag.AbsX | AMFlag.AbsY | AMFlag.Rel);
					}
				}
			}
			else
			{
				// No operands so assume Implied addressing mode (unless one of the shift/rotate instructions that can operate on the
				// accumulator, in which case we assume Accumulator addressing mode).
				string[] accumulatorInstructions = new string[] { "ASL", "LSR", "ROL", "ROR" };
				AMFlag flag = accumulatorInstructions.Any(x => x.Equals(instruction)) ? AMFlag.Acc : AMFlag.Impl;
				amFlag |= flag;
			}

			return (amFlag, byteOp, wordOp);
		}

		/// <summary>
		/// Define a byte data segment (for example, for a DB or DEFB directive).
		/// </summary>
		/// <remarks>
		/// During the first pass each element of the data segment will be populated with an empty
		/// placeholder (except for quoted strings, whose value can be determined during the first pass)
		/// this is because it may not be possible to fully resolve the final element value until after
		/// completion of the first pass (when the values of all EQUs and labels have been determined).
		/// </remarks>
		/// <param name="tokens">Collection of token strings making up this data segment.</param>
		/// <param name="aii">
		///		An <see cref="AssemblerHelpers"/> instance corresponding to this data segment.
		///		Will be null during the first pass but will contain a valid instance during the second pass.
		///	</param>
		private void DefineByteSegment(List<string> tokens, AssemblerInstructionInfo aii)
		{
			if (aii != null && aii.DataSegment != null)
			{
				tokens = aii.DataSegment.Tokens;
			}

			if (tokens.Count < 2)
			{
				return;
			}

			List<byte> dataSegment = new List<byte>();
			for (int tokenNum = 1; tokenNum < tokens.Count; tokenNum++)
			{
				string token = tokens[tokenNum];

				// Check if token is a quoted string (either single or double quotes can be used)
				if (token.Length > 2 && (token[0] == '\"' || token[0] == '\'') && (token[^1] == '\"' || token[^1] == '\''))
				{
					string quotedString = token[1..^1];
					foreach (var chr in quotedString)
					{
						dataSegment.Add((byte)chr);
					}
				}
				else
				{
					// Not a quoted string so we expect a numeric value.
					byte dataValue = 0;

					// Only evaluate the tokens on the second pass (when we have been supplied a non-null AssemblerInstructionInfo object).
					// On the first pass we just insert placeholder bytes (with zero value).
					if (aii != null)
					{
						var (evaluated, value) = Evaluate(token);
						if (evaluated)
						{
							if (value >= -128 && value <= 255)
							{
								dataValue = (byte)(value & 0xFF);
							}
							else
							{
								LogError(Errors.ByteSegmentValueOutOfRange, token);
							}
						}
						else
						{
							LogError(Errors.InvalidByteSegmentValue, token);
						}
					}

					dataSegment.Add(dataValue);
				}
			}

			var dsi = new DataSegmentInfo(DataSegmentType.Byte, tokens, dataSegment);
			if (aii != null)
			{
				aii.DataSegment = dsi;
				_ = aii.GenerateBinaryData();
			}
			else
			{
				var asmInstrInfo = new AssemblerInstructionInfo(_currentLineNumber, (ushort)_currentAddress, null, null, true, dsi);
				var binData = asmInstrInfo.GenerateBinaryData();
				_instructions.Add(asmInstrInfo);
				DataSegments.Add(((ushort)_currentAddress, (ushort)binData.Count));
				_currentAddress += binData.Count;
			}
		}

		/// <summary>
		/// Define a word data segment (for example, for a DW or DEFW directive).
		/// </summary>
		/// <remarks>
		/// During the first pass each element of the data segment will be populated with an empty
		/// placeholder (except for quoted strings, whose value can be determined during the first pass)
		/// this is because it may not be possible to fully resolve the final element value until after
		/// completion of the first pass (when the values of all EQUs and labels have been determined).
		/// </remarks>
		/// <param name="tokens">Collection of token strings making up this data segment.</param>
		/// <param name="aii">
		///		An <see cref="AssemblerHelpers"/> instance corresponding to this data segment.
		///		Will be null during the first pass but will contain a valid instance during the second pass.
		///	</param>
		private void DefineWordSegment(List<string> tokens, AssemblerInstructionInfo aii)
		{
			if (aii != null && aii.DataSegment != null)
			{
				tokens = aii.DataSegment.Tokens;
			}

			if (tokens.Count < 2)
			{
				return;
			}

			List<byte> dataSegment = new List<byte>();
			for (int tokenNum = 1; tokenNum < tokens.Count; tokenNum++)
			{
				ushort dataValue = 0;

				// Only evaluate the tokens on the second pass (when we have been supplied a non-null AssemblerInstructionInfo object).
				// On the first pass we just insert placeholder words (with zero value).
				if (aii != null)
				{
					var (evaluated, value) = Evaluate(tokens[tokenNum]);
					if (evaluated)
					{
						if (value >= -32768 && value <= 65535)
						{
							dataValue = (ushort)(value & 0xFFFF);
						}
						else
						{
							LogError(Errors.WordSegmentValueOutOfRange, tokens[tokenNum]);
						}
					}
					else
					{
						LogError(Errors.InvalidWordSegmentValue, tokens[tokenNum]);
					}
				}

				dataSegment.Add((byte)dataValue);
				dataSegment.Add((byte)(dataValue >> 8 & 0xFF));
			}

			var dsi = new DataSegmentInfo(DataSegmentType.Word, tokens, dataSegment);
			if (aii != null)
			{
				aii.DataSegment = dsi;
				_ = aii.GenerateBinaryData();
			}
			else
			{
				var asmInstrInfo = new AssemblerInstructionInfo(_currentLineNumber, (ushort)_currentAddress, null, null, false, dsi);
				var binData = asmInstrInfo.GenerateBinaryData();
				_instructions.Add(asmInstrInfo);
				DataSegments.Add(((ushort)_currentAddress, (ushort)binData.Count));
				_currentAddress += binData.Count;
			}
		}

		/// <summary>
		/// Define a space data segment (for example, for a DS or DEFS directive).
		/// </summary>
		/// <remarks>
		/// This data segment must be fully resolved during the first pass (as the correct amount
		/// of memory space needs to be allocated for it during the first pass).
		/// </remarks>
		/// <param name="tokens">Collection of token strings making up this data segment.</param>
		private void DefineSpaceSegment(List<string> tokens)
		{
			ushort size = 0;
			byte initValue = 0;

			if (tokens.Count < 2)
			{
				return;
			}

			// Tokens must be evaluated during the first pass (as we need to know how many
			// bytes we need to allocate during the first pass)
			var (evaluated, sizeValue) = Evaluate(tokens[1]);
			if (evaluated)
			{
				if (sizeValue >= 1 && sizeValue <= 65535)
				{
					size = (ushort)(sizeValue & 0xFFFF);
				}
				else
				{
					LogError(Errors.SpaceSegmentSizeOutOfRange, tokens[1]);
				}
			}
			else
			{
				LogError(Errors.SpaceSegmentInvalidParameter, tokens[1]);
			}

			if (tokens.Count > 2)
			{
				var (eval, value) = Evaluate(tokens[2]);
				if (eval)
				{
					if (value >= -128 && value <= 255)
					{
						initValue = (byte)(value & 0xFF);
					}
					else
					{
						LogError(Errors.SpaceSegmentInitializeValueOutOfRange, tokens[2]);
					}
				}
				else
				{
					LogError(Errors.SpaceSegmentInvalidParameter, tokens[2]);
				}

			}

			List<byte> dataSegment = Enumerable.Repeat(initValue, size).ToList();

			var dsi = new DataSegmentInfo(DataSegmentType.Space, tokens, dataSegment);

			var asmInstrInfo = new AssemblerInstructionInfo(_currentLineNumber, (ushort)_currentAddress, null, null, false, dsi);
			var binData = asmInstrInfo.GenerateBinaryData();
			_instructions.Add(asmInstrInfo);
			DataSegments.Add(((ushort)_currentAddress, (ushort)binData.Count));
			_currentAddress += binData.Count;
		}

		/// <summary>
		/// Log an assembler error.
		/// </summary>
		/// <param name="error">Enumerated value of the error to be logged.</param>
		/// <param name="additionalInfo">A string containing additional information related to the error (e.g. invalid operand value, etc.)</param>
		/// <param name="lineNumber">The line number in the source code where the error occurs (or the current line number if null).</param>
		private void LogError(Errors error, string additionalInfo, int? lineNumber = null)
		{
			int lineNum = lineNumber ?? _currentLineNumber;

			// Make sure we don't log an identical error more than once.
			if (AsmErrors.Where(x => x.LineNumber == lineNum && x.Error == error).Any())
			{
				return;
			}

			AsmErrors.Add((lineNum, error, additionalInfo));
		}

		/// <summary>
		/// Attempt to get the specified string data as a numeric value.
		/// </summary>
		/// <remarks>
		/// This method handles values specified as decimal, binary or hexadecimal strings, as character constants, or
		/// as values that correspond to registered EQUs and labels.
		/// </remarks>
		/// <param name="data">The string to be converted to a numeric value.</param>
		/// <returns>The integer numeric value if successfully converted, otherwise null.</returns>
		private int? GetAsNumber(string data)
		{
			string dataWithoutSign = data;
			int multiplier = 1;

			// If the data starts with a '-' then set the multiplier to -1 and strip off the
			// first character (so the number we will get will be positive and when we multiply by
			// the multiplier, we get a negative number).
			if (data[0] == '-')
			{
				multiplier = -1;
				dataWithoutSign = data[1..];
			}
			else if (data[0] == '+')
			{
				// If the data starts with a '+' then just strip it off.
				dataWithoutSign = data[1..];
			}

			// Check if decimal number
			Match match = Regex.Match(data, @"^([-+]?[0-9]+)$");
			int? num;
			if (match.Success)
			{
				try
				{
					num = int.Parse(match.Groups[1].Value);
					return num;
				}
				catch { }
			}

			// Check if hexadecimal number (which can be prefixed with an ampersand or a dollar sign,
			// or followed by H or h).
			match = Regex.Match(dataWithoutSign, @"^[&$]([0-9A-Fa-f]+)$");
			if (match.Success)
			{
				try
				{
					num = Convert.ToInt32(match.Groups[1].Value, 16);
					return num * multiplier;
				}
				catch { }
			}

			match = Regex.Match(dataWithoutSign, @"^([0-9A-Fa-f]+)[Hh]$");
			if (match.Success)
			{
				try
				{
					num = Convert.ToInt32(match.Groups[1].Value, 16);
					return num * multiplier;
				}
				catch { }
			}


			// Check if binary number (which can be prefixed with a percentage sign
			// or followed by B or b).
			match = Regex.Match(data, @"^[%]([01]+)$");
			if (match.Success)
			{
				try
				{
					num = Convert.ToInt32(match.Groups[1].Value, 2);
					return num;
				}
				catch { }
			}

			match = Regex.Match(data, @"^([01]+)[Bb]+$");
			if (match.Success)
			{
				try
				{
					num = Convert.ToInt32(match.Groups[1].Value, 2);
					return num;
				}
				catch { }
			}

			//Check if a char constant
			if (data.Length == 3 && data[0] == '\'' && data[2] == '\'')
			{
				int val = data[1];
				return val;
			}

			// Doesn't seem to be a numeric value or a character constant so check if it corresponds to a registered EQU.
			if (_equates.ContainsKey(dataWithoutSign))
			{
				// Try to evaluate it assuming it might contain some arithmetic operations.
				var (evaluated, value) = Evaluate(_equates[dataWithoutSign]);
				if (evaluated)
				{
					return value * multiplier;
				}
			}

			// Check if data is a registered label name.
			if (_labels.ContainsKey(data))
			{
				return _labels[data];
			}

			// The current address should be returned if the token is a dollar sign.
			if (data.Equals("$"))
			{
				return _currentAddress;
			}

			return null;
		}

		/// <summary>
		/// Determine if the specified character is one of the supported arithmetic operators (+, -, *, /, or %).
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private bool IsOperator(char ch) => _operators.Contains(ch);

		/// <summary>
		/// Perform simple tokenization of arithmetic expressions.
		/// </summary>
		/// <param name="expression">The expression to be tokenized.</param>
		/// <returns>A collection of strings containing the tokens extracted from the supplied expression.</returns>
		private List<string> ArithmeticLexer(string expression)
		{
			List<string> tokens = new List<string>();
			StringBuilder sb = new StringBuilder();

			// Replace double operators with corresponding single operators (e.g. replace a double-minus "--" with a plus "+", etc.)
			expression = expression.Replace("--", "+");
			expression = expression.Replace("+-", "-");
			expression = expression.Replace("-+", "-");

			for (var i = 0; i < expression.Length; i++)
			{
				var ch = expression[i];

				// Ignore any whitespace characters.
				if (char.IsWhiteSpace(ch))
				{
					continue;
				}

				if (!IsOperator(ch))
				{
					sb.Append(ch);

					while (i + 1 < expression.Length && (!IsOperator(expression[i + 1])))
					{
						sb.Append(expression[++i]);
					}

					tokens.Add(sb.ToString());
					sb.Clear();
					continue;
				}

				tokens.Add(ch.ToString());
			}

			// if the first token is a '-' or a '+' then merge it into the second token and remove the first (so
			// an expression starting with tokens "-" and "20" will have them merged so it will start with a single
			// token of "-20")
			if (tokens.Count > 1 && (tokens[0] == "-" || tokens[0] == "+"))
			{
				tokens[1] = tokens[0] + tokens[1];
				tokens.RemoveAt(0);
			}

			return tokens;
		}

		/// <summary>
		/// Attempt to evaluate a given expression.
		/// </summary>
		/// <remarks>
		/// This method provides support for evaluating expressions that may contain arithmetic operations.
		/// </remarks>
		/// <param name="expression">The expression to be evaluated.</param>
		/// <returns>A tuple consisting of the following:
		///		evaluated - <c>true</c> if the expression was successfully evaluated, otherwise <c>false</c>.
		///		value - The result of the evaluation (if successful)
		/// </returns>
		private (bool evaluated, int value) Evaluate(string expression)
		{
			// Nothing to do if the expression is empty.
			if (string.IsNullOrEmpty(expression))
			{
				return (false, 0);
			}

			// If the expression is enclosed within brackets then evaluate the value within those brackets.
			if (expression[0] == '(' && expression[^1] == ')')
			{
				expression = expression[1..^1];
			}

			// Try to evaluate the expression as a number as-is (because it might not contain any arithmetic operators)
			var equNum = GetAsNumber(expression);
			if (equNum.HasValue)
			{
				return (true, equNum.Value);
			}

			// The expression could not simply be converted to a number as-is so treat it as though it may
			// contain arithmetic operations.
			bool evaluated = true;
			int value = 0;
			var tokens = ArithmeticLexer(expression);

			var numOperators = tokens.Where(x => x.Length == 1 && IsOperator(x[0])).Count();
			if (numOperators > 0)
			{
				var asNumber = GetAsNumber(tokens[0].Trim());
				if (asNumber.HasValue)
				{
					value = asNumber.Value;
				}
				else
				{
					evaluated = false;
				}

				int tokNum = 1;
				while (tokNum < tokens.Count)
				{
					string op = tokens[tokNum++];
					if (tokNum < tokens.Count)
					{
						string tokVal = tokens[tokNum++];
						asNumber = GetAsNumber(tokVal.Trim());
						if (asNumber.HasValue)
						{
							switch (op)
							{
								case "+":
									value += asNumber.Value;
									break;
								case "-":
									value -= asNumber.Value;
									break;
								case "*":
									value *= asNumber.Value;
									break;
								case "/":
									if (asNumber.Value == 0)
									{
										LogError(Errors.DivideByZero, expression);
									}
									else
									{
										value /= asNumber.Value;
									}
									break;
								case "%":
									if (asNumber.Value == 0)
									{
										LogError(Errors.DivideByZero, expression);
									}
									else
									{
										value %= asNumber.Value;
									}
									break;
							}
						}
						else
						{
							evaluated = false;
						}
					}
				}
			}
			else
			{
				evaluated = false;
			}

			return (evaluated, value);
		}

		/// <summary>
		/// Generate a list of reserved words (i.e. 6502 instructions, assembler directives, register names, etc.).
		/// </summary>
		/// <remarks>
		/// This list is used to check that these reserved words are not used for label names, EQUs, etc.)
		/// </remarks>
		private void GenerateReservedWordList()
		{
			// Add 6502 instruction mnemonics.
			foreach (var inst in InstructionInfo.Instructions)
			{
				var name = inst.Value.Name;
				if (!_reservedWords.Contains(name))
				{
					_reservedWords.Add(name);
				}
			}

			// Add register names.
			_reservedWords.Add("A");
			_reservedWords.Add("X");
			_reservedWords.Add("Y");

			// Add assembler directives.
			_reservedWords.Add("ORG");
			_reservedWords.Add("DB");
			_reservedWords.Add("DEFB");
			_reservedWords.Add("DW");
			_reservedWords.Add("DEFW");
			_reservedWords.Add("DS");
			_reservedWords.Add("DEFS");
			_reservedWords.Add("DM");
			_reservedWords.Add("DEFM");
			_reservedWords.Add("EQU");
			_reservedWords.Add("$");
		}

		/// <summary>
		/// Count the number of bits that are set (i.e. have a value of 1) in the specified 16-bit value.
		/// </summary>
		/// <param name="value">The 16-bit value for which we want to count the number of set bits.</param>
		/// <returns>The number of bits that are set in the specified 16-bit value.</returns>
		private static int BitCount(ushort value)
		{
			byte[] bitCounts = new byte[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
											1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
											1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
											2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
											1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
											2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
											2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
											3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
											1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
											2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
											2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
											3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
											2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
											3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
											3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
											4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8 };

			int count = 0;
			ulong val = value;
			count += bitCounts[val & 0xff];
			val >>= 8;
			count += bitCounts[val & 0xff];

			return count;
		}
	}
}
