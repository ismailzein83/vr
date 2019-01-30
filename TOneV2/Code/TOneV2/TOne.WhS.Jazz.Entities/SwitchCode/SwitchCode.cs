using System;
using System.Collections.Generic;
using System.Text;


namespace TOne.WhS.Jazz.Entities
{
    public class SwitchCode
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int SwitchId { get; set; }
        public string Code { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
}