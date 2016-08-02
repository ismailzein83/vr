﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public class GetRPCodeMatchesInput
    {
        public int RoutingDatabaseId { get; set; }
        public long FromZoneId { get; set; }
        public long ToZoneId { get; set; }
        public BaseQueue<RPCodeMatchesByZone> OutputQueue { get; set; }

    }

    public sealed class GetRPCodeMatches : BaseAsyncActivity<GetRPCodeMatchesInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        [RequiredArgument]
        public InArgument<long> FromZoneId { get; set; }
        [RequiredArgument]
        public InArgument<long> ToZoneId { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<RPCodeMatchesByZone>> OutputQueue { get; set; }

        protected override void DoWork(GetRPCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            var codeMatches = dataManager.GetCodeMatches(inputArgument.FromZoneId, inputArgument.ToZoneId);
            long currentZoneId = 0;
            Dictionary<long, SupplierCodeMatchWithRate> currentSupplierCodeMatchesWithRate = null;
            foreach (var codeMatch in codeMatches.OrderBy(c => c.SaleZoneId))
            {
                if (currentZoneId != codeMatch.SaleZoneId)
                {
                    if (currentSupplierCodeMatchesWithRate != null)
                    {
                        RPCodeMatchesByZone codeMatchByZone = new RPCodeMatchesByZone()
                       {
                           SaleZoneId = currentZoneId,
                           SupplierCodeMatches = currentSupplierCodeMatchesWithRate.Values,
                           SupplierCodeMatchesBySupplier = GetSupplierCodeMatchesBySupplier(currentSupplierCodeMatchesWithRate.Values)
                       };

                        inputArgument.OutputQueue.Enqueue(codeMatchByZone);
                    }
                    currentSupplierCodeMatchesWithRate = new Dictionary<long, SupplierCodeMatchWithRate>();
                    currentZoneId = codeMatch.SaleZoneId;
                }
                foreach (var item in codeMatch.SupplierCodeMatches)
                {
                    if (!currentSupplierCodeMatchesWithRate.ContainsKey(item.CodeMatch.SupplierZoneId))
                        currentSupplierCodeMatchesWithRate.Add(item.CodeMatch.SupplierZoneId, item);
                }
            }
            if (currentSupplierCodeMatchesWithRate != null)
            {
                RPCodeMatchesByZone codeMatchByZone = new RPCodeMatchesByZone()
                {
                    SaleZoneId = currentZoneId,
                    SupplierCodeMatches = currentSupplierCodeMatchesWithRate.Values,
                    SupplierCodeMatchesBySupplier = GetSupplierCodeMatchesBySupplier(currentSupplierCodeMatchesWithRate.Values)
                };

                inputArgument.OutputQueue.Enqueue(codeMatchByZone);
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Getting RP Code Matches is done", null);
        }

        private SupplierCodeMatchesWithRateBySupplier GetSupplierCodeMatchesBySupplier(IEnumerable<SupplierCodeMatchWithRate> supplierCodeMatchesWithRate)
        {
            SupplierCodeMatchesWithRateBySupplier codeMatchBySupplierId = new SupplierCodeMatchesWithRateBySupplier();
            foreach (var supplierCodeMatch in supplierCodeMatchesWithRate)
            {
                List<SupplierCodeMatchWithRate> currentCodeMatches;
                if (!codeMatchBySupplierId.TryGetValue(supplierCodeMatch.CodeMatch.SupplierId, out currentCodeMatches))
                {
                    currentCodeMatches = new List<SupplierCodeMatchWithRate>();
                    codeMatchBySupplierId.Add(supplierCodeMatch.CodeMatch.SupplierId, currentCodeMatches);
                }
                currentCodeMatches.Add(supplierCodeMatch);
            }

            return codeMatchBySupplierId;
        }

        protected override GetRPCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetRPCodeMatchesInput()
            {
                OutputQueue = this.OutputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                FromZoneId = this.FromZoneId.Get(context),
                ToZoneId = this.ToZoneId.Get(context)
            };
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue == null)
                this.OutputQueue.Set(context, new MemoryQueue<RPCodeMatchesByZone>());
            base.OnBeforeExecute(context, handle);
        }

    }
}
