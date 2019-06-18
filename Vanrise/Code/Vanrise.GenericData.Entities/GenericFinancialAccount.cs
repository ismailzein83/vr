using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericFinancialAccount
    {
        public string FinancialAccountId { get; set; }
        public string Name { get; set; }
        public VRAccountStatus Status { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public int CurrencyId { get; set; }
    }
    public class GenericFinancialAccountConfiguration
    {
        public Guid FinancialAccountBEDefinitionId { get; set; }
        public string FinancialAccountIdFieldName { get; set; }
        public string AccountNameFieldName { get; set; }
        public string StatusIdFieldName { get; set; }
        public string BEDFieldName { get; set; }
        public string EEDFieldName { get; set; }
        public string CurrencyIdFieldName { get; set; }
    }
}
