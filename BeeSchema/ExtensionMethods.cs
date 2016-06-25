using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BeeSchema {
	static class ExtensionMethods {
		public static void Add(this ILGenerator il)
			=> il.Emit(OpCodes.Add);

		public static void Box<T>(this ILGenerator il)
			=> il.Emit(OpCodes.Box, typeof(T));

		public static void Br(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Br, lbl);

		public static void BrFalse(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Brfalse, lbl);

		public static void Call(this ILGenerator il, MethodInfo meth)
			=> il.Emit(OpCodes.Call, meth);
		public static void Call<T>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Call, typeof(T).GetMethod(meth));

		public static void CallVirt<T>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Callvirt, typeof(T).GetMethod(meth));
		public static void CallVirt<T, T1>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Callvirt, typeof(T).GetMethod(meth, new[] { typeof(T1) }));

		public static void CEq(this ILGenerator il)
			=> il.Emit(OpCodes.Ceq);

		public static void ConvI8(this ILGenerator il)
			=> il.Emit(OpCodes.Conv_I8);

		public static void Div(this ILGenerator il)
			=> il.Emit(OpCodes.Div);

		public static Label Label(this ILGenerator il)
			=> il.DefineLabel();

		public static void LdArg0(this ILGenerator il)
			=> il.Emit(OpCodes.Ldarg_0);

		public static void LdCI4(this ILGenerator il, int i4)
			=> il.Emit(OpCodes.Ldc_I4, i4);

		public static void LdCI8(this ILGenerator il, long i8)
			=> il.Emit(OpCodes.Ldc_I8, i8);

		public static void LdFld<T>(this ILGenerator il, string fld)
			=> il.Emit(OpCodes.Ldfld, typeof(T).GetField(fld));

		public static void LdLoc(this ILGenerator il, LocalBuilder loc)
			=> il.Emit(OpCodes.Ldloc, loc);

		public static void LdNull(this ILGenerator il)
			=> il.Emit(OpCodes.Ldnull);

		public static void LdStr(this ILGenerator il, string str)
			=> il.Emit(OpCodes.Ldstr, str);

		public static LocalBuilder Local<T>(this ILGenerator il)
			=> il.DeclareLocal(typeof(T));

		public static void Mark(this ILGenerator il, Label lbl)
			=> il.MarkLabel(lbl);

		public static void Mul(this ILGenerator il)
			=> il.Emit(OpCodes.Mul);

		public static void NewObj<T>(this ILGenerator il)
			=> il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));

		public static void NewObj<T, T1, T2>(this ILGenerator il)
			=> il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2) }));

		public static void Ret(this ILGenerator il)
			=> il.Emit(OpCodes.Ret);

		public static void StFld<T>(this ILGenerator il, string fld)
			=> il.Emit(OpCodes.Stfld, typeof(T).GetField(fld));

		public static void StLoc(this ILGenerator il, LocalBuilder loc)
			=> il.Emit(OpCodes.Stloc, loc);

		public static void Sub(this ILGenerator il)
			=> il.Emit(OpCodes.Sub);

		public static void Unbox<T>(this ILGenerator il)
			=> il.Emit(OpCodes.Unbox, typeof(T));

		public static void UnboxAny<T>(this ILGenerator il)
			=> il.Emit(OpCodes.Unbox_Any, typeof(T));
	}
}
