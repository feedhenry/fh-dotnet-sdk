using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FHSDK.Adaptation
{
	// An implementation IAdapterResolver that probes for platforms-specific adapters by dynamically
	// looking for concrete types in platform-specific assemblies, such as Portable.Silverlight.
	internal class ProbingAdapterResolver : IAdapterResolver
	{
		private readonly Func<AssemblyName, Assembly> _assemblyLoader;
		private readonly object _lock = new object();
		private Dictionary<Type, object> _adapters = new Dictionary<Type, object>();
		private Assembly _assembly;

		public ProbingAdapterResolver()
			: this(Assembly.Load)
		{
		}

		public ProbingAdapterResolver(Func<AssemblyName, Assembly> assemblyLoader)
		{
			Debug.Assert(assemblyLoader != null);

			_assemblyLoader = assemblyLoader;
		}

		public object Resolve(Type type, AssemblyName assemblyName)
		{
			Debug.Assert(type != null);

			lock (_lock)
			{
				object instance;
				if (!_adapters.TryGetValue(type, out instance))
				{
					Assembly assembly = GetAssembly(assemblyName);
					instance = ResolveAdapter(assembly, type);
					_adapters.Add(type, instance);
				}

				return instance;
			}
		}

		private static object ResolveAdapter(Assembly assembly, Type interfaceType)
		{
			string typeName = MakeAdapterTypeName(interfaceType);

			Type type = assembly.GetType(typeName);
			if (type != null)
				return Activator.CreateInstance(type);

			return type;
		}

		private static string MakeAdapterTypeName(Type interfaceType)
		{
			Debug.Assert(interfaceType.DeclaringType == null);
			Debug.Assert(interfaceType.Name.StartsWith("I", StringComparison.Ordinal));

			// For example, if we're looking for an implementation of FHSDK.Services.IDeviceService 
			// then we'll look for FHSDK.Services.DeviceService
			return interfaceType.Namespace + "." + interfaceType.Name.Substring(1);
		}

		private Assembly GetAssembly(AssemblyName assemblyName)
		{   // We should be under a lock

			if (_assembly == null)
			{
				_assembly = LoadAssembly(assemblyName);
				if (_assembly == null)
					throw new InvalidOperationException("Can not find assembly");
			}

			return _assembly;
		}

		private Assembly LoadAssembly(AssemblyName assemblyName)
		{
			try
			{
				return _assemblyLoader(assemblyName);
			}
			catch (Exception e)
			{
				Debug.WriteLine (e.StackTrace);
			}

			return null;
		}
	}
}