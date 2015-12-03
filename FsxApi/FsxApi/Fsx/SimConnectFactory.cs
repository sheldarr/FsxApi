﻿namespace FsxApi.Fsx
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Config;
    using Config.Enums;
    using Microsoft.FlightSimulator.SimConnect;

    public static class SimConnectFactory
    {
        private const int WmUserSimconnect = 0x0402;

        public static SimConnect GetSimConnectObject(FsxConnection connection)
        {
            var mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            var simconnect = new SimConnect("User Requests", mainWindowHandle, WmUserSimconnect, null, 0);

            InitializeSimConnect(simconnect, connection);

            return simconnect;
        }

        private static void InitializeSimConnect(SimConnect simconnect, FsxConnection fscConnection)
        {
            try
            {
                simconnect.OnRecvQuit += fscConnection.Fsx_UserClosedFsxEventHandler;

                simconnect.OnRecvException += fscConnection.Fsx_ExceptionEventHandler;

                simconnect.AddToDataDefinition(Definition.Plane, "Plane Latitude", "degrees", 
                    SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(Definition.Plane, "Plane Longitude", "degrees", 
                    SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                simconnect.RegisterDataDefineStruct<PlaneDataStruct>(Definition.Plane);

                simconnect.OnRecvSimobjectDataBytype += fscConnection.Fsx_ReceiveDataEventHandler;
            }
            catch (COMException)
            {
            }
        } 
    }
}
