﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountPackageProvider : AccountPackageProvider, IAccountPackageProvider
    {
        public override Guid ConfigId { get { return new Guid("C8E34E16-B5F9-41B6-BC84-BA5E7F637E95"); } }
        public override Dictionary<AccountEventTime, List<RetailAccountPackage>> GetRetailAccountPackages(IAccountPackageProviderGetRetailAccountPackagesContext context)
        {
            AccountPackageManager accountPackageManager = new AccountPackageManager();
            var retailAccountPackagesByAccountEventTime = new Dictionary<AccountEventTime, List<RetailAccountPackage>>();

            foreach (var accountEventTime in context.AccountEventTimeList)
            {
                accountPackageManager.LoadAccountPackagesByPriority(context.AccountBEDefinitionId, accountEventTime.AccountId, accountEventTime.EventTime, true,
                    (processedAccountPackage, handle) =>
                    {
                        RetailAccountPackage retailAccountPackage = new RetailAccountPackage()
                        {
                            AccountBEDefinitionId = context.AccountBEDefinitionId,
                            AccountId = accountEventTime.AccountId,
                            AccountPackageId = processedAccountPackage.AccountPackage.AccountPackageId,
                            PackageId = processedAccountPackage.Package.PackageId,
                            BED = processedAccountPackage.AccountPackage.BED,
                            EED = processedAccountPackage.AccountPackage.EED
                        };

                        List<RetailAccountPackage> retailAccountPackages = retailAccountPackagesByAccountEventTime.GetOrCreateItem(accountEventTime);
                        retailAccountPackages.Add(retailAccountPackage);
                    });
            }

            return retailAccountPackagesByAccountEventTime;
        }
    }
}