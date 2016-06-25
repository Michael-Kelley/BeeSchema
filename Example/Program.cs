using System;

using BeeSchema;

namespace Example {
	class Program {
		static void Main() {
			var m = typeof(ResultCollection).GetMethod("get_Item", new[] { typeof(string) });
			// Create an instance of a Schema object from a file.
			var schema = Schema.FromFile("example.bee");
			var compiled = schema.Compile();
			// Parse our example binary data with the resulting Schema instance.
			var _result = compiled.Read("example.bin");
			var result = schema.Parse("example.bin");

			/* The Result class supports both implicit and explicit conversion.
			   This makes it easy to get the Result's value if you know the expected data type.
			   Declared variables can be retrieved by both index and name. */
			byte first = result[0];
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
		}
	}
}