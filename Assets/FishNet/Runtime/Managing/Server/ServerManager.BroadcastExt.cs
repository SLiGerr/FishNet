using FishNet.Broadcast;
using FishNet.Broadcast.Helping;
using FishNet.Connection;
using FishNet.Managing.Logging;
using FishNet.Managing.Utility;
using FishNet.Object;
using FishNet.Serializing;
using FishNet.Serializing.Helping;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FishNet.Managing.Server
{
    public sealed partial class ServerManager : MonoBehaviour
    {
        /// <summary>
        /// CUSTOM: Registers a method to call when a Broadcast arrives.
        /// </summary>
        /// <typeparam name="T">Type of broadcast being registered.</typeparam>
        /// <param name="handler">Method to call.</param>
        /// <param name="requireAuthentication">True if the client must be authenticated for the method to call.</param>
        public void RegisterBroadcast(Type type, object handler, bool requireAuthentication = true) 
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
                var genericType = typeof(ClientBroadcastHandler<>).MakeGenericType(type);
                bhs = (BroadcastHandlerBase)Activator.CreateInstance(genericType, args: new object[] { requireAuthentication });
                _broadcastHandlers.Add(key, bhs);
            }
            //Register handler to IBroadcastHandler.
            bhs.RegisterHandler(handler);
        }

        /// <summary>
        /// CUSTOM: Unregisters a method call from a Broadcast type.
        /// </summary>
        /// <typeparam name="T">Type of broadcast being unregistered.</typeparam>
        /// <param name="handler">Method to unregister.</param>
        public void UnregisterBroadcast(Type type, object handler)
        {
            ushort key = BroadcastExtensions.GetKey(type);
            if (_broadcastHandlers.TryGetValueIL2CPP(key, out BroadcastHandlerBase bhs))
                bhs.UnregisterHandler(handler);
        }
    }
}
