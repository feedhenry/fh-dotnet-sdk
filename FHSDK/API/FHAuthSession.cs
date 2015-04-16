using FHSDK.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.API
{
    public sealed class FHAuthSession
    {
        private static FHAuthSession instance = new FHAuthSession();
        private const string SESSION_TOKEN_KEY = "sessionToken";
        private const string VERIFY_PATH = "box/srv/1.1/admin/authpolicy/verifysession";
        private const string REVOKE_PATH = "box/srv/1.1/admin/authpolicy/revokesession";
        private TimeSpan timeout = TimeSpan.FromMilliseconds(5*1000);

        private FHAuthSession()
        {

        }

        public static FHAuthSession Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Save the session token
        /// </summary>
        /// <param name="sessionToken"></param>
        internal void SaveSession(string sessionToken)
        {
            IDataService data = ServiceFinder.Resolve<IDataService>();
            data.SaveData(SESSION_TOKEN_KEY, sessionToken);
        }

        /// <summary>
        /// Check if a session token exists
        /// </summary>
        /// <returns></returns>
        public Boolean Exists()
        {
            IDataService data = ServiceFinder.Resolve<IDataService>();
            string saved = data.GetData(SESSION_TOKEN_KEY);
            return null != saved;
        }

        /// <summary>
        /// Return the saved session token value
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            IDataService data = ServiceFinder.Resolve<IDataService>();
            string saved = data.GetData(SESSION_TOKEN_KEY);
            return saved;
        }

        /// <summary>
        /// Verify if the local session token is valid
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Verify()
        {
            string saved = GetToken();
            if (null != saved)
            {
                FHResponse fhres = await CallRemote(VERIFY_PATH, saved);
                JObject json = fhres.GetResponseAsJObject();
                bool isValid = (bool)json["isValid"];
                return isValid;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clear the local session and delete from remote too.
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            IDataService dataService = ServiceFinder.Resolve<IDataService>();
            string saved = GetToken();
            if (null != saved)
            {
                dataService.DeleteData(SESSION_TOKEN_KEY);
                FHResponse fhres = await CallRemote(REVOKE_PATH, saved);
                return;
            }
        }

        private async Task<FHResponse> CallRemote(string path, string sessionToken)
        {
            Uri uri = new Uri(String.Format("{0}/{1}", FHConfig.getInstance().GetHost(), path));
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add(SESSION_TOKEN_KEY, sessionToken);
            FHResponse fhres = await FHHttpClient.FHHttpClient.SendAsync(uri, "POST", null, data, timeout);
            return fhres;
        }
    }
}
