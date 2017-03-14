using AeroGear.Push;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    /// <summary>
    /// Push interface to register to Push notification, add categorie or aliases.
    /// </summary>
    public interface IPush
    {
        /// <summary>
        /// Register a event handler to receive push notifications.
        /// </summary>
        /// <param name="handleNotification">The handler to receive the notifications</param>
        /// <returns>Void</returns>
        Task Register(EventHandler<PushReceivedEvent> handleNotification);

        /// <summary>
        /// Set the categories
        /// </summary>
        /// <param name="categories">The categories that can be used by push</param>
        /// <returns></returns>
        Task SetCategories(List<string> categories);

        /// <summary>
        /// Set the alias of this device
        /// </summary>
        /// <param name="alias">The new alias to use</param>
        /// <returns></returns>
        Task SetAlias(string alias);
    }
}
