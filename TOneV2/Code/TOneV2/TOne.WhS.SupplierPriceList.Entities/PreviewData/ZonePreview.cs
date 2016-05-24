using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum ZoneChangeType
    {
        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Closed")]
        Closed = 2,

        [Description("Deleted")]
        Deleted = 3,

        [Description("Renamed")]
        Renamed = 4
    };


    public class ZonePreview
    {
        public string Name { get; set; }

        public ZoneChangeType ChangeType { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ZonePreviewDetail
    {
        public ZonePreview Entity { get; set; }

        public string ChangeTypeDecription { get; set; }
    }
}
