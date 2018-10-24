using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ExcludedItem
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public ExcludedItemTypeEnum ItemType { get; set; }
        public string Reason { get; set; }
        public int? ParentId { get; set; }
        public long ProcessInstanceId { get; set; }
    }
    public class ExcludedItemDetail
    {
         public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemTypeDescription { get; set; }
        public string Reason { get; set; }
        public int? ParentId { get; set; }
        public long ProcessInstanceId { get; set; }
        public ExcludedItem Entity { get; set; }
    }

    public class ExcludedItemsQuery
    {
           public long ProcessInstanceId { get; set; }
    }
      public enum ExcludedItemTypeEnum
    {
        [Description("Country")]
        Country = 0,
        [Description("Zone")]
        Zone = 1,
    }


}
  

