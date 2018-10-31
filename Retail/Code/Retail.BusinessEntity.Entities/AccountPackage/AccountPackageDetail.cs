using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageDetail
    {
        public AccountPackage Entity { get; set; }
        public string AccountName { get; set; }
        public string PackageName { get; set; }
		public string StatusName { get; set; }
		public decimal? ChargeValue { get; set; }
		public string CurrencySymbol { get; set; }
		public string PeriodType { get; set; }
		public string PackageType { get; set; }
	}
}
