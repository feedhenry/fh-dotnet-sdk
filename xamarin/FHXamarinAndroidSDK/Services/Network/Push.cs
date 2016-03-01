using System;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
	public class Push : PushBase
	{
		public Push ()
		{
		}

		protected override AeroGear.Push.Registration CreateRegistration ()
		{
			return new GcmRegistration ();
		}

	}
}

