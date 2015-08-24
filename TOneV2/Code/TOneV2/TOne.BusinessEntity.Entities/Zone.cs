using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Zone
    {
        public int ZoneId { get; set; }

        public string CodeGroupId { get; set; }

        public string CodeGroupName { get; set; }
        public string SupplierID { get; set; }

        public string Name { get; set; }

        public short ServiceFlag { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
    }
}
