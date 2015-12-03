﻿namespace FsxApi.Fsx
{
    using System;
    using Config;
    using Config.Enums;
    using Microsoft.FlightSimulator.SimConnect;
    using Model;

    public class FsxConnection
    {
        private readonly SimConnect _simConnect;
        private bool _receivedMessage;
        private PlanePosition _planePosition;

        public FsxConnection()
        {
            Console.WriteLine("FSX: Connecting");

            try
            {
                _simConnect = SimConnectFactory.GetSimConnectObject(this);
            }
            catch(Exception)
            {
                Console.WriteLine("FSX: Connection failure");
                throw;
            }

            Console.WriteLine("FSX: Connection estabilished");
        }

        public PlanePosition GetPlanePosition()
        {
            _simConnect.RequestDataOnSimObjectType(DataRequest.FromBrowser, Definition.Plane, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            
            do
            {
                try
                {
                    _simConnect.ReceiveMessage();
                }
                catch (Exception)
                {
                    return null;
                }
            } while (!_receivedMessage);

            _receivedMessage = false;

            return _planePosition;
        }

        internal void Fsx_ReceiveDataEventHandler(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE fsxData)
        {
            switch ((DataRequest)fsxData.dwRequestID)
            {
                case DataRequest.FromBrowser:
                    var userPlaneData = (PlaneDataStruct)fsxData.dwData[0];

                    _planePosition = new PlanePosition
                    {
                        Latitude = userPlaneData.Location.Latitude,
                        Longitude = userPlaneData.Location.Longitude
                    };

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _receivedMessage = true;
        }

        internal void Fsx_UserClosedFsxEventHandler(SimConnect sender, SIMCONNECT_RECV data)
        {
            _simConnect.Dispose();
            Console.WriteLine("FSX: Connection closed by user");
        }

        internal void Fsx_ExceptionEventHandler(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("FSX: Error during connection");
        }
    }
}
