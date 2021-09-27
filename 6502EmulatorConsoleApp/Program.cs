using PendleCodeMonkey.MOS6502EmulatorLib;
using System;

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
	}
}
