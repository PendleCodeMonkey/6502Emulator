using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Disassembler"/> class.
	/// </summary>
	public class Disassembler
	{
		private const int MaxNonExecDataBlockSize = 16;
		private readonly OpCodes[] _opcodeToEnumMap = new OpCodes[256];
		private readonly List<(ushort Address, ushort Length)> _nonExecutableSections = new List<(ushort, ushort)>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Disassembler"/> class.
		/// </summary>
		/// <param name="machine">The <see cref="Machine"/> instance for which this object is handling the disassembly of instructions.</param>
		/// <param name="startAddress">The start address of the block of memory being disassembled.</param>
		/// <param name="length">The length (in bytes) of the block of memory being disassembled.</param>
		public Disassembler(Machine machine, ushort startAddress, ushort length)
		{
			Machine = machine ?? throw new ArgumentNullException(nameof(machine));
			PopulateOpcodeToEnumMap();
			StartAddress = startAddress;
			Length = length;
			CurrentAddress = StartAddress;
		}

		/// <summary>
		/// Gets or sets the <see cref="Machine"/> instance for which this <see cref="Disassembler"/> instance
		/// is handling the disassembly of instructions
		/// </summary>
		private Machine Machine { get; set; }

		/// <summary>
		/// Gets or sets the start address of the block of memory being disassembled.
		/// </summary>
		private ushort StartAddress { get; set; }

		/// <summary>
		/// Gets or sets the length of the block of memory being disassembled.
		/// </summary>
		private ushort Length { get; set; }

		/// <summary>
		/// Gets or sets the address of the current byte in the block of memory being disassembled.
		/// </summary>
		internal ushort CurrentAddress { get; set; }

		/// <summary>
		/// Gets a value indicating if the disassembly has reached the end of the specified block of memory.
		/// </summary>
		internal bool IsEndOfData => CurrentAddress >= StartAddress + Length;

		/// <summary>
		/// Gets the list of non-executable sections (i.e. blocks of memory that the disassembler treats as non-executable)
		/// </summary>
		public List<(ushort Address, ushort Length)> NonExecutableSections => _nonExecutableSections;

		/// <summary>
		/// Add details of a non-executable block of data.
		/// </summary>
		/// <remarks>
		/// Non-executable sections are blocks of memory that contain data that is not executable code.
		/// Such data blocks are shown in the disassembly output using a DB directive.
		/// </remarks>
		/// <param name="startAddress">The start address of the block of non-executable data.</param>
		/// <param name="length">The length (in bytes) of the block of non-executable data.</param>
		public void AddNonExecutableSection(ushort startAddress, ushort length)
		{
			_nonExecutableSections.Add((startAddress, length));
		}

		/// <summary>
		/// Remove the record of a specific non-executable block of data.
		/// </summary>
		/// <remarks>
		/// Note that this does not actually remove the data itself, it just stops the disassembler treating
		/// that block of data as non-executable.
		/// </remarks>
		/// <param name="sectionIndex">Zero-based index of the non-executable block to be removed.</param>
		/// <returns><c>true</c> if the record of the non-executabe block was removed, otherwise <c>false</c>.</returns>
		public bool RemoveNonExecutableSection(int sectionIndex)
		{
			if (sectionIndex >= 0 && sectionIndex < _nonExecutableSections.Count)
			{
				_nonExecutableSections.RemoveAt(sectionIndex);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Perform a full disassembly of the specified block of memory.
		/// </summary>
		/// <returns>A list of tuples/ Each tuple contains:
		///   1. The memory address of the instruction.
		///   2. A string containing the disassembled version of the instruction/data at that address.
		/// </returns>
		public List<(ushort Address, string Disassembly)> Disassemble()
		{
			List<(ushort, string)> result = new List<(ushort, string)>();
			while (!IsEndOfData)
			{
				var nonExecSection = WithinNonExecutableSection();
				if (nonExecSection >= 0)
				{
					result.Add((CurrentAddress, NonExecutableData(nonExecSection)));
				}
				else
				{
					result.Add((CurrentAddress, DisassembleInstruction()));
				}
			}
			return result;
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
		/// Return the byte located at the current address, and then increment the current address value.
		/// </summary>
		/// <returns>The byte located at the current address.</returns>
		private byte ReadNextByte()
		{
			if (IsEndOfData)
			{
				throw new InvalidOperationException("Disassembly has run past the end of the loaded data.");
			}
			byte value = Machine.Memory.Read(CurrentAddress);
			CurrentAddress++;
			return value;
		}

		/// <summary>
		/// Helper method to get the next 8 bit value (for instructions that use Immediate or Zero Page addressing modes, etc.)
		/// </summary>
		/// <returns>The next 8 bit value read from the data.</returns>
		private ushort Get8BitValue() => ReadNextByte();

		/// <summary>
		/// Helper method to get the next 16 bit value (for instructions that use Absolute addressing modes, etc.)
		/// </summary>
		/// <returns>The next 16 bit value read from the data.</returns>
		private ushort Get16BitValue()
		{
			byte low_byte = ReadNextByte();
			byte high_byte = ReadNextByte();
			ushort address = (ushort)((high_byte << 8) + low_byte);
			return address;
		}

		/// <summary>
		/// Helper method to get the address for instructions that use the Relative addressing mode.
		/// </summary>
		/// <returns>The 16-bit Absolute address obtained by applying the relative offset to the current address.</returns>
		private ushort GetRelativeAddress()
		{
			sbyte offset = (sbyte)ReadNextByte();
			ushort address = (ushort)(CurrentAddress + offset);
			return address;
		}

		/// <summary>
		/// Determines if the current address is within a non-executable data block.
		/// </summary>
		/// <returns>The zero-based index of the non-executable data block that the current address falls within, or -1 if
		/// the current address is within executable code.</returns>
		private int WithinNonExecutableSection()
		{
			foreach (var nonExec in _nonExecutableSections)
			{
				if (CurrentAddress >= nonExec.Address && CurrentAddress < (nonExec.Address + nonExec.Length))
				{
					return _nonExecutableSections.IndexOf(nonExec);
				}
			}

			return -1;
		}

		/// <summary>
		/// Returns a string containing a block of non-executable data.
		/// </summary>
		/// <remarks>
		/// Non-executable data it output in the disassembly using a DB directive.
		/// Non-executable data sections are output in blocks of a maximum of 16 bytes (as set by MaxNonExecDataBlockSize).
		/// </remarks>
		/// <param name="nonExecSection">Zero-based index of the non-executable section.</param>
		/// <returns>A string containing the disassembled output for the block of non-executable data.</returns>
		private string NonExecutableData(int nonExecSection)
		{
			StringBuilder sb = new StringBuilder();
			var section = _nonExecutableSections[nonExecSection];

			sb.Append("DB ");

			int bytesRemaining = section.Address + section.Length - CurrentAddress;
			for (int i = 0; i < Math.Min(MaxNonExecDataBlockSize, bytesRemaining); i++)
			{
				byte value = ReadNextByte();
				if (i > 0)
				{
					sb.Append(", ");
				}
				sb.Append($"0x{value:X2}");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Disassemble the instruction located at the current address in memory.
		/// </summary>
		/// <returns>A string containing the disassembled version of the instruction at the current address.</returns>
		private string DisassembleInstruction()
		{
			byte instruction = ReadNextByte();
			var opcode = _opcodeToEnumMap[instruction];
			if (opcode == OpCodes.BRK && instruction != 0)
			{
				// If opcode is retrieved as "BRK" but the byte instruction value is anything other than zero (which
				// actually does corrsespond to "BRK") then it's an illegal instruction.
				return "???";
			}
			var (Name, AddrMode) = InstructionInfo.Instructions[opcode];
			string format = InstructionInfo.AddressingModeFormat[AddrMode];
			ushort value = 0;
			switch (AddrMode)
			{
				case AddressingModes.Absolute:
				case AddressingModes.AbsoluteX:
				case AddressingModes.AbsoluteY:
				case AddressingModes.Indirect:
					value = Get16BitValue();
					break;
				case AddressingModes.Immediate:
				case AddressingModes.ZeroPage:
				case AddressingModes.ZeroPageX:
				case AddressingModes.ZeroPageY:
				case AddressingModes.IndexedXIndirect:
				case AddressingModes.IndirectIndexedY:
					value = Get8BitValue();
					break;
				case AddressingModes.Relative:
					value = GetRelativeAddress();
					break;
				default:
					break;
			}
			return Name + string.Format(format, value);
		}
	}
}
