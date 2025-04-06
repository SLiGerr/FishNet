using FishNet.Broadcast.Helping;
using GameKit.Dependencies.Utilities;
using System;
using UnityEngine;

namespace FishNet.Managing.Client
{
    public sealed partial class ClientManager : MonoBehaviour
    {
        /// <summary>
        /// CUSTOM: Registers a method to call when a Broadcast arrives.
        /// </summary>
        /// <typeparam name="T">Type of broadcast being registered.</typeparam>
        /// <param name="handler">Method to call.</param>
        public void RegisterBroadcast(Type type, object handler)
        {
            if (handler == null)
            {
                NetworkManager.LogError($"Broadcast cannot be registered because handler is null. This may occur when trying to register to objects which require initialization, such as events.");
                return;
            }

            ushort key = BroadcastExtensions.GetKey(type);
            //Create new IBroadcastHandler if needed.
            BroadcastHandlerBase bhs;
            if (!_broadcastHandlers.TryGetValueIL2CPP(key, out bhs))
            {
                var genericType = typeof(ServerBroadcastHandler<>).MakeGenericType(type);
                bhs = (BroadcastHandlerBase)Activator.CreateInstance(genericType);
                _broadcastHandlers.Add(key, bhs);
            }
            //Register handler to IBroadcastHandler.
            bhs.RegisterHandler(handler);
        }

        /// <summary>
        /// CUSTOM: Unregisters a method call from a Broadcast type.
        /// </summary>
        /// <param name="Type">Type of broadcast being unregistered.</param>
        /// <param name="handler">Method to unregister. (must )</param>
        public void UnregisterBroadcast(Type type, object handler)
        {
            ushort key = BroadcastExtensions.GetKey(type);
            if (_broadcastHandlers.TryGetValueIL2CPP(key, out BroadcastHandlerBase bhs))
                bhs.UnregisterHandler(handler);
        }
    }
}
