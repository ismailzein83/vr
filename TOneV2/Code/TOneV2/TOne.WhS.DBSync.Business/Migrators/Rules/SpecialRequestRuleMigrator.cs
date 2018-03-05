using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.MainExtensions.Country;
using Vanrise.Entities;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SpecialRequestRuleMigrator : RuleBaseMigrator
    {
        readonly int _routeRuleTypeId;
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;

        public override string EntityName { get { return "Special Request"; } }

        public SpecialRequestRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _routeRuleTypeId = new RouteRuleManager().GetRuleTypeId();
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;
        }

        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> rules = new List<SourceRule>();
            SourceSpecialRequestDataManager dataManager = new SourceSpecialRequestDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            IEnumerable<SourceSpecialRequest> sourceRules = dataManager.GetSpecialRequestRules();

            Dictionary<string, long> codeSaleZoneMappings = this.GetCodeSaleZoneMappings();
            Dictionary<string, CodeGroup> codeGroups = new CodeGroupDBSyncDataManager(true).GetSingleCodeGroupContriesCodeGroups();
            Dictionary<string, FixedRuleSaleCodeZones> fixedRuleSaleCodeZonesByCustomerId = this.GetFixedRuleSaleCodeZonesByCustomerId();

            Dictionary<string, List<SourceSpecialRequest>> groupedSourceRules = GroupSpecialRequests(sourceRules);
            foreach (var groupedRule in groupedSourceRules.Values)
            {
                var outputRules = StructureSpecialRequestRulesByDate(groupedRule);
                foreach (var outputRule in outputRules)
                {
                    SourceSpecialRequest defaultSourceSpecialRequest = outputRule.SpecialRequests.FirstOrDefault();
                    SourceSpecialRequest modifiedSourceSpecialRequestRule = this.GetModifiedSourceSpecialRequestRule(defaultSourceSpecialRequest, codeSaleZoneMappings, fixedRuleSaleCodeZonesByCustomerId);
                    if (modifiedSourceSpecialRequestRule == null)
                        continue;

                    IEnumerable<SpecialRequestSupplierOption> supplierOptions = outputRule.SpecialRequests.Select(s => s.SupplierOption);
                    SourceRule rule = GetSourceRule(modifiedSourceSpecialRequestRule, supplierOptions, outputRule.BED, outputRule.EED, codeGroups);
                    rules.Add(rule);
                }
            }
            return rules;
        }


        #region Private Methods

        private SourceSpecialRequest GetModifiedSourceSpecialRequestRule(SourceSpecialRequest sourceSpecialRequest, Dictionary<string, long> codeSaleZoneMappings,
            Dictionary<string, FixedRuleSaleCodeZones> fixedRuleSaleCodeZonesByCustomerId)
        {
            FixedRuleSaleCodeZones fixedRuleSaleCodeZones;
            if (!fixedRuleSaleCodeZonesByCustomerId.TryGetValue(sourceSpecialRequest.CustomerId, out fixedRuleSaleCodeZones))
                return sourceSpecialRequest;

            if (!sourceSpecialRequest.IncludeSubCode)
            {
                if (IsCodeOverridenByCode(fixedRuleSaleCodeZones.FixedRulesByCode, sourceSpecialRequest.Code, sourceSpecialRequest.EED))
                    return null;

                if (IsCodeOverridenByParentCode(fixedRuleSaleCodeZones.FixedRulesByParentCode, sourceSpecialRequest.Code, sourceSpecialRequest.EED))
                    return null;

                if (IsCodeOverridenByZone(fixedRuleSaleCodeZones.FixedRulesBySaleZoneId, sourceSpecialRequest.Code, sourceSpecialRequest.EED, codeSaleZoneMappings))
                    return null;
            }
            else
            {
                IEnumerable<string> ruleMatchingCodes = this.GetRuleMatchingCodes(sourceSpecialRequest, codeSaleZoneMappings.Keys);
                if (ruleMatchingCodes == null)
                    return sourceSpecialRequest;

                List<string> excludedCodes = new List<string>();

                foreach (var code in ruleMatchingCodes)
                {
                    if (IsCodeOverridenByCode(fixedRuleSaleCodeZones.FixedRulesByCode, code, sourceSpecialRequest.EED))
                    {
                        excludedCodes.Add(code);
                        continue;
                    }

                    if (IsCodeOverridenByParentCode(fixedRuleSaleCodeZones.FixedRulesByParentCode, code, sourceSpecialRequest.EED))
                    {
                        excludedCodes.Add(code);
                        continue;
                    }

                    if (IsCodeOverridenByZone(fixedRuleSaleCodeZones.FixedRulesBySaleZoneId, code, sourceSpecialRequest.EED, codeSaleZoneMappings))
                    {
                        excludedCodes.Add(code);
                        continue;
                    }
                }

                if (excludedCodes.Count > 0)
                {
                    if (sourceSpecialRequest.ExcludedCodesList != null)
                        sourceSpecialRequest.ExcludedCodesList.UnionWith(excludedCodes);
                    else
                        sourceSpecialRequest.ExcludedCodesList = new HashSet<string>(excludedCodes);
                }
            }

            return sourceSpecialRequest;
        }

        private bool IsCodeOverridenByCode(Dictionary<string, List<SourceRouteOverrideRule>> fixedRulesByCode, string ruleCode, DateTime? ruleEED)
        {
            List<SourceRouteOverrideRule> codeRouteOverrideRules;
            if (!fixedRulesByCode.TryGetValue(ruleCode, out codeRouteOverrideRules))
                return false;

            if (!IsCodeFiltered(codeRouteOverrideRules, ruleCode, ruleEED))
                return false;

            return true;
        }

        private bool IsCodeOverridenByParentCode(Dictionary<string, List<SourceRouteOverrideRule>> fixedRulesByParentCode, string ruleCode, DateTime? ruleEED)
        {
            List<SourceRouteOverrideRule> parentCodeFixedRules = this.GetParentCodeFixedRules(fixedRulesByParentCode, ruleCode);
            if (parentCodeFixedRules == null)
                return false;

            if (!IsCodeFiltered(parentCodeFixedRules, ruleCode, ruleEED))
                return false;

            return true;
        }

        private bool IsCodeOverridenByZone(Dictionary<long, List<SourceRouteOverrideRule>> fixedRuleBySaleZoneId, string ruleCode, DateTime? ruleEED, Dictionary<string, long> distinctCodeZones)
        {
            long sourceSaleZoneId;
            if (distinctCodeZones == null || !distinctCodeZones.TryGetValue(ruleCode, out sourceSaleZoneId))
                return false;

            List<SourceRouteOverrideRule> saleZoneRouteOverrideRules;
            if (!fixedRuleBySaleZoneId.TryGetValue(sourceSaleZoneId, out saleZoneRouteOverrideRules))
                return false;

            if (!IsCodeFiltered(saleZoneRouteOverrideRules, ruleCode, ruleEED))
                return false;

            return true;
        }

        private bool IsCodeFiltered(List<SourceRouteOverrideRule> matchingRouteOverrideRules, string ruleCode, DateTime? ruleEED)
        {
            foreach (var routeOverrideRule in matchingRouteOverrideRules)
            {
                if (routeOverrideRule.EED.HasValue && (!ruleEED.HasValue || ruleEED.Value > routeOverrideRule.EED.Value))
                    continue;

                if (routeOverrideRule.ExcludedCodesList == null || !routeOverrideRule.ExcludedCodesList.Contains(ruleCode))
                    return true;
            }

            return false;
        }

        private List<SourceRouteOverrideRule> GetParentCodeFixedRules(Dictionary<string, List<SourceRouteOverrideRule>> fixedRulesByParentCode, string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length == 1)
                return null;

            List<SourceRouteOverrideRule> results = new List<SourceRouteOverrideRule>();

            string prefix = code.Substring(0, code.Length - 1);
            while (prefix.Length > 0)
            {
                List<SourceRouteOverrideRule> tempSourceRouteOverrideRule;
                if (fixedRulesByParentCode.TryGetValue(prefix, out tempSourceRouteOverrideRule))
                    results.AddRange(tempSourceRouteOverrideRule);

                prefix = prefix.Substring(0, prefix.Length - 1);
            }

            return results.Count > 0 ? results : null;
        }

        private IEnumerable<string> GetRuleMatchingCodes(SourceSpecialRequest sourceSpecialRequest, IEnumerable<string> distinctCodes)
        {
            Func<string, bool> predicate = (code) =>
            {
                if (sourceSpecialRequest.ExcludedCodesList != null && sourceSpecialRequest.ExcludedCodesList.Contains(code))
                    return false;

                if (!code.StartsWith(sourceSpecialRequest.Code))
                    return false;

                return true;
            };
            return distinctCodes.Where(predicate);
        }

        private Dictionary<string, long> GetCodeSaleZoneMappings()
        {
            List<SaleCodeZone> saleCodeZones = new SaleCodeDBSyncDataManager(true).GetDistinctSaleCodeZones();
            List<SupplierCodeZone> supplierCodeZones = new SupplierCodeDBSyncDataManager(true).GetDictinctSupplierCodeZones();
            Dictionary<long, SaleZone> saleZones = new SaleZoneDBSyncDataManager(true).GetSaleZonesById();
            CodeIterator<SaleCodeZone> codeIterator = new CodeIterator<SaleCodeZone>(saleCodeZones);

            Dictionary<string, long> results = new Dictionary<string, long>();

            //SaleCodeZones
            foreach (var saleCodeZone in saleCodeZones)
            {
                long? sourceSaleZoneId = this.GetSourceSaleZoneId(saleZones, saleCodeZone.ZoneId);
                if (sourceSaleZoneId.HasValue)
                    results.Add(saleCodeZone.Code, sourceSaleZoneId.Value);
            }

            //SupplierCodeZones
            foreach (var supplierCodeZone in supplierCodeZones)
            {
                if (results.ContainsKey(supplierCodeZone.Code))
                    continue;

                SaleCodeZone matchingSaleCodeZone = codeIterator.GetLongestMatch(supplierCodeZone.Code);
                if (matchingSaleCodeZone != null)
                {
                    long? sourceSaleZoneId = this.GetSourceSaleZoneId(saleZones, matchingSaleCodeZone.ZoneId);
                    if (sourceSaleZoneId.HasValue)
                        results.Add(supplierCodeZone.Code, sourceSaleZoneId.Value);
                }
            }

            return results.Any() ? results : null;
        }

        private long? GetSourceSaleZoneId(Dictionary<long, SaleZone> saleZones, long saleZoneId)
        {
            SaleZone saleZone;
            if (!saleZones.TryGetValue(saleZoneId, out saleZone))
                return null;

            long sourceIdAsLong;
            if (!long.TryParse(saleZone.SourceId, out sourceIdAsLong))
                return null;

            return sourceIdAsLong;
        }

        private Dictionary<string, FixedRuleSaleCodeZones> GetFixedRuleSaleCodeZonesByCustomerId()
        {
            Dictionary<string, FixedRuleSaleCodeZones> results = new Dictionary<string, FixedRuleSaleCodeZones>();

            SourceRouteOverrideRuleDataManager dataManager = new SourceRouteOverrideRuleDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            IEnumerable<SourceRouteOverrideRule> routeOverrideRules = dataManager.GetRouteOverrideRules();

            foreach (var routeOverrideRule in routeOverrideRules)
            {
                FixedRuleSaleCodeZones fixedRuleSaleCodeZones = results.GetOrCreateItem(routeOverrideRule.CustomerId);

                if (!string.IsNullOrEmpty(routeOverrideRule.Code))
                {
                    List<SourceRouteOverrideRule> codeRouteOverrideRules = fixedRuleSaleCodeZones.FixedRulesByCode.GetOrCreateItem(routeOverrideRule.Code);
                    codeRouteOverrideRules.Add(routeOverrideRule);

                    if (routeOverrideRule.IncludeSubCode)
                    {
                        List<SourceRouteOverrideRule> parentCodeRouteOverrideRules = fixedRuleSaleCodeZones.FixedRulesByParentCode.GetOrCreateItem(routeOverrideRule.Code);
                        parentCodeRouteOverrideRules.Add(routeOverrideRule);
                    }
                }

                if (routeOverrideRule.SaleZoneId.HasValue)
                {
                    List<SourceRouteOverrideRule> saleZoneRouteOverrideRules = fixedRuleSaleCodeZones.FixedRulesBySaleZoneId.GetOrCreateItem(routeOverrideRule.SaleZoneId.Value);
                    saleZoneRouteOverrideRules.Add(routeOverrideRule);
                }
            }

            return results;
        }

        private bool IsSpecialRequestRuleByCountry(SourceSpecialRequest sourceSpecialRequest, Dictionary<string, CodeGroup> codeGroups, out int? countryId)
        {
            countryId = null;

            if (!sourceSpecialRequest.IncludeSubCode)
                return false;

            CodeGroup codeGroup;
            if (!codeGroups.TryGetValue(sourceSpecialRequest.Code, out codeGroup))
                return false;

            countryId = codeGroup.CountryId;
            return true;
        }

        private Dictionary<string, List<SourceSpecialRequest>> GroupSpecialRequests(IEnumerable<SourceSpecialRequest> sourceRules)
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

        private string GetKey(SourceSpecialRequest specialRequest)
        {
            return string.Format("{0}-{1}-{2}-{3}", specialRequest.CustomerId, specialRequest.Code, specialRequest.IncludeSubCode, specialRequest.ExcludedCodesList == null ? "" : specialRequest.ExcludedCodesList.Aggregate((i, j) => i + j));
        }

        private static List<SpecialRequestOutput> StructureSpecialRequestRulesByDate(List<SourceSpecialRequest> specialRequestRules)
        {
            List<SpecialRequestOutput> outputRules = new List<SpecialRequestOutput>();
            HashSet<DateTime> distinctDateTimes = new HashSet<DateTime>();

            foreach (var specialRequest in specialRequestRules)
            {
                distinctDateTimes.Add(specialRequest.BED);
                if (specialRequest.EED.HasValue)
                    distinctDateTimes.Add(specialRequest.EED.Value);
            }
            List<DateTime> intervalDates = distinctDateTimes.OrderBy(itm => itm).ToList();

            for (var index = 0; index < intervalDates.Count; index++)
            {
                bool isLastItem = index == intervalDates.Count - 1;
                DateTime bed = intervalDates[index];
                DateTime? eed = !isLastItem ? intervalDates[index + 1] : (DateTime?)null;

                Func<SourceSpecialRequest, bool> predicate = (itm) =>
                {
                    if (!isLastItem && !itm.EED.HasValue)
                        return false;

                    if (!isLastItem && itm.BED > bed)
                        return false;

                    if (itm.EED.HasValue && itm.EED.Value <= bed)
                        return false;

                    return true;
                };

                IEnumerable<SourceSpecialRequest> matchingSpecialRequestRules = specialRequestRules.FindAllRecords(predicate);
                if (matchingSpecialRequestRules.Count() > 0)
                {
                    outputRules.Add(new SpecialRequestOutput
                    {
                        SpecialRequests = matchingSpecialRequestRules,
                        BED = bed,
                        EED = eed
                    });
                }
            }

            return outputRules;
        }

        private SourceRule GetSourceRule(SourceSpecialRequest defaultRule, IEnumerable<SpecialRequestSupplierOption> supplierOptions, DateTime bed, DateTime? eed, Dictionary<string, CodeGroup> codeGroups)
        {
            RouteRule routeRule = GetRouteRuleDetails(defaultRule, supplierOptions, bed, eed, codeGroups);
            SourceRule rule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = bed,
                    EED = eed,
                    RuleDetails = Serializer.Serialize(routeRule),
                    TypeId = _routeRuleTypeId
                }
            };
            return rule;
        }

        private RouteRule GetRouteRuleDetails(SourceSpecialRequest defaultRule, IEnumerable<SpecialRequestSupplierOption> supplierOptions, DateTime bed, DateTime? eed, Dictionary<string, CodeGroup> codeGroups)
        {
            CarrierAccount customer;
            if (!_allCarrierAccounts.TryGetValue(defaultRule.CustomerId, out customer))
                throw new NullReferenceException(string.Format("customer not found. Customer Source Id {0}.", defaultRule.CustomerId));

            CodeCriteriaGroupSettings codeCriteriaGroupSettings = null;
            CountryCriteriaGroupSettings countryCriteriaGroupSettings = null;

            int? countryId;
            if (IsSpecialRequestRuleByCountry(defaultRule, codeGroups, out countryId))
                countryCriteriaGroupSettings = new SelectiveCountryCriteriaGroup { CountryIds = new List<int>() { countryId.Value } };
            else
                codeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup { Codes = GetCodeCriteria(defaultRule) };

            RouteRule routeRule = new RouteRule
            {
                BeginEffectiveTime = bed,
                EndEffectiveTime = eed,
                Description = defaultRule.Reason,
                Name = string.Format("Migrated Special Request Rule {0}", Context.Counter++),
                Criteria = new RouteRuleCriteria
                {
                    CodeCriteriaGroupSettings = codeCriteriaGroupSettings,
                    CountryCriteriaGroupSettings = countryCriteriaGroupSettings,
                    CustomerGroupSettings = new SelectiveCustomerGroup { CustomerIds = new List<int>() { customer.CarrierAccountId }, },
                    ExcludedDestinations = defaultRule.ExcludedCodesList != null && defaultRule.ExcludedCodesList.Count > 0 ? new ExcludedCodes() { Codes = defaultRule.ExcludedCodesList.ToList() } : null
                },
                Settings = new SpecialRequestRouteRule
                {
                    Options = GetSupplierOptions(supplierOptions)
                }
            };

            return routeRule;
        }

        private List<SpecialRequestRouteOptionSettings> GetSupplierOptions(IEnumerable<SpecialRequestSupplierOption> suppliers)
        {
            List<SpecialRequestRouteOptionSettings> results = new List<SpecialRequestRouteOptionSettings>();

            Dictionary<int, List<SpecialRequestSupplierOption>> groupedOptionsByPriority = new Dictionary<int, List<SpecialRequestSupplierOption>>();
            suppliers = suppliers.OrderByDescending(s => s.Priority).ThenByDescending(s => s.SourceId);

            foreach (var item in suppliers)
            {
                List<SpecialRequestSupplierOption> supplierOptions = groupedOptionsByPriority.GetOrCreateItem(item.Priority);
                supplierOptions.Add(item);
            }

            foreach (var item in groupedOptionsByPriority)
            {
                int priority = item.Key;
                if (priority != 0)
                {
                    SpecialRequestSupplierOption option = item.Value[0];
                    AddSpecialRequestRouteOptionSettings(results, option);
                }
                else
                {
                    foreach (var option in item.Value)
                        AddSpecialRequestRouteOptionSettings(results, option);
                }
            }

            return results;
        }

        private void AddSpecialRequestRouteOptionSettings(List<SpecialRequestRouteOptionSettings> options, SpecialRequestSupplierOption option)
        {
            CarrierAccount supplier;
            if (!_allCarrierAccounts.TryGetValue(option.SupplierId, out supplier))
                throw new NullReferenceException(string.Format("supplier not found. Supplier Source Id {0}.", option.SupplierId));

            SpecialRequestRouteOptionSettings specialRequestOptionSettings = new SpecialRequestRouteOptionSettings
            {
                SupplierId = supplier.CarrierAccountId,
                NumberOfTries = option.NumberOfTries,
                Percentage = option.Percentage == 0 ? default(int?) : option.Percentage,
                ForceOption = option.ForcedOption,
                Backups = null
            };

            options.Add(specialRequestOptionSettings);
        }

        private List<CodeCriteria> GetCodeCriteria(SourceSpecialRequest groupedRule)
        {
            return new List<CodeCriteria> { 
             new CodeCriteria{
              Code = groupedRule.Code,
               WithSubCodes = groupedRule.IncludeSubCode
             }
            };
        }

        #endregion
    }

    class SpecialRequestOutput
    {
        public IEnumerable<SourceSpecialRequest> SpecialRequests { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }

    class FixedRuleSaleCodeZones
    {
        public FixedRuleSaleCodeZones()
        {
            FixedRulesByCode = new Dictionary<string, List<SourceRouteOverrideRule>>();
            FixedRulesByParentCode = new Dictionary<string, List<SourceRouteOverrideRule>>();
            FixedRulesBySaleZoneId = new Dictionary<long, List<SourceRouteOverrideRule>>();
        }

        public Dictionary<string, List<SourceRouteOverrideRule>> FixedRulesByCode { get; set; }

        public Dictionary<string, List<SourceRouteOverrideRule>> FixedRulesByParentCode { get; set; }

        public Dictionary<long, List<SourceRouteOverrideRule>> FixedRulesBySaleZoneId { get; set; }
    }
}