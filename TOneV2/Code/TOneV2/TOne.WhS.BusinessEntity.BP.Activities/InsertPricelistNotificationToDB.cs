using System;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class InsertPricelistLoggerInput
    {
        public int PricelistId { get; set; }
        public int CustomerId { get; set; }
        public long FileId { get; set; }
    }
    public class InsertPricelistLoggerOutput
    {
    }
    public sealed class InsertPricelistNotificationToDB : BaseAsyncActivity<InsertPricelistLoggerInput, InsertPricelistLoggerOutput>
    {
        [RequiredArgument]
        public InArgument<int> PricelistId { get; set; }
        [RequiredArgument]
        public InArgument<int> CustomerId { get; set; }
        [RequiredArgument]
        public InArgument<long> FileId { get; set; }

        protected override InsertPricelistLoggerOutput DoWorkWithResult(InsertPricelistLoggerInput inputArgument, AsyncActivityHandle handle)
        {
            var emailSenderManager = new SalePricelistNotificationManager();
            emailSenderManager.Insert(inputArgument.CustomerId, inputArgument.PricelistId,inputArgument.FileId);

            return new InsertPricelistLoggerOutput { };
        }

        protected override InsertPricelistLoggerInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new InsertPricelistLoggerInput
            {
                PricelistId = this.PricelistId.Get(context),
                CustomerId = this.CustomerId.Get(context),
                FileId = this.FileId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, InsertPricelistLoggerOutput result)
        {
        }
    }
}
