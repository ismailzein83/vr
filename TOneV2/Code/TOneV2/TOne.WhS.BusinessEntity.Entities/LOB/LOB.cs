using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class LOB
    {
        public static Guid DefaultLOBId = new Guid("2E48F875-2663-4158-9FD1-A276507EF99E");
        public Guid LOBId { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
