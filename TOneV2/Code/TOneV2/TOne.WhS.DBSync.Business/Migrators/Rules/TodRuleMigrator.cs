using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan;
using TOne.WhS.BusinessEntity.MainExtensions.SupplierZoneGroups;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Entities;
using Vanrise.Rules.Pricing;
using Vanrise.Rules.Pricing.MainExtensions.ExtraCharge;
using Vanrise.Rules.Pricing.MainExtensions.RateType;

namespace TOne.WhS.DBSync.Business.Migrators
{
    public enum RuleType { Sale, Purchase }
    public class TodRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Tod"; }
        }
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _ruleTypeId;
        readonly int _offPeakRateTypeId;
        readonly int _weekendRateTypeId;
        readonly int _holidayRateTypeId;
        Guid _purchaseRuleDefinitionId = new Guid("FCF48F89-D6BD-4E93-8221-1B7E2153116B");
        Guid _saleRuleDefinitionId = new Guid("8A637067-0056-4BAE-B4D5-F80F00C0141B");
        public TodRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            RateTypeRuleManager manager = new RateTypeRuleManager();
            _ruleTypeId = manager.GetRuleTypeId();
            _offPeakRateTypeId = context.MigrationContext.OffPeakRateTypeId;
            _weekendRateTypeId = context.MigrationContext.WeekendRateTypeId;
            _holidayRateTypeId = context.MigrationContext.HolidayRateTypeId;

        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> sourceRules = new List<SourceRule>();
            SourceTodDataManager dataManager = new SourceTodDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            var todRules = dataManager.GetSourceTods();
            sourceRules = GetRules(todRules);

            return sourceRules;
        }

        #region Private Methods
        List<SourceRule> GetRules(IEnumerable<SourceTod> tods)
        {
            List<SourceRule> result = new List<SourceRule>();

            var sellingRules = tods.FindAllRecords(s => s.SupplierId.Equals("SYS"));
            var purchaseRules = tods.FindAllRecords(s => !s.SupplierId.Equals("SYS"));

            var rulesDictionary = GroupTodRules(sellingRules, RuleType.Sale);

            foreach (var key in rulesDictionary.Keys)
            {
                var todRules = rulesDictionary[key];
                result.Add(GetSellingSourceRule(todRules));
            }

            rulesDictionary = GroupTodRules(purchaseRules, RuleType.Purchase);
            foreach (var key in rulesDictionary.Keys)
            {
                var todRules = rulesDictionary[key];
                result.Add(GetPurchaseSourceRule(todRules));
            }

            return result;
        }
        SourceRule GetPurchaseSourceRule(List<SourceTod> todRules)
        {
            HashSet<long> zoneIds = new HashSet<long>();
            SourceTod defaultTod = todRules.FirstOrDefault();
            foreach (SourceTod sourceTod in todRules)
            {
                if (!_allSupplierZones.ContainsKey(sourceTod.ZoneId.ToString()))
                {
                    Context.MigrationContext.WriteWarning(string.Format("Failed migrating TOD, Source Id: {0}, Supplier Zone Id {1}", sourceTod.TodId, sourceTod.ZoneId));
                    this.TotalRowsFailed++;
                }
                else
                    zoneIds.Add(_allSupplierZones[sourceTod.ZoneId.ToString()].SupplierZoneId);
            }

            RateTypeRule extraChargeRule = new RateTypeRule
            {
                Settings = new PricingRuleRateTypeSettings
                {
                    Items = GetPricingRuleRateTypeSettings(todRules)

                },
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                BeginEffectiveTime = defaultTod.BED,
                EndEffectiveTime = defaultTod.EED,
                Description = string.Format("Migrated Purchase TOD Rule {0}", Context.Counter++),
                DefinitionId = _purchaseRuleDefinitionId
            };

            extraChargeRule.Criteria.FieldsValues.Add("SupplierId", new StaticValues
            {
                Values = new List<object>() { _allCarrierAccounts[defaultTod.SupplierId].CarrierAccountId }
            });

            extraChargeRule.Criteria.FieldsValues.Add("SupplierZoneId", new StaticValues()
            {
                Values = zoneIds.Cast<Object>().ToList()
            });

            return GetSourceRule(extraChargeRule, defaultTod);
        }
        SourceRule GetSellingSourceRule(List<SourceTod> todRules)
        {
            List<long> zoneIds = new List<long>();
            HashSet<int> customers = new HashSet<int>();
            SourceTod defaultTod = todRules.FirstOrDefault();

            if (!_allSaleZones.ContainsKey(defaultTod.ZoneId.ToString()))
                this.TotalRowsFailed++;
            else
                zoneIds.Add(_allSaleZones[defaultTod.ZoneId.ToString()].SaleZoneId);

            foreach (SourceTod sourceTod in todRules)
            {
                if (!_allCarrierAccounts.ContainsKey(sourceTod.CustomerId))
                {
                    Context.MigrationContext.WriteWarning(string.Format("Failed migrating TOD, Source Id: {0}, Carrier Account Id {1}", sourceTod.TodId, sourceTod.CustomerId));
                    this.TotalRowsFailed++;
                }
                else
                    customers.Add(_allCarrierAccounts[defaultTod.CustomerId].CarrierAccountId);

            }

            RateTypeRule extraChargeRule = new RateTypeRule
            {
                Settings = new PricingRuleRateTypeSettings
                {
                    Items = GetPricingRuleRateTypeSettings(todRules)
                },
                Criteria = new GenericRuleCriteria
                {

                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                BeginEffectiveTime = defaultTod.BED,
                EndEffectiveTime = defaultTod.EED,
                Description = string.Format("Migrated Sale TOD Rule {0}", Context.Counter++),
                DefinitionId = _saleRuleDefinitionId
            };

            extraChargeRule.Criteria.FieldsValues.Add("CustomerId", new StaticValues
            {
                Values = customers.Cast<Object>().ToList()
            });

            extraChargeRule.Criteria.FieldsValues.Add("SaleZoneId", new StaticValues
            {
                Values = zoneIds.Cast<Object>().ToList()
            });

            return GetSourceRule(extraChargeRule, defaultTod);
        }
        List<PricingRuleRateTypeItemSettings> GetPricingRuleRateTypeSettings(List<SourceTod> todRules)
        {
            Dictionary<string, PricingRuleRateTypeItemSettings> settings = new Dictionary<string, PricingRuleRateTypeItemSettings>();
            foreach (var todRule in todRules)
            {
                string key = GetTodSettingsKey(todRule, todRule.RateType);
                if (settings.ContainsKey(key))
                    continue;
                settings.Add(key, new DaysOfWeekRateTypeSettings());

                switch (todRule.RateType)
                {
                    case ToDRateType.OffPeak:
                        settings[key] = GetDaysOfWeekRateTypeSettings(todRule);
                        settings[key].RateTypeId = _offPeakRateTypeId;
                        break;
                    case ToDRateType.Weekend:
                        settings[key] = GetDaysOfWeekRateTypeSettings(todRule);
                        settings[key].RateTypeId = _weekendRateTypeId;
                        break;
                    case ToDRateType.Holiday:
                        settings[key] = GetSpecificDayRateTypeSettings(todRule);
                        settings[key].RateTypeId = _holidayRateTypeId;
                        break;
                    default:
                        continue;
                }
            }

            return settings.Values.ToList();
        }
        SpecificDayRateTypeSettings GetSpecificDayRateTypeSettings(SourceTod todRule)
        {
            return new SpecificDayRateTypeSettings
            {
                Date = todRule.HolidayDateTime.Value,
                RateTypeId = _holidayRateTypeId
            };
        }
        DaysOfWeekRateTypeSettings GetDaysOfWeekRateTypeSettings(SourceTod todRule)
        {
            return new DaysOfWeekRateTypeSettings
            {
                Days = new List<DayOfWeek> {  
                            todRule.DayOfWeek.Value                            
                            },

                TimeRanges = new List<TimeRange> 
                        {
                            new TimeRange
                             {
                              FromTime = new Time
                              {
                               Hour = todRule.BeginTime.Value.Hours,
                               Minute = todRule.BeginTime.Value.Minutes,
                               Second = todRule.BeginTime.Value.Seconds,
                               MilliSecond = todRule.BeginTime.Value.Milliseconds
                              },
                              ToTime = new Time
                              {
                                Hour = todRule.EndTime.Value.Hours,
                                Minute = todRule.EndTime.Value.Minutes,
                                Second = todRule.EndTime.Value.Seconds,
                                MilliSecond = todRule.EndTime.Value.Milliseconds
                              }
                             }
                         }
            };
        }
        string GetTodSettingsKey(SourceTod rule, ToDRateType type)
        {
            switch (type)
            {
                case ToDRateType.OffPeak:
                case ToDRateType.Weekend:
                    return string.Format("{0}-{1}-{2}", rule.DayOfWeek, rule.BeginTime.Value, rule.EndTime.Value);
                case ToDRateType.Holiday:
                    return string.Format("{0}-{1}", rule.HolidayDateTime, rule.HolidayName);
                default:
                    return "";
            }
        }
        SourceRule GetSourceRule(RateTypeRule extraChargeRule, SourceTod defaultTod)
        {
            return new SourceRule
            {
                Rule = new Rule
                {
                    RuleDetails = Serializer.Serialize(extraChargeRule),
                    TypeId = _ruleTypeId,
                    BED = defaultTod.BED,
                    EED = defaultTod.EED
                }
            };
        }
        Dictionary<string, List<SourceTod>> GroupTodRules(IEnumerable<SourceTod> sellingRules, RuleType type)
        {
            Dictionary<string, List<SourceTod>> tods = new Dictionary<string, List<SourceTod>>();
            List<SourceTod> sourceTods;
            foreach (SourceTod tod in sellingRules)
            {
                string key = GetTodKey(tod, type);
                if (!tods.TryGetValue(key, out sourceTods))
                {
                    sourceTods = new List<SourceTod>();
                    tods.Add(key, sourceTods);
                }
                sourceTods.Add(tod);
            }

            return tods;
        }
        string GetTodKey(SourceTod tod, RuleType type)
        {
            switch (type)
            {
                case RuleType.Sale:
                    return string.Format("{0}-{1}-{2}-{3}-{4}", tod.ZoneId, tod.BeginTime, tod.EndTime, tod.RateType, tod.BED, tod.EED, tod.HolidayDateTime, tod.HolidayName);
                case RuleType.Purchase:
                    return string.Format("{0}-{1}-{2}-{3}-{4}", tod.SupplierId, tod.BeginTime, tod.EndTime, tod.ZoneId, tod.BED, tod.EED, tod.HolidayDateTime, tod.HolidayName);
                default:
                    return "";
            }
        }

        #endregion
    }
}
