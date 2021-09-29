using System;
using PendleCodeMonkey.MOS6502EmulatorLib.Enumerations;

namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Machine"/> class.
	/// </summary>
	public class Machine
	{
		private ushort _loadedAddress;
		private ushort _dataLength;

		/// <summary>
		/// Initializes a new instance of the <see cref="Machine"/> class.
		/// </summary>
		public Machine()
		{
			CPU = new CPU();
			Memory = new Memory();
			Stack = new Stack(Memory);
			ExecutionHandler = new OpcodeExecutionHandler(this);
			IsEndOfExecution = false;
		}

		/// <summary>
		/// Gets the <see cref="CPU"/> instance used by this machine.
		/// </summary>
		internal CPU CPU { get; private set; }

		/// <summary>
		/// Gets the <see cref="Memory"/> instance used by this machine.
		/// </summary>
		internal Memory Memory { get; private set; }

		/// <summary>
		/// Gets the <see cref="Stack"/> instance used by this machine.
		/// </summary>
		internal Stack Stack { get; private set; }

		/// <summary>
		/// Gets the <see cref="OpcodeExecutionHandler"/> instance used by this machine.
		/// </summary>
		/// <remarks>
		/// This instance performs the execution of the 6502 instructions.
		/// </remarks>
		internal OpcodeExecutionHandler ExecutionHandler { get; private set; }

		/// <summary>
		/// Gets a value indicating if the machine has reached the end of the loaded executable data.
		/// </summary>
		internal bool IsEndOfData => CPU.PC >= _loadedAddress + _dataLength;

		/// <summary>
		/// Gets a value indicating if the execution of code has been terminated.
		/// </summary>
		/// <remarks>
		/// This can occur when an RTS instruction has been executed that was not within a subroutine invoked
		/// via the JSR instruction (i.e. an RTS instruction intended to mark the end of execution).
		/// </remarks>
		internal bool IsEndOfExecution { get; set; }

		/// <summary>
		/// Load executable data into memory at the specified address.
		/// </summary>
		/// <remarks>
		/// Loading executable data also sets the Program Counter to the address of the loaded data.
		/// </remarks>
		/// <param name="data">The executable data to be loaded.</param>
		/// <param name="loadAddress">The address at which the executable data should be loaded.</param>
		/// <param name="clearBeforeLoad"><c>true</c> if all memory should be cleared prior to loading the data, otherwise <c>false</c>.</param>
		/// <returns><c>true</c> if the executable data was successfully loaded, otherwise <c>false</c>.</returns>
		public bool LoadExecutableData(byte[] data, ushort loadAddress, bool clearBeforeLoad = true)
		{
			if (Memory.LoadData(data, loadAddress, clearBeforeLoad))
			{
				CPU.PC = loadAddress;
				_loadedAddress = loadAddress;
				_dataLength = (ushort)data.Length;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Load non-executable data into memory at the specified address.
		/// </summary>
		/// <param name="data">The data to be loaded.</param>
		/// <param name="loadAddress">The address at which the data should be loaded.</param>
		/// <param name="clearBeforeLoad"><c>true</c> if all memory should be cleared prior to loading the data, otherwise <c>false</c>.</param>
		/// <returns><c>true</c> if the data was successfully loaded, otherwise <c>false</c>.</returns>
		public bool LoadData(byte[] data, ushort loadAddress, bool clearBeforeLoad = true)
		{
			return Memory.LoadData(data, loadAddress, clearBeforeLoad);
		}

		/// <summary>
		/// Sets the state of CPU registers, Program Counter, Stack Pointer, and Flag settings.
		/// </summary>
		/// <param name="A">Accumulator value (optional).</param>
		/// <param name="X">X Register value (optional).</param>
		/// <param name="Y">Y Register value (optional).</param>
		/// <param name="PC">Program Counter value (optional).</param>
		/// <param name="S">Stack Pointer value (optional).</param>
		/// <param name="flags">Flags setting (optional).</param>
		public void SetState(byte? A = null, byte? X = null, byte? Y = null, ushort? PC = null, byte? S = null, ProcessorFlags? flags = null)
		{
			CPU.SetState(A, X, Y, PC, flags);
			if (S.HasValue)
			{
				Stack.S = S.Value;
			}
		}

		/// <summary>
		/// Gets the current state of the CPU settings.
		/// </summary>
		/// <returns>A tuple containing the values of the Accumulator, X Register, Y Register, Program Counter, Stack Pointer, and Flags).</returns>
		public (byte A, byte X, byte Y, ushort PC, byte S, ProcessorFlags Flags) GetState()
		{
			return (CPU.A, CPU.X, CPU.Y, CPU.PC, Stack.S, CPU.SR.Flags);
		}

		/// <summary>
		/// Return the byte located at the Program Counter, and then increment the Program Counter.
		/// </summary>
		/// <returns>The byte located at the Program Counter.</returns>
		internal byte ReadNextPCByte()
		{
			if (IsEndOfData)
			{
				throw new InvalidOperationException("Execution has run past the end of the loaded data.");
			}
			byte value = Memory.Read(CPU.PC);
			CPU.IncrementPC();
			return value;
		}

		/// <summary>
		/// Start executing instructions from the current Program Counter address.
		/// </summary>
		/// <remarks>
		/// This method keeps executing instructions until the program terminates and is therefore the
		/// main entry point for executing 6502 code.
		/// </remarks>
		public void Execute()
		{
			while (!IsEndOfData && !IsEndOfExecution)
			{
				ExecuteInstruction();
			}
		}

		/// <summary>
		/// Execute a single instruction located at the current Program Counter address.
		/// </summary>
		public void ExecuteInstruction()
		{
			byte value = ReadNextPCByte();
			ExecutionHandler.Execute(value);
		}

		/// <summary>
		/// Reset the machine to its default state.
		/// </summary>
		public void Reset()
		{
			Memory.Clear();
			CPU.Reset();
			Stack.Reset();
			IsEndOfExecution = false;
			_loadedAddress = 0;
			_dataLength = 0;
		}

		/// <summary>
		/// Dumps the current state of the machine in a string format.
		/// </summary>
		/// <returns>A string containing details of the current state of the machine.</returns>
		public string Dump()
		{
			string dump = "";

			dump += $"A: 0x{CPU.A:X2} ({CPU.A})";
			dump += Environment.NewLine;
			dump += $"X: 0x{CPU.X:X2} ({CPU.X})";
			dump += Environment.NewLine;
			dump += $"Y: 0x{CPU.Y:X2} ({CPU.Y})";
			dump += Environment.NewLine;
			dump += $"PC: 0x{CPU.PC:X4} ({CPU.PC})";
			dump += Environment.NewLine;
			dump += $"SP: 0x{Stack.S:X2} ({Stack.S})";
			dump += Environment.NewLine;
			dump += "Flags: " + CPU.SR.Flags.ToString();
			dump += Environment.NewLine;

			return dump;
		}

		/// <summary>
		/// Return the specified block of memory.
		/// </summary>
		/// <param name="address">Start address of the requested block of memory.</param>
		/// <param name="length">Length (in bytes) of the block of memory to be retrieved.</param>
		/// <returns>Read-only copy of the requested memory.</returns>
		public ReadOnlySpan<byte> DumpMemory(ushort address, ushort length)
		{
			return Memory.DumpMemory(address, length);
		}


		// Addressing Mode helper methods.

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Zero Page addressing mode - $nn.
		/// </summary>
		/// <returns>The Zero Page address.</returns>
		internal byte GetZeroPageAddress()
		{
			return ReadNextPCByte();
		}

		/// <summary>
		/// Helper method to get the address for an instruction that uses the X-Indexed Zero Page addressing mode - $nn,X.
		/// </summary>
		/// <remarks>
		/// Expect wrap-around when adding X to the zero page address exceeds the bounds of page zero.
		/// </remarks>
		/// <returns>The X-Indexed Zero Page address.</returns>
		internal byte GetZeroPageXAddress() => (byte)(ReadNextPCByte() + CPU.X);

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Y-Indexed Zero Page addressing mode - $nn,Y.
		/// </summary>
		/// <remarks>
		/// Expect wrap-around when adding Y to the zero page address exceeds the bounds of page zero.
		/// </remarks>
		/// <returns>The Y-Indexed Zero Page address.</returns>
		internal byte GetZeroPageYAddress() => (byte)(ReadNextPCByte() + CPU.Y);

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Absolute addressing mode - $nnnn.
		/// </summary>
		/// <returns>The Absolute address.</returns>
		internal ushort GetAbsoluteAddress()
		{
			byte low_byte = ReadNextPCByte();
			byte high_byte = ReadNextPCByte();
			ushort address = (ushort)((high_byte << 8) + low_byte);
			return address;
		}

		/// <summary>
		/// Helper method to get the address for an instruction that uses the X-Indexed Absolute addressing mode - $nnnn,X.
		/// </summary>
		/// <returns>The X-Indexed Absolute address.</returns>
		internal ushort GetAbsoluteXAddress()
		{
			byte low_byte = ReadNextPCByte();
			byte high_byte = ReadNextPCByte();
			ushort address = (ushort)((high_byte << 8) + low_byte + CPU.X);
			return address;
		}

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Y-Indexed Absolute addressing mode - $nnnn,Y.
		/// </summary>
		/// <returns>The Y-Indexed Absolute address.</returns>
		internal ushort GetAbsoluteYAddress()
		{
			byte low_byte = ReadNextPCByte();
			byte high_byte = ReadNextPCByte();
			ushort address = (ushort)((high_byte << 8) + low_byte + CPU.Y);
			return address;
		}

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Indirect addressing mode - ($nnnn).
		/// </summary>
		/// <returns>The Indirect address.</returns>
		internal ushort GetIndirectAddress()
		{
			byte low_byte = ReadNextPCByte();
			byte high_byte = ReadNextPCByte();
			ushort pAddress = (ushort)((high_byte << 8) + low_byte);

			byte low = Memory.Read(pAddress);
			byte high = Memory.Read((ushort)(pAddress + 1));
			ushort address = (ushort)((high << 8) + low);

			return address;
		}

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Indexed Indirect addressing mode - ($nn,X).
		/// </summary>
		/// <returns>The Indexed Indirect address.</returns>
		internal ushort GetIndexedIndirectAddress()
		{
			ushort pAddress = (ushort)((CPU.X + ReadNextPCByte()) & 0xFF);
			byte low = Memory.Read(pAddress);
			byte high = Memory.Read((ushort)((pAddress + 1) & 0xFF));
			ushort address = (ushort)((high << 8) + low);
			return address;
		}

		/// <summary>
		/// Helper method to get the address for an instruction that uses the Indirect Indexed addressing mode - ($nn),Y.
		/// </summary>
		/// <returns>The Indirect Indexed address.</returns>
		internal ushort GetIndirectIndexedAddress()
		{
			ushort offset = ReadNextPCByte();
			byte low = Memory.Read(offset);
			byte high = Memory.Read((ushort)((offset + 1) & 0xFF));
			ushort address = (ushort)((high << 8) + low + CPU.Y);
			return address;
		}
	}
}
