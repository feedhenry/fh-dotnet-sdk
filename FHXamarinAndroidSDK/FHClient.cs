using System;
using System.Threading.Tasks;

namespace FHSDK.Droid
{
	public class FHClient: FH
	{
		public new static async Task<bool> Init()
		{
			return await FH.Init ();
		}
	}
}


