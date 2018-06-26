using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierPriceList
    {
        public int PriceListId { get; set; }

        public int SupplierId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime EffectiveOn { get; set; }

        public SupplierPricelistType? PricelistType { get; set; }

        public long? FileId { get; set; }

        public DateTime CreateTime { get; set; }

        public string SourceId { get; set; }

        public long? ProcessInstanceId { get; set; }

        public long? SPLStateBackupId { get; set; }

        public int UserId { get; set; }
    }
}
