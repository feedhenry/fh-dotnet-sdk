using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FHSDK.Adaptation
{
    // An implementation IAdapterResolver that probes for platforms-specific adapters by dynamically
    // looking for concrete types in platform-specific assemblies
    internal class ProbingAdapterResolver : IAdapterResolver
    {
        private readonly string[] _platformNames;
        private readonly object _lock = new object();
        private Dictionary<Type, object> _adapters = new Dictionary<Type, object>();
        private Assembly _assembly;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="platformNames"> the names of the assemblies to look for the implementations</param>
        public ProbingAdapterResolver(params string[] platformNames)
        {
            Debug.Assert(platformNames != null);

            _platformNames = platformNames;
        }

        /// <summary>
        /// Return the implementation of a type
        /// </summary>
        /// <param name="type">the interface</param>
        /// <returns>The correctly implementation instance of the type</returns>
        public object Resolve(Type type)
        {
            Debug.Assert(type != null);

            lock (_lock)
            {
                object instance;
                if (!_adapters.TryGetValue(type, out instance))
                {
                    Assembly assembly = GetPlatformSpecificAssembly();
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
            Debug.Assert(interfaceType.GetTypeInfo().IsInterface);
            Debug.Assert(interfaceType.DeclaringType == null);
            Debug.Assert(interfaceType.Name.StartsWith("I", StringComparison.Ordinal));

            // For example, if we're looking for an implementation of FHSDK.Services.IDeviceService 
            // then we'll look for FHSDK.Services.DeviceService
            return interfaceType.Namespace + "." + interfaceType.Name.Substring(1);
        }

        private Assembly GetPlatformSpecificAssembly()
        {   // We should be under a lock

            if (_assembly == null)
            {
                _assembly = ProbeForPlatformSpecificAssembly();
                if (_assembly == null)
                    throw new InvalidOperationException("Can not find assembly");
            }

            return _assembly;
        }

        private Assembly ProbeForPlatformSpecificAssembly()
        {
            foreach (string platformName in _platformNames)
            {
                Assembly assembly = ProbeForPlatformSpecificAssembly(platformName);
                if (assembly != null)
                    return assembly;
            }

            return null;
        }

        private Assembly ProbeForPlatformSpecificAssembly(string platformName)
        {
            AssemblyName assemblyName = new AssemblyName(typeof(ProbingAdapterResolver).GetTypeInfo().Assembly.FullName);
            assemblyName.Name = platformName;

            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (Exception e)
            {
            }

            return null;
        }
    }
}