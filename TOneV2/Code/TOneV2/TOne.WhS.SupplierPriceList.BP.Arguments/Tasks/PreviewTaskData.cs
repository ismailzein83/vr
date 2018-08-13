using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Arguments.Tasks
{
    public class PreviewTaskData : BPTaskData
    {
        public long FileId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime PricelistDate { get; set; }
        public SupplierPricelistType SupplierPricelistType { get; set; }
        public int SupplierId { get; set; }
    }
}
