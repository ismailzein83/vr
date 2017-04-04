using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountBESettings : BusinessEntityDefinitionSettings
    {
        public Guid AccountTypeId { get; set; }

        #region BusinessEntityDefinitionSettings

        public override Guid ConfigId
        {
            get { return new Guid("44EC183A-5FC9-447A-BB63-80237ED5F558"); }
        }

        public override string IdType
        {
            get { return "System.Guid"; }
        }

        public override string ManagerFQTN
        {
            get { return "TOne.WhS.AccountBalance.Business.FinancialAccountManager, TOne.WhS.AccountBalance.Business"; }
        }

        public override string SelectorUIControl
        {
            get { return ""; }
        }

        #endregion
    }
}
