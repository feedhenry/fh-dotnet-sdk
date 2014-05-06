using FHSDK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Phone
{
    public class FH : FHBase
    {
        static FH()
        {
            ServiceFinder.SetTargetAssemblyName(typeof(FH).Assembly.GetName());
        }

        public new static void SetLogLevel(int level)
        {
            FHBase.SetLogLevel(level);
        }

        public new static async Task<bool> Init()
        {
            return await FHBase.Init();
        }
    }
}
