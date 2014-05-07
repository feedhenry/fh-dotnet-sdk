using System;
using System.Threading.Tasks;

namespace FHSDK.Touch
{
	public class FHClient: FH
	{
		public new static async Task<bool> Init()
		{
			return await FH.Init ();
		}
	}
}

