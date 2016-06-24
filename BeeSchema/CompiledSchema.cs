using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using static BeeSchema.ReflectionHelpers;

namespace BeeSchema {
	public class CompiledSchema {
		delegate Result Reader(BinaryReader reader);
		delegate void Writer(BinaryWriter writer, Result result);

		Dictionary<string, DynamicMethod> customTypeReaders, enumReaders;
		Dictionary<string, DynamicMethod> customTypeWriters;
		readonly Reader schemaReader;
		readonly Writer schemaWriter;

		internal CompiledSchema(Schema schema) {
			customTypeReaders = new Dictionary<string, DynamicMethod>();
			enumReaders = new Dictionary<string, DynamicMethod>();
			customTypeWriters = new Dictionary<string, DynamicMethod>();

			foreach (var t in schema.Types.Values) {
				if (t.Type == NodeType.EnumDef) {
					var method = CreateEnumReader(t);
					var del = method.CreateDelegate(typeof(Func<BinaryReader, object>));
					var m = del.Method;
					enumReaders[t.Name] = method;
					continue;
				}

				DynamicMethod r, w;
				CreateReaderWriter(t, out r, out w);
				customTypeReaders[t.Name] = r;
				customTypeWriters[t.Name] = w;
			}

			DynamicMethod reader, writer;
			CreateReaderWriter(schema.Root, out reader, out writer);
			schemaReader = (Reader)reader.CreateDelegate(typeof(Reader));
			//schemaWriter = (Writer)writer.CreateDelegate(typeof(Writer));
		}

		DynamicMethod CreateEnumReader(Node node) {
			var r = new DynamicMethod($"Read{node.Name}", typeof(object), new[] { T_BinaryReader });
			var il = r.GetILGenerator();

			Action _ret = () => il.Emit(OpCodes.Ret);
			Func<Type, LocalBuilder> _local = il.DeclareLocal;
			Action<ConstructorInfo> _newobj = (x) => il.Emit(OpCodes.Newobj, x);
			Action<LocalBuilder> _stloc = (x) => il.Emit(OpCodes.Stloc, x);
			Action<LocalBuilder> _ldloc = (x) => il.Emit(OpCodes.Ldloc, x);
			Action _ldarg0 = () => il.Emit(OpCodes.Ldarg_0);
			Action<string> _ldstr = (x) => il.Emit(OpCodes.Ldstr, x);
			Action<int> _ldci4 = (x) => il.Emit(OpCodes.Ldc_I4, x);
			Action<long> _ldci8 = (x) => il.Emit(OpCodes.Ldc_I8, x);
			Action<MethodInfo> _callvirt = (x) => il.Emit(OpCodes.Callvirt, x);
			Action<Type> _box = (x) => il.Emit(OpCodes.Box, x);
			Action _ceq = () => il.Emit(OpCodes.Ceq);
			Action<Label> _br = (x) => il.Emit(OpCodes.Br, x);
			Action<Label> _brfalse = (x) => il.Emit(OpCodes.Brfalse, x);
			Func<Label> _label = il.DefineLabel;
			Action<Label> _mark = il.MarkLabel;
			Action _convi8 = () => il.Emit(OpCodes.Conv_I8);

			var type = node.Children[0].Type;
			LocalBuilder value = null;
			ConstructorInfo tupleCtor = null;

			switch (type) {
				case NodeType.Byte:
					value = _local(typeof(byte));
					tupleCtor = Tuple_StringByte_Ctor;
					_ldarg0();
					_callvirt(BinaryReader_ReadByte);
					break;
				case NodeType.Short:
					value = _local(typeof(short));
					tupleCtor = Tuple_StringShort_Ctor;
					_ldarg0();
					_callvirt(BinaryReader_ReadInt16);
					break;
				case NodeType.Int:
					value = _local(typeof(int));
					tupleCtor = Tuple_StringInt_Ctor;
					_ldarg0();
					_callvirt(BinaryReader_ReadInt32);
					break;
				case NodeType.Long:
					value = _local(typeof(long));
					tupleCtor = Tuple_StringLong_Ctor;
					_ldarg0();
					_callvirt(BinaryReader_ReadInt64);
					break;
			}

			_stloc(value);
			var end = _label();

			foreach (var c in node.Children) {
				var label = _label();

				_ldloc(value);
				_convi8();
				_ldci8((long)c.Value);

				_ceq();
				_brfalse(label);

				_ldstr(c.Name);
				_br(end);

				_mark(label);
			}

			_ldstr("");

			_mark(end);

			_ldloc(value);
			_newobj(tupleCtor);
			_ret();

			return r;
		}

