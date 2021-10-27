using System;
using System.Collections.Generic;
using System.Text;

namespace PendleCodeMonkey.MOS6502EmulatorLib.Assembler
{
	/// <summary>
	/// Implementation of the <see cref="AssemblerEnumerations"/> class.
	/// </summary>
	public class AssemblerEnumerations
	{

		/// <summary>
		/// Enumeration of possible addressing modes (bit masks used by assembler functionality).
		/// </summary>
		[Flags]
		public enum AMFlag : short
		{
			Acc = 0x0001,
			Abs = 0x0002,               // $nnnn
			AbsX = 0x0004,              // $nnnn,X
			AbsY = 0x0008,              // $nnnn,Y
			Imm = 0x0010,				// #$nn
			Impl = 0x0020,
			Ind = 0x0040,               // ($nnnn)
			IndexXInd = 0x0080,			// ($nn,X)
			IndIndexY = 0x0100,			// ($nn),Y
			Rel = 0x0200,               // $nnnn
			Zero = 0x0400,				// $nn
			ZeroX = 0x0800,             // $nn,X
			ZeroY = 0x1000              // $nn,Y
		};

		/// <summary>
		/// Enumeration of data segment types.
		/// </summary>
		/// <remarks>
		/// These correspond to the DB, DW, and DS assembler directives; Byte for DB, Word for DW, and Space for DS.
		/// </remarks>
		public enum DataSegmentType
		{
			None,
			Byte,
			Word,
			Space
		}

		/// <summary>
		/// Enumeration of errors that can occur during assembler operation.
		/// </summary>
		public enum Errors
		{
			None,
			CannotHaveDuplicateLabelNames,
			OrgAddressOutOfValidRange,
			InvalidOrgAddress,
			InvalidInstruction,
			UnresolvedOperandValue,
			CannotRedefineEquValue,
			CurrentAddressOutOfRange,
			InvalidByteSegmentValue,
			ByteSegmentValueOutOfRange,
			InvalidWordSegmentValue,
			WordSegmentValueOutOfRange,
			SpaceSegmentSizeOutOfRange,
			SpaceSegmentInitializeValueOutOfRange,
			SpaceSegmentInvalidParameter,
			OperandValueOutOfRange,
			DisplacementOutOfRange,
			DivideByZero,
			EQUNameCannotBeReservedWord,
			LabelNameCannotBeReservedWord,
			InvalidInstructionOperand
		}
	}
}
