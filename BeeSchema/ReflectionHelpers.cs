using System;
using System.Reflection.Emit;

namespace BeeSchema {
	static class ReflectionHelpers {
		public static DynamicMethod CreateActionDM<T, T1>(string name, bool restrictedSkipVisibility = false)
			=> new DynamicMethod(name, typeof(T), new[] { typeof(T1) }, restrictedSkipVisibility);
		public static DynamicMethod CreateActionDM<T1>(Type returnType, string name, bool restrictedSkipVisibility = false)
			=> new DynamicMethod(name, returnType, new[] { typeof(T1) }, restrictedSkipVisibility);
		public static DynamicMethod CreateFuncDM<T1, T2>(string name, bool restrictedSkipVisibility = false)
			=> new DynamicMethod(name, typeof(void), new[] { typeof(T1), typeof(T2) }, restrictedSkipVisibility);
		public static DynamicMethod CreateFuncDM<T1>(Type secondParamType, string name, bool restrictedSkipVisibility = false)
			=> new DynamicMethod(name, typeof(void), new[] { typeof(T1), secondParamType }, restrictedSkipVisibility);
	}
}