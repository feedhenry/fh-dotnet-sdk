using FHSDK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Phone
{
    public class FHClient : FH
    {
        public new static async Task<bool> Init()
        {
            return await FH.Init();
        }
    }
}
