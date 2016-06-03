using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountDetail
    {
        public Account Entity { get; set; }
        public int DirectSubAccountCount { get; set; }
        public int InDirectSubAccountCount { get; set; }
    }
}
