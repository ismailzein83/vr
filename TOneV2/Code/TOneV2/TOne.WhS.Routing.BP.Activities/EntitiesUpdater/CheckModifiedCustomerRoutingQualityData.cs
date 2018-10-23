using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Queueing;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public class CheckModifiedCustomerRoutingQualityDataInput
    {
        public RoutingDatabase RoutingDatabase { get; set; }
        public BaseQueue<RouteRuleQualityConfigurationDataBatch> InputQueue { get; set; }
        public CustomerRouteQualityDataBySupplierZone CustomerRouteQualityDataBySupplierZone { get; set; }
        public BaseQueue<List<CustomerRouteQualityConfigurationData>> OutputQueueToUpdate { get; set; }
        public BaseQueue<RouteRuleQualityConfigurationDataBatch> OutputQueueToInsert { get; set; }
    }
    public class CheckModifiedCustomerRoutingQualityDataOutput
    {
        public bool HasModifiedRoutingQualityData { get; set; }
    }

    public sealed class CheckModifiedCustomerRoutingQualityData : DependentAsyncActivity<CheckModifiedCustomerRoutingQualityDataInput, CheckModifiedCustomerRoutingQualityDataOutput>
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RouteRuleQualityConfigurationDataBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<CustomerRouteQualityDataBySupplierZone> CustomerRouteQualityDataBySupplierZone { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<List<CustomerRouteQualityConfigurationData>>> OutputQueueToUpdate { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RouteRuleQualityConfigurationDataBatch>> OutputQueueToInsert { get; set; }

        [RequiredArgument]
        public OutArgument<bool> HasModifiedRoutingQualityData { get; set; }

        protected override CheckModifiedCustomerRoutingQualityDataOutput DoWorkWithResult(CheckModifiedCustomerRoutingQualityDataInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var customerRouteQualityDataBySupplierZone = inputArgument.CustomerRouteQualityDataBySupplierZone;
            bool hasModifiedRoutingQualityData = false;
            var triggerThresholdByRouteRuleQualityConfiguration = new ConfigManager().GetTriggerThresholdByRouteRuleQualityConfiguration();

            RouteRuleQualityConfigurationDataBatch dataToInsert = new RouteRuleQualityConfigurationDataBatch();
            dataToInsert.RoutingQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCustomerRouteQualityConfiguration) =>
                    {
                        if (preparedCustomerRouteQualityConfiguration != null && preparedCustomerRouteQualityConfiguration.RoutingQualityConfigurationDataList != null)
                        {
                            var routingQualityConfigurationDataList = preparedCustomerRouteQualityConfiguration.RoutingQualityConfigurationDataList;

                            List<CustomerRouteQualityConfigurationData> dataToUpdate = new List<CustomerRouteQualityConfigurationData>();

                            foreach (CustomerRouteQualityConfigurationData customerRouteQualityConfiguration in routingQualityConfigurationDataList)
                            {
                                var previousCustomerRouteQualityConfiguration = customerRouteQualityDataBySupplierZone.GetRecord(customerRouteQualityConfiguration.SupplierZoneId);
                                if (previousCustomerRouteQualityConfiguration != null)
                                {
                                    var previousCustomerRouteQualityConfigurationData = previousCustomerRouteQualityConfiguration.GetRecord(customerRouteQualityConfiguration.QualityConfigurationId);
                                    if (previousCustomerRouteQualityConfigurationData != null)
                                    {
                                        decimal routingTriggerThreshold = triggerThresholdByRouteRuleQualityConfiguration.GetRecord(customerRouteQualityConfiguration.QualityConfigurationId);
                                        if (Math.Abs(customerRouteQualityConfiguration.QualityData - previousCustomerRouteQualityConfigurationData.QualityData) > routingTriggerThreshold)
                                        {
                                            dataToUpdate.Add(customerRouteQualityConfiguration);
                                        }
                                    }
                                    else
                                    {
                                        dataToInsert.RoutingQualityConfigurationDataList.Add(customerRouteQualityConfiguration);
                                    }
                                }
                                else
                                {
                                    dataToInsert.RoutingQualityConfigurationDataList.Add(customerRouteQualityConfiguration);
                                }
                            }

                            if (dataToUpdate.Count > 0)
                            {
                                hasModifiedRoutingQualityData = true;
                                inputArgument.OutputQueueToUpdate.Enqueue(dataToUpdate);
                            }

                             if (dataToInsert.RoutingQualityConfigurationDataList.Count >= 10000)
                            {
                                hasModifiedRoutingQualityData = true;
                                inputArgument.OutputQueueToInsert.Enqueue(dataToInsert);
                                dataToInsert = new RouteRuleQualityConfigurationDataBatch();
                                dataToInsert.RoutingQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItem);
            });

            if (dataToInsert.RoutingQualityConfigurationDataList.Count > 0)
            {
                hasModifiedRoutingQualityData = true;
                inputArgument.OutputQueueToInsert.Enqueue(dataToInsert);
                dataToInsert = new RouteRuleQualityConfigurationDataBatch();
                dataToInsert.RoutingQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();
            }

            return new CheckModifiedCustomerRoutingQualityDataOutput() { HasModifiedRoutingQualityData = hasModifiedRoutingQualityData };
        }

        protected override CheckModifiedCustomerRoutingQualityDataInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CheckModifiedCustomerRoutingQualityDataInput
            {
                RoutingDatabase = this.RoutingDatabase.Get(context),
                InputQueue = this.InputQueue.Get(context),
                CustomerRouteQualityDataBySupplierZone = this.CustomerRouteQualityDataBySupplierZone.Get(context),
                OutputQueueToUpdate = this.OutputQueueToUpdate.Get(context),
                OutputQueueToInsert = this.OutputQueueToInsert.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckModifiedCustomerRoutingQualityDataOutput result)
        {
            this.HasModifiedRoutingQualityData.Set(context, result.HasModifiedRoutingQualityData);
        }
    }
}
