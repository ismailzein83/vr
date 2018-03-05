using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Entities
{
    public class SaleCodeZone : ICode
    {
        public string Code { get; set; }

        public long ZoneId { get; set; }
    }
}
