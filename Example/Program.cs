using System;

using BeeSchema;

namespace Example {
	class Program {
		static SomeEnum Get(System.IO.BinaryReader r) {
			return (SomeEnum)r.ReadByte();
		}

		static void Main() {
			var _s = new ReflectionSchema<Example>();
			var _enum = _s.typeWriters[typeof(SomeEnum)];
			var _ms = new System.IO.MemoryStream();
			var _bw = new System.IO.BinaryWriter(_ms);
			var _val = _enum.Invoke(null, new object[] { _bw, SomeEnum.Five });

			var asd = new ReflectionSchema<Example>();
			var sw = new System.Diagnostics.Stopwatch();

			Console.WriteLine("Testing each method 10000 times, twice\n");
			Console.Write("Schema : ");
			sw.Start();
			for (int i = 0; i < 10000; i++) {
				var result = Schema.FromFile("example.bee").Parse("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("CompiledSchema : ");
			sw.Start();
			for (int i = 0; i < 10000; i++) {
				var result = Schema.FromFile("example.bee").Compile().Read("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("Schema (cached) : ");
			sw.Start();
			var _schema = Schema.FromFile("example.bee");
			for (int i = 0; i < 10000; i++) {
				var result = _schema.Parse("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("CompiledSchema (cached) : ");
			sw.Start();
			var __schema = Schema.FromFile("example.bee");
			var __compiled = __schema.Compile();
			for (int i = 0; i < 10000; i++) {
				var result = __compiled.Read("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("Schema : ");
			sw.Start();
			for (int i = 0; i < 10000; i++) {
				var result = Schema.FromFile("example.bee").Parse("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("CompiledSchema : ");
			sw.Start();
			for (int i = 0; i < 10000; i++) {
				var result = Schema.FromFile("example.bee").Compile().Read("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("Schema (cached) : ");
			sw.Start();
			_schema = Schema.FromFile("example.bee");
			for (int i = 0; i < 10000; i++) {
				var result = _schema.Parse("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.Write("CompiledSchema (cached) : ");
			sw.Start();
			__schema = Schema.FromFile("example.bee");
			__compiled = __schema.Compile();
			for (int i = 0; i < 10000; i++) {
				var result = __compiled.Read("example.bin");
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			sw.Reset();

			Console.ReadKey();
		}

		/*static void Main() {
			// Create an instance of a Schema object from a file.
			var schema = Schema.FromFile("example.bee");
			var compiled = schema.Compile();
			// Parse our example binary data with the resulting Schema instance.
			//var result = schema.Parse("example.bin");
			var result = compiled.Read("example.bin");

			/* The Result class supports both implicit and explicit conversion.
			   This makes it easy to get the Result's value if you know the expected data type.
			   Declared variables can be retrieved by both index and name. */
		/*byte first = result[0];
		var anInt = (int)result["an_int"];
		byte[] bytes = result["a_struct"]["bytes"];

		// Print the values grabbed from the returned Result.
		Console.WriteLine($"first result: {first}");
		Console.WriteLine($"an_int: {anInt}");
		Console.Write("a_struct.bytes: ");

		foreach (var b in bytes)
			Console.Write($"{b}, ");

		// Print all values from the returned Result, recursing into any values that have children.
		Console.WriteLine("\n\nresults:");
		WriteResults(result);

		Console.ReadKey();
	}

	static void WriteResults(ResultCollection r, int tab = 0) {
		foreach (var v in r) {
			// Print the current value's type, name and value.
			Console.WriteLine($"{new string('\t', tab)}{v.TypeName} {v.Name} : {v.Value}");

			// If the current value is a struct, bitfield or array, print all its children.
			if (v.HasChildren)
				WriteResults(v.Children, tab + 1);
		}
	}*/
	}
}