using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    /// <summary>
    /// A service interface provides information about the device
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Return the unique id of the device
        /// </summary>
        /// <returns>the unique device id</returns>
        string GetDeviceId();
        /// <summary>
        /// Return the FeedHenry app configurations
        /// </summary>
        /// <returns>the FeedHenry app configurations </returns>
		AppProps ReadAppProps(); 
        /// <summary>
        /// Get the device type. E.g. iphone, android, windowsphone8
        /// </summary>
        /// <returns>the device type</returns>
		string GetDeviceDestination();
    }
}
