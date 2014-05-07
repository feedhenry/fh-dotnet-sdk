using System;
using System.Collections.Generic;
using System.Linq;
using FHSDK.Adaptation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FHSDK.Services
{
    class ServiceFinder
    {
        private static readonly string[] KnownPlatformNames = new[] { "FHSDKPhone", "FHXamarinAndroidSDK", "FHXmarinIOSSDK" };
        private static IAdapterResolver _resolver = new ProbingAdapterResolver(KnownPlatformNames);

        public static T Resolve<T>()
        {
            Type tType = typeof(T);
            T value = (T)_resolver.Resolve(tType);

            if (value == null)
                throw new PlatformNotSupportedException("No implementation found for " + tType.FullName);

            return value;
        }
    }
}
