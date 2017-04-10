using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions
{
    public enum FinancialAccountField { FinancialAccountId = 0 }

    public class FinancialAccountPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("DEFE74EF-2E24-43A7-8420-E8A194AD189C"); } 
        }

        public FinancialAccountField FinancialAccountField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            FinancialAccount financialAccount = context.Object as FinancialAccount;

            if (financialAccount == null)
                throw new NullReferenceException("financialAccount");

            switch (this.FinancialAccountField)
            {
                case FinancialAccountField.FinancialAccountId:
                    return financialAccount.FinancialAccountId;
            }

            return null;
        }
    }
}
