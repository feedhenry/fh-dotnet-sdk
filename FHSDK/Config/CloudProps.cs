using System;
using Newtonsoft.Json.Linq;

namespace FHSDK
{
    /// <summary>
    /// Class represents the cloud app instance (MBAAS service) the app should be communication with.
    /// </summary>
	public class CloudProps
	{
		private JObject cloudPropsJson;

		private string hostUrl;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="props">The json format of the cloud host info</param>
		public CloudProps(JObject props)
		{
			cloudPropsJson = props;
		}

        /// <summary>
        /// Return the cloud host info as URL
        /// </summary>
        /// <returns>the cloud host url</returns>
		public string GetCloudHost()
		{
			if (null == hostUrl) {
				if (null != cloudPropsJson["url"])
				{
					hostUrl = (string) cloudPropsJson["url"];
				}
				else
				{
					JObject hosts = (JObject) cloudPropsJson["hosts"];
					if (null != hosts ["url"]) {
						hostUrl = (string) hosts ["url"];
					} else {
						string appMode = FHConfig.getInstance ().GetMode ();
						if ("dev" == appMode)
						{
							hostUrl = (string) hosts["debugCloudUrl"];
						}
						else
						{
							hostUrl = (string) hosts["releaseCloudUrl"];
						}
					}

				}
				hostUrl = hostUrl.EndsWith("/") ? hostUrl.Substring(0, hostUrl.Length - 1) : hostUrl;
			}
			return hostUrl;
		}
	}

}

