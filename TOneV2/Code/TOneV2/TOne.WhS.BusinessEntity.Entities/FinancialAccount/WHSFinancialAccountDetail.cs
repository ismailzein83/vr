using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class WHSFinancialAccountDetail
    {
        public WHSFinancialAccount Entity { get; set; }
        public string AccountTypeDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
