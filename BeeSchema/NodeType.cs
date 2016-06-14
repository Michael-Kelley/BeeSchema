
namespace BeeSchema {
	public enum NodeType {
		None,

		Bool,

		Byte, SByte,
		UShort, Short,
		UInt, Int,
		ULong, Long,
		Float, Double,

		Char, String,

		IPAddress, Epoch,

		Array,
		Pointer,

		Struct,
		Enum,
		Bitfield,

		SchemaDef,
		StructDef,
		EnumDef,
		BitfieldDef,

		IfCond,
		UnlessCond,
		ElseCond,
		ElifCond,

		WhileLoop,
		UntilLoop,
		ForLoop,

		Operation,
		AddOper,
		SubOper,
		MulOper,
		DivOper,

		GreaterComp,
		LessComp,
		EqualComp,
		NEqualComp,
		NotComp,
		GoEComp,
		LoEComp,
		OrCond,
		AndCond,

		EofMacro,
		SizeMacro,
		PosMacro,

		Error = -1
	}
}