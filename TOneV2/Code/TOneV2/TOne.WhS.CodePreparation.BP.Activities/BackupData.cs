﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public sealed class BackupData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);

            StateBackupAllCustomers backupAllCustomers = new StateBackupAllCustomers()
            {
                SellingNumberPlanId = sellingNumberPlanId
            };

            StateBackupManager manager = new StateBackupManager();
            manager.BackupData(backupAllCustomers);

        }
    }
}
