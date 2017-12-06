using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Entities;
using System.Linq;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CheckModifiedCustomerZoneDetailsInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public BaseQueue<CustomerZoneDetailBatch> InputQueue { get; set; }
        public CustomerZoneDetailByZone CustomerZoneDetailByZone { get; set; }
        public BaseQueue<List<CustomerZoneDetail>> OutputQueue { get; set; }
    }

    public class CheckModifiedCustomerZoneDetailsOutput
    {
        public bool HasModifiedCustomerZoneDetails { get; set; }
    }

    public sealed class CheckModifiedCustomerZoneDetails : DependentAsyncActivity<CheckModifiedCustomerZoneDetailsInput, CheckModifiedCustomerZoneDetailsOutput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CustomerZoneDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<CustomerZoneDetailByZone> CustomerZoneDetailByZone { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerZoneDetail>>> OutputQueue { get; set; }

        [RequiredArgument]
        public OutArgument<bool> HasModifiedCustomerZoneDetails { get; set; }

        protected override CheckModifiedCustomerZoneDetailsOutput DoWorkWithResult(CheckModifiedCustomerZoneDetailsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Dictionary<CustomerSaleZone, CustomerZoneDetail> customerZoneDetailByCustomerSaleZone = new Dictionary<CustomerSaleZone, CustomerZoneDetail>();
            foreach (var customerZoneDetailByZoneItem in inputArgument.CustomerZoneDetailByZone)
            {
                foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetailByZoneItem.Value)
                {
                    customerZoneDetailByCustomerSaleZone.Add(new CustomerSaleZone() { CustomerId = customerZoneDetail.CustomerId, SaleZoneId = customerZoneDetail.SaleZoneId }, customerZoneDetail);
                }
            }

            bool hasModifiedCustomerZoneDetails = false;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCustomerZoneDetail) =>
                    {
                        if (preparedCustomerZoneDetail != null && preparedCustomerZoneDetail.CustomerZoneDetails != null)
                        {
                            List<CustomerZoneDetail> customerZoneDetailsToUpdate = new List<CustomerZoneDetail>();
                            foreach (CustomerZoneDetail customerZoneDetail in preparedCustomerZoneDetail.CustomerZoneDetails)
                            {
                                CustomerZoneDetail previousCustomerZoneDetail = customerZoneDetailByCustomerSaleZone.GetRecord(new CustomerSaleZone() { CustomerId = customerZoneDetail.CustomerId, SaleZoneId = customerZoneDetail.SaleZoneId });
                                if (previousCustomerZoneDetail != null)
                                {
                                    if (!AreEquals(customerZoneDetail, previousCustomerZoneDetail))
                                        customerZoneDetailsToUpdate.Add(customerZoneDetail);
                                }
                            }

                            if (customerZoneDetailsToUpdate.Count > 0)
                            {
                                hasModifiedCustomerZoneDetails = true;
                                inputArgument.OutputQueue.Enqueue(customerZoneDetailsToUpdate);
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            return new CheckModifiedCustomerZoneDetailsOutput() { HasModifiedCustomerZoneDetails = hasModifiedCustomerZoneDetails };
        }

        protected override CheckModifiedCustomerZoneDetailsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CheckModifiedCustomerZoneDetailsInput
            {
                RoutingDatabase = this.RoutingDatabase.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                CustomerZoneDetailByZone = this.CustomerZoneDetailByZone.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckModifiedCustomerZoneDetailsOutput result)
        {
            this.HasModifiedCustomerZoneDetails.Set(context, result.HasModifiedCustomerZoneDetails);
        }

        private bool AreEquals(CustomerZoneDetail firstCustomerZoneDetail, CustomerZoneDetail secondCustomerZoneDetail)
        {
            if (firstCustomerZoneDetail.RoutingProductId != secondCustomerZoneDetail.RoutingProductId)
                return false;

            if (firstCustomerZoneDetail.RoutingProductSource != secondCustomerZoneDetail.RoutingProductSource)
                return false;

            if (firstCustomerZoneDetail.SellingProductId != secondCustomerZoneDetail.SellingProductId)
                return false;

            if (decimal.Round(firstCustomerZoneDetail.EffectiveRateValue, 8) != decimal.Round(secondCustomerZoneDetail.EffectiveRateValue, 8))
                return false;

            if (firstCustomerZoneDetail.RateSource != secondCustomerZoneDetail.RateSource)
                return false;

            string serializedFirstSaleZoneServiceIds = firstCustomerZoneDetail.SaleZoneServiceIds != null ? string.Join<int>("", firstCustomerZoneDetail.SaleZoneServiceIds.OrderBy(itm => itm)) : string.Empty;
            string serializedSecondSaleZoneServiceIds = secondCustomerZoneDetail.SaleZoneServiceIds != null ? string.Join<int>("", secondCustomerZoneDetail.SaleZoneServiceIds.OrderBy(itm => itm)) : string.Empty;

            if (string.Compare(serializedFirstSaleZoneServiceIds, serializedSecondSaleZoneServiceIds) != 0)
                return false;

            return true;
        }
    }
}