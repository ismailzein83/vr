using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleAreaSettingsData : SettingData
    {
        public decimal DefaultRate { get; set; }
        public IEnumerable<string> FixedKeywords { get; set; }
        public IEnumerable<string> MobileKeywords { get; set; }
        public PrimarySaleEntity PrimarySaleEntity { get; set; }
        public Guid DefaultSalePLMailTemplateId { get; set; }
		public int DefaultSalePLTemplateId { get; set; }
    }

    public enum PrimarySaleEntity { SellingProduct = 0, Customer = 1 }
}
