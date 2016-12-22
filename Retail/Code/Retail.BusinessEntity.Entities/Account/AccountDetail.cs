using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountDetail
    {
        public Account Entity { get; set; }
        public string AccountTypeTitle { get; set; }
        public int DirectSubAccountCount { get; set; }
        public int TotalSubAccountCount { get; set; }
        public bool CanAddSubAccounts { get; set; }
        public IEnumerable<ActionDefinitionInfo>  ActionDefinitions { get; set; }
        public string  StatusDesciption { get; set; }
        public StyleFormatingSettings Style { get; set; }
        public int NumberOfServices { get; set; }
        public int NumberOfPackages { get; set; }

        public Dictionary<string, AccountFieldValue> FieldValues { get; set; }
        public List<Guid> AvailableAccountViews { get; set; }
    }

    public class AccountFieldValue
    {
        public Object Value { get; set; }

        public string Description { get; set; }
    }
}
        