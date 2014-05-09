using System;


namespace FHSDK
{
	/// <summary>
	/// Describe the app configuration options specified in the FeedHenry config file.
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
        /// Get or set the connection tag.
        /// </summary>
		public string connectiontag { get; set; }
        /// <summary>
        /// Get or set the app mode. Deprecated.
        /// </summary>
        public string mode { get; set; }
	}
}

