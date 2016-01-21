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
using Vanrise.Common;

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



    }
    public class CheckDWDimensionsChangesOutput
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

    #endregion

    public class CheckDWDimensionsChanges : DependentAsyncActivity<CheckDWDimensionsChangesInput, CheckDWDimensionsChangesOutput>
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
                    dwDictionary.Add(dwDimension.Id, dwDimension);
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
                    dwDictionary.Add(dwDimension.Id, dwDimension);
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

            };
        }

        protected override CheckDWDimensionsChangesOutput DoWorkWithResult(CheckDWDimensionsChangesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            List<DWDimension> ToBeInsertedCallClasses = new List<DWDimension>();
            List<DWDimension> ToBeInsertedCallTypes = new List<DWDimension>();
            List<DWDimension> ToBeInsertedCaseStatuses = new List<DWDimension>();
            List<DWDimension> ToBeInsertedFilters = new List<DWDimension>();
            List<DWDimension> ToBeInsertedNetworkTypes = new List<DWDimension>();
            List<DWDimension> ToBeInsertedPeriods = new List<DWDimension>();
            List<DWDimension> ToBeInsertedStrategyKinds = new List<DWDimension>();
            List<DWDimension> ToBeInsertedSubscriberTypes = new List<DWDimension>();
            List<DWDimension> ToBeInsertedSuspicionLevels = new List<DWDimension>();
            List<DWDimension> ToBeInsertedUsers = new List<DWDimension>();


            CallClassManager callClassManager = new CallClassManager();
            IEnumerable<CallClass> listCallClasses = callClassManager.GetClasses();


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing call classes  and network types");
            foreach (var i in listCallClasses)
            {
                if (!inputArgument.CallClasses.ContainsKey(i.Id))
                {
                    var dwDimension = new DWDimension();
                    dwDimension.Id = i.Id;
                    dwDimension.Description = i.Description;
                    ToBeInsertedCallClasses.Add(dwDimension);
                    inputArgument.CallClasses.Add(dwDimension.Id, dwDimension);
                }

                if (!inputArgument.NetworkTypes.ContainsKey((int)i.NetType))
                {
                    var dwDimension = new DWDimension();
                    dwDimension.Id = (int)i.NetType;
                    dwDimension.Description = Vanrise.Common.Utilities.GetEnumDescription<NetType>(i.NetType);
                    ToBeInsertedNetworkTypes.Add(dwDimension);
                    inputArgument.NetworkTypes.Add(dwDimension.Id, dwDimension);
                }
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing call classes  and network types");


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing call types");
            ToBeInsertedCallTypes = GetToBeInserted<CallType>(inputArgument.CallTypes);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing call types");

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing case statuses");
            ToBeInsertedCaseStatuses = GetToBeInserted<CaseStatus>(inputArgument.CaseStatuses);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing case statuses");

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing periods");
            ToBeInsertedPeriods = GetToBeInserted<PeriodEnum>(inputArgument.Periods);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing periods");

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing strategy kinds");
            ToBeInsertedStrategyKinds = GetToBeInserted<StrategyKindEnum>(inputArgument.StrategyKinds);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing strategy kinds");

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing subscriber types");
            ToBeInsertedSubscriberTypes = GetToBeInserted<SubscriberType>(inputArgument.SubscriberTypes);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing subscriber types");

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing suspicion levels");
            ToBeInsertedSuspicionLevels = GetToBeInserted<SuspicionLevel>(inputArgument.SuspicionLevels);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing suspicion levels");


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing filters");
            FilterManager filterManager = new FilterManager();
            Dictionary<int, FilterDefinition> dictFilters = filterManager.GetCriteriaDefinitions();
            ToBeInsertedFilters = GetToBeInserted(inputArgument.Filters, dictFilters);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing filters");


            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing users");
            UserManager userManager = new UserManager();
            IEnumerable<UserInfo> listUsers = userManager.GetUsersInfo();

            foreach (var i in listUsers)
            {
                if (!inputArgument.Users.ContainsKey(i.UserId))
                {
                    var dwDimension = new DWDimension();
                    dwDimension.Id = i.UserId;
                    dwDimension.Description = i.Name;
                    ToBeInsertedUsers.Add(dwDimension);
                    inputArgument.Users.Add(dwDimension.Id, dwDimension);
                }
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing users");

            return new CheckDWDimensionsChangesOutput()
            {
                ToBeInsertedCallClasses = ToBeInsertedCallClasses,
                ToBeInsertedCallTypes = ToBeInsertedCallTypes,
                ToBeInsertedCaseStatuses = ToBeInsertedCaseStatuses,
                ToBeInsertedFilters = ToBeInsertedFilters,
                ToBeInsertedNetworkTypes = ToBeInsertedNetworkTypes,
                ToBeInsertedPeriods = ToBeInsertedPeriods,
                ToBeInsertedStrategyKinds = ToBeInsertedStrategyKinds,
                ToBeInsertedSubscriberTypes = ToBeInsertedSubscriberTypes,
                ToBeInsertedSuspicionLevels = ToBeInsertedSuspicionLevels,
                ToBeInsertedUsers = ToBeInsertedUsers
            };

        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CheckDWDimensionsChangesOutput result)
        {
            this.ToBeInsertedCallClasses.Set(context, result.ToBeInsertedCallClasses);
            this.ToBeInsertedCallTypes.Set(context, result.ToBeInsertedCallTypes);
            this.ToBeInsertedCaseStatuses.Set(context, result.ToBeInsertedCaseStatuses);
            this.ToBeInsertedFilters.Set(context, result.ToBeInsertedFilters);
            this.ToBeInsertedNetworkTypes.Set(context, result.ToBeInsertedNetworkTypes);
            this.ToBeInsertedPeriods.Set(context, result.ToBeInsertedPeriods);
            this.ToBeInsertedStrategyKinds.Set(context, result.ToBeInsertedStrategyKinds);
            this.ToBeInsertedSubscriberTypes.Set(context, result.ToBeInsertedSubscriberTypes);
            this.ToBeInsertedSuspicionLevels.Set(context, result.ToBeInsertedSuspicionLevels);
            this.ToBeInsertedUsers.Set(context, result.ToBeInsertedUsers);
        }
    }
}
