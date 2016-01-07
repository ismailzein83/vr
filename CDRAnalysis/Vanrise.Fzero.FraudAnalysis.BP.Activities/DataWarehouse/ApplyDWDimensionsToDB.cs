using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    public class ApplyDWDimensionsToDBInput
    {
        public List<DWDimension> ToBeInsertedCallClasses { get; set; }

        public List<DWDimension> ToBeInsertedCallTypes { get; set; }

        public List<DWDimension> ToBeInsertedCaseStatuses { get; set; }

        public List<DWDimension> ToBeInsertedFilters { get; set; }

        public List<DWDimension> ToBeInsertedNetworkTypes { get; set; }

        public List<DWDimension> ToBeInsertedPeriods { get; set; }

        public List<DWDimension> ToBeInsertedStrategyKinds { get; set; }

        public List<DWDimension> ToBeInsertedSubscriberTypes { get; set; }

        public List<DWDimension> ToBeInsertedSuspicionLevels { get; set; }

        public List<DWDimension> ToBeInsertedUsers { get; set; }

    }

    public sealed class ApplyDWDimensionsToDB : DependentAsyncActivity<ApplyDWDimensionsToDBInput>
    {
        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedCallClasses { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedCallTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedCaseStatuses { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedFilters { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedNetworkTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedPeriods { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedStrategyKinds { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedSubscriberTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedSuspicionLevels { get; set; }

        [RequiredArgument]
        public InOutArgument<List<DWDimension>> ToBeInsertedUsers { get; set; }

        protected override void DoWork(ApplyDWDimensionsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IDWDimensionDataManager dataManager = FraudDataManagerFactory.GetDataManager<IDWDimensionDataManager>();

            dataManager.TableName= "[dbo].[Dim_CallClass]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedCallClasses);

            dataManager.TableName = "[dbo].[Dim_CallType]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedCallTypes);

            dataManager.TableName = "[dbo].[Dim_CaseStatus]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedCaseStatuses);

            dataManager.TableName = "[dbo].[Dim_Filters]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedFilters);

            dataManager.TableName = "[dbo].[Dim_NetworkType]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedNetworkTypes);

            dataManager.TableName = "[dbo].[Dim_Period]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedPeriods);

            dataManager.TableName = "[dbo].[Dim_StrategyKind]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedStrategyKinds);

            dataManager.TableName = "[dbo].[Dim_SubscriberType]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedSubscriberTypes);

            dataManager.TableName = "[dbo].[Dim_SuspicionLevel]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedSuspicionLevels);

        }

        protected override ApplyDWDimensionsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyDWDimensionsToDBInput()
            {
                ToBeInsertedCallClasses = this.ToBeInsertedCallClasses.Get(context),

                ToBeInsertedCallTypes = this.ToBeInsertedCallTypes.Get(context),

                ToBeInsertedCaseStatuses = this.ToBeInsertedCaseStatuses.Get(context),

                ToBeInsertedFilters = this.ToBeInsertedFilters.Get(context),

                ToBeInsertedNetworkTypes = this.ToBeInsertedNetworkTypes.Get(context),

                ToBeInsertedPeriods = this.ToBeInsertedPeriods.Get(context),

                ToBeInsertedStrategyKinds = this.ToBeInsertedStrategyKinds.Get(context),

                ToBeInsertedSubscriberTypes = this.ToBeInsertedSubscriberTypes.Get(context),

                ToBeInsertedSuspicionLevels = this.ToBeInsertedSuspicionLevels.Get(context),

                ToBeInsertedUsers = this.ToBeInsertedUsers.Get(context)
            };
        }
    }
}
