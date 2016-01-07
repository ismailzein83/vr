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

            DWDimensionManager dwDimensionManager = new DWDimensionManager();

            IEnumerable<DWDimension> CallClasses = dwDimensionManager.GetDimensions("[dbo].[Dim_CallClass]");
            if (CallClasses.Count() > 0)
                inputArgument.CallClasses = (DWDimensionDictionary)CallClasses.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> CallTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_CallType]");
            if (CallTypes.Count() > 0)
                inputArgument.CallTypes = (DWDimensionDictionary)CallTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> CaseStatuses = dwDimensionManager.GetDimensions("[dbo].[Dim_CaseStatus]");
            if (CaseStatuses.Count() > 0)
                inputArgument.CaseStatuses = (DWDimensionDictionary)CaseStatuses.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> Filters = dwDimensionManager.GetDimensions("[dbo].[Dim_Filters]");
            if (Filters.Count() > 0)
                inputArgument.Filters = (DWDimensionDictionary)Filters.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> NetworkTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_NetworkType]");
            if (NetworkTypes.Count() > 0)
                inputArgument.NetworkTypes = (DWDimensionDictionary)NetworkTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> Periods = dwDimensionManager.GetDimensions("[dbo].[Dim_Period]");
            if (Periods.Count() > 0)
                inputArgument.Periods = (DWDimensionDictionary)Periods.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> StrategyKinds = dwDimensionManager.GetDimensions("[dbo].[Dim_StrategyKind]");
            if (StrategyKinds.Count() > 0)
                inputArgument.StrategyKinds = (DWDimensionDictionary)StrategyKinds.ToDictionary(dim => dim.Id, dim => dim);

            IEnumerable<DWDimension> SubscriberTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_SubscriberType]");
            if (SubscriberTypes.Count() > 0)
                inputArgument.SubscriberTypes = (DWDimensionDictionary)SubscriberTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> SuspicionLevels = dwDimensionManager.GetDimensions("[dbo].[Dim_SuspicionLevel]");
            if (SuspicionLevels.Count() > 0)
                inputArgument.SuspicionLevels = (DWDimensionDictionary)SuspicionLevels.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> Users = dwDimensionManager.GetDimensions("[dbo].[Dim_Users]");
            if (Users.Count() > 0)
                inputArgument.Users = (DWDimensionDictionary)Users.ToDictionary(dim => dim.Id, dim => dim);

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
