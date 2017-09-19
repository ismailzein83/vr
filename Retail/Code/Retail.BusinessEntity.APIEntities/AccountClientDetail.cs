using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.APIEntities
{
    public class AccountClientDetail
    {
        public long AccountId { get; set; }
        public Dictionary<string, DataRecordFieldValue> FieldValues { get; set; }
        public bool HasSubAccounts { get; set; }
    }
}
