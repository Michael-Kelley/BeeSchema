using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BeeSchema {
	public class Result : IEnumerable<Result> {
		public NodeType Type;
		public string TypeName;
		public string Name;
		public long Position;
		public int Size;
		public object Value;
		public string Comment;

		public bool HasChildren => Value is ResultCollection;
		public ResultCollection Children => (ResultCollection)Value;
		public int Count => Children.Count;

		public Result this[int index] => Children[index];
		public Result this[string name] => Children[name];

		public IEnumerator<Result> GetEnumerator() => Children.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

		public static implicit operator bool(Result r) => (bool)r.Value;
		public static implicit operator byte(Result r) => (byte)(long)r.Value;
		public static implicit operator sbyte(Result r) => (sbyte)(long)r.Value;
		public static implicit operator ushort(Result r) => (ushort)(long)r.Value;
		public static implicit operator short(Result r) => (short)(long)r.Value;
		public static implicit operator uint(Result r) => (uint)(long)r.Value;
		public static implicit operator int(Result r) => (int)(long)r.Value;
		public static implicit operator ulong(Result r) => (ulong)(long)r.Value;
		public static implicit operator long(Result r) => (long)r.Value;
		public static implicit operator float(Result r) => (float)r.Value;
		public static implicit operator double(Result r) => (double)r.Value;
		public static implicit operator char(Result r) => (char)r.Value;
		public static implicit operator string(Result r) => (string)r.Value;
		public static implicit operator IPAddress(Result r) => (IPAddress)r.Value;
		public static implicit operator DateTime(Result r) => (DateTime)r.Value;

		public static implicit operator bool[] (Result r) => r.Select(a => (bool)a).ToArray();
		public static implicit operator byte[] (Result r) => r.Select(a => (byte)(long)a).ToArray();
		public static implicit operator sbyte[] (Result r) => r.Select(a => (sbyte)(long)a).ToArray();
		public static implicit operator ushort[] (Result r) => r.Select(a => (ushort)(long)a).ToArray();
		public static implicit operator short[] (Result r) => r.Select(a => (short)(long)a).ToArray();
		public static implicit operator uint[] (Result r) => r.Select(a => (uint)(long)a).ToArray();
		public static implicit operator int[] (Result r) => r.Select(a => (int)(long)a).ToArray();
		public static implicit operator ulong[] (Result r) => r.Select(a => (ulong)(long)a).ToArray();
		public static implicit operator long[] (Result r) => r.Select(a => (long)a).ToArray();
		public static implicit operator float[] (Result r) => r.Select(a => (float)a).ToArray();
		public static implicit operator double[] (Result r) => r.Select(a => (double)a).ToArray();
		public static implicit operator char[] (Result r) => ((string)r.Value).ToCharArray();
		public static implicit operator string[] (Result r) => r.Select(a => (string)a).ToArray();
		public static implicit operator IPAddress[] (Result r) => r.Select(a => (IPAddress)a).ToArray();
		public static implicit operator DateTime[] (Result r) => r.Select(a => (DateTime)a).ToArray();
	}
}