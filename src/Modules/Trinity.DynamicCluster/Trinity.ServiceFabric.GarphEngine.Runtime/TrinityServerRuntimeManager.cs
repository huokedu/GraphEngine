﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Utilities;

namespace Trinity.ServiceFabric.GarphEngine.Infrastructure
{
    public class TrinityServerRuntimeManager : TrinitySeverRuntimeMangerBase
    {
        public TrinityServerRuntimeManager(ref (List<Partition> Partitions, 
                                               int PartitionCount, 
                                               int PartitionId, 
                                               int Port, 
                                               int HttpPort, 
                                               string IPAddress, 
                                               ReplicaRole Role, 
                                               StatefulServiceContext Context) runtimeContext) : base(ref runtimeContext)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TrinityErrorCode Start()
        {
            lock (SingletonLockObject)
            {
                //  Initialize Trinity server.
                if (ServiceFabricTrinityServerInstance == null)
                {
                    ServiceFabricTrinityServerInstance = AssemblyUtility.GetAllClassInstances(
                        t => t != typeof(TrinityServer)
                            ? t.GetConstructor(new Type[] { })?.Invoke(new object[] { }) as TrinityServer
                            : null).FirstOrDefault();
                }
                if (ServiceFabricTrinityServerInstance == null)
                {
                    ServiceFabricTrinityServerInstance = new TrinityServer();
                    Log.WriteLine(LogLevel.Warning, "GraphEngineService: using the default communication instance.");
                }
                else
                {
                    Log.WriteLine(LogLevel.Info, "{0}", $"GraphEngineService: using [{ServiceFabricTrinityServerInstance.GetType().Name}] communication instance.");
                }

                ServiceFabricTrinityServerInstance.Start();
                return TrinityErrorCode.E_SUCCESS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override TrinityErrorCode Stop()
        {
            lock (SingletonLockObject)
            {
                ServiceFabricTrinityServerInstance?.Stop();
                ServiceFabricTrinityServerInstance = null;
                return TrinityErrorCode.E_SUCCESS;
            }
        }
    }
}