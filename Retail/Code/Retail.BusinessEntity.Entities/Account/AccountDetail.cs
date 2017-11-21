using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountDetail
    {
        public long AccountId { get; set; } 
        public string AccountTypeTitle { get; set; }
        public int DirectSubAccountCount { get; set; }
        public int TotalSubAccountCount { get; set; }
        public bool CanAddSubAccounts { get; set; }
        public string  StatusDesciption { get; set; }
        public StyleFormatingSettings Style { get; set; }

        public Dictionary<string, DataRecordFieldValue> FieldValues { get; set; }
        public List<Guid> AvailableAccountViews { get; set; }
        public List<Guid> AvailableAccountActions { get; set; }
    }
}
        