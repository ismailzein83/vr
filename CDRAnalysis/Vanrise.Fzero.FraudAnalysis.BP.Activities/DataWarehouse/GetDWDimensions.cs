using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetDWDimensionsInput
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

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }

    #endregion

    public sealed class GetDWDimensions : BaseAsyncActivity<GetDWDimensionsInput>
    {
        #region Arguments

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

        protected override void DoWork(GetDWDimensionsInput inputArgument, AsyncActivityHandle handle)
        {
            inputArgument.CallClasses = GetandSet(inputArgument.CallClasses, "[dbo].[Dim_CallClass]");
            inputArgument.CallTypes = GetandSet(inputArgument.CallTypes, "[dbo].[Dim_CallType]");
            inputArgument.CaseStatuses = GetandSet(inputArgument.CaseStatuses, "[dbo].[Dim_CaseStatus]");
            inputArgument.Filters = GetandSet(inputArgument.Filters, "[dbo].[Dim_Filters]");
            inputArgument.NetworkTypes = GetandSet(inputArgument.NetworkTypes, "[dbo].[Dim_NetworkType]");
            inputArgument.Periods = GetandSet(inputArgument.Periods, "[dbo].[Dim_Period]");
            inputArgument.StrategyKinds = GetandSet(inputArgument.StrategyKinds, "[dbo].[Dim_StrategyKind]");
            inputArgument.SubscriberTypes = GetandSet(inputArgument.SubscriberTypes, "[dbo].[Dim_SubscriberType]");
            inputArgument.SuspicionLevels = GetandSet(inputArgument.SuspicionLevels, "[dbo].[Dim_SuspicionLevel]");
            inputArgument.Users = GetandSet(inputArgument.Users, "[dbo].[Dim_Users]");
        }

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


        protected override GetDWDimensionsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWDimensionsInput
            {

                CallClasses = this.CallClasses.Get(context),
                CallTypes = this.CallTypes.Get(context),
                CaseStatuses = this.CaseStatuses.Get(context),
                Filters = this.Filters.Get(context),
                NetworkTypes = this.NetworkTypes.Get(context),
                Periods = this.Periods.Get(context),
                StrategyKinds = this.StrategyKinds.Get(context),
                SubscriberTypes = this.SubscriberTypes.Get(context),
                SuspicionLevels = this.SuspicionLevels.Get(context),
                Users = this.Users.Get(context),
            };
        }
    }
}
