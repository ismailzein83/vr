using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialAccountInfoFilter
    {
    //    public FinancialAccountEffective? FinancialAccountEffective { get; set; }
        public List<long> AccountIds { get; set; }
        public VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
    }
}
