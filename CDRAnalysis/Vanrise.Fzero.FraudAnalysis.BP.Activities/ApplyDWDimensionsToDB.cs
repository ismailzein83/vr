using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

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
            if (inputArgument.ToBeInsertedCallClasses.Count > 0)
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} call classe(s) ", inputArgument.ToBeInsertedCallClasses.Count);

            dataManager.TableName = "[dbo].[Dim_CallType]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedCallTypes);
            if (inputArgument.ToBeInsertedCallTypes.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} call type(s) ", inputArgument.ToBeInsertedCallTypes.Count);

            dataManager.TableName = "[dbo].[Dim_CaseStatus]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedCaseStatuses);
            if (inputArgument.ToBeInsertedCaseStatuses.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} case statuse(s) ", inputArgument.ToBeInsertedCaseStatuses.Count);

            dataManager.TableName = "[dbo].[Dim_Filters]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedFilters);
            if (inputArgument.ToBeInsertedFilters.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} filter(s) ", inputArgument.ToBeInsertedFilters.Count);

            dataManager.TableName = "[dbo].[Dim_NetworkType]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedNetworkTypes);
            if (inputArgument.ToBeInsertedNetworkTypes.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} network type(s) ", inputArgument.ToBeInsertedNetworkTypes.Count);

            dataManager.TableName = "[dbo].[Dim_Period]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedPeriods);
            if (inputArgument.ToBeInsertedPeriods.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} period(s) ", inputArgument.ToBeInsertedPeriods.Count);

            dataManager.TableName = "[dbo].[Dim_StrategyKind]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedStrategyKinds);
            if (inputArgument.ToBeInsertedStrategyKinds.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} strategy kind(s) ", inputArgument.ToBeInsertedStrategyKinds.Count);

            dataManager.TableName = "[dbo].[Dim_SubscriberType]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedSubscriberTypes);
            if (inputArgument.ToBeInsertedSubscriberTypes.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} subscriber type(s) ", inputArgument.ToBeInsertedSubscriberTypes.Count);

            dataManager.TableName = "[dbo].[Dim_SuspicionLevel]"; 
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedSuspicionLevels);
            if (inputArgument.ToBeInsertedSuspicionLevels.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} suspicion level(s) ", inputArgument.ToBeInsertedSuspicionLevels.Count);

            dataManager.TableName = "[dbo].[Dim_User]";
            dataManager.SaveDWDimensionsToDB(inputArgument.ToBeInsertedUsers);
            if (inputArgument.ToBeInsertedUsers.Count > 0)
                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished adding {0} user(s) ", inputArgument.ToBeInsertedUsers.Count);

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
