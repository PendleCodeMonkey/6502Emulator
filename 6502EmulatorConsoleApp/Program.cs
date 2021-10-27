using PendleCodeMonkey.MOS6502EmulatorLib;
using PendleCodeMonkey.MOS6502EmulatorLib.Assembler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PendleCodeMonkey.MOS6502EmulatorConsoleApp
{
	class Program
	{
		static void Main(string[] _)
		{
			FibonacciCalculator();

			Console.WriteLine();
			Console.WriteLine();

			DayOfWeekCalculator();

			Console.WriteLine();
			Console.WriteLine();

			Console.WriteLine("Disassembler test:");
			Console.WriteLine("------------------");
			TestDisassembler();

			AssemblerTestHardCodedSource();
		}

		public static void FibonacciCalculator()
		{
			// Byte array containing the binary code for a 'Fibonacci sequence' calculator in 6502 Assembly Language.
			// Supplies the value 0x0C (in the 7th byte) to get the 13th number in the sequence (Note: values are zero-based so, even though 0x0C is 12 in
			// decimal, this gives the 13th value).  Note that this only works with 8 bit values; therefore, it can only calculate the
			// first 14 values in the sequence (the 14th being 233) because values beyond that do not fit within 8 bits.
			byte[] fibonacci = new byte[] { 0xA2, 0x01, 0x86, 0x00, 0x38, 0xA0, 0x0C, 0x98, 0xE9, 0x03, 0xA8, 0x18, 0xA9, 0x02, 0x85, 0x01,
									0xA6, 0x01, 0x65, 0x00, 0x85, 0x01, 0x86, 0x00, 0x88, 0xD0, 0xF5};

			Machine machine = new Machine();
			machine.LoadExecutableData(fibonacci, 512);
			machine.Execute();
			Console.WriteLine("Fibonacci calculator.  13th number in fibonacci sequence (which is 144) is in A register:");
			string dump = machine.Dump();
			Console.WriteLine(dump);
		}

		public static void DayOfWeekCalculator()
		{
			// Byte array containing the binary code for a 'Day of the week' calculator in 6502 Assembly Language.
			byte[] dayOfWeek = new byte[] { 0xE0, 0x03, 0xB0, 0x01, 0x88, 0x49, 0x7F, 0xC0, 0xC8, 0x7D, 0x20, 0x00, 0x85, 0x86, 0x98, 0x20, 0x1C, 0x00,
									0xE5, 0x86, 0x85, 0x86, 0x98, 0x4A, 0x4A, 0x18, 0x65, 0x86, 0x69, 0x07, 0x90, 0xFC, 0x60,
									0x01, 0x05, 0x06, 0x03, 0x01, 0x05, 0x03, 0x00, 0x04, 0x02, 0x06, 0x04};
			string[] days = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

			Machine machine = new Machine();
			machine.LoadExecutableData(dayOfWeek, 0);
			DateTime today = DateTime.Today;
			machine.SetState(A: (byte)today.Day, X: (byte)today.Month, Y: (byte)(today.Year - 1900), flags: 0);
			machine.Execute();
			var (A, _, _, _, _, _) = machine.GetState();
			Console.WriteLine("Day of week calculator. Result is in A register:    **** Today is " + days[A] + " ****");
			Console.WriteLine("[ 0 - Sunday, 1 - Monday, 2 - Tuesday, 3 - Wednesday, 4 - Thursday, 5 - Friday, 6 - Saturday ]");
			string dump = machine.Dump();
			Console.Write(dump);
		}

		public static void TestDisassembler()
		{
			// Binary code consisting of 33 bytes of executable code, followed by 12 bytes of non-executable data, then a further
			// 27 bytes of executable code.
			byte[] code = new byte[] { 0xE0, 0x03, 0xB0, 0x01, 0x88, 0x49, 0x7F, 0xC0, 0xC8, 0x7D, 0x20, 0x00, 0x85, 0x86, 0x98, 0x20, 0x1C, 0x00,
									0xE5, 0x86, 0x85, 0x86, 0x98, 0x4A, 0x4A, 0x18, 0x65, 0x86, 0x69, 0x07, 0x90, 0xFC, 0x60,
									0x01, 0x05, 0x06, 0x03, 0x01, 0x05, 0x03, 0x00, 0x04, 0x02, 0x06, 0x04,
									0xA2, 0x01, 0x86, 0x00, 0x38, 0xA0, 0x0C, 0x98, 0xE9, 0x03, 0xA8, 0x18, 0xA9, 0x02, 0x85, 0x01,
									0xA6, 0x01, 0x65, 0x00, 0x85, 0x01, 0x86, 0x00, 0x88, 0xD0, 0xF5};

			Machine machine = new Machine();
			machine.LoadExecutableData(code, 0);
			Disassembler disassembler = new Disassembler(machine, startAddress: 0, length: (ushort)code.Length);
			disassembler.AddNonExecutableSection(33, 12);
			var dis = disassembler.Disassemble();

			foreach (var (Address, Disassembly) in dis)
			{
				Console.WriteLine($"0x{Address:X4} - {Disassembly}");
			}
		}





		static void AssemblerTestHardCodedSource()
		{
			Console.WriteLine();
			Console.WriteLine("Testing Assembler using hard-coded source code:");
			Console.WriteLine("-----------------------------------------------");
			Console.WriteLine();

			var code = Get6502SourceCode();

			if (code != null && code.Count > 0)
			{
				Console.WriteLine("ASSEMBLING CODE.  Please wait...");
				Console.WriteLine();

				Assembler asm = new Assembler();
				var (success, binData) = asm.Assemble(code);

				if (!success)
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("FAILED TO ASSEMBLE CODE:");
					Console.ResetColor();
					Console.WriteLine();
					Console.WriteLine();
					foreach (var (LineNumber, Error, AdditionalInfo) in asm.AsmErrors)
					{
						Console.WriteLine($"{Error} [ {AdditionalInfo} ] on line {LineNumber}");
					}
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.DarkGreen;
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write("CODE WAS SUCCESSFULLY ASSEMBLED.");
					Console.ResetColor();
					Console.WriteLine();
					Console.WriteLine();

					// Write the generated binary data to a file.
					byte[] binDataArray = binData.ToArray();
					string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
					path = Path.Combine(path, "AsmGenerated.bin");
					File.WriteAllBytes(path, binDataArray);

					// Fire up an instance of the emulator and get it to execute the generated binary code and produce a disassembly of it.
					Machine machine = new Machine();
					machine.LoadExecutableData(binData.ToArray(), 0x1000);

					DateTime today = DateTime.Today;
					machine.SetState(A: (byte)today.Day, X: (byte)today.Month, Y: (byte)(today.Year - 1900), flags: 0);

					// Execute the assembled binary data.
					machine.Execute();

					string[] days = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
					var (A, _, _, _, _, _) = machine.GetState();
					Console.WriteLine($"Assembled day of week calculator. Result is in A register: A = {A}    **** Today is {days[A]} ****");
					Console.WriteLine("[ 0 - Sunday, 1 - Monday, 2 - Tuesday, 3 - Wednesday, 4 - Thursday, 5 - Friday, 6 - Saturday ]");

					// Disassemble the generated binary data.
					Disassembler disassembler = new Disassembler(machine, startAddress: 0x1000, length: (ushort)binData.Count);

					// Mark the data segments that were created during assembly as non-executable blocks.
					foreach (var (startAddress, size) in asm.DataSegments)
					{
						disassembler.AddNonExecutableSection(startAddress, size);
					}

					var dis = disassembler.Disassemble();

					// Write disassembly output to a file.
					string path2 = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
					path2 = Path.Combine(path2, "Disassembled.asm");
					try
					{
						using StreamWriter writer = new StreamWriter(path2);
						foreach (var (Address, Disassembly) in dis)
						{
							writer.WriteLine($"{Disassembly}\t\t; [{Address:X4}]");
						}
					}
					catch (Exception)
					{
						Console.WriteLine("Error occurred attempting to write file: Disassembled.asm");
					}
				}
			}
		}

		static List<string> Get6502SourceCode()
		{
			List<string> source = new List<string>
			{
				"ORG $1000",
				"TEMP      EQU $FF",
				"CPX #3",
				"BCS MARCHORLATER",
				"DEY",
				"MARCHORLATER:   EOR #$7F",
				"CPY #200",
				"ADC MONTHTABLE-1,X",
				"STA TEMP",
				"TYA",
				"JSR MODULO7",
				"SBC TEMP",
				"STA TEMP",
				"TYA",
				"LSR",
				"LSR",
				"CLC",
				"ADC TEMP",
				"MODULO7:    ADC #7",
				"BCC MODULO7",
				"RTS",
				"MONTHTABLE:    DB 1, 5, 6, 3, 1, 5, 3, 0, 4, 2, 6, 4"
			};

			return source;
		}
	}
}
