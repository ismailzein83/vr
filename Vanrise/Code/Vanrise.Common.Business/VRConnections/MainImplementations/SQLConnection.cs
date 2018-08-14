using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Vanrise.Common.Business
{
    public class SQLConnection : VRConnectionSettings
    {
        public override Guid ConfigId { get { return s_ConfigId; } }

        public static Guid s_ConfigId = new Guid("8224B27C-C128-4150-A4E4-5E2034BB3A36");

        public string ConnectionString { get; set; }
    }

    public class SQLConnectionFilter : IVRConnectionFilter
    {
        public bool IsMatched(VRConnection vrConnection)
        {
            vrConnection.ThrowIfNull("vrConnection");
            vrConnection.Settings.ThrowIfNull("vrConnection.Settings", vrConnection.VRConnectionId);

            if (vrConnection.Settings.ConfigId != SQLConnection.s_ConfigId)
                return false;

            return true;
        }
    }
}