using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using System;
using System.Linq;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class CheckDWDimensionsChangesInput
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

    #endregion

    public class CheckDWDimensionsChanges : DependentAsyncActivity<CheckDWDimensionsChangesInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> CallClasses { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> CallTypes { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> CaseStatuses { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> Filters { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> NetworkTypes { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> Periods { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> StrategyKinds { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> SubscriberTypes { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> SuspicionLevels { get; set; }

        [RequiredArgument]
        public InArgument<DWDimensionDictionary> Users { get; set; }

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


        #endregion


        protected override void DoWork(CheckDWDimensionsChangesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            CallClassManager callClassManager = new CallClassManager();
            IEnumerable<CallClass> listCallClasses = callClassManager.GetClasses();

            foreach (var i in listCallClasses)
            {
                if (!inputArgument.CallClasses.ContainsKey(i.Id))
                {
                    var dwDimension = new DWDimension();
                    dwDimension.Id = i.Id;
                    dwDimension.Description = i.Description;
                    inputArgument.ToBeInsertedCallTypes.Add(dwDimension);
                }

                if (!inputArgument.NetworkTypes.ContainsKey((int)i.NetType))
                {
                    var dwDimension = new DWDimension();
                    dwDimension.Id = i.Id;
                    dwDimension.Description = i.Description;
                    inputArgument.ToBeInsertedNetworkTypes.Add(dwDimension);
                }
            }


            inputArgument.ToBeInsertedCallTypes = GetToBeInserted<CallType>(inputArgument.CallTypes);

            inputArgument.ToBeInsertedCaseStatuses = GetToBeInserted<CaseStatus>(inputArgument.CaseStatuses);

            inputArgument.ToBeInsertedPeriods = GetToBeInserted<PeriodEnum>(inputArgument.Periods);

            inputArgument.ToBeInsertedStrategyKinds = GetToBeInserted<StrategyKindEnum>(inputArgument.StrategyKinds);

            inputArgument.ToBeInsertedSubscriberTypes = GetToBeInserted<SubscriberType>(inputArgument.SubscriberTypes);

            inputArgument.ToBeInsertedSuspicionLevels = GetToBeInserted<SuspicionLevel>(inputArgument.SuspicionLevels);





            FilterManager filterManager = new FilterManager();
            Dictionary<int, FilterDefinition> dictFilters = filterManager.GetCriteriaDefinitions();

            inputArgument.ToBeInsertedFilters = GetToBeInserted(inputArgument.Filters, dictFilters);



            UserManager userManager = new UserManager();
            IEnumerable<UserInfo> listUsers = userManager.GetUsers();

            foreach (var i in listUsers)
            {
                if (!inputArgument.Users.ContainsKey(i.UserId))
                {
                    var dwDimension = new DWDimension();
                    dwDimension.Id = i.UserId;
                    dwDimension.Description = i.Name;
                    inputArgument.ToBeInsertedUsers.Add(dwDimension);
                }
            }



        }

        private static List<DWDimension> GetToBeInserted(DWDimensionDictionary dwDictionary, Dictionary<int, FilterDefinition> dbDictionary)
        {
            List<DWDimension> toBeInsertedList = new List<DWDimension>();
            foreach (var item in dbDictionary)
            {
                if (!dwDictionary.ContainsKey(item.Key))
                {
                    DWDimension dwDimension = new DWDimension();
                    dwDimension.Id = item.Key;
                    dwDimension.Description = item.Value.Description;
                    toBeInsertedList.Add(dwDimension);
                }
            }

            return toBeInsertedList;
        }

        private static List<DWDimension> GetToBeInserted<T>(DWDimensionDictionary dwDictionary) where T : struct
        {
            List<DWDimension> toBeInsertedList = new List<DWDimension>();

            var listEnums = (T[])Enum.GetValues(typeof(T));

            foreach (var i in listEnums)
            {
                T enumValue = (T)i;
                if (!dwDictionary.ContainsKey((int)(object)i))
                {
                    var dwDimension = new DWDimension();

                    dwDimension.Id = (int)(object)i;
                    dwDimension.Description = Vanrise.Common.Utilities.GetEnumDescription(enumValue);
                    toBeInsertedList.Add(dwDimension);
                }
            }
            return toBeInsertedList;
        }

        protected override CheckDWDimensionsChangesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CheckDWDimensionsChangesInput
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
