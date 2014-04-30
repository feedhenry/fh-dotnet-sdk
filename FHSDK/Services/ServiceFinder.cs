using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FHSDK.Adaptation;
using System.Reflection;

namespace FHSDK.Services
{
	public class ServiceFinder
    {
		/*private static Dictionary<Type, object> _adapters = new Dictionary<Type, object>();

        public static T Resolve<T>()
        {
            Type tType = typeof(T);
			object instance;

			if (!_adapters.TryGetValue (tType, out instance)) 
			{
				throw new PlatformNotSupportedException("No implementation found for " + tType.FullName);
			}

			return (T) instance;
        }

		public static void AddServiceImplementation<T> (object instance)
		{
			Type tType = typeof(T);
			_adapters.Add (tType, instance);
		}*/

		private static IAdapterResolver _resolver = new ProbingAdapterResolver();
		private static AssemblyName _targetAssemblyName;

		public static void SetTargetAssemblyName(AssemblyName targetAssembly)
		{
			_targetAssemblyName = targetAssembly;
		}


		public static T Resolve<T>()
		{
			Type tType = typeof(T);
			T value = (T)_resolver.Resolve(tType, _targetAssemblyName);

			if (value == null)
				throw new PlatformNotSupportedException("No implementation found for " + tType.FullName);

			return value;
		}
    }
}
