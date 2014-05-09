using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FHSDK.Adaptation
{
    /// <summary>
    /// An interface to resolve the correct implmentation of a type
    /// </summary>
    internal interface IAdapterResolver
    {
        object Resolve(Type type);
    }
}