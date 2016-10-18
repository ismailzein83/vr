using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountBalance.MainExtensions
{
    public class BalanceRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("EBB2BF06-4FD8-4942-8DF4-8892A22AA6FD"); }
        }
    }
}
