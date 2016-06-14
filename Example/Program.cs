using System;

using BeeSchema;

namespace Example {
	class Program {
		static void Main() {
			var schema = Schema.FromFile("example.bee");
			var result = schema.Parse("example.bin");

			byte first = result[0];
			int anInt = result["an_int"];
			byte[] bytes = result["a_struct"]["bytes"];

			Console.WriteLine($"first result: {first}");
			Console.WriteLine($"an_int: {anInt}");
			Console.Write("a_struct.bytes: ");

			foreach (var b in bytes)
				Console.Write($"{b}, ");

			Console.WriteLine("\n\nresults:");
			WriteResults(result);


			Console.ReadKey();
		}

		static void WriteResults(ResultCollection r, int tab = 0) {
			foreach (var v in r) {
				Console.WriteLine($"{new string('\t', tab)}{v.TypeName} {v.Name} : {v.Value}");

				if (v.HasChildren)
					WriteResults(v.Values, tab + 1);
			}
		}
	}
}