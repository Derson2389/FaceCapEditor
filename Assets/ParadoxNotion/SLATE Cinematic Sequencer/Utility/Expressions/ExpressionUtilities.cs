#if SLATE_USE_EXPRESSIONS

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using StagPoint.Eval;
using Environment = StagPoint.Eval.Environment;

namespace Slate{

	///Helpers for StagPoint.Eval expression environment
	public static class ExpressionUtilities{

		public static VariableBase GetVariable(this Environment environment, string name ) {
			VariableBase result = null;
			environment.Variables.TryGetValue( name, out result );
			return result;
		}

		public static object GetConstant(this Environment environment, string name ){
			object result = null;
			environment.Constants.TryGetValue( name, out result );
			return result;
		}



		public static void AddBoundProperty(this Environment environment, string name, object target, string propertyName){
			var property = target.GetType().RTGetProperty(propertyName);
			if (property != null && !property.RTIsStatic()){
				var boundVariable = new BoundVariable(name, target, property);
				environment.AddVariable(boundVariable);
			}
		}

		public static void AddBoundProperty(this Environment environment, string name, object target, PropertyInfo property){
			var boundVariable = new BoundVariable( name, target, property );
			environment.AddVariable( boundVariable );
		}

		public static void AddBoundMethod(this Environment environment, string name, object target, string methodName ){
			var method = target.GetType().RTGetMethod( methodName );
			if (method != null && !method.IsStatic){
				var boundVariable = new BoundVariable( name, target, method );
				environment.AddVariable( boundVariable );
			}
		}

		public static void AddBoundMethod(this Environment environment, string name, object target, MethodInfo method ){
			var boundVariable = new BoundVariable( name, target, method );
			environment.AddVariable( boundVariable );
		}



		public static void AddStaticProperty(this Environment environment, string name, System.Type type, string propertyName ){
			var property = type.RTGetProperty(propertyName);
			if (property != null && property.RTIsStatic() ){
				var boundVariable = new BoundVariable(name, null, property);
				environment.AddVariable(boundVariable);
			}
		}

		public static void AddStaticProperty(this Environment environment, string name, System.Type type, PropertyInfo property){
			var boundVariable = new BoundVariable(name, null, property);
			environment.AddVariable(boundVariable);
		}

		public static void AddStaticMethod(this Environment environment, string name, System.Type type, string methodName ){
			var method = type.RTGetMethod(methodName);
			if (method != null && method.IsStatic){
				var boundVariable = new BoundVariable(name, null, method);
				environment.AddVariable(boundVariable);
			}
		}

		public static void AddStaticMethod(this Environment environment, string name, System.Type type, MethodInfo method){
			var boundVariable = new BoundVariable(name, null, method);
			environment.AddVariable(boundVariable);
		}



		public static void AddDelegateMethod<TResult>(this Environment environment, string name, Func<TResult> callback ) {
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<TResult, T1>(this Environment environment, string name, Func<T1, TResult> callback ) {
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<TResult, T1, T2>(this Environment environment, string name, Func<T1, T2, TResult> callback ) {
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<TResult, T1, T2, T3>(this Environment environment, string name, Func<T1, T2, T3, TResult> callback ) {
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<TResult, T1, T2, T3, T4>(this Environment environment, string name, Func<T1, T2, T3, T4, TResult> callback ) {
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}


		public static void AddDelegateMethod(this Environment environment, string name, Action callback ){
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<T1>(this Environment environment, string name, Action<T1> callback ){
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<T1, T2>(this Environment environment, string name, Action<T1, T2> callback ){
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<T1, T2, T3>(this Environment environment, string name, Action<T1, T2, T3> callback ){
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

		public static void AddDelegateMethod<T1, T2, T3, T4>(this Environment environment, string name, Action<T1, T2, T3, T4> callback ){
			environment.AddBoundMethod( name, callback.Target, callback.Method );
		}

	}
}

#endif