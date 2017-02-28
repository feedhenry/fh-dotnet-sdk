using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FHSDK.Config;
using FHSDK.Services;
using FHSDK.Services.Data;

namespace FHSDK.API
{
    /// <summary>
    /// FHAuthSession is resposible to manage OAuth tokens.
    /// </summary>
    public sealed class FHAuthSession
    {
        private const string SessionTokenKey = "sessionToken";
        private const string VerifyPath = "box/srv/1.1/admin/authpolicy/verifysession";
        private const string RevokePath = "box/srv/1.1/admin/authpolicy/revokesession";
        private static readonly FHAuthSession Instance = new FHAuthSession();
        private readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(5*1000);

        private FHAuthSession()
        {
        }

        /// <summary>
        ///     Get unique instance of FHAuthSession singleton.
        /// </summary>
        public static FHAuthSession GetInstance
        {
            get { return Instance; }
        }

        /// <summary>
        ///     Save the session token
        /// </summary>
        /// <param name="sessionToken"></param>
        internal static void SaveSession(string sessionToken)
        {
            var data = ServiceFinder.Resolve<IDataService>();
            data.SaveData(SessionTokenKey, sessionToken);
        }

        /// <summary>
        ///     Check if a session token exists
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            var data = ServiceFinder.Resolve<IDataService>();
            var saved = data.GetData(SessionTokenKey);
            return null != saved;
        }

        /// <summary>
        ///     Return the saved session token value
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            var data = ServiceFinder.Resolve<IDataService>();
            var saved = data.GetData(SessionTokenKey);
            return saved;
        }

        /// <summary>
        ///     Verify if the local session token is valid
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Verify()
        {
            var saved = GetToken();
            if (null != saved)
            {
                var fhres = await CallRemote(VerifyPath, saved);
                var json = fhres.GetResponseAsJObject();
                var isValid = (bool) json["isValid"];
                return isValid;
            }
            return false;
        }

        /// <summary>
        ///     Clear the local session and delete from remote too.
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            var dataService = ServiceFinder.Resolve<IDataService>();
            var saved = GetToken();
            if (null != saved)
            {
                dataService.DeleteData(SessionTokenKey);
                await CallRemote(RevokePath, saved);
            }
        }

        private async Task<FHResponse> CallRemote(string path, string sessionToken)
        {
            var uri = new Uri(string.Format("{0}/{1}", FHConfig.GetInstance().GetHost(), path));
            var data = new Dictionary<string, object> {{SessionTokenKey, sessionToken}};
            var fhres = await FHHttpClient.FHHttpClient.SendAsync(uri, "POST", null, data, _timeout);
            return fhres;
        }
    }
}