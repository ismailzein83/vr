using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities.RatePlanning
{
    public class ExcludedItem
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Reason { get; set; }
        public int ParentId { get; set; }
        public long ProcessInstanceId { get; set; }
    }
    public enum ItemTypeEnum
    {
        Country = 0,
        Zone = 1
    }
}
