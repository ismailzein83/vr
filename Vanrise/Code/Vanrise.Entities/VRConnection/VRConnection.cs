using System;

namespace Vanrise.Entities
{
    public class VRConnection
    {
        public Guid VRConnectionId { get; set; }

        public string Name { get; set; }

        public VRConnectionSettings Settings { get; set; }
    }

    public abstract class VRConnectionSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
