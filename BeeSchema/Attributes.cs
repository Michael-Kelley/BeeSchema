using System;

namespace BeeSchema {
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ArrayLengthAttribute : Attribute {
		public Type SpecifierType;
		public int Length;

		public ArrayLengthAttribute(int length) {
			Length = length;
		}

		public ArrayLengthAttribute(Type type) {
			SpecifierType = type;
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ConditionalAttribute : Attribute {
		public string Condition;

		public ConditionalAttribute(string cond) {
			Condition = cond;
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class LoopAttribute : Attribute {
		public string Condition;

		public LoopAttribute(string cond) {
			Condition = cond;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class BitfieldAttribute : Attribute {
		public Type BaseType;

		public BitfieldAttribute(Type basetype) {
			BaseType = basetype;
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SizeAttribute : Attribute {
		public int Size;

		public SizeAttribute(int size) {
			Size = size;
		}
	}
}