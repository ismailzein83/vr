﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.BP.Arguments;
using TOne.WhS.SupplierPriceList.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListProcessBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
       public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
       {
           context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
           SupplierPriceListProcessInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<SupplierPriceListProcessInput>("context.IntanceToRun.InputArgument");
           foreach (var startedBPInstance in context.GetStartedBPInstances())
           {
               SupplierPriceListProcessInput startedBPInstanceInputArg = startedBPInstance.InputArgument as SupplierPriceListProcessInput;
               if (startedBPInstanceInputArg != null)
               {
                   if (startedBPInstanceInputArg.SupplierAccountId == inputArg.SupplierAccountId)
                   {
                       context.Reason = "Another process is running for the same supplier";
                       return false;
                   }
               }
           }
           return true;
       }

       public override void OnBPExecutionCompleted(Vanrise.BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
       {
           if (context.BPInstance.Status != BPInstanceStatus.Completed)
           {
               long processInstanceId = context.BPInstance.ProcessInstanceID;
               SupplierPriceListManager SupplierPriceListManager = new SupplierPriceListManager();
               SupplierPriceListManager.CleanTemporaryTables(processInstanceId);
           }
           base.OnBPExecutionCompleted(context);  
       }

    }
}
