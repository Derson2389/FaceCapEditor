using System;
using System.Reflection;

namespace DigitalSky.Recorder
{
    public static class Singleton<T> where T : class
    {
        private static volatile T _instance;
        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            Type type = typeof(T);
                            ConstructorInfo ctor;
                            ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                          null, new Type[0], new ParameterModifier[0]);
                            _instance = (T)ctor.Invoke(new object[0]);
                        }
                    }
                }

                return _instance;
            }
        }
    }
}