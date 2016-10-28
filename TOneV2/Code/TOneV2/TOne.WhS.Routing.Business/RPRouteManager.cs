using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.Business
{
	public class RPRouteManager
	{
		#region Fields

		CarrierAccountManager _carrierAccountManager;
		RoutingProductManager _routingProductManager;
		SaleZoneManager _saleZoneManager;
		CurrencyExchangeRateManager _currencyExchangeRateManager;

		#endregion

		#region Constructors

		public RPRouteManager()
		{
			_carrierAccountManager = new CarrierAccountManager();
			_routingProductManager = new RoutingProductManager();
			_saleZoneManager = new SaleZoneManager();
			_currencyExchangeRateManager = new CurrencyExchangeRateManager();
		}

		#endregion

		#region Public Methods

		public Vanrise.Entities.IDataRetrievalResult<RPRouteDetail> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<RPRouteQuery> input)
		{
			var latestRoutingDatabase = GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);

			IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
			dataManager.RoutingDatabase = latestRoutingDatabase;

			bool includeBlockedSupplierZones = GetIncludeBlockedSupplierZones(latestRoutingDatabase);

			BigResult<RPRoute> rpRouteResult = dataManager.GetFilteredRPRoutes(input);

			BigResult<RPRouteDetail> customerRouteDetailResult = new BigResult<RPRouteDetail>()
			{
				ResultKey = rpRouteResult.ResultKey,
				TotalCount = rpRouteResult.TotalCount,
				Data = rpRouteResult.Data.MapRecords(x => RPRouteDetailMapper(x, input.Query.PolicyConfigId, input.Query.NumberOfOptions, null, null, includeBlockedSupplierZones, null))
			};

			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
		}

		public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones)
		{
			return GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, null, null);
		}

		public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones, int? toCurrencyId, int? customerId)
		{
			var latestRoutingDatabase = GetLatestRoutingDatabase(routingDatabaseId);

			IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
			dataManager.RoutingDatabase = latestRoutingDatabase;

			bool includeBlockedSupplierZones = GetIncludeBlockedSupplierZones(latestRoutingDatabase);

			int? customerProfileId = null;
			if (customerId.HasValue)
				customerProfileId = GetCarrierProfileId(customerId.Value);

			IEnumerable<RPRoute> rpRoutes = dataManager.GetRPRoutes(rpZones);
			int systemCurrencyId = GetSystemCurrencyId();
			return rpRoutes.MapRecords(x => RPRouteDetailMapper(x, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, includeBlockedSupplierZones, customerProfileId));
		}

		public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId)
		{
			return GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId, null);
		}

		public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId, int? toCurrencyId)
		{
			IRPRouteDataManager routeManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
			RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
			routeManager.RoutingDatabase = GetLatestRoutingDatabase(routingDatabaseId);

			Dictionary<int, RPRouteOptionSupplier> dicRouteOptionSuppliers = routeManager.GetRouteOptionSuppliers(routingProductId, saleZoneId);

			if (dicRouteOptionSuppliers == null || !dicRouteOptionSuppliers.ContainsKey(supplierId))
				return null;


			CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

			int? systemCurrencyId = null;
			if (toCurrencyId.HasValue)
				systemCurrencyId = GetSystemCurrencyId();

			return new RPRouteOptionSupplierDetail()
			{
				SupplierName = carrierAccountManager.GetCarrierAccountName(supplierId),
				SupplierZones = dicRouteOptionSuppliers[supplierId].SupplierZones.MapRecords(x => RPRouteOptionSupplierZoneDetailMapper(x, systemCurrencyId, toCurrencyId))
			};
		}

		public Vanrise.Entities.IDataRetrievalResult<RPRouteOptionDetail> GetFilteredRPRouteOptions(Vanrise.Entities.DataRetrievalInput<RPRouteOptionQuery> input)
		{
			IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
			manager.RoutingDatabase = GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);

			Dictionary<Guid, IEnumerable<RPRouteOption>> allOptions = manager.GetRouteOptions(input.Query.RoutingProductId, input.Query.SaleZoneId);
			if (allOptions == null || !allOptions.ContainsKey(input.Query.PolicyOptionConfigId))
				return null;

			IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[input.Query.PolicyOptionConfigId];
			int counter = 0;
			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<RPRouteOptionDetail>(input, routeOptionsByPolicy.ToBigResult(input, null, x => RPRouteOptionMapper(x, null, null, counter++)));
		}

		public IEnumerable<RPRouteOptionPolicySetting> GetPoliciesOptionTemplates(RPRouteOptionPolicyFilter filter)
		{
			var extensionConfigManager = new ExtensionConfigurationManager();
			IEnumerable<RPRouteOptionPolicySetting> cachedConfigs = extensionConfigManager.GetExtensionConfigurations<RPRouteOptionPolicySetting>(Constants.SupplierZoneToRPOptionConfigType);

			if (filter == null)
				return cachedConfigs.OrderBy(x => x.Title);

			Guid defaultPolicyId;
			var routingDatabase = GetLatestRoutingDatabase(filter.RoutingDatabaseId);


			IEnumerable<Guid> selectedPolicyIds = this.GetRoutingDatabasePolicyIds(routingDatabase.ID, out defaultPolicyId);
			Func<RPRouteOptionPolicySetting, bool> filterExpression = (itm) => selectedPolicyIds.Contains(itm.ExtensionConfigurationId);

			IEnumerable<RPRouteOptionPolicySetting> cachedFilteredConfigs = cachedConfigs.FindAllRecords(filterExpression);

			if (cachedFilteredConfigs == null || cachedFilteredConfigs.Count() == 0)
				throw new NullReferenceException("cachedFilteredConfigs");

			List<RPRouteOptionPolicySetting> filteredConfigs = new List<RPRouteOptionPolicySetting>();

			// Set the default policy
			bool isDefaultPolicySet = false;
			foreach (RPRouteOptionPolicySetting config in cachedFilteredConfigs)
			{
				RPRouteOptionPolicySetting item = new RPRouteOptionPolicySetting()
				{
					BehaviorFQTN = config.BehaviorFQTN,
					Editor = config.Editor,
					ExtensionConfigurationId = config.ExtensionConfigurationId,
					IsDefault = config.IsDefault,
					Name = config.Name,
					Title = config.Title
				};

				if (item.ExtensionConfigurationId == defaultPolicyId)
				{
					item.IsDefault = true;
					isDefaultPolicySet = true;
				}
				filteredConfigs.Add(item);
			}

			if (!isDefaultPolicySet)
				throw new DataIntegrityValidationException(String.Format("RPRoutingDatabase '{0}' does not have a default policy", filter.RoutingDatabaseId));

			return filteredConfigs.OrderBy(x => x.Title);
		}

		#endregion

		#region Private Members

		private RoutingDatabase GetLatestRoutingDatabase(int routingDatabaseId)
		{
			RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();

			var routingDatabase = routingDatabaseManager.GetRoutingDatabase(routingDatabaseId);

			if (routingDatabase == null)//in case of deleted database
				routingDatabase = routingDatabaseManager.GetRoutingDatabaseFromDB(routingDatabaseId);

			if (routingDatabase == null)
				throw new NullReferenceException(string.Format("routingDatabase. RoutingDatabaseId:{0}", routingDatabaseId));

			var productRoutingDatabases = routingDatabaseManager.GetRoutingDatabasesReady(routingDatabase.ProcessType, routingDatabase.Type).OrderByDescending(itm => itm.CreatedTime);
			return productRoutingDatabases.First();
		}

		private RPRouteDetail RPRouteDetailMapper(RPRoute rpRoute, Guid policyConfigId, int numberOfOptions, int? systemCurrencyId, int? toCurrencyId, bool includeBlockedSupplierZones, int? customerProfileId)
		{
			return new RPRouteDetail()
			{
				RoutingProductId = rpRoute.RoutingProductId,
				SaleZoneId = rpRoute.SaleZoneId,
                SaleZoneServiceIds = rpRoute.SaleZoneServiceIds,
				RoutingProductName = _routingProductManager.GetRoutingProductName(rpRoute.RoutingProductId),
				SaleZoneName = _saleZoneManager.GetSaleZoneName(rpRoute.SaleZoneId),
				IsBlocked = rpRoute.IsBlocked,
				RouteOptionsDetails = this.GetRouteOptionDetails(rpRoute.RPOptionsByPolicy, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, includeBlockedSupplierZones, customerProfileId),
				ExecutedRuleId = rpRoute.ExecutedRuleId
			};
		}

		private RPRouteOptionDetail RPRouteOptionMapper(RPRouteOption routeOption, int? systemCurrencyId, int? toCurrencyId, int optionOrder)
		{
			if (routeOption == null)
				return null;

			var routeOptionDetail = new RPRouteOptionDetail()
			{
				Entity = routeOption,
				SupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId),
				ConvertedSupplierRate = routeOption.SupplierRate,
				OptionOrder = optionOrder
			};

			if (toCurrencyId.HasValue)
			{
				if (!systemCurrencyId.HasValue)
					throw new ArgumentNullException("systemCurrencyId");
				routeOptionDetail.ConvertedSupplierRate = GetRateConvertedToCurrency(routeOption.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, DateTime.Now);
			}

			return routeOptionDetail;
		}

		private RPRouteOptionSupplierZoneDetail RPRouteOptionSupplierZoneDetailMapper(RPRouteOptionSupplierZone rpRouteOptionSupplierZone, int? systemCurrencyId, int? toCurrencyId)
		{
			SupplierZoneManager manager = new SupplierZoneManager();
			SupplierZone supplierZone = manager.GetSupplierZone(rpRouteOptionSupplierZone.SupplierZoneId);

			var detailEntity = new RPRouteOptionSupplierZoneDetail()
			{
				Entity = rpRouteOptionSupplierZone,
				SupplierZoneName = supplierZone != null ? supplierZone.Name : null,
				ConvertedSupplierRate = rpRouteOptionSupplierZone.SupplierRate
			};

			if (toCurrencyId.HasValue)
			{
				if (!systemCurrencyId.HasValue)
					throw new ArgumentNullException("systemCurrencyId");
				detailEntity.ConvertedSupplierRate = GetRateConvertedToCurrency(rpRouteOptionSupplierZone.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, DateTime.Now);
			}

			return detailEntity;
		}

		private IEnumerable<RPRouteOptionDetail> GetRouteOptionDetails(Dictionary<Guid, IEnumerable<RPRouteOption>> dicRouteOptions, Guid policyConfigId, int numberOfOptions, int? systemCurrencyId, int? toCurrencyId, bool includeBlockedSupplierZones, int? customerProfileId)
		{
			if (dicRouteOptions == null || !dicRouteOptions.ContainsKey(policyConfigId))
				return null;

			IEnumerable<RPRouteOption> rpRouteOptions;
			dicRouteOptions.TryGetValue(policyConfigId, out rpRouteOptions);

			if (rpRouteOptions == null)
				return null;

			Func<RPRouteOption, bool> filterFunc = (rpRouteOption) =>
			{
				if (!includeBlockedSupplierZones && rpRouteOption.SupplierStatus == SupplierStatus.Block)
					return false;
				if (customerProfileId.HasValue && GetCarrierProfileId(rpRouteOption.SupplierId) == customerProfileId.Value)
					return false;
				return true;
			};

			int counter = 0;
			return rpRouteOptions.FindAllRecords(x => filterFunc(x)).Take(numberOfOptions).MapRecords(x => RPRouteOptionMapper(x, systemCurrencyId, toCurrencyId, counter++));
		}

		private int GetCarrierProfileId(int carrierAccountId)
		{
			int? carrierProfileId = _carrierAccountManager.GetCarrierProfileId(carrierAccountId);
			if (!carrierProfileId.HasValue)
				throw new NullReferenceException(String.Format("CarrierAccount '{0}' was not found", carrierAccountId));
			return carrierProfileId.Value;
		}

		private IEnumerable<Guid> GetRoutingDatabasePolicyIds(int routingDbId, out Guid defaultPolicyId)
		{
			var routingDbManager = new RoutingDatabaseManager();
			var routingDbInfoFilter = new RoutingDatabaseInfoFilter() { ProcessType = RoutingProcessType.RoutingProductRoute };
			IEnumerable<RoutingDatabaseInfo> routingDbInfoEntities = routingDbManager.GetRoutingDatabaseInfo(routingDbInfoFilter);

			RoutingDatabaseInfo routingDbInfo = routingDbInfoEntities.FindRecord(itm => itm.RoutingDatabaseId == routingDbId);
			if (routingDbInfo == null)
				throw new NullReferenceException("routingDbInfo");
			if (routingDbInfo.Information == null)
				throw new NullReferenceException("routingDbInfo.Information");

			var rpRoutingDbInfo = routingDbInfo.Information as RPRoutingDatabaseInformation;
			if (rpRoutingDbInfo == null)
				throw new NullReferenceException("rpRoutingDbInfo");
			if (rpRoutingDbInfo.SelectedPoliciesIds == null || rpRoutingDbInfo.SelectedPoliciesIds.Count() == 0)
				throw new NullReferenceException("rpRoutingDbInfo.SelectedPoliciesIds");

			defaultPolicyId = rpRoutingDbInfo.DefaultPolicyId;
			return rpRoutingDbInfo.SelectedPoliciesIds;
		}

		private bool GetIncludeBlockedSupplierZones(RoutingDatabase routingDatabase)
		{
			if (routingDatabase.Information == null)
				throw new NullReferenceException("routingDatabase.Information");

			RPRoutingDatabaseInformation rpRoutingDatabaseInformation = routingDatabase.Information as RPRoutingDatabaseInformation;

			return rpRoutingDatabaseInformation.IncludeBlockedSupplierZones;
		}

		private int GetSystemCurrencyId()
		{
			var currencyManager = new CurrencyManager();
			Currency systemCurrency = currencyManager.GetSystemCurrency();
			if (systemCurrency == null)
				throw new NullReferenceException("systemCurrency");
			return systemCurrency.CurrencyId;
		}

		private decimal GetRateConvertedToCurrency(decimal rate, int systemCurrencyId, int toCurrencyId, DateTime effectiveOn)
		{
			return _currencyExchangeRateManager.ConvertValueToCurrency(rate, systemCurrencyId, toCurrencyId, effectiveOn);
		}

		#endregion
	}
}
