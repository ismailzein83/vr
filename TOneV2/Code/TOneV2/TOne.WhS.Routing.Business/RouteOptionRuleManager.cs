using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Rules;
using Vanrise.Security.Business;

namespace TOne.WhS.Routing.Business
{
	public class RouteOptionRuleManager : Vanrise.Rules.RuleManager<RouteOptionRule, RouteOptionRuleDetail>
	{
		#region Variables/Ctor

		static RouteOptionRuleManager()
		{
			RouteOptionRuleManager instance = new RouteOptionRuleManager();
			instance.AddRuleCachingExpirationChecker(new RouteOptionRuleCachingExpirationChecker());
		}

		#endregion

		#region Public Methods

		public IEnumerable<RouteOptionRule> GetEffectiveLinkedRouteOptionRules(int customerId, string code, int supplierId, long supplierZoneId, DateTime effectiveDate)
		{
			RouteOptionRuleIdentifier routeOptionRuleIdentifier = new Entities.RouteOptionRuleIdentifier() { Code = code, CustomerId = customerId, SupplierId = supplierId, SupplierZoneId = supplierZoneId };
			var linkedRules = GetCachedLinkedRouteOptionRules();
			if (linkedRules == null)
				return null;

			List<RouteOptionRule> linkedRouteOptionRules = linkedRules.GetRecord(routeOptionRuleIdentifier);
			if (linkedRouteOptionRules == null)
				return null;
			return linkedRouteOptionRules.FindAllRecords(itm => itm.IsEffectiveOrFuture(effectiveDate));
		}

		public RouteOptionRule GetRouteOptionRuleHistoryDetailbyHistoryId(int routeOptionRuleHistoryId)
		{
			VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
			var routeOptionRule = s_vrObjectTrackingManager.GetObjectDetailById(routeOptionRuleHistoryId);
			return routeOptionRule.CastWithValidate<RouteOptionRule>("RouteOptionRule : historyId ", routeOptionRuleHistoryId);
		}

		public RouteOptionRule BuildLinkedRouteOptionRule(int? ruleId, int? customerId, string code, int? supplierId, long? supplierZoneId)
		{
			RouteOptionRule relatedRouteOptionRule;
			if (ruleId.HasValue)
				relatedRouteOptionRule = base.GetRuleWithDeleted(ruleId.Value);
			else
				relatedRouteOptionRule = new RouteOptionRule() { Settings = new BlockRouteOptionRule() };

			if (relatedRouteOptionRule == null)
				throw new NullReferenceException(string.Format("relatedRouteOptionRule. RuleId: {0}", ruleId));

			if (relatedRouteOptionRule.Settings == null)
				throw new NullReferenceException(string.Format("relatedRouteOptionRule.Settings. RuleId: {0}", ruleId));

			LinkedRouteOptionRuleContext context = new LinkedRouteOptionRuleContext();
			DateTime now = DateTime.Now;
			RouteOptionRule linkedRouteOptionRule = new RouteOptionRule()
			{
				BeginEffectiveTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second),
				Settings = relatedRouteOptionRule.Settings.BuildLinkedRouteOptionRuleSettings(context),
				Criteria = new RouteOptionRuleCriteria()
			};

			if (customerId.HasValue && customerId.Value > 0)
				linkedRouteOptionRule.Criteria.CustomerGroupSettings = new SelectiveCustomerGroup() { CustomerIds = new List<int>() { customerId.Value } };

			if (supplierId.HasValue && supplierId.Value > 0)
			{
				SupplierWithZones supplierWithZones = new SupplierWithZones() { SupplierId = supplierId.Value };

				if (supplierZoneId.HasValue && supplierZoneId.Value > 0)
				{
					supplierWithZones.SupplierZoneIds = new List<long>() { supplierZoneId.Value };
				}
				SelectiveSuppliersWithZonesGroup selectiveSuppliersWithZonesGroup = new SelectiveSuppliersWithZonesGroup() { SuppliersWithZones = new List<SupplierWithZones>() { supplierWithZones } };
				linkedRouteOptionRule.Criteria.SuppliersWithZonesGroupSettings = selectiveSuppliersWithZonesGroup;
			}

			if (!string.IsNullOrEmpty(code))
			{
				CodeCriteria codeCriteria = new BusinessEntity.Entities.CodeCriteria() { Code = code };
				linkedRouteOptionRule.Criteria.CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup() { Codes = new List<CodeCriteria>() { codeCriteria } };
			}

