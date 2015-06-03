using AeroGear.Push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    public interface IPush
    {
        Task Register(EventHandler<PushReceivedEvent> HandleNotification);
    }
}
