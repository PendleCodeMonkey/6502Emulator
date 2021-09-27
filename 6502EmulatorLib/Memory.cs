using System;

namespace PendleCodeMonkey.MOS6502EmulatorLib
{
	/// <summary>
	/// Implementation of the <see cref="Memory"/> class.
	/// </summary>
	class Memory
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Memory"/> class.
		/// </summary>
		public Memory()
		{
			Data = new byte[0x10000];
		}

		/// <summary>
		/// Gets or sets the byte array that holds the memory contents.
		/// </summary>
		internal byte[] Data { get; set; }

		/// <summary>
		/// Load data into the specified address, optionally clearing all memory before doing so.
		/// </summary>
		/// <param name="data">The data to be loaded.</param>
		/// <param name="loadAddress">The address at which the data sound be loaded.</param>
		/// <param name="clearBeforeLoad"><c>true</c> if all memory should be cleared before loading, otherwise <c>false</c>.</param>
		/// <returns></returns>
		public bool LoadData(byte[] data, ushort loadAddress, bool clearBeforeLoad = true)
		{
			// Check that the data being loaded will actually fit at the specified load address.
			if (loadAddress + data.Length > 0xFFFF)
			{
				return false;
			}

			Span<byte> machineMemorySpan = Data;
			if (clearBeforeLoad)
			{
				machineMemorySpan.Fill(0);
			}
			Span<byte> dataSpan = data;
			Span<byte> loadMemorySpan = machineMemorySpan.Slice(loadAddress, dataSpan.Length);
			dataSpan.CopyTo(loadMemorySpan);
			return true;
		}

		/// <summary>
		/// Clear all of the <see cref="Memory"/> instance's data.
		/// </summary>
		public void Clear() => Data.AsSpan().Fill(0);

		/// <summary>
		/// Return the specified block of memory.
		/// </summary>
		/// <param name="address">Start address of the requested block of memory.</param>
		/// <param name="length">Length (in bytes) of the block of memory to be retrieved.</param>
		/// <returns>Read-only copy of the requested memory.</returns>
		public ReadOnlySpan<byte> DumpMemory(ushort address, ushort length)
		{
			ReadOnlySpan<byte> dumpMem = Data.AsSpan().Slice(address, length);
			return dumpMem;
		}

		/// <summary>
		/// Read the value at the specified address.
		/// </summary>
		/// <param name="address">The address of the memory to be read.</param>
		/// <returns>The value that was read from the specified address.</returns>
		public byte Read(ushort address) => Data[address];

		/// <summary>
		/// Write a value to the specified address.
		/// </summary>
		/// <param name="address">The address at which the value should be written.</param>
		/// <param name="value">The value to be written to the specified address.</param>
		public void Write(ushort address, byte value) => Data[address] = value;

		/// <summary>
		/// Read a value from a location in the specified page of memory.
		/// </summary>
		/// <param name="page">The page number.</param>
		/// <param name="offset">The offset into the specified page of the value to be read.</param>
		/// <returns>The value that was read from the specified location.</returns>
		public byte ReadFromPage(byte page, byte offset) => Read((ushort)(PageOffset(page) + offset));

		/// <summary>
		/// Write a value to a location in the specified page of memory.
		/// </summary>
		/// <param name="page">The page number.</param>
		/// <param name="offset">The offset into the specified page to which the value is to be written.</param>
		/// <param name="value">The value to be written to the specified location.</param>
		public void WriteOnPage(byte page, byte offset, byte value) => Write((ushort)(PageOffset(page) + offset), value);

		/// <summary>
		/// Calculate the start address of the specified page in memory.
		/// </summary>
		/// <param name="page">The page number.</param>
		/// <returns>The start address of the specified page in memory.</returns>
		internal ushort PageOffset(byte page) => (ushort)(page * 0x0100);
	}
}
