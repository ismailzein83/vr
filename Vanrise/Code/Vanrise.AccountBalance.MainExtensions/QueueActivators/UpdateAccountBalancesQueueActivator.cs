using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.QueueActivators;
using Vanrise.GenericData.Transformation;

namespace Vanrise.AccountBalance.MainExtensions.QueueActivators
{
    public class UpdateAccountBalancesQueueActivator : Vanrise.Queueing.Entities.QueueActivator
    {
        public Guid AccountTypeId { get; set; }
        public string AccountId { get; set; }
        public string EffectiveOn { get; set; }
        public string Amount { get; set; }
        public string CurrencyId { get; set; }
        public Guid TransactionTypeId { get; set; }

        public override void ProcessItem(Queueing.Entities.IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            UpdateUsageBalancePayload balanceUsageDetail = new UpdateUsageBalancePayload();
            balanceUsageDetail.UpdateUsageBalanceItems = new List<UpdateUsageBalanceItem>();
            balanceUsageDetail.TransactionTypeId = this.TransactionTypeId;
            foreach (var record in batchRecords)
            {
                decimal? amount = Vanrise.Common.Utilities.GetPropValueReader(this.Amount).GetPropertyValue(record);

                if (amount.HasValue && amount.Value > 0)
                {
                    UpdateUsageBalanceItem usageBalanceUpdate = new UpdateUsageBalanceItem();
                    usageBalanceUpdate.Value = amount.Value;
                    usageBalanceUpdate.AccountId = Vanrise.Common.Utilities.GetPropValueReader(this.AccountId).GetPropertyValue(record);
                    usageBalanceUpdate.EffectiveOn = Vanrise.Common.Utilities.GetPropValueReader(this.EffectiveOn).GetPropertyValue(record);
                    usageBalanceUpdate.CurrencyId = Vanrise.Common.Utilities.GetPropValueReader(this.CurrencyId).GetPropertyValue(record);

                    balanceUsageDetail.UpdateUsageBalanceItems.Add(usageBalanceUpdate);
                }
            }

            if (balanceUsageDetail.UpdateUsageBalanceItems.Count > 0)
            {
                UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
                usageBalanceManager.UpdateUsageBalance(this.AccountTypeId, balanceUsageDetail);
            }
        }

        public override void ProcessItem(Queueing.Entities.PersistentQueueItem item, Queueing.Entities.ItemsToEnqueue outputItems)
        {
        }

        public override void OnDisposed()
        {
        }
    }
}
