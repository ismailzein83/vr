using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum WHSFinancialAccountField { FinancialAccountId = 0 }

    public class WHSFinancialAccountPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId
        {
            get { return new Guid("DEFE74EF-2E24-43A7-8420-E8A194AD189C"); } 
        }

        public WHSFinancialAccountField FinancialAccountField { get; set; }

        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            WHSFinancialAccount financialAccount = context.Object as WHSFinancialAccount;

            if (financialAccount == null)
                throw new NullReferenceException("financialAccount");

            switch (this.FinancialAccountField)
            {
                case WHSFinancialAccountField.FinancialAccountId:
                    return financialAccount.FinancialAccountId;
            }

            return null;
        }
    }
}
