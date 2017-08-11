using System;
using System.Collections.Generic;
using System.Linq;
using System.Activities;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class GetAccountPackages : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> WithFutureAccountPackages { get; set; }

        [RequiredArgument]
        public OutArgument<List<AccountPackage>> AccountPackages { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = context.GetValue(this.EffectiveDate);
            bool withFutureAccountPackages = context.GetValue(this.WithFutureAccountPackages);

            AccountPackageManager accountPackageManager = new AccountPackageManager();
            IEnumerable<AccountPackage> effectiveAssignedPackages = accountPackageManager.GetEffectiveAssignedPackages(effectiveDate, true);

            List<AccountPackage> accountPackages = effectiveAssignedPackages != null ? effectiveAssignedPackages.ToList() : null;
            this.AccountPackages.Set(context, accountPackages);
        }
    }
}
