using System;
using FHSDK;
using FHSDK.Services;
using System.Threading.Tasks;

namespace FHSDK.Touch
{
	public class FH: FHBase
	{
		static FH ()
		{
			ServiceFinder.SetTargetAssemblyName(typeof(FH).Assembly.GetName());
		}

		public new static void SetLogLevel(int level)
		{
			FHBase.SetLogLevel (level);
		}


		public new static async Task<bool> Init()
		{
			return await FHBase.Init ();
		}
	}
}

