using System;
using System.Threading.Tasks;

namespace FHSDK.Touch
{
    /// <summary>
    /// Contains the entry class of the FeedHenry Xamarin SDK for iOS platform. It's defined in the FHXamarinIOSSDK.dll.
    /// To use the FeedHenry SDK, both FHSDK.dll and FHXamarinIOSSDK.dll should be referenced by your Xamarain iOS project, and initialise the SDK using the FHClient class in this name space.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

	public class FHClient: FH
	{
        /// <summary>
        /// Initialise the FeedHenry SDK. This should be called before any other API functions are invoked. Usually this should be called after the app finish intialising.
        /// </summary>
        /// <example>
        /// <code>
        ///  public override void ViewDidLoad ()
        ///  {
        ///      //Other app init work
        ///      InitApp();
        ///  }
        ///
        ///  private async void InitApp()
        ///  {
        ///      try
        ///      {
        ///          bool inited = await FHClient.Init();
        ///          if(inited)
        ///          {
        ///            //Initialisation is successful
        ///          }
        ///       }
        ///       catch(FHException e)
        ///       {
        ///           //Initialisation failed, handle exception
        ///       }
        ///    }
        /// 
        /// </code>
        /// </example>
        /// <returns>If Init is success or not</returns>
        /// <exception cref="FHException"></exception>
		public new static async Task<bool> Init()
		{
			return await FH.Init ();
		}
	}
}

