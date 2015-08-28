using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class SupplierTODConsiderationInfo : BaseTODConsiderationInfo
    {
        public string UserName { get; set; }

        public string CustomerName { get; set; }

        public string WeekDayName { get; set; }
    }
}
