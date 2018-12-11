using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Common.Business
{
    public class VRFTPConnection : VRConnectionSettings
    {
        public override Guid ConfigId { get { return s_ConfigId; } }

        public static Guid s_ConfigId = new Guid("661BD539-9F80-4510-A91C-75D7A5543D41");

        public FTPCommunicatorSettings FTPCommunicatorSettings { get; set; }
        public bool TrySendFTP(Stream stream, string fileName, string subDirectory, out string errorMessage)
        {
            return new FTPCommunicator(FTPCommunicatorSettings).TryWriteFile(stream, fileName, subDirectory, out errorMessage);
        }
    }
    public class FTPConnectionFilter : IVRConnectionFilter
    {
        public bool IsMatched(VRConnection vrConnection)
        {
            vrConnection.ThrowIfNull("vrConnection");
            vrConnection.Settings.ThrowIfNull("vrConnection.Settings", vrConnection.VRConnectionId);

            if (vrConnection.Settings.ConfigId != VRFTPConnection.s_ConfigId)
                return false;

            return true;
        }
    }

}