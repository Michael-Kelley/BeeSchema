using System.Collections.Generic;
using System.IO;

using BeeSchema;

namespace Example {
	[Bitfield(typeof(int))]
	class SomeBitfield {
		[Size(3)]
		public int First3Bits;
		[Size(1)]
		public int NextBit;
		[Size(4)]
		public int Last4Bits;
	}

	public enum SomeEnum : byte {
		Zero,
		One,
		Five = 5,
		Eight = 8,
		Nine,
		EleventyOne = 111
	}

	class SomeStruct {
		[ArrayLength(4)]
		public byte[] Bytes;
	}

	class Example {
		public byte AByte;
		public short AShort;
		public int AnInt;
		public long ALong;
		public float AFloat, AnotherFloat;
		public SomeStruct AStruct;
		[ArrayLength(typeof(byte))]
		public char[] ACharArray;
		public SomeEnum AnEnum;
		public SomeBitfield ABitfield;
		[Conditional("ABool_Cond")]
		public bool ABool;
		[Loop("AnotherBool_Loop")]
		public List<bool> AnotherBool;

		bool ABool_Cond(Stream stream)
			=> this.AShort + 5 != 14 / 2;

		bool AnotherBool_Loop(Stream stream)
			=> stream.Position < stream.Length;
	}
}