using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BeeSchema;

namespace UnitTest {
	[TestClass]
	public class UnitTest1 {
		[TestMethod]
		public void Schema_FromText_ReadValidEnum_BaseOfByte_AutoIncrementingValues() {
			var bee = @"
enum MyEnum : byte {
	Zero,
	One,
	Two
}
";
			var schema = Schema.FromText(bee);
			Assert.AreEqual(1, schema.Types.Count, "Expected 1 type. Got {0}.", schema.Types.Count);

			var myEnum = schema.Types["MyEnum"];
			Assert.AreEqual("MyEnum", myEnum.Name, "Enum name not parsed correctly.");
			Assert.AreEqual(NodeType.EnumDef, myEnum.Type, "Expected type of EnumDef. Got {0}.", myEnum.Type);
			Assert.AreEqual(3, myEnum.Children.Count, "Expected 3 children. Got {0}.", myEnum.Children.Count);

			Assert.AreEqual(NodeType.Byte, myEnum.Children[0].Type, "Base type not parsed correctly.");

			Assert.AreEqual("Zero", myEnum.Children[0].Name, "Child name not parsed correctly. (i: 0)");
			Assert.AreEqual(0L, myEnum.Children[0].Value, "Child value not parsed correctly. (i: 0)");
			Assert.AreEqual("One", myEnum.Children[1].Name, "Child name not parsed correctly. (i: 1)");
			Assert.AreEqual(1L, myEnum.Children[1].Value, "Child value not parsed correctly. (i: 1)");
			Assert.AreEqual("Two", myEnum.Children[2].Name, "Child name not parsed correctly. (i: 2)");
			Assert.AreEqual(2L, myEnum.Children[2].Value, "Child value not parsed correctly. (i: 2)");
		}

		[TestMethod]
		public void Schema_FromText_ReadValidEnum_ManuallySetValues() {
			var bee = @"
enum MyEnum : int {
	One = 1,
	Three = 3,
	Seven = 7
}
";
			var schema = Schema.FromText(bee);
			Assert.AreEqual(1, schema.Types.Count, "Expected 1 type. Got {0}.", schema.Types.Count);

			var myEnum = schema.Types["MyEnum"];
			Assert.AreEqual(3, myEnum.Children.Count, "Expected 3 children. Got {0}.", myEnum.Children.Count);

			Assert.AreEqual("One", myEnum.Children[0].Name, "Child name not parsed correctly. (i: 0)");
			Assert.AreEqual(1L, myEnum.Children[0].Value, "Child value not parsed correctly. (i: 0)");
			Assert.AreEqual("Three", myEnum.Children[1].Name, "Child name not parsed correctly. (i: 1)");
			Assert.AreEqual(3L, myEnum.Children[1].Value, "Child value not parsed correctly. (i: 1)");
			Assert.AreEqual("Seven", myEnum.Children[2].Name, "Child name not parsed correctly. (i: 2)");
			Assert.AreEqual(7L, myEnum.Children[2].Value, "Child value not parsed correctly. (i: 2)");
		}

		[TestMethod]
		public void Schema_FromText_ReadValidEnum_MixedValues() {
			var bee = @"
enum MyEnum : long {
	One = 1,
	Two,
	Seven = 7
}
";
			var schema = Schema.FromText(bee);
			var myEnum = schema.Types["MyEnum"];

			Assert.AreEqual(1L, myEnum.Children[0].Value, "Child value not parsed correctly. (i: 0)");
			Assert.AreEqual(2L, myEnum.Children[1].Value, "Child value not parsed correctly. (i: 1)");
			Assert.AreEqual(7L, myEnum.Children[2].Value, "Child value not parsed correctly. (i: 2)");
		}
	}
}