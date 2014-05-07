using FHSDK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Phone
{
    public class FH : FH
    {
        static FH()
        {
            ServiceFinder.SetTargetAssemblyName(typeof(FH).Assembly.GetName());
        }
    }
}
