using System;
using System.Threading.Tasks;
using FHSDK.Services;
using System.Reflection;

namespace FHSDK.Droid
{
	public class FH : FHBase
	{
		static FH()
		{
			ServiceFinder.SetTargetAssemblyName(typeof(FH).Assembly.GetName());
		}

		public new static async Task<bool> Init()
		{
			return await FHBase.Init ();
		}
	}
}

