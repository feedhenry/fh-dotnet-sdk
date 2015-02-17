using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace FHSDK.Services
{
    /// <summary>
    /// Contains implementations of a few services used by the FeedHenry .Net SDK. The interfaces are defined in the FHSDK.dll assemably, then each platform's 
    /// assembly contains platform-specific implementations of these services.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

    /// <summary>
    /// A helper class to resolve the correct implementation if a type using IAdapterResolver
    /// </summary>
    public class ServiceFinder
    {
		private static UnityContainer container = new UnityContainer();

        /// <summary>
        /// Resolve the correct implementation for the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>an instance of the correct implementation class</returns>
        public static T Resolve<T>()
        {
			return container.Resolve<T>();
        }

		public static void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			container.RegisterType<TFrom, TTo>();
		}
    }
}
