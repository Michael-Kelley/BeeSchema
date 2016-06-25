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
			var r = CreateActionDM<object, BinaryReader>($"Read{node.Name}");
			var _ = r.GetILGenerator();

			var type = node.Children[0].Type;
			var value = _.Local<long>();

			switch (type) {
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
				case NodeType.Int:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt32");
					_.ConvI8();
					break;
				case NodeType.Long:
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt64");
					break;
			}

			_.StLoc(value);
			var end = _.Label();

			foreach (var c in node.Children) {
				var label = _.Label();

				_.LdLoc(value);
				_.LdCI8((long)c.Value);

				_.CEq();
				_.BrFalse(label);

				_.LdStr(c.Name);
				_.Br(end);

				_.Mark(label);
			}

			_.LdStr("");

			_.Mark(end);

			_.LdLoc(value);
			_.NewObj<Tuple<string, long>, string, long>();

			_.Ret();

			return r;
		}

		void CreateReaderWriter(Node node, out DynamicMethod reader, out DynamicMethod writer) {
			reader = CreateActionDM<Result, BinaryReader>($"Read{node.Name}");
			writer = CreateFuncDM<BinaryReader, Result>($"Write{node.Name}");
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
				ProcessChildNode(c, _, wil, collection);

			_.LdLoc(result);
			_.Ret();
		}

		void ProcessChildNode(Node node, ILGenerator read, ILGenerator write, LocalBuilder collection) {
			var _ = read;

			if (node.Type == NodeType.IfCond) {
				var cond = node.Children[0].Children;
				cond = InfixToPostFix(cond);

				var end = _.Label();

				foreach (var c in cond) {
					switch(c.Type) {
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
							_.CEq();
							_.BrFalse(end);
							break;
					}
				}

				var body = node.Children[1].Children;

				foreach (var n in body)
					ProcessChildNode(n, read, write, collection);

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

			switch (node.Type) {
				case NodeType.Bool:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadBoolean");
					_.Box<bool>();
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
				case NodeType.Int:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt32");
					_.ConvI8();
					_.Box<long>();
					break;
				case NodeType.Long:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadInt64");
					_.Box<long>();
					break;
				case NodeType.Float:
					_.LdLoc(r);
					_.LdArg0();
					_.CallVirt<BinaryReader>("ReadSingle");
					_.Box<float>();
					break;
				case NodeType.Struct:
					_.LdLoc(r);
					_.LdArg0();
					_.Call(customTypeReaders[(string)node.Value]);
					_.LdFld<Result>("Value");
					break;
				case NodeType.Enum:
					_.LdLoc(r);
					_.LdArg0();
					_.Call(enumReaders[(string)node.Value]);
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