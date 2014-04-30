using System;
using Newtonsoft.Json.Linq;

namespace FHSDK
{
	public class CloudProps
	{
		private JObject cloudPropsJson;

		private string hostUrl;

		public CloudProps(JObject props)
		{
			cloudPropsJson = props;
		}

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

