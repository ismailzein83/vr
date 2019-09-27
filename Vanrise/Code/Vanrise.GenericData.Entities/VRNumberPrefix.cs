using System;

namespace Vanrise.GenericData.Entities
{
    public class VRNumberPrefix
    {
        public int ID { get; set; }

        public string Number { get; set; }

        public Guid Type { get; set; }

        public bool IsExact { get; set; }
    }
}