using AeroGear.Push;
using System;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    public interface IPush
    {
        /// <summary>
        /// Register a event handler to receive push notifications.
        /// </summary>
        /// <param name="HandleNotification">The handler to receive the notifications</param>
        /// <returns>Void</returns>
        Task Register(EventHandler<PushReceivedEvent> HandleNotification);
    }
}