			return linkedRouteOptionRule;
		}

		public Vanrise.Entities.IDataRetrievalResult<RouteOptionRuleDetail> GetFilteredRouteOptionRules(Vanrise.Entities.DataRetrievalInput<RouteOptionRuleQuery> input)
		{
			var routeOptionRules = base.GetAllRules();
			string ruleNameLower = !string.IsNullOrEmpty(input.Query.Name) ? input.Query.Name.ToLower() : null;
			Func<RouteOptionRule, bool> filterExpression = (routeOptionRule) =>
			{
				if (!input.Query.RoutingProductId.HasValue && routeOptionRule.Criteria.RoutingProductId.HasValue)
					return false;

				if (input.Query.RoutingProductId.HasValue && (!routeOptionRule.Criteria.RoutingProductId.HasValue || routeOptionRule.Criteria.RoutingProductId.Value != input.Query.RoutingProductId.Value))
					return false;

				if (!string.IsNullOrEmpty(ruleNameLower) && (string.IsNullOrEmpty(routeOptionRule.Name) || !routeOptionRule.Name.ToLower().Contains(ruleNameLower)))
					return false;

				if (!string.IsNullOrEmpty(input.Query.Code) && !CheckIfCodeCriteriaSettingsContains(routeOptionRule, input.Query.Code))
					return false;

				if (input.Query.CustomerIds != null && !CheckIfCustomerSettingsContains(routeOptionRule, input.Query.CustomerIds))
					return false;

				if (input.Query.SaleZoneIds != null && !CheckIfSaleZoneSettingsContains(routeOptionRule, input.Query.SaleZoneIds))
					return false;

				if (input.Query.CountryIds != null && !CheckIfCountrySettingsContains(routeOptionRule, input.Query.CountryIds))
					return false;

				if (input.Query.SupplierId.HasValue && !CheckIfSupplierSettingsContains(routeOptionRule, input.Query.SupplierId.Value))
					return false;

				if (input.Query.SupplierZoneIds != null && !CheckIfSupplierZoneSettingsContains(routeOptionRule, input.Query.SupplierZoneIds))
					return false;

				if (input.Query.RouteOptionRuleSettingsConfigIds != null && !CheckIfSameRouteOptionRuleSettingsConfigId(routeOptionRule, input.Query.RouteOptionRuleSettingsConfigIds))
					return false;

				if (input.Query.EffectiveOn.HasValue && (routeOptionRule.BeginEffectiveTime > input.Query.EffectiveOn || (routeOptionRule.EndEffectiveTime.HasValue && routeOptionRule.EndEffectiveTime <= input.Query.EffectiveOn)))
					return false;

				if (input.Query.LinkedRouteOptionRuleIds != null && !input.Query.LinkedRouteOptionRuleIds.Contains(routeOptionRule.RuleId))
					return false;

				return true;
			};


			ResultProcessingHandler<RouteOptionRuleDetail> handler = new ResultProcessingHandler<RouteOptionRuleDetail>()
			{
				ExportExcelHandler = new RouteOptionRuleExcelExportHandler()
			};
			VRActionLogger.Current.LogGetFilteredAction(RouteOptionRuleLoggableEntity.Instance, input);
			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeOptionRules.ToBigResult(input, filterExpression, MapToDetails), handler);
		}

		public RouteOptionRuleEditorRuntime GetRuleEditorRuntime(int ruleId)
		{
			RouteOptionRuleEditorRuntime routeOptionRuleEditorRuntime = new RouteOptionRuleEditorRuntime();
			routeOptionRuleEditorRuntime.Entity = base.GetRule(ruleId);
			if (routeOptionRuleEditorRuntime.Entity == null)
				return null;

			if (routeOptionRuleEditorRuntime.Entity.Settings == null)
				throw new NullReferenceException(string.Format("routeOptionRuleEditorRuntime.Entity.Settings for Rule ID: {0} is null", ruleId));

			routeOptionRuleEditorRuntime.SettingsEditorRuntime = routeOptionRuleEditorRuntime.Entity.Settings.GetEditorRuntime();

			return routeOptionRuleEditorRuntime;
		}

		public Vanrise.Rules.RuleTree[] GetRuleTreesByPriorityForCustomerRoutes()
		{
			return GetCachedOrCreate("GetRuleTreesByPriorityForCustomerRoutes",
				() =>
				{
					return BuildRuleTree((allRouteOptionRuleConfig) =>
					{
						List<RouteOptionRuleConfig> results = new List<RouteOptionRuleConfig>();
						RouteOptionRuleTypeConfiguration routeOptionRuleTypeConfiguration;
						Dictionary<Guid, RouteOptionRuleTypeConfiguration> routeOptionRuleTypeConfigurationDic = new ConfigManager().GetRouteOptionRuleTypeConfigurationForCustomerRoutes();

						foreach (var itm in allRouteOptionRuleConfig)
						{
							routeOptionRuleTypeConfiguration = routeOptionRuleTypeConfigurationDic.GetRecord(itm.ExtensionConfigurationId);
							if (!itm.CanExcludeFromRouteBuildProcess || (routeOptionRuleTypeConfiguration != null && !routeOptionRuleTypeConfiguration.IsExcluded))
								results.Add(itm);
						}
						return results;
					});
				});
		}

		public Vanrise.Rules.RuleTree[] GetRuleTreesByPriorityForProductRoutes()
		{
			return GetCachedOrCreate("GetRuleTreesByPriorityForProductRoutes",
				() =>
				{
					return BuildRuleTree((allRouteOptionRuleConfig) =>
					{
						List<RouteOptionRuleConfig> results = new List<RouteOptionRuleConfig>();
						RouteOptionRuleTypeConfiguration routeOptionRuleTypeConfiguration;
						Dictionary<Guid, RouteOptionRuleTypeConfiguration> routeOptionRuleTypeConfigurationDic = new ConfigManager().GetRouteOptionRuleTypeConfigurationForProductRoutes();

						foreach (var itm in allRouteOptionRuleConfig)
						{
							routeOptionRuleTypeConfiguration = routeOptionRuleTypeConfigurationDic.GetRecord(itm.ExtensionConfigurationId);
							if (!itm.CanExcludeFromProductCostProcess || (routeOptionRuleTypeConfiguration != null && !routeOptionRuleTypeConfiguration.IsExcluded))
								results.Add(itm);
						}
						return results;
					});
				});
		}

		private Vanrise.Rules.RuleTree[] BuildRuleTree(Func<IEnumerable<RouteOptionRuleConfig>, IEnumerable<RouteOptionRuleConfig>> GetIncludedRouteOptionRuleTypes)
		{
			List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
			var structureBehaviors = GetRuleStructureBehaviors();
			var routeOptionRuleTypes = GetRouteOptionRuleTypesTemplates();
			IEnumerable<RouteOptionRuleConfig> includedRouteOptionRuleTypes = GetIncludedRouteOptionRuleTypes(routeOptionRuleTypes);

			int? currentPriority = null;
			List<Vanrise.Rules.IVRRule> currentRules = null;
			foreach (var ruleType in includedRouteOptionRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
			{
				int priority = GetRuleTypePriority(ruleType);
				if (currentPriority == null || currentPriority.Value != priority)
				{
					if (currentRules != null && currentRules.Count > 0)
						ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
					currentPriority = priority;
					currentRules = new List<Vanrise.Rules.IVRRule>();
				}
				var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.ExtensionConfigurationId);
				if (ruleTypeRules != null)
					currentRules.AddRange(ruleTypeRules);
			}
			if (currentRules != null && currentRules.Count > 0)
				ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
			return ruleTrees.ToArray();
		}

		public IEnumerable<RouteOptionRuleConfig> GetRouteOptionRuleTypesTemplates()
		{
			ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
			return manager.GetExtensionConfigurations<RouteOptionRuleConfig>(RouteOptionRuleConfig.EXTENSION_TYPE);
		}

		public override RouteOptionRuleDetail MapToDetails(RouteOptionRule rule)
		{
			string _cssClass = null;
			string _routeoptionRuleSettingsTypeName = null;
			Dictionary<Guid, RouteOptionRuleConfig> routeOptionRuleConfigDic = GetRouteOptionRuleTypesTemplatesDict();


			if (rule != null && rule.Settings != null && rule.Settings.ConfigId != null)
			{
				RouteOptionRuleConfig routeRuleSettingsConfig = routeOptionRuleConfigDic.GetRecord(rule.Settings.ConfigId);
				_cssClass = routeRuleSettingsConfig.CssClass;
				_routeoptionRuleSettingsTypeName = routeRuleSettingsConfig.Title;
			}

			UserManager userManager = new UserManager();
			string lastModifiedByUserName = null;

			if (rule != null && rule.LastModifiedBy.HasValue)
				lastModifiedByUserName = userManager.GetUserName(rule.LastModifiedBy.Value);

			return new RouteOptionRuleDetail
			{
				Entity = rule,
				CssClass = _cssClass,
				RouteOptionRuleSettingsTypeName = _routeoptionRuleSettingsTypeName,
				LastModifiedByUserName = lastModifiedByUserName
			};
		}

		public Dictionary<Guid, RouteOptionRuleConfig> GetRouteOptionRuleTypesTemplatesDict()
		{
			ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
			return manager.GetExtensionConfigurationsByType<RouteOptionRuleConfig>(RouteOptionRuleConfig.EXTENSION_TYPE);
		}

		public IEnumerable<ProcessRouteOptionRuleConfig> GetRouteOptionRuleSettingsTemplatesByProcessType(RoutingProcessType routingProcessType)
		{
			List<ProcessRouteOptionRuleConfig> results = new List<ProcessRouteOptionRuleConfig>();

			IEnumerable<RouteOptionRuleConfig> allRouteOptionRuleConfig = GetRouteOptionRuleTypesTemplates();

			if (allRouteOptionRuleConfig == null)
				return null;

			switch (routingProcessType)
			{
				case RoutingProcessType.CustomerRoute:
					foreach (var itm in allRouteOptionRuleConfig)
					{
						results.Add(new ProcessRouteOptionRuleConfig
						{
							ExtensionConfigurationId = itm.ExtensionConfigurationId,
							Title = itm.Title,
							CanExclude = itm.CanExcludeFromRouteBuildProcess
						});
					}
					break;

				case RoutingProcessType.RoutingProductRoute:
					foreach (var itm in allRouteOptionRuleConfig)
					{
						results.Add(new ProcessRouteOptionRuleConfig
						{
							ExtensionConfigurationId = itm.ExtensionConfigurationId,
							Title = itm.Title,
							CanExclude = itm.CanExcludeFromProductCostProcess
						});
					}
					break;

				default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", routingProcessType));
			}
			return results;
		}

		public override Vanrise.Rules.RuleLoggableEntity GetLoggableEntity(RouteOptionRule rule)
		{
			return RouteOptionRuleLoggableEntity.Instance;
		}

		#endregion

		#region Private Methods

		private IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
		{
			List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
			ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());
			ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
			ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCode());
			ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
			ruleStructureBehaviors.Add(new Vanrise.Common.MainExtensions.Country.Rules.StructureRulesBehaviors.RuleBehaviorByCountry());
			ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
			//ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
			return ruleStructureBehaviors;
		}

		private int GetRuleTypePriority(RouteOptionRuleConfig ruleTypeConfig)
		{
			return ruleTypeConfig.Priority.HasValue ? ruleTypeConfig.Priority.Value : int.MaxValue;
		}

		private bool CheckIfCodeCriteriaSettingsContains(RouteOptionRule routeOptionRule, string code)
		{
			if (routeOptionRule.Criteria.CodeCriteriaGroupSettings != null)
			{
				IRuleCodeCriteria ruleCode = routeOptionRule as IRuleCodeCriteria;
				if (ruleCode.CodeCriterias != null && ruleCode.CodeCriterias.Any(x => x.Code.StartsWith(code)))
					return true;
			}

			return false;
		}
		private bool CheckIfCustomerSettingsContains(RouteOptionRule routeOptionRule, IEnumerable<int> customerIds)
		{
			if (routeOptionRule.Criteria.CustomerGroupSettings != null)
			{
				IRuleCustomerCriteria ruleCode = routeOptionRule as IRuleCustomerCriteria;
				if (ruleCode.CustomerIds == null || ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
					return true;
			}

			return false;
		}
		private bool CheckIfSupplierSettingsContains(RouteOptionRule routeOptionRule, int supplierId)
		{
			if (routeOptionRule.Criteria.SuppliersWithZonesGroupSettings != null)
			{
				IRuleSupplierCriteria ruleCode = routeOptionRule as IRuleSupplierCriteria;
				if (ruleCode.SupplierIds != null && ruleCode.SupplierIds.Contains(supplierId))
					return true;
			}
			return false;
		}

		private bool CheckIfSupplierZoneSettingsContains(RouteOptionRule routeOptionRule, IEnumerable<long> supplierZoneIds)
		{
			if (routeOptionRule.Criteria.SuppliersWithZonesGroupSettings != null)
			{
				IRuleSupplierZoneCriteria ruleCode = routeOptionRule as IRuleSupplierZoneCriteria;
				//supplierZoneIds = null -> all zones for the supplier
				if (ruleCode.SupplierZoneIds == null || ruleCode.SupplierZoneIds.Intersect(supplierZoneIds).Count() > 0)
					return true;
			}
			return false;
		}
		private bool CheckIfSaleZoneSettingsContains(RouteOptionRule routeOptionRule, IEnumerable<long> saleZoneIds)
		{
			if (routeOptionRule.Criteria.SaleZoneGroupSettings != null)
			{
				IRuleSaleZoneCriteria ruleCode = routeOptionRule as IRuleSaleZoneCriteria;
				if (ruleCode.SaleZoneIds != null && ruleCode.SaleZoneIds.Intersect(saleZoneIds).Count() > 0)
					return true;
			}

			return false;
		}

		private bool CheckIfCountrySettingsContains(RouteOptionRule routeOptionRule, IEnumerable<int> countryIds)
		{
			if (routeOptionRule.Criteria.CountryCriteriaGroupSettings != null)
			{
				IRuleCountryCriteria ruleCode = routeOptionRule as IRuleCountryCriteria;
				if (ruleCode.CountryIds != null && ruleCode.CountryIds.Intersect(countryIds).Count() > 0)
					return true;
			}

			return false;
		}

		private bool CheckIfSameRouteOptionRuleSettingsConfigId(RouteOptionRule routeOptionRule, List<Guid> RouteOptionRuleSettingsConfigIds)
		{
			if (RouteOptionRuleSettingsConfigIds.Contains(routeOptionRule.Settings.ConfigId))
				return true;

			return false;
		}

		private Dictionary<RouteOptionRuleIdentifier, List<RouteOptionRule>> GetCachedLinkedRouteOptionRules()
		{
			return GetCachedOrCreate(string.Format("GetCachedLinkedRouteOptionRules"),
				() =>
				{
					Dictionary<RouteOptionRuleIdentifier, List<RouteOptionRule>> linkedRouteOptionRules = new Dictionary<RouteOptionRuleIdentifier, List<RouteOptionRule>>();
					Dictionary<int, RouteOptionRule> routeOptionRules = GetAllRules();

					if (routeOptionRules != null)
					{
						foreach (var routeOptionRule in routeOptionRules)
						{
							if (routeOptionRule.Value.Criteria != null)
							{
								RouteOptionRuleCriteria criteria = routeOptionRule.Value.Criteria;

								string code = CheckAndReturnValidCode(criteria, routeOptionRule.Value.RuleId);
								if (string.IsNullOrEmpty(code))
									continue;

								int? customerId = CheckAndReturnValidCustomer(criteria);
								if (!customerId.HasValue)
									continue;

								long? supplierZoneId;
								int? supplierId = CheckAndReturnValidSupplier(criteria, out supplierZoneId);
								if (!supplierId.HasValue || !supplierZoneId.HasValue)
									continue;

								RouteOptionRuleIdentifier routeOptionRuleIdentifier = new Entities.RouteOptionRuleIdentifier() { Code = code, CustomerId = customerId.Value, SupplierId = supplierId.Value, SupplierZoneId = supplierZoneId.Value };
								List<RouteOptionRule> relatedRouteOptionRules = linkedRouteOptionRules.GetOrCreateItem(routeOptionRuleIdentifier);
								relatedRouteOptionRules.Add(routeOptionRule.Value);
							}
						}
					}

					return linkedRouteOptionRules;
				});
		}

		public int? CheckAndReturnValidCustomer(RouteOptionRuleCriteria criteria)
		{
			if (criteria.CustomerGroupSettings == null)
				return null;

			SelectiveCustomerGroup selectiveCustomerGroup = criteria.CustomerGroupSettings as SelectiveCustomerGroup;
			if (selectiveCustomerGroup == null)
				return null;

			if (selectiveCustomerGroup.CustomerIds == null || selectiveCustomerGroup.CustomerIds.Count != 1)
				return null;

			return selectiveCustomerGroup.CustomerIds.First();
		}

		public string CheckAndReturnValidCode(RouteOptionRuleCriteria criteria, int ruleId)
		{
			if (criteria.CodeCriteriaGroupSettings == null)
				return null;

			SelectiveCodeCriteriaGroup selectiveCodeCriteriaGroup = criteria.CodeCriteriaGroupSettings as SelectiveCodeCriteriaGroup;
			if (selectiveCodeCriteriaGroup == null)
				return null;

			if (selectiveCodeCriteriaGroup.Codes == null || selectiveCodeCriteriaGroup.Codes.Count != 1)
				return null;

			string code = selectiveCodeCriteriaGroup.Codes.First().Code;

			if (criteria.ExcludedDestinations != null)
			{
				RoutingExcludedDestinationContext context = new RoutingExcludedDestinationContext(code, ruleId);
				if (criteria.ExcludedDestinations.IsExcludedDestination(context))
					return null;
			}

			return code;
		}

		public int? CheckAndReturnValidSupplier(RouteOptionRuleCriteria criteria, out long? supplierZoneId)
		{
			supplierZoneId = null;

			if (criteria.SuppliersWithZonesGroupSettings == null)
				return null;

			SelectiveSuppliersWithZonesGroup selectiveSuppliersWithZonesGroup = criteria.SuppliersWithZonesGroupSettings as SelectiveSuppliersWithZonesGroup;
			if (selectiveSuppliersWithZonesGroup == null)
				return null;

			if (selectiveSuppliersWithZonesGroup.SuppliersWithZones == null || selectiveSuppliersWithZonesGroup.SuppliersWithZones.Count != 1)
				return null;

			SupplierWithZones supplierWithZones = selectiveSuppliersWithZonesGroup.SuppliersWithZones.First();
			if (supplierWithZones.SupplierZoneIds == null || supplierWithZones.SupplierZoneIds.Count != 1)
				return null;

			supplierZoneId = supplierWithZones.SupplierZoneIds.First();
			return supplierWithZones.SupplierId;
		}
		#endregion

		#region Private Classes

		private class RouteOptionRuleExcelExportHandler : ExcelExportHandler<RouteOptionRuleDetail>
		{
			public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RouteOptionRuleDetail> context)
			{
				ExportExcelSheet sheet = new ExportExcelSheet()
				{
					SheetName = "Route Options",
					Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
				};
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rule Type" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name", Width = 30 });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Suppliers", Width = 45 });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Destinations" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customers", Width = 30 });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Excluded Codes" });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
				sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });

				sheet.Rows = new List<ExportExcelRow>();
				if (context.BigResult != null && context.BigResult.Data != null)
				{
					foreach (var record in context.BigResult.Data)
					{
						if (record.Entity != null)
						{
							var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
							sheet.Rows.Add(row);
							row.Cells.Add(new ExportExcelCell { Value = record.RouteOptionRuleSettingsTypeName });
							row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
							row.Cells.Add(new ExportExcelCell { Value = record.Suppliers });
							row.Cells.Add(new ExportExcelCell { Value = record.Destinations });
							row.Cells.Add(new ExportExcelCell { Value = record.Customers });
							row.Cells.Add(new ExportExcelCell { Value = record.Excluded });
							row.Cells.Add(new ExportExcelCell { Value = record.Entity.BeginEffectiveTime });
							row.Cells.Add(new ExportExcelCell { Value = record.Entity.EndEffectiveTime });
						}
					}
				}
				context.MainSheet = sheet;
			}
		}

		private class RouteOptionRuleLoggableEntity : Vanrise.Rules.RuleLoggableEntity
		{
			static RouteOptionRuleLoggableEntity s_instance = new RouteOptionRuleLoggableEntity();
			public static RouteOptionRuleLoggableEntity Instance
			{
				get
				{
					return s_instance;
				}
			}
			public override string EntityDisplayName
			{
				get { return "Route Option Rules"; }
			}

			public override string EntityUniqueName
			{
				get { return "WhS_Routing_RouteOptionRules"; }
			}

			public override string ViewHistoryItemClientActionName
			{
				get { return "WhS_Routing_RouteOptionRules_ViewHistoryItem"; }
			}
		}


		#endregion
	}

	public class RouteOptionRuleCachingExpirationChecker : RuleCachingExpirationChecker
	{
		DateTime? _settingsCacheLastCheck;

		public override bool IsRuleDependenciesCacheExpired()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _settingsCacheLastCheck);
		}
	}
}