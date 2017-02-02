using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SpecialRequestRuleMigrator : RuleBaseMigrator
    {
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly int _routeRuleTypeId;
        public SpecialRequestRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            RouteRuleManager manager = new RouteRuleManager();
            _routeRuleTypeId = manager.GetRuleTypeId();
        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> rules = new List<SourceRule>();
            SourceSpecialRequestDataManager dataManager = new SourceSpecialRequestDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.OnlyEffective);
            IEnumerable<SourceSpecialRequest> sourceRules = dataManager.GetSpecialRequestRules();

            Dictionary<string, List<SourceSpecialRequest>> groupedSourceRules = GroupSpecialRequests(sourceRules);
            foreach (var groupedRule in groupedSourceRules.Values)
            {

                var outputRules = StructureSpecialRequestRulesByDate(groupedRule);
                foreach (var outputRule in outputRules)
                {
                    SourceRule rule = GetSourceRule(outputRule);
                    rules.Add(rule);
                }
            }
            return rules;
        }

        private SourceRule GetSourceRule(SpecialRequestOutput outputRule)
        {
            RouteRule routeRule = GetRouteRuleDetails(outputRule.SpecialRequests, outputRule.BED, outputRule.EED);
            SourceRule rule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = outputRule.BED,
                    EED = outputRule.EED,
                    RuleDetails = Serializer.Serialize(routeRule),
                    TypeId = _routeRuleTypeId
                }
            };
            return rule;
        }

        #region Private Methods
        static List<SpecialRequestOutput> StructureSpecialRequestRulesByDate(List<SourceSpecialRequest> specialRequestRules)
        {
            List<SpecialRequestOutput> outputRules = new List<SpecialRequestOutput>();
            HashSet<DateTime> distinctDateTimes = new HashSet<DateTime>();
            foreach (var specialRequest in specialRequestRules)
            {
                distinctDateTimes.Add(specialRequest.BED);
                if (specialRequest.EED.HasValue)
                    distinctDateTimes.Add(specialRequest.EED.Value);
            }
            List<DateTime> intervalDates = distinctDateTimes.ToList();

            for (var index = 0; index < intervalDates.Count; index++)
            {
                bool isLastItem = index == intervalDates.Count - 1;
                DateTime bed = intervalDates[index];
                DateTime? eed = !isLastItem ? intervalDates[index + 1] : (DateTime?)null;

                Func<SourceSpecialRequest, bool> predicate = (itm) =>
                {
                    if (!isLastItem && itm.BED > bed)
                        return false;

                    if (itm.EED.HasValue && itm.EED.Value <= bed)
                        return false;

                    return true;
                };

                IEnumerable<SourceSpecialRequest> matchingSpecialRequestRules = specialRequestRules.FindAllRecords(predicate);
                outputRules.Add(new SpecialRequestOutput
                {
                    SpecialRequests = matchingSpecialRequestRules,
                    BED = bed,
                    EED = eed
                });
            }

            return outputRules;
        }
        RouteRule GetRouteRuleDetails(IEnumerable<SourceSpecialRequest> specialRequestRules, DateTime bed, DateTime? eed)
        {
            CarrierAccount customer;
            SourceSpecialRequest defaultRule = specialRequestRules.FirstOrDefault();

            if (!_allCarrierAccounts.TryGetValue(defaultRule.CustomerId, out customer))
                throw new NullReferenceException(string.Format("customer not found. Customer Source Id {0}.", defaultRule.CustomerId));
            RouteRule routeRule = new RouteRule
              {
                  BeginEffectiveTime = bed,
                  EndEffectiveTime = eed,
                  Description = defaultRule.Reason,
                  Name = string.Format("Migrated Special Request Rule {0}", Context.Counter++),
                  Criteria = new RouteRuleCriteria
                  {
                      CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup
                      {
                          Codes = GetCodeCriteria(defaultRule)
                      },
                      CustomerGroupSettings = new SelectiveCustomerGroup
                    {
                        CustomerIds = new List<int>() { customer.CarrierAccountId },
                    },
                      ExcludedCodes = defaultRule.ExcludedCodesList == null ? null : defaultRule.ExcludedCodesList.ToList()
                  },
                  Settings = new LCRRouteRule
                  {
                      Options = GetSupplierOptions(specialRequestRules.Select(s => s.SupplierOption))
                  }
              };

            return routeRule;
        }
        Dictionary<int, LCRRouteOptionSettings> GetSupplierOptions(IEnumerable<SpecialRequestSupplierOption> suppliers)
        {
            Dictionary<int, LCRRouteOptionSettings> options = new Dictionary<int, LCRRouteOptionSettings>();
            int position = 0;

            foreach (var option in suppliers.OrderByDescending(s => s.Priority).ThenBy(itm => itm.SupplierId))
            {
                CarrierAccount supplier;
                if (!_allCarrierAccounts.TryGetValue(option.SupplierId, out supplier))
                    throw new NullReferenceException(string.Format("supplier not found. Supplier Source Id {0}.", option.SupplierId));
                LCRRouteOptionSettings specialRequestOptionSettings = new LCRRouteOptionSettings
                {
                    ForceOption = option.ForcedOption,
                    NumberOfTries = option.NumberOfTries,
                    Percentage = option.Percentage == 0 ? (decimal?)null : option.Percentage,
                    Position = ++position,
                    SupplierId = supplier.CarrierAccountId
                };
                options.Add(supplier.CarrierAccountId, specialRequestOptionSettings);
            }
            return options;
        }
        List<CodeCriteria> GetCodeCriteria(SourceSpecialRequest groupedRule)
        {
            return new List<CodeCriteria> { 
             new CodeCriteria{
              Code = groupedRule.Code,
               WithSubCodes = groupedRule.IncludeSubCode
             }
            };
        }
        Dictionary<string, List<SourceSpecialRequest>> GroupSpecialRequests(IEnumerable<SourceSpecialRequest> sourceRules)
        {
            Dictionary<string, List<SourceSpecialRequest>> groupedRules = new Dictionary<string, List<SourceSpecialRequest>>();

            foreach (var specialRequest in sourceRules)
            {
                List<SourceSpecialRequest> rules;
                string key = GetKey(specialRequest);
                if (!groupedRules.TryGetValue(key, out rules))
                {
                    rules = new List<SourceSpecialRequest>();
                    groupedRules.Add(key, rules);
                }
                rules.Add(specialRequest);
            }
            return groupedRules;
        }
        string GetKey(SourceSpecialRequest specialRequest)
        {
            return string.Format("{0}-{1}-{2}-{3}", specialRequest.CustomerId, specialRequest.Code, specialRequest.IncludeSubCode, specialRequest.ExcludedCodesList == null ? "" : specialRequest.ExcludedCodesList.Aggregate((i, j) => i + j));
        }

        #endregion
    }

    class SpecialRequestOutput
    {
        public IEnumerable<SourceSpecialRequest> SpecialRequests { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

    }
}
