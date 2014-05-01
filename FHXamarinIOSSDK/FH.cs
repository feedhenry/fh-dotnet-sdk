using System;
using FHSDK;
using FHSDK.Services;
using System.Threading.Tasks;

namespace FHSK.Touch
{
	public class FH: FHBase
	{
		static FH ()
		{
			ServiceFinder.SetTargetAssemblyName(typeof(FH).Assembly.GetName());
		}

		public new static async Task<bool> Init()
		{
			return await FHBase.Init ();
		}
	}
}