		void CreateReaderWriter(Node node, out DynamicMethod reader, out DynamicMethod writer) {
			reader = new DynamicMethod($"Read{node.Name}", T_Result, new[] { T_BinaryReader });
			writer = new DynamicMethod($"Write{node.Name}", typeof(void), new[] { T_BinaryReader, T_Result });
			var il = reader.GetILGenerator();

			Action _ret = () => il.Emit(OpCodes.Ret);
			Func<Type, LocalBuilder> _local = il.DeclareLocal;
			Action<ConstructorInfo> _newobj = (x) => il.Emit(OpCodes.Newobj, x);
			Action<LocalBuilder> _stloc = (x) => il.Emit(OpCodes.Stloc, x);
			Action<LocalBuilder> _ldloc = (x) => il.Emit(OpCodes.Ldloc, x);
			Action<FieldInfo> _stfld = (x) => il.Emit(OpCodes.Stfld, x);
			Action<FieldInfo> _ldfld = (x) => il.Emit(OpCodes.Ldfld, x);
			Action<string> _ldstr = (x) => il.Emit(OpCodes.Ldstr, x);
			Action<int> _ldci4s = (x) => il.Emit(OpCodes.Ldc_I4_S, x);
			Action _ldarg0 = () => il.Emit(OpCodes.Ldarg_0);
			Action<MethodInfo> _call = (x) => il.Emit(OpCodes.Call, x);
			Action<MethodInfo> _callvirt = (x) => il.Emit(OpCodes.Callvirt, x);
			Action<Type> _box = (x) => il.Emit(OpCodes.Box, x);
			Action _pop = () => il.Emit(OpCodes.Pop);

			var result = _local(T_Result);
			var collection = _local(T_ResultCollection);

			_newobj(Result_Ctor);
			_stloc(result);

			_newobj(ResultCollection_Ctor);
			_stloc(collection);
			_ldloc(result);
			_ldloc(collection);
			_stfld(Result_Value);

			foreach (var c in node.Children) {
				var r = _local(T_Result);
				_newobj(Result_Ctor);
				_stloc(r);

				_ldloc(r);
				_ldci4s((int)c.Type);
				_stfld(Result_Type);

				_ldloc(r);
				_ldstr(c.Name);
				_stfld(Result_Name);

				_ldloc(r);
				_ldarg0();
				_callvirt(BinaryReader_GetBaseStream);
				_callvirt(Stream_GetPosition);
				_stfld(Result_Position);

				if (c.Comment != null) {
					_ldloc(r);
					_ldstr(c.Comment);
					_stfld(Result_Comment);
				}

				switch (c.Type) {
					case NodeType.Byte:
						_ldloc(r);
						_ldarg0();
						_callvirt(BinaryReader_ReadByte);
						_box(typeof(byte));
						break;
					case NodeType.Short:
						_ldloc(r);
						_ldarg0();
						_callvirt(BinaryReader_ReadInt16);
						_box(typeof(short));
						break;
					case NodeType.Int:
						_ldloc(r);
						_ldarg0();
						_callvirt(BinaryReader_ReadInt32);
						_box(typeof(int));
						break;
					case NodeType.Long:
						_ldloc(r);
						_ldarg0();
						_callvirt(BinaryReader_ReadInt64);
						_box(typeof(long));
						break;
					case NodeType.Float:
						_ldloc(r);
						_ldarg0();
						_callvirt(BinaryReader_ReadSingle);
						_box(typeof(float));
						break;
					case NodeType.Struct:
						_ldloc(r);
						_ldarg0();
						_call(customTypeReaders[(string)c.Value]);
						_ldfld(Result_Value);
						break;
					case NodeType.Enum:
						_ldloc(r);
						_ldarg0();
						_call(enumReaders[(string)c.Value]);
						break;
					default:
						_ldloc(r);
						il.Emit(OpCodes.Ldnull);
						break;
				}

				_stfld(Result_Value);

				_ldloc(collection);
				_ldloc(r);
				_callvirt(ResultCollection_Add);
			}

			_ldloc(result);
			_ret();
		}

		public ResultCollection Read(string filename) {
			var s = File.OpenRead(filename);

			return Read(s);
		}

		public ResultCollection Read(byte[] data) {
			var s = new MemoryStream(data);

			return Read(s);
		}

		public ResultCollection Read(Stream stream) {
			var b = new BinaryReader(stream, Encoding.ASCII);
			var r = schemaReader(b);
			b.Close();

			return r.Children;
		}
	}
}