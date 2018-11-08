using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Slate{

	///A collection of helper tools relevant to runtime
	public static class ReflectionTools {

		#if !NETFX_CORE
		private const BindingFlags flagsEverything = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		#endif

		///Assemblies
		private static List<Assembly> _loadedAssemblies;
		private static List<Assembly> loadedAssemblies{
        	get
        	{
        		if (_loadedAssemblies == null){

	        		#if NETFX_CORE

				    _loadedAssemblies = new List<Assembly>();
		 		    var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
				    var folderFilesAsync = folder.GetFilesAsync();
				    folderFilesAsync.AsTask().Wait();

				    foreach (var file in folderFilesAsync.GetResults()){
				        if (file.FileType == ".dll" || file.FileType == ".exe"){
				            try
				            {
				                var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
				                AssemblyName name = new AssemblyName { Name = filename };
				                Assembly asm = Assembly.Load(name);
				                _loadedAssemblies.Add(asm);
				            }
				            catch { continue; }
				        }
				    }

	        		#else

	        		_loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

	        		#endif
	        	}

	        	return _loadedAssemblies;
        	}
        }


		//Alternative to Type.GetType to work with FullName instead of AssemblyQualifiedName when looking up a type by string
		private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();
		public static Type GetType(string typeName){

			Type type = null;

			if (typeMap.TryGetValue(typeName, out type)){
				return type;
			}

			type = Type.GetType(typeName);
			if (type != null){
				return typeMap[typeName] = type;
			}

            foreach (var asm in loadedAssemblies) {
                try { type = asm.GetType(typeName); }
                catch { continue; }
                if (type != null) {
                    return typeMap[typeName] = type;
                }
            }

            //worst case scenario
            foreach(var t in GetAllTypes()){
            	if (t.Name == typeName){
            		return typeMap[typeName] = t;
            	}
            }

			UnityEngine.Debug.LogError(string.Format("Requested Type with name '{0}', could not be loaded", typeName));
            return null;
		}

		///Get every single type in loaded assemblies
		public static Type[] GetAllTypes(){
			var result = new List<Type>();   
			foreach (var asm in loadedAssemblies){
				try {result.AddRange(asm.RTGetExportedTypes());}
				catch { continue; }
			}
			return result.ToArray();
		}

		///Get a list of types deriving provided base type
		public static Type[] GetDerivedTypesOf(Type baseType){
			var result = new List<Type>();
			foreach(var asm in loadedAssemblies){
				try {result.AddRange(asm.RTGetExportedTypes().Where(t => t.RTIsSubclassOf(baseType) && !t.RTIsAbstract() ));}
				catch { continue; }
			}
			return result.ToArray();
		}

       
        private static Type[] RTGetExportedTypes(this Assembly asm){
			#if NETFX_CORE
			return asm.ExportedTypes.ToArray();
			#else
			return asm.GetExportedTypes();
			#endif
		}

		//Just a more friendly name for certain (few) types.
		public static string FriendlyName(this Type type){
			if (type == null){ return "NULL"; }
			if (type == typeof(float)){ return "Float"; }
			if (type == typeof(int)){ return "Integer"; }
			return type.Name;
		}

        //Is property static?
        public static bool RTIsStatic(this PropertyInfo propertyInfo){
            return ((propertyInfo.CanRead && propertyInfo.RTGetGetMethod().IsStatic) || (propertyInfo.CanWrite && propertyInfo.RTGetSetMethod().IsStatic));
        }

		public static bool RTIsAbstract(this Type type){
			#if NETFX_CORE
			return type.GetTypeInfo().IsAbstract;
			#else           
			return type.IsAbstract;
			#endif			
		}

		public static bool RTIsSubclassOf(this Type type, Type other){
			#if NETFX_CORE
			return type.GetTypeInfo().IsSubclassOf(other);
			#else
			return type.IsSubclassOf(other);
			#endif						
		}

		public static bool RTIsAssignableFrom(this Type type, Type second){
			#if NETFX_CORE
			return type.GetTypeInfo().IsAssignableFrom(second.GetTypeInfo());
			#else
			return type.IsAssignableFrom(second);
			#endif
		}

		public static FieldInfo RTGetField(this Type type, string name){
			#if NETFX_CORE
			return type.GetRuntimeFields().FirstOrDefault(f => f.Name == name);
			#else
			return type.GetField(name, flagsEverything);
			#endif
		}

		public static PropertyInfo RTGetProperty(this Type type, string name){
			#if NETFX_CORE
			return type.GetRuntimeProperties().FirstOrDefault(p => p.Name == name);
			#else
			return type.GetProperty(name, flagsEverything);
			#endif
		}

		public static MethodInfo RTGetMethod(this Type type, string name){
			#if NETFX_CORE
			return type.GetRuntimeMethods().FirstOrDefault(m => m.Name == name);
			#else
			return type.GetMethod(name, flagsEverything);
			#endif
		}

		public static FieldInfo[] RTGetFields(this Type type){
			#if NETFX_CORE
			return type.GetRuntimeFields().ToArray();
			#else
			return type.GetFields(flagsEverything);
			#endif
		}

		public static PropertyInfo[] RTGetProperties(this Type type){
			#if NETFX_CORE
			return type.GetRuntimeProperties().ToArray();
			#else
			return type.GetProperties(flagsEverything);
			#endif
		}

		public static MemberInfo[] RTGetPropsAndFields(this Type type){
			var result = new List<MemberInfo>();
			result.AddRange(type.RTGetFields());
			result.AddRange(type.RTGetProperties());
			return result.ToArray();
		}

		public static MethodInfo RTGetGetMethod(this PropertyInfo prop){
			#if NETFX_CORE
			return prop.GetMethod;
			#else
			return prop.GetGetMethod();
			#endif			
		}

		public static MethodInfo RTGetSetMethod(this PropertyInfo prop){
			#if NETFX_CORE
			return prop.SetMethod;
			#else
			return prop.GetSetMethod();
			#endif
		}

		public static Type RTReflectedType(this Type type){
			#if NETFX_CORE
			return type.GetTypeInfo().DeclaringType; //no way to get ReflectedType here that I know of...
			#else
			return type.ReflectedType;
			#endif			
		}

		public static Type RTReflectedType(this MemberInfo member){
			#if NETFX_CORE
			return member.DeclaringType; //no way to get ReflectedType here that I know of...
			#else
			return member.ReflectedType;
			#endif						
		}

		public static T RTGetAttribute<T>(this Type type, bool inherited) where T : Attribute {
			#if NETFX_CORE
			return (T)type.GetTypeInfo().GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
			#else
			return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
			#endif			
		}

		public static T RTGetAttribute<T>(this MemberInfo member, bool inherited) where T : Attribute{
			#if NETFX_CORE
			return (T)member.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
			#else
			return (T)member.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
			#endif			
		}

		///Creates a delegate out of Method for target instance
		public static T RTCreateDelegate<T>(this MethodInfo method, object instance){
			#if NETFX_CORE
			return (T)(object)method.CreateDelegate(typeof(T), instance);
			#else
			return (T)(object)Delegate.CreateDelegate(typeof(T), instance, method);
			#endif
		}

	}
}