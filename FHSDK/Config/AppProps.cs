using System;


namespace FHSDK
{
	/// <summary>
	/// Describe the app configuration options in fh.config file.
	/// </summary>
	public class AppProps
	{
		/// <summary>
		/// Get or Set the app host
		/// </summary>
		public string host { get; set; }
		/// <summary>
		/// Get or set the project id
		/// </summary>
		public string projectid { get; set; }
		/// <summary>
		/// Get or set the appid
		/// </summary>
		public string appid { get; set; }
		/// <summary>
		/// Get or set the app API key
		/// </summary>
		public string appkey { get; set; }
		/// <summary>
		/// Get or set the app mode
		/// </summary>
		public string mode { get; set; }

		public string connectiontag { get; set; }
	}
}

