using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialValidationData
    {
        public IEnumerable<AccountType> FinancialAccountTypes { get; set; }
        public IEnumerable<FinancialAccountData> ProfileFinancialAccounts { get; set; }

        public FinancialCarrierProfile FinancialCarrierProfile { get; set; }
        public FinancialCarrierAccount FinancialCarrierAccount { get; set; }
    }
    public class FinancialCarrierProfile
    {
        public CarrierProfile CarrierProfile { get; set; }
        public IEnumerable<CarrierAccount> ProfileCarrierAccounts { get; set; }
        public Dictionary<int, IEnumerable<FinancialAccountData>> FinancialAccountsByAccount { get; set; }

    }
    public class FinancialAccountData
    {
        public FinancialAccount FinancialAccount { get; set; }
        public bool IsApplicableToCustomer { get; set; }
        public bool IsApplicableToSupplier { get; set; }

    }
    public class FinancialCarrierAccount
    {
        public IEnumerable<FinancialAccountData> FinancialAccounts { get; set; }
        public CarrierAccount CarrierAccount { get; set; }

    }
}
