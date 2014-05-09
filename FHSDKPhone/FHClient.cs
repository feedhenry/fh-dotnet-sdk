using FHSDK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Phone
{
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
            return await FH.Init();
        }
    }
}
