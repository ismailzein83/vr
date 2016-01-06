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
        public Dictionary<int, DWDimension> CallClasses { get; set; }

        public Dictionary<int, DWDimension> CallTypes { get; set; }

        public Dictionary<int, DWDimension> CaseStatuses { get; set; }

        public Dictionary<int, DWDimension> Filters { get; set; }

        public Dictionary<int, DWDimension> NetworkTypes { get; set; }

        public Dictionary<int, DWDimension> Periods { get; set; }

        public Dictionary<int, DWDimension> StrategyKinds { get; set; }

        public Dictionary<int, DWDimension> SubscriberTypes { get; set; }

        public Dictionary<int, DWDimension> SuspicionLevels { get; set; }

        public Dictionary<int, DWDimension> Users { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }

    #endregion

    public sealed class GetDWDimensions : BaseAsyncActivity<GetDWDimensionsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> CallClasses { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> CallTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> CaseStatuses { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> Filters { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> NetworkTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> Periods { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> StrategyKinds { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> SubscriberTypes { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> SuspicionLevels { get; set; }

        [RequiredArgument]
        public InOutArgument<Dictionary<int, DWDimension>> Users { get; set; }


        #endregion

        protected override void DoWork(GetDWDimensionsInput inputArgument, AsyncActivityHandle handle)
        {

            DWDimensionManager dwDimensionManager = new DWDimensionManager();

            IEnumerable<DWDimension> CallClasses = dwDimensionManager.GetDimensions("[dbo].[Dim_CallClass]");
            inputArgument.CallClasses = CallClasses.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> CallTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_CallType]");
            inputArgument.CallTypes = CallTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> CaseStatuses = dwDimensionManager.GetDimensions("[dbo].[Dim_CaseStatus]");
            inputArgument.CaseStatuses = CaseStatuses.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> Filters = dwDimensionManager.GetDimensions("[dbo].[Dim_Filters]");
            inputArgument.Filters = Filters.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> NetworkTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_NetworkType]");
            inputArgument.NetworkTypes = NetworkTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> Periods = dwDimensionManager.GetDimensions("[dbo].[Dim_Period]");
            inputArgument.Periods = Periods.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> StrategyKinds = dwDimensionManager.GetDimensions("[dbo].[Dim_StrategyKind]");
            inputArgument.StrategyKinds = StrategyKinds.ToDictionary(dim => dim.Id, dim => dim);

            IEnumerable<DWDimension> SubscriberTypes = dwDimensionManager.GetDimensions("[dbo].[Dim_SubscriberType]");
            inputArgument.SubscriberTypes = SubscriberTypes.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> SuspicionLevels = dwDimensionManager.GetDimensions("[dbo].[Dim_SuspicionLevel]");
            inputArgument.SuspicionLevels = SuspicionLevels.ToDictionary(dim => dim.Id, dim => dim);


            IEnumerable<DWDimension> Users = dwDimensionManager.GetDimensions("[dbo].[Dim_Users]");
            inputArgument.Users = Users.ToDictionary(dim => dim.Id, dim => dim);

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
