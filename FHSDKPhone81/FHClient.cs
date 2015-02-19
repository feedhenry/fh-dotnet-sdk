using FHSDK;
using FHSDK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK81.Phone
{
    /// <summary>
    /// Contains the entry class of the FeedHenry SDK for Windows Phone 8 platform. It's defined in the FHSDKPhone.dll.
    /// To use the FeedHenry SDK, both FHSDK.dll and FHSDKPhone.dll should be referenced by your WP8 project, and initialise the SDK using the FHClient class in this name space.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

    /// <summary>
    /// Initialise the FeedHenry SDK. This should be called before any other API functions are invoked. Usually this should be called after the app finish intialising.
    /// </summary>
    /// <example>
    /// <code>
    ///  public MainPage()
    ///    {
    ///        InitializeComponent();
    ///        InitApp();
    ///    }
    ///
    ///    private async void InitApp()
    ///    {
    ///        try
    ///        {
    ///            bool inited = await FHClient.Init();
    ///            if(inited)
    ///            {
    ///              //Initialisation is successful
    ///            }
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
    public class FHClient : FH
    {
        public new static async Task<bool> Init()
        {
            RegisterServices();
            return await FH.Init();
        }
        
		private static void RegisterServices()
		{
			ServiceFinder.RegisterType<IOAuthClientHandlerService, OAuthClientHandlerService> ();
			ServiceFinder.RegisterType<IDataService, DataService> ();
			ServiceFinder.RegisterType<IIOService, IOService> ();
			ServiceFinder.RegisterType<IDeviceService, DeviceService> ();
			ServiceFinder.RegisterType<IHashService, HashService> ();
			ServiceFinder.RegisterType<ILogService, LogService> ();
			ServiceFinder.RegisterType<IMonitorService, MonitorService> ();
			ServiceFinder.RegisterType<INetworkService, NetworkService> ();
		}
    }
}
