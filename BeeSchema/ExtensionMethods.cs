using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BeeSchema {
	static class ExtensionMethods {
		/// <summary>
		/// Pop two values from the stack and push the sum on to the stack.
		/// </summary>
		/// <param name="il">this</param>
		public static void Add(this ILGenerator il)
			=> il.Emit(OpCodes.Add);

		public static void And(this ILGenerator il)
			=> il.Emit(OpCodes.And);

		/// <summary>
		/// Pop two values from the stack and branch to the label if equal.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the values are equal.</param>
		public static void BEq(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Beq, lbl);

		/// <summary>
		/// Pop two values from the stack and branch to the label if the first is greater than or equal to the second.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the first value is greater than or equal to the second.</param>
		public static void BGE(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Bge, lbl);

		/// <summary>
		/// Pop two values from the stack and branch to the label if the first is greater than the second.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the first value is greater than the second.</param>
		public static void BGT(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Bgt, lbl);

		/// <summary>
		/// Pop two values from the stack and branch to the label if the first is less than or equal to the second.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the first value is less than or equal to the second.</param>
		public static void BLE(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Ble, lbl);

		/// <summary>
		/// Pop two values from the stack and branch to the label if the first is less than the second.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the first value is less than the second.</param>
		public static void BLT(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Blt, lbl);

		/// <summary>
		/// Pop two values from the stack and branch to the label if not equal.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the values are not equal.</param>
		public static void BNEUn(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Bne_Un, lbl);

		/// <summary>
		/// Pop a valuetype from the stack and box it as an object instance.
		/// </summary>
		/// <typeparam name="T">The type of the value on the stack.</typeparam>
		/// <param name="il">this</param>
		public static void Box<T>(this ILGenerator il)
			=> il.Emit(OpCodes.Box, typeof(T));

		/// <summary>
		/// Branch to the label.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to.</param>
		public static void Br(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Br, lbl);

		/// <summary>
		/// Pop a value from the stack and branch to the label if 0, false, or null.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="lbl">The label to branch to if the value is 0, false, or null.</param>
		public static void BrFalse(this ILGenerator il, Label lbl)
			=> il.Emit(OpCodes.Brfalse, lbl);

		/// <summary>
		/// Call a given method with a methodinfo using arguments on the stack.
		/// </summary>
		/// <param name="il">this</param>
		/// <param name="meth">The MethodInfo of the method to call.</param>
		public static void Call(this ILGenerator il, MethodInfo meth)
			=> il.Emit(OpCodes.Call, meth);
		/// <summary>
		/// Call the method that matches the provided name using arguments on the stack.
		/// </summary>
		/// <typeparam name="T">The type containing the method.</typeparam>
		/// <param name="il">this</param>
		/// <param name="meth">The name of the method to call.</param>
		public static void Call<T>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Call, typeof(T).GetMethod(meth));
		public static void Call<T, T1>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Call, typeof(T).GetMethod(meth, new[] { typeof(T1) }));
		public static void Call<T, T1, T2, T3>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Call, typeof(T).GetMethod(meth, new[] { typeof(T1), typeof(T2), typeof(T3) }));

		public static void CallVirt<T>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Callvirt, typeof(T).GetMethod(meth));
		public static void CallVirt<T, T1>(this ILGenerator il, string meth)
			=> il.Emit(OpCodes.Callvirt, typeof(T).GetMethod(meth, new[] { typeof(T1) }));

		public static void CEq(this ILGenerator il)
			=> il.Emit(OpCodes.Ceq);

		public static void ConvI4(this ILGenerator il)
			=> il.Emit(OpCodes.Conv_I4);

		public static void ConvI8(this ILGenerator il)
			=> il.Emit(OpCodes.Conv_I8);

		public static void Div(this ILGenerator il)
			=> il.Emit(OpCodes.Div);

		public static void Dup(this ILGenerator il)
			=> il.Emit(OpCodes.Dup);

		public static Label Label(this ILGenerator il)
			=> il.DefineLabel();

		public static void LdArg0(this ILGenerator il)
			=> il.Emit(OpCodes.Ldarg_0);

		public static void LdCI4(this ILGenerator il, int i4)
			=> il.Emit(OpCodes.Ldc_I4, i4);

		public static void LdCI4_1(this ILGenerator il)
			=> il.Emit(OpCodes.Ldc_I4_1);

		public static void LdCI4_4(this ILGenerator il)
			=> il.Emit(OpCodes.Ldc_I4_4);

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
		public static void NewObj<T, T1>(this ILGenerator il)
			=> il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new[] { typeof(T1) }));
		public static void NewObj<T, T1, T2>(this ILGenerator il)
			=> il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2) }));
		public static void NewObj<T, T1, T2, T3, T4, T5, T6, T7, T8>(this ILGenerator il)
			=> il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }));

		public static void Ret(this ILGenerator il)
			=> il.Emit(OpCodes.Ret);

		public static void ShR(this ILGenerator il)
			=> il.Emit(OpCodes.Shr);

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