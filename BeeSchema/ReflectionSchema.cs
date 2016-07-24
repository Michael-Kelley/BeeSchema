using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;

using static BeeSchema.ReflectionHelpers;
using System.Reflection;

namespace BeeSchema {
	public class ReflectionSchema<T> where T : class {
		delegate T ReadDelegate(BinaryReader stream);
		delegate void WriteDelegate(BinaryWriter stream, T obj);
		ReadDelegate read;
		WriteDelegate write;

		public Dictionary<Type, DynamicMethod> typeReaders;
		public Dictionary<Type, DynamicMethod> typeWriters;

		public ReflectionSchema() {
			typeReaders = new Dictionary<Type, DynamicMethod>();
			typeWriters = new Dictionary<Type, DynamicMethod>();

			var t = typeof(T);
			CreateReaderWriter(t);
		}

		public T Read(Stream stream)
			=> read(new BinaryReader(stream));

		public T Read(byte[] data) {
			var ms = new MemoryStream(data);
			var r = Read(ms);
			ms.Dispose();

			return r;
		}

		public T Read(string file) {
			var f = File.OpenRead(file);
			var r = Read(f);
			f.Dispose();

			return r;
		}

		public void Write(Stream stream, T obj)
			=> write(new BinaryWriter(stream), obj);

		public byte[] Write(T obj) {
			var ms = new MemoryStream();
			Write(ms, obj);
			var r = ms.GetBuffer();
			ms.Dispose();

			return r;
		}

		public void Write(string file, T obj) {
			var f = File.Create(file);
			Write(f, obj);
			f.Flush();
			f.Dispose();
		}

		void CreateReaderWriter(Type t) {
			var r = CreateActionDM<BinaryReader>(t, $"Read{t.Name}", true);
			var w = CreateFuncDM<BinaryWriter>(t, $"Write{t.Name}", true);

			var _ = r.GetILGenerator();
			var __ = w.GetILGenerator();

			typeReaders[t] = r;
			typeWriters[t] = w;

			var attrbf = t.GetCustomAttributes<BitfieldAttribute>(false);
			var flds = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

			var rv = _.DeclareLocal(t);
			_.NewObj(t);
			_.StLoc(rv);

			foreach (var f in flds) {
				var ft = f.FieldType;

				_.LdLoc(rv);

				if (ft.IsArray) {
					ft = ft.GetElementType();
					var attr = f.GetCustomAttribute<ArrayLengthAttribute>(false);
					_.Dup();
					var len = _.Local<int>();
					
					ReadWritePrimitive(_, __, ft);
				}

				if (!ft.IsPrimitive && !ft.IsEnum) {
					if (!typeReaders.ContainsKey(ft))
						CreateReaderWriter(ft);

					_.LdArg0();
					_.Call(typeReaders[ft]);
				}

				if (ft.IsEnum)
					ft = ft.GetEnumUnderlyingType();

				ReadWritePrimitive(_, __, ft);

				_.StFld(f);
			}

			_.LdLoc(rv);
			_.Ret();
		}

		void ReadWritePrimitive(ILGenerator ril, ILGenerator wil, Type type) {
			var _ = ril;
			var __ = wil;

			if (type == typeof(sbyte)) {
				_.CallVirt<BinaryReader>("ReadSByte");

				__.CallVirt<BinaryWriter, sbyte>("Write");
			}
			else if (type == typeof(byte)) {
				_.CallVirt<BinaryReader>("ReadByte");

				__.CallVirt<BinaryWriter, byte>("Write");
			}
			else if (type == typeof(short)) {
				_.CallVirt<BinaryReader>("ReadInt16");

				__.CallVirt<BinaryWriter, short>("Write");
			}
			else if (type == typeof(ushort)) {
				_.CallVirt<BinaryReader>("ReadUInt16");

				__.CallVirt<BinaryWriter, ushort>("Write");
			}
			else if (type == typeof(int)) {
				_.CallVirt<BinaryReader>("ReadInt32");

				__.CallVirt<BinaryWriter, int>("Write");
			}
			else if (type == typeof(uint)) {
				_.CallVirt<BinaryReader>("ReadUInt32");

				__.CallVirt<BinaryWriter, uint>("Write");
			}
			else if (type == typeof(long)) {
				_.CallVirt<BinaryReader>("ReadInt64");

				__.CallVirt<BinaryWriter, long>("Write");
			}
			else
				if (type == typeof(ulong)) {
				_.CallVirt<BinaryReader>("ReadUInt64");

				__.CallVirt<BinaryWriter, ulong>("Write");
			}
		}
	}
}