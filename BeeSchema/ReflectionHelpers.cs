using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace BeeSchema {
	static class ReflectionHelpers {
		public static DynamicMethod CreateActionDM<T, T1>(string name)
			=> new DynamicMethod(name, typeof(T), new[] { typeof(T1) });
		public static DynamicMethod CreateFuncDM<T1, T2>(string name)
			=> new DynamicMethod(name, typeof(void), new[] { typeof(T1), typeof(T2) });
	}
}