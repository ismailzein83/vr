using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum ZoneChangeType {

        [Description("New")]
        New = 0,

        [Description("Closed")]
        Closed = 1,

        [Description("Deleted")]
        Deleted = 2
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
