using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetDWDimensionsInput
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }

    public class GetDWDimensionsOutput
    {
        public DWDimensionDictionary CallClasses { get; set; }

        public DWDimensionDictionary CallTypes { get; set; }

        public DWDimensionDictionary CaseStatuses { get; set; }

        public DWDimensionDictionary Filters { get; set; }

        public DWDimensionDictionary NetworkTypes { get; set; }

        public DWDimensionDictionary Periods { get; set; }

        public DWDimensionDictionary StrategyKinds { get; set; }

        public DWDimensionDictionary SubscriberTypes { get; set; }

        public DWDimensionDictionary SuspicionLevels { get; set; }

        public DWDimensionDictionary Users { get; set; }

    }

    #endregion

    public sealed class GetDWDimensions : BaseAsyncActivity<GetDWDimensionsInput, GetDWDimensionsOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> CallClasses { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> CallTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> CaseStatuses { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> Filters { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> NetworkTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> Periods { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> StrategyKinds { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> SubscriberTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> SuspicionLevels { get; set; }

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> Users { get; set; }

        #endregion

        private static DWDimensionDictionary GetandSet(DWDimensionDictionary dwDimensionDictionary, string tableName)
        {
            DWDimensionManager dwDimensionManager = new DWDimensionManager();
            IEnumerable<DWDimension> list = dwDimensionManager.GetDimensions(tableName);
            dwDimensionDictionary = new DWDimensionDictionary();
            if (list.Count() > 0)
                foreach (var i in list)
                    dwDimensionDictionary.Add(i.Id, i);
            return dwDimensionDictionary;
        }

        protected override GetDWDimensionsOutput DoWorkWithResult(GetDWDimensionsInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing dimensions");

            DWDimensionDictionary CallClasses = new DWDimensionDictionary();
            CallClasses = GetandSet(CallClasses, "[dbo].[Dim_CallClass]");

            DWDimensionDictionary CallTypes = new DWDimensionDictionary(); 
            CallTypes = GetandSet(CallTypes, "[dbo].[Dim_CallType]");

            DWDimensionDictionary CaseStatuses = new DWDimensionDictionary();
            CaseStatuses = GetandSet(CaseStatuses, "[dbo].[Dim_CaseStatus]");

            DWDimensionDictionary Filters = new DWDimensionDictionary();
            Filters = GetandSet(Filters, "[dbo].[Dim_Filters]");

            DWDimensionDictionary NetworkTypes = new DWDimensionDictionary();
            NetworkTypes = GetandSet(NetworkTypes, "[dbo].[Dim_NetworkType]");

            DWDimensionDictionary Periods = new DWDimensionDictionary(); 
            Periods = GetandSet(Periods, "[dbo].[Dim_Period]");

            DWDimensionDictionary StrategyKinds = new DWDimensionDictionary(); 
            StrategyKinds = GetandSet(StrategyKinds, "[dbo].[Dim_StrategyKind]");

            DWDimensionDictionary SubscriberTypes = new DWDimensionDictionary();
            SubscriberTypes = GetandSet(SubscriberTypes, "[dbo].[Dim_SubscriberType]");

            DWDimensionDictionary SuspicionLevels = new DWDimensionDictionary(); 
            SuspicionLevels = GetandSet(SuspicionLevels, "[dbo].[Dim_SuspicionLevel]");

            DWDimensionDictionary Users = new DWDimensionDictionary(); 
            Users = GetandSet(Users, "[dbo].[Dim_User]");

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing dimensions");


            return new GetDWDimensionsOutput()
            {
                CallClasses = CallClasses,
                CallTypes = CallTypes,
                CaseStatuses = CaseStatuses,
                Filters = Filters,
                NetworkTypes = NetworkTypes,
                Periods = Periods,
                StrategyKinds = StrategyKinds,
                SubscriberTypes = SubscriberTypes,
                SuspicionLevels = SuspicionLevels,
                Users = Users
            };

        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWDimensionsOutput result)
        {
            this.CallClasses.Set(context, result.CallClasses);
            this.CallTypes.Set(context, result.CallTypes);
            this.CaseStatuses.Set(context, result.CaseStatuses);
            this.Filters.Set(context, result.Filters);
            this.NetworkTypes.Set(context, result.NetworkTypes);
            this.Periods.Set(context, result.Periods);
            this.StrategyKinds.Set(context, result.StrategyKinds);
            this.SubscriberTypes.Set(context, result.SubscriberTypes);
            this.SuspicionLevels.Set(context, result.SuspicionLevels);
            this.Users.Set(context, result.Users);
        }

        protected override GetDWDimensionsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWDimensionsInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
            };
        }
    }
}
