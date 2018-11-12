using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageQuery
    {
        public Guid? AccountBEDefinitionId { get; set; }
        public long? AssignedToAccountId { get; set; }
		public int? PackageId { get; set; }
		public string PackageName { get; set; }
		public List<Guid> PackageTypes { get; set; }
		public List<long> AccountIds { get; set; }
		public List<Guid> StatusIds { get; set; }
		public List<int> CurrencyIds { get; set; }
		public DateTime? BED { get; set; }
		public DateTime? EED { get; set; }





	}
}
