using System;
using System.IO;
using System.Reflection;

namespace BeeSchema {
	static class ReflectionHelpers {
		public static readonly Type
			T_Result = typeof(Result),
			T_ResultCollection = typeof(ResultCollection),
			T_BinaryReader = typeof(BinaryReader),
			T_Stream = typeof(Stream),
			T_Tuple_StringByte = typeof(Tuple<string, byte>),
			T_Tuple_StringShort = typeof(Tuple<string, short>),
			T_Tuple_StringInt = typeof(Tuple<string, int>),
			T_Tuple_StringLong = typeof(Tuple<string, long>);

		public static readonly ConstructorInfo
			// Result
			Result_Ctor = T_Result.GetConstructor(Type.EmptyTypes),
			// ResultCollection
			ResultCollection_Ctor = T_ResultCollection.GetConstructor(Type.EmptyTypes),
			// Tuple
			Tuple_StringByte_Ctor = T_Tuple_StringByte.GetConstructor(new[] { typeof(string), typeof(byte) }),
			Tuple_StringShort_Ctor = T_Tuple_StringShort.GetConstructor(new[] { typeof(string), typeof(short) }),
			Tuple_StringInt_Ctor = T_Tuple_StringInt.GetConstructor(new[] { typeof(string), typeof(int) }),
			Tuple_StringLong_Ctor = T_Tuple_StringLong.GetConstructor(new[] { typeof(string), typeof(long) });

		public static readonly FieldInfo
			// Result
			Result_Type = T_Result.GetField("Type"),
			Result_TypeName = T_Result.GetField("TypeName"),
			Result_Name = T_Result.GetField("Name"),
			Result_Position = T_Result.GetField("Position"),
			Result_Size = T_Result.GetField("Size"),
			Result_Value = T_Result.GetField("Value"),
			Result_Comment = T_Result.GetField("Comment");

		public static readonly MethodInfo
			// ResultCollection
			ResultCollection_Add = T_ResultCollection.GetMethod("Add"),
			// BinaryReader
			BinaryReader_ReadBoolean = T_BinaryReader.GetMethod("ReadBoolean"),
			BinaryReader_ReadByte = T_BinaryReader.GetMethod("ReadByte"),
			BinaryReader_ReadSByte = T_BinaryReader.GetMethod("ReadSByte"),
			BinaryReader_ReadChar = T_BinaryReader.GetMethod("ReadChar"),
			BinaryReader_ReadInt16 = T_BinaryReader.GetMethod("ReadInt16"),
			BinaryReader_ReadUInt16 = T_BinaryReader.GetMethod("ReadUInt16"),
			BinaryReader_ReadInt32 = T_BinaryReader.GetMethod("ReadInt32"),
			BinaryReader_ReadUInt32 = T_BinaryReader.GetMethod("ReadUInt32"),
			BinaryReader_ReadInt64 = T_BinaryReader.GetMethod("ReadInt64"),
			BinaryReader_ReadUInt64 = T_BinaryReader.GetMethod("ReadUInt64"),
			BinaryReader_ReadSingle = T_BinaryReader.GetMethod("ReadSingle"),
			BinaryReader_ReadDouble = T_BinaryReader.GetMethod("ReadDouble"),
			BinaryReader_ReadString = T_BinaryReader.GetMethod("ReadString"),
			BinaryReader_ReadChars = T_BinaryReader.GetMethod("ReadChars"),
			BinaryReader_ReadBytes = T_BinaryReader.GetMethod("ReadBytes"),
			BinaryReader_GetBaseStream = T_BinaryReader.GetMethod("get_BaseStream"),
			// Stream
			Stream_GetPosition = T_Stream.GetMethod("get_Position");
	}
}