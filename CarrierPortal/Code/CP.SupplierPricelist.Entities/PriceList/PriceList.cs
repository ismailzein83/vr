using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class PriceList
    {
        public long PriceListId { get; set; }
        public int UserId { get; set; }
        public long FileId { get; set; }
        public int PriceListType { get; set; }
        public PriceListStatus Status { get; set; }
        public PriceListResult Result { get; set; }
        public string UploadedInformation { get; set; }
        public string PriceListProgress { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime EffectiveOnDate { get; set; }
    }
}
