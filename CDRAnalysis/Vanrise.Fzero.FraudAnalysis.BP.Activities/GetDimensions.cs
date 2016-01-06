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

    public class GetDimensionsInput
    {
        public Dictionary<int, Dimension> CallClasses { get; set; }

        public Dictionary<int, Dimension> CallTypes { get; set; }

        public Dictionary<int, Dimension> CaseStatuses { get; set; }

        public Dictionary<int, Dimension> Filters { get; set; }

        public Dictionary<int, Dimension> NetworkTypes { get; set; }

        public Dictionary<int, Dimension> Periods { get; set; }

        public Dictionary<int, Dimension> StrategyKinds { get; set; }

        public Dictionary<int, Dimension> SubscriberTypes { get; set; }

        public Dictionary<int, Dimension> SuspicionLevels { get; set; }

        public Dictionary<int, Dimension> Users { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }

    #endregion

    public sealed class GetDimensions : BaseAsyncActivity<GetDimensionsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> CallClasses { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> CallTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> CaseStatuses { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> Filters { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> NetworkTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> Periods { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> StrategyKinds { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> SubscriberTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> SuspicionLevels { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, Dimension>> Users { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion

        protected override void DoWork(GetDimensionsInput inputArgument, AsyncActivityHandle handle)
        {

            DWDimensionManager dwDimensionManager = new DWDimensionManager();

            IEnumerable<Dimension> listCallClasses = dwDimensionManager.GetDimensions("[dbo].[Dim_CallClass]");
            inputArgument.CallClasses = listCallClasses.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listCallTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_CallType]");
            inputArgument.CallTypes = listCallTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listCaseStatuses = dwDimensionManager.GetDimensions("[dbo].[Dim_CaseStatus]");
            inputArgument.CaseStatuses = listCaseStatuses.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listFilters = dwDimensionManager.GetDimensions("[dbo].[Dim_Filters]");
            inputArgument.Filters = listFilters.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listNetworkTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_NetworkType]");
            inputArgument.NetworkTypes = listNetworkTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listPeriods = dwDimensionManager.GetDimensions("[dbo].[Dim_Period]");
            inputArgument.Periods = listPeriods.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listStrategyKinds = dwDimensionManager.GetDimensions("[dbo].[Dim_StrategyKind]");
            inputArgument.StrategyKinds = listStrategyKinds.ToDictionary(dim => dim.Id, dim => dim);

            IEnumerable<Dimension> listSubscriberTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_SubscriberType]");
            inputArgument.SubscriberTypes = listSubscriberTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listSuspicionLevels = dwDimensionManager.GetDimensions("[dbo].[Dim_SuspicionLevel]");
            inputArgument.SuspicionLevels = listSuspicionLevels.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<Dimension> listUsers = dwDimensionManager.GetDimensions("[dbo].[Dim_Users]");
            inputArgument.Users = listUsers.ToDictionary(dim => dim.Id, dim => dim);

        }


        protected override GetDimensionsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDimensionsInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
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
