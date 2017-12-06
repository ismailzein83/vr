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
    public class CheckModifiedSupplierZoneDetailsInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public BaseQueue<SupplierZoneDetailBatch> InputQueue { get; set; }
        public SupplierZoneDetailByZone SupplierZoneDetailByZone { get; set; }
        public BaseQueue<List<SupplierZoneDetail>> OutputQueue { get; set; }
    }

    public class CheckModifiedSupplierZoneDetailsOutput
    {
        public bool HasModifiedSupplierZoneDetails { get; set; }
    }

    public sealed class CheckModifiedSupplierZoneDetails : DependentAsyncActivity<CheckModifiedSupplierZoneDetailsInput, CheckModifiedSupplierZoneDetailsOutput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<SupplierZoneDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<SupplierZoneDetailByZone> SupplierZoneDetailByZone { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<SupplierZoneDetail>>> OutputQueue { get; set; }

        [RequiredArgument]
        public OutArgument<bool> HasModifiedSupplierZoneDetails { get; set; }

        protected override CheckModifiedSupplierZoneDetailsOutput DoWorkWithResult(CheckModifiedSupplierZoneDetailsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            SupplierZoneDetailByZone supplierZoneDetailByZone = inputArgument.SupplierZoneDetailByZone;

            bool hasModifiedSupplierZoneDetails = false;

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedSupplierZoneDetail) =>
                    {
                        if (preparedSupplierZoneDetail != null && preparedSupplierZoneDetail.SupplierZoneDetails != null)
                        {
                            List<SupplierZoneDetail> supplierZoneDetailsToUpdate = new List<SupplierZoneDetail>();
                            foreach (SupplierZoneDetail supplierZoneDetail in preparedSupplierZoneDetail.SupplierZoneDetails)
                            {
                                SupplierZoneDetail previousSupplierZoneDetail = supplierZoneDetailByZone.GetRecord(supplierZoneDetail.SupplierZoneId);
                                if (previousSupplierZoneDetail != null)
                                {
                                    if (!AreEquals(supplierZoneDetail, previousSupplierZoneDetail))
                                        supplierZoneDetailsToUpdate.Add(supplierZoneDetail);
                                }
                            }

                            if (supplierZoneDetailsToUpdate.Count > 0)
                            {
                                hasModifiedSupplierZoneDetails = true;
                                inputArgument.OutputQueue.Enqueue(supplierZoneDetailsToUpdate);
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            return new CheckModifiedSupplierZoneDetailsOutput() { HasModifiedSupplierZoneDetails = hasModifiedSupplierZoneDetails };
        }

        protected override CheckModifiedSupplierZoneDetailsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CheckModifiedSupplierZoneDetailsInput
            {
                RoutingDatabase = this.RoutingDatabase.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                SupplierZoneDetailByZone = this.SupplierZoneDetailByZone.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckModifiedSupplierZoneDetailsOutput result)
        {
            this.HasModifiedSupplierZoneDetails.Set(context, result.HasModifiedSupplierZoneDetails);
        }

        private bool AreEquals(SupplierZoneDetail firstSupplierZoneDetail, SupplierZoneDetail secondSupplierZoneDetail)
        {
            if (decimal.Round(firstSupplierZoneDetail.EffectiveRateValue, 8) != decimal.Round(secondSupplierZoneDetail.EffectiveRateValue, 8))
                return false;

            if (firstSupplierZoneDetail.SupplierServiceWeight != secondSupplierZoneDetail.SupplierServiceWeight)
                return false;

            if (firstSupplierZoneDetail.SupplierRateId != secondSupplierZoneDetail.SupplierRateId)
                return false;

            DateTime firstSupplierRateEED = firstSupplierZoneDetail.SupplierRateEED.HasValue ? firstSupplierZoneDetail.SupplierRateEED.Value : default(DateTime);
            DateTime secondSupplierRateEED = secondSupplierZoneDetail.SupplierRateEED.HasValue ? secondSupplierZoneDetail.SupplierRateEED.Value : default(DateTime);

            if (firstSupplierRateEED != secondSupplierRateEED)
                return false;

            string serializedFirstSupplierZoneServiceIds = firstSupplierZoneDetail.SupplierServiceIds != null ? string.Join<int>("", firstSupplierZoneDetail.SupplierServiceIds.OrderBy(itm => itm)) : string.Empty;
            string serializedSecondSupplierZoneServiceIds = secondSupplierZoneDetail.SupplierServiceIds != null ? string.Join<int>("", secondSupplierZoneDetail.SupplierServiceIds.OrderBy(itm => itm)) : string.Empty;

            if (string.Compare(serializedFirstSupplierZoneServiceIds, serializedSecondSupplierZoneServiceIds) != 0)
                return false;

            string serializedFirstExactSupplierZoneServiceIds = firstSupplierZoneDetail.ExactSupplierServiceIds != null ? string.Join<int>("", firstSupplierZoneDetail.ExactSupplierServiceIds.OrderBy(itm => itm)) : string.Empty;
            string serializedSecondExactSupplierZoneServiceIds = secondSupplierZoneDetail.ExactSupplierServiceIds != null ? string.Join<int>("", secondSupplierZoneDetail.ExactSupplierServiceIds.OrderBy(itm => itm)) : string.Empty;

            if (string.Compare(serializedFirstExactSupplierZoneServiceIds, serializedSecondExactSupplierZoneServiceIds) != 0)
                return false;

            return true;
        }
    }
}