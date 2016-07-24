using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using static BeeSchema.ReflectionHelpers;

namespace BeeSchema {
	public class CompiledSchema {
		delegate Result Reader(BinaryReader reader);
		delegate void Writer(BinaryWriter writer, Result result);

		readonly Dictionary<string, DynamicMethod>
			customTypeReaders,
			customTypeWriters;

		readonly Reader schemaReader;
		readonly Writer schemaWriter;

		internal CompiledSchema(Schema schema) {
			customTypeReaders = new Dictionary<string, DynamicMethod>();
			customTypeWriters = new Dictionary<string, DynamicMethod>();

			foreach (var t in schema.Types.Values) {
				DynamicMethod r, w;

				if (t.Type == NodeType.EnumDef)
					CreateEnumReaderWriter(t, out r, out w);
				else if (t.Type == NodeType.BitfieldDef)
					CreateBitfieldReaderWriter(t, out r, out w);
				else
					CreateReaderWriter(t, out r, out w);

				customTypeReaders[t.Name] = r;
				customTypeWriters[t.Name] = w;
			}

			DynamicMethod reader, writer;
			CreateReaderWriter(schema.Root, out reader, out writer);
			schemaReader = (Reader)reader.CreateDelegate(typeof(Reader));
			//schemaWriter = (Writer)writer.CreateDelegate(typeof(Writer));
		}

		void CreateEnumReaderWriter(Node node, out DynamicMethod reader, out DynamicMethod writer) {
			reader = CreateActionDM<object, BinaryReader>($"Read{node.Name}");
			writer = CreateFuncDM<BinaryWriter, Result>($"Write{node.Name}");
			var _ = reader.GetILGenerator();
			var __ = writer.GetILGenerator();

			var type = node.Children[0].Type;
			var value = _.Local<long>();

			_.LdArg0();

			__.LdArg0();
			__.LdArg1();
			__.LdFld<Result>("Value");
			__.CastClass<Tuple<string, long>>();
			__.CallVirt<Tuple<string, long>>("get_Item2");

			switch (type) {
				case NodeType.SByte:
					_.CallVirt<BinaryReader>("ReadSByte");

					__.ConvI1();
					__.CallVirt<BinaryWriter, sbyte>("Write");
					break;
				case NodeType.Byte:
					_.CallVirt<BinaryReader>("ReadByte");

					__.ConvU1();
					__.CallVirt<BinaryWriter, byte>("Write");
					break;
				case NodeType.Short:
					_.CallVirt<BinaryReader>("ReadInt16");

					__.ConvI2();
					__.CallVirt<BinaryWriter, short>("Write");
					break;
				case NodeType.UShort:
					_.CallVirt<BinaryReader>("ReadUInt16");

					__.CallVirt<BinaryWriter, ushort>("Write");
					break;
				case NodeType.Int:
					_.CallVirt<BinaryReader>("ReadInt32");

					__.CallVirt<BinaryWriter, int>("Write");
					break;
				case NodeType.UInt:
					_.CallVirt<BinaryReader>("ReadUInt32");

					__.CallVirt<BinaryWriter, uint>("Write");
					break;
				case NodeType.Long:
					_.CallVirt<BinaryReader>("ReadInt64");

					__.CallVirt<BinaryWriter, long>("Write");
					break;
				case NodeType.ULong:
					_.CallVirt<BinaryReader>("ReadUInt64");

					__.CallVirt<BinaryWriter, ulong>("Write");
					break;
			}

			_.ConvI8();
			_.StLoc(value);
			var end = _.Label();

			foreach (var c in node.Children) {
				var label = _.Label();

				_.LdLoc(value);
				_.LdCI8((long)c.Value);

				_.BNEUn(label);

				_.LdStr(c.Name);
				_.Br(end);

				_.Mark(label);
			}

			_.LdStr("");

			_.Mark(end);

			_.LdLoc(value);
			_.NewObj<Tuple<string, long>, string, long>();

			_.Ret();

			__.Ret();
		}

		void CreateBitfieldReaderWriter(Node node, out DynamicMethod reader, out DynamicMethod writer) {
			reader = CreateActionDM<ResultCollection, BinaryReader>($"Read{node.Name}");
			writer = CreateFuncDM<BinaryWriter, Result>($"Write{node.Name}");
			var _ = reader.GetILGenerator();
			var __ = writer.GetILGenerator();

			// ## READ ##
			var type = node.Children[0].Type;
			var value = _.Local<long>();

			switch (type) {
				case NodeType.SByte:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadSByte");
					_.ConvI8();
					break;
				case NodeType.Byte:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadByte");
					_.ConvI8();
					break;
				case NodeType.Short:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt16");
					_.ConvI8();
					break;
				case NodeType.UShort:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt16");
					_.ConvI8();
					break;
				case NodeType.Int:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt32");
					_.ConvI8();
					break;
				case NodeType.UInt:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt32");
					_.ConvI8();
					break;
				case NodeType.Long:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt64");
					break;
				case NodeType.ULong:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt64");
					_.ConvI8();
					break;
			}

			_.StLoc(value);

			var col = _.Local<ResultCollection>();
			_.NewObj<ResultCollection>();
			_.StLoc(col);

			var pos = _.Local<long>();
			_.LdArg0();
			_.CallVirt<BinaryReader>("get_BaseStream");
			_.CallVirt<Stream>("get_Position");
			_.StLoc(pos);


			// ## WRITE ##
			__.LdArg0();
			var shift = 0;

			foreach (var c in node.Children) {
				// ## READ ##
				var re = _.Local<Result>();
				_.NewObj<Result>();
				_.StLoc(re);

				_.LdLoc(re);
				_.LdCI4((int)c.Type);
				_.StFld<Result>("Type");

				_.LdLoc(re);
				_.LdStr(c.Name);

				_.StFld<Result>("Name");

				_.LdLoc(re);
				_.LdLoc(pos);
				_.StFld<Result>("Position");

				if (c.Comment != null) {
					_.LdLoc(re);
					_.LdStr(c.Comment);
					_.StFld<Result>("Comment");
				}

				_.LdLoc(re);
				_.LdStr(c.Type.ToString().ToLower());
				_.StFld<Result>("TypeName");

				_.LdLoc(re);
				_.LdLoc(value);
				_.ConvI8();
				_.LdCI8((1 << (int)(long)c.Value) - 1);
				_.And();
				_.Box<long>();
				_.StFld<Result>("Value");

				_.LdLoc(value);
				_.LdCI4((int)(long)c.Value);
				_.ShR();
				_.StLoc(value);

				_.LdLoc(col);
				_.LdLoc(re);
				_.CallVirt<ResultCollection>("Add");


				// ## WRITE ##
				__.LdArg1();
				__.LdStr(c.Name);
				__.CallVirt<Result, string>("get_Item");
				__.LdFld<Result>("Value");
				__.UnboxAny<long>();

				if (shift != 0) {
					__.LdCI4(shift);
					__.ShL();
					__.Add();
				}

				shift += (int)(long)c.Value;
			}

			// ## WRITE ##
			switch (type) {
				case NodeType.SByte:
					__.ConvI1();
					__.CallVirt<BinaryWriter, sbyte>("Write");
					break;
				case NodeType.Byte:
					__.ConvU1();
					__.CallVirt<BinaryWriter, byte>("Write");
					break;
				case NodeType.Short:
					__.ConvI2();
					__.CallVirt<BinaryWriter, short>("Write");
					break;
				case NodeType.UShort:
					__.ConvU2();
					__.CallVirt<BinaryWriter, ushort>("Write");
					break;
				case NodeType.Int:
					__.ConvI4();
					__.CallVirt<BinaryWriter, int>("Write");
					break;
				case NodeType.UInt:
					__.ConvU4();
					__.CallVirt<BinaryWriter, uint>("Write");
					break;
				case NodeType.Long:
					__.CallVirt<BinaryWriter, long>("Write");
					break;
				case NodeType.ULong:
					__.ConvU8();
					__.CallVirt<BinaryWriter, ulong>("Write");
					break;
			}

			__.Ret();


			// ## READ ##
			_.LdLoc(col);
			_.Ret();
		}

		void CreateReaderWriter(Node node, out DynamicMethod reader, out DynamicMethod writer) {
			reader = CreateActionDM<Result, BinaryReader>($"Read{node.Name}");
			writer = CreateFuncDM<BinaryWriter, Result>($"Write{node.Name}");
			var ril = reader.GetILGenerator();
			var wil = writer.GetILGenerator();

			var _ = ril;

			var result = _.Local<Result>();
			var collection = _.Local<ResultCollection>();

			_.NewObj<Result>();
			_.StLoc(result);

			_.NewObj<ResultCollection>();
			_.StLoc(collection);
			_.LdLoc(result);
			_.LdLoc(collection);
			_.StFld<Result>("Value");

			foreach (var c in node.Children)
				ProcessChildNode(c, ril, wil, collection);

			_.LdLoc(result);
			_.Ret();
		}

		void ProcessChildNode(Node node, ILGenerator read, ILGenerator write, LocalBuilder collection, LocalBuilder index = null) {
			var _ = read;

			if (node.Type == NodeType.IfCond
				|| node.Type == NodeType.UnlessCond
				|| node.Type == NodeType.WhileLoop
				|| node.Type == NodeType.UntilLoop) {
				var cond = node.Children[0].Children;
				cond = InfixToPostFix(cond);

				var end = _.Label();
				var condition = _.Label();
				LocalBuilder ind = null;

				if (node.Type == NodeType.WhileLoop
					|| node.Type == NodeType.UntilLoop) {
					ind = _.Local<int>();
					_.Mark(condition);
				}

				foreach (var c in cond) {
					switch (c.Type) {
						case NodeType.Long:
							_.LdCI8((long)c.Value);
							break;
						case NodeType.String:
							_.LdLoc(collection);
							_.LdStr((string)c.Value);
							_.CallVirt<ResultCollection, string>("get_Item");
							_.LdFld<Result>("Value");
							_.UnboxAny<long>();
							break;
						case NodeType.MulOper:
							_.Mul();
							break;
						case NodeType.DivOper:
							_.Div();
							break;
						case NodeType.AddOper:
							_.Add();
							break;
						case NodeType.SubOper:
							_.Sub();
							break;
						case NodeType.EqualComp:
							if (node.Type == NodeType.IfCond
								|| node.Type == NodeType.WhileLoop)
								_.BNEUn(end);
							else
								_.BEq(end);
							break;
						case NodeType.NEqualComp:
							if (node.Type == NodeType.IfCond
								|| node.Type == NodeType.WhileLoop)
								_.BEq(end);
							else
								_.BNEUn(end);
							break;
						case NodeType.GreaterComp:
							if (node.Type == NodeType.IfCond)
								_.BLE(end);
							else
								_.BGT(end);
							break;
						case NodeType.LessComp:
							if (node.Type == NodeType.IfCond
								|| node.Type == NodeType.WhileLoop)
								_.BGE(end);
							else
								_.BLT(end);
							break;
						case NodeType.GoEComp:
							if (node.Type == NodeType.IfCond
								|| node.Type == NodeType.WhileLoop)
								_.BLT(end);
							else
								_.BGE(end);
							break;
						case NodeType.LoEComp:
							if (node.Type == NodeType.IfCond
								|| node.Type == NodeType.WhileLoop)
								_.BGT(end);
							else
								_.BLE(end);
							break;
						case NodeType.EofMacro:
							_.LdArg0();
							_.CallVirt<BinaryReader>("get_BaseStream");
							_.CallVirt<Stream>("get_Position");
							_.LdArg0();
							_.CallVirt<BinaryReader>("get_BaseStream");
							_.CallVirt<Stream>("get_Length");

							if (node.Type == NodeType.IfCond
								|| node.Type == NodeType.WhileLoop)
								_.BLT(end);
							else
								_.BGE(end);
							break;
						case NodeType.PosMacro:
							_.LdArg0();
							_.CallVirt<BinaryReader>("get_BaseStream");
							_.CallVirt<Stream>("get_Position");
							break;
						case NodeType.SizeMacro:
							_.LdArg0();
							_.CallVirt<BinaryReader>("get_BaseStream");
							_.CallVirt<Stream>("get_Length");
							break;
					}
				}

				var body = node.Children[1].Children;

				foreach (var n in body) {
					ProcessChildNode(n, read, write, collection, ind);

					if (ind != null) {
						_.LdLoc(ind);
						_.LdCI4_1();
						_.Add();
						_.StLoc(ind);
					}
				}

				if (node.Type == NodeType.WhileLoop
					|| node.Type == NodeType.UntilLoop)
					_.Br(condition);

				_.Mark(end);

				return;
			}

			var r = _.Local<Result>();
			_.NewObj<Result>();
			_.StLoc(r);

			_.LdLoc(r);
			_.LdCI4((int)node.Type);
			_.StFld<Result>("Type");

			_.LdLoc(r);

			if (index != null) {
				_.LdStr("{0}[{1}]");
				_.LdStr(node.Name);
				_.LdLoc(index);
				_.Box<int>();
				_.Call<string, string, object, object>("Format");
			}
			else
				_.LdStr(node.Name);

			_.StFld<Result>("Name");

			_.LdLoc(r);
			_.LdArg0();
			_.CallVirt<BinaryReader>("get_BaseStream");
			_.CallVirt<Stream>("get_Position");
			_.StFld<Result>("Position");

			if (node.Comment != null) {
				_.LdLoc(r);
				_.LdStr(node.Comment);
				_.StFld<Result>("Comment");
			}

			_.LdLoc(r);

			if (node.Type == NodeType.Struct
				|| node.Type == NodeType.Enum
				|| node.Type == NodeType.Bitfield)
				_.LdStr((string)node.Value);
			else if (node.Type == NodeType.Array) {
				var tnode = node.Children[0];

				if (tnode.Type == NodeType.Struct
					|| tnode.Type == NodeType.Enum
					|| tnode.Type == NodeType.Bitfield)
					_.LdStr($"{tnode.Value}[]");
				else
					_.LdStr($"{tnode.Type.ToString().ToLower()}[]");
			}
			else
				_.LdStr(node.Type.ToString().ToLower());

			_.StFld<Result>("TypeName");

			switch (node.Type) {
				case NodeType.Bool:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadBoolean");
					_.Box<bool>();
					break;
				case NodeType.SByte:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadSByte");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.Byte:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadByte");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.Short:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt16");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.UShort:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt16");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.Int:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt32");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.UInt:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt32");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.Long:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt64");
					_.Box<long>();
					break;
				case NodeType.ULong:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt64");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.Float:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadSingle");
					_.Box<float>();
					break;
				case NodeType.Double:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadDouble");
					_.Box<double>();
					break;
				case NodeType.Char:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadChar");
					_.Box<char>();
					break;
				case NodeType.String:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadString");
					break;
				case NodeType.IPAddress:
					_.LdLoc(r);
					_.LdArg0();
					_.LdCI4_4();
					_.CallVirt<BinaryReader>("ReadBytes");
					_.NewObj<IPAddress, byte[]>();
					break;
				case NodeType.Epoch:
					_.LdLoc(r);
					_.NewObj<DateTime, int, int, int, int, int, int, int, DateTimeKind>();
					_.Dup();
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadUInt32");
					_.Call<DateTime, double>("AddSeconds");
					break;
				case NodeType.Struct:
					_.LdLoc(r);
					_.LdArg0();
					_.Call(customTypeReaders[(string)node.Value]);
					_.LdFld<Result>("Value");
					break;
				case NodeType.Bitfield:
					_.LdLoc(r);
					_.LdArg0();
					_.Call(customTypeReaders[(string)node.Value]);
					break;
				case NodeType.Enum:
					_.LdLoc(r);
					_.LdArg0();
					_.Call(customTypeReaders[(string)node.Value]);
					break;
				case NodeType.Array:
					var tnode = node.Children[0];
					var lnode = node.Children[1];

					var als = lnode.Children;
					als = InfixToPostFix(als);

					var len = _.Local<long>();

					foreach (var c in als) {
						switch (c.Type) {
							case NodeType.Long:
								_.LdCI8((long)c.Value);
								break;
							case NodeType.String:
								_.LdLoc(collection);
								_.LdStr((string)c.Value);
								_.CallVirt<ResultCollection, string>("get_Item");
								_.LdFld<Result>("Value");
								_.UnboxAny<long>();
								break;
							case NodeType.MulOper:
								_.Mul();
								break;
							case NodeType.DivOper:
								_.Div();
								break;
							case NodeType.AddOper:
								_.Add();
								break;
							case NodeType.SubOper:
								_.Sub();
								break;
							case NodeType.PosMacro:
								_.LdArg0();
								_.CallVirt<BinaryReader>("get_BaseStream");
								_.CallVirt<Stream>("get_Position");
								break;
							case NodeType.SizeMacro:
								_.LdArg0();
								_.CallVirt<BinaryReader>("get_BaseStream");
								_.CallVirt<Stream>("get_Length");
								break;
						}
					}

					_.StLoc(len);

					if (tnode.Type == NodeType.Char) {
						_.LdLoc(r);
						_.LdArg0();
						_.LdLoc(len);
						_.ConvI4();
						_.CallVirt<BinaryReader, int>("ReadChars");
						_.NewObj<string, char[]>();
						break;
					}

					var ind = _.Local<int>();
					var col = _.Local<ResultCollection>();

					_.NewObj<ResultCollection>();
					_.StLoc(col);

					var loop = _.Label();
					var end = _.Label();

					_.Mark(loop);
					_.LdLoc(ind);
					_.ConvI8();
					_.LdLoc(len);
					_.BEq(end);

					var cnode = new Node {
						Type = tnode.Type,
						Name = node.Name
					};

					ProcessChildNode(cnode, read, write, col, ind);

					_.LdLoc(ind);
					_.LdCI4_1();
					_.Add();
					_.StLoc(ind);
					_.Br(loop);

					_.Mark(end);
					_.LdLoc(r);
					_.LdLoc(col);
					break;
				default:
					_.LdLoc(r);
					_.LdNull();
					break;
			}

			_.StFld<Result>("Value");

			_.LdLoc(collection);
			_.LdLoc(r);
			_.CallVirt<ResultCollection>("Add");
		}

		List<Node> InfixToPostFix(List<Node> nodes) {
			var r = new List<Node>();
			var stack = new Stack<Node>();

			foreach (var n in nodes) {
				if (n.Type > NodeType.Operation && n.Type < NodeType.OrCond) {
					while (stack.Count > 0 && stack.Peek().Type != NodeType.OpenParens) {
						if (Precedence(stack.Peek().Type) >= Precedence(n.Type))
							r.Add(stack.Pop());
						else
							break;
					}

					stack.Push(n);
				}
				else if (n.Type == NodeType.OpenParens)
					stack.Push(n);
				else if (n.Type == NodeType.CloseParens) {
					while (stack.Count > 0 && stack.Peek().Type != NodeType.OpenParens)
						r.Add(stack.Pop());

					if (stack.Count > 0)
						stack.Pop();
				}
				else
					r.Add(n);
			}

			while (stack.Count > 0)
				r.Add(stack.Pop());

			return r;
		}

		int Precedence(NodeType n) {
			int result = 0;
			switch (n) {
				case NodeType.MulOper:
				case NodeType.DivOper:
					result = 10;
					break;
				case NodeType.AddOper:
				case NodeType.SubOper:
					result = 9;
					break;
				case NodeType.NotComp:
					result = 8;
					break;
				case NodeType.EqualComp:
				case NodeType.GoEComp:
				case NodeType.GreaterComp:
				case NodeType.LessComp:
				case NodeType.LoEComp:
				case NodeType.NEqualComp:
					result = 7;
					break;
				default:
					result = 0;
					break;
			}
			return result;
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