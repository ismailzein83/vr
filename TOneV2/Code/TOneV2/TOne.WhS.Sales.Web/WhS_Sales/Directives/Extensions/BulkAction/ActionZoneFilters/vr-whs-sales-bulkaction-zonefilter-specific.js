'use strict';

app.directive('vrWhsSalesBulkactionZonefilterSpecific', ['WhS_BE_SalePriceListOwnerTypeEnum', 'WhS_Sales_SpecificApplicableZoneEntityTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_SalePriceListOwnerTypeEnum, WhS_Sales_SpecificApplicableZoneEntityTypeEnum, UtilsService, VRUIUtilsService) {
	return {
		restrict: "E",
		scope: {
			onReady: "=",
			normalColNum: '@',
			isrequired: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var specificBulkActionZoneFilter = new SpecificBulkActionZoneFilter($scope, ctrl, $attrs);
			specificBulkActionZoneFilter.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Sales/Directives/Extensions/BulkAction/ActionZoneFilters/Templates/SpecificBulkActionZoneFilterTemplate.html'
	};

	function SpecificBulkActionZoneFilter($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var zoneFilter;
		var bulkActionContext;

		var entityTypeSelectorAPI;
		var entityTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var countrySelectorAPI;
		var countrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var saleZoneSelectorAPI;
		var saleZoneSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var gridAPI;
		var gridReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController() {

			$scope.scopeModel = {};
			$scope.scopeModel.gridDataSource = [];
			$scope.scopeModel.entityTypes = UtilsService.getArrayEnum(WhS_Sales_SpecificApplicableZoneEntityTypeEnum);
			$scope.scopeModel.selectedEntityType = UtilsService.getItemByVal($scope.scopeModel.entityTypes, WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value, 'value');

			$scope.scopeModel.onEntityTypeSelectorReady = function (api) {
				entityTypeSelectorAPI = api;
				entityTypeSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onEntityTypeSelectionChanged = function (selectedEntityType) {
				if (selectedEntityType == undefined)
					return;
				$scope.scopeModel.selectedCountry = undefined;
				$scope.scopeModel.isSaleZoneSelectorVisible = (selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value);
				loadSaleZoneSelector();
			};

			$scope.scopeModel.onCountrySelectorReady = function (api) {
				countrySelectorAPI = api;
				countrySelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onCountrySelectionChanged = function (selectedCountry) {
				if ($scope.scopeModel.selectedEntityType != undefined && $scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value)
					loadSaleZoneSelector();
			};

			$scope.scopeModel.onSaleZoneSelectorReady = function (api) {
				saleZoneSelectorAPI = api;
				saleZoneSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.isAddButtonDisabled = function () {
				if ($scope.scopeModel.selectedEntityType == undefined) {
					return true;
				}
				else if ($scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value) {
					return ($scope.scopeModel.selectedCountry == undefined);
				}
				else {
					return ($scope.scopeModel.selectedSaleZones == undefined || $scope.scopeModel.selectedSaleZones.length == 0);
				}
			};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridReadyDeferred.resolve();
			};

			$scope.scopeModel.add = function ()
			{
				if ($scope.scopeModel.selectedEntityType == undefined)
					return;

				var entityTypeValue = $scope.scopeModel.selectedEntityType.value;
				var entityTypeDescription = $scope.scopeModel.selectedEntityType.description;

				if ($scope.scopeModel.selectedEntityType.value == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value)
				{
					var entity = {
						id: ($scope.scopeModel.gridDataSource.length + 1),
						entityTypeValue: entityTypeValue,
						entityTypeDescription: entityTypeDescription,
						isCountryEntityType: true
					};
					if ($scope.scopeModel.selectedCountry != undefined) {
						entity.entityId = $scope.scopeModel.selectedCountry.CountryId;
						entity.entityName = $scope.scopeModel.selectedCountry.Name;
					}
					entity.onSaleZoneSelectorReady = function (api) {
						entity.saleZoneSelectorAPI = api;

						$scope.scopeModel.isLoading = true;
						var saleZoneSelectorPayload = getSaleZoneSelectorPayload();

						entity.saleZoneSelectorAPI.load(saleZoneSelectorPayload).finally(function () {
							loadCountrySelector().finally(function () {
								$scope.scopeModel.isLoading = false;
							});
						});
					};
					$scope.scopeModel.gridDataSource.push({ Entity: entity });
				}
				else if ($scope.scopeModel.selectedSaleZones != undefined)
				{
					for (var i = 0; i < $scope.scopeModel.selectedSaleZones.length; i++)
					{
						var saleZone = $scope.scopeModel.selectedSaleZones[i];
						var entity = {
							id: ($scope.scopeModel.gridDataSource.length + 1),
							entityTypeValue: entityTypeValue,
							entityTypeDescription: entityTypeDescription,
							entityId: saleZone.SaleZoneId,
							entityName: saleZone.Name
						};
						$scope.scopeModel.gridDataSource.push({ Entity: entity });
					}
					$scope.scopeModel.isLoading = true;
					loadSaleZoneSelector().finally(function () {
						$scope.scopeModel.isLoading = false;
					});
				}
			};

			$scope.scopeModel.remove = function (dataRow) {
				var entities = UtilsService.getPropValuesFromArray($scope.scopeModel.gridDataSource, 'Entity');
				if (entities == undefined)
					return;
				var index = UtilsService.getItemIndexByVal(entities, dataRow.Entity.id, 'id');
				$scope.scopeModel.gridDataSource.splice(index, 1);

				$scope.scopeModel.isLoading = true;
				UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadSaleZoneSelector]).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};

			$scope.scopeModel.isGridDataValid = function () {
				return ($scope.scopeModel.gridDataSource.length == 0) ? 'No filters exist' : null;
			};

			UtilsService.waitMultiplePromises([entityTypeSelectorReadyDeferred.promise, countrySelectorReadyDeferred.promise, saleZoneSelectorReadyDeferred.promise, gridReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}
		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var promises = [];

				if (payload != undefined) {
					zoneFilter = payload.zoneFilter;
					bulkActionContext = payload.bulkActionContext;
				}

				extendBulkActionContext();

				var loadCountrySelectorPromise = loadCountrySelector();
				promises.push(loadCountrySelectorPromise);

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function () {
				var data = {
					$type: 'TOne.WhS.Sales.MainExtensions.SpecificApplicableZones, TOne.WhS.Sales.MainExtensions',
					CountryZonesByCountry: {},
					IncludedZoneIds: []
				};
				for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
					var entity = $scope.scopeModel.gridDataSource[i].Entity;
					if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value)
						data.IncludedZoneIds.push(entity.entityId);
					else {
						data.CountryZonesByCountry[entity.entityId] = {
							CountryId: entity.entityId,
							ExcludedZoneIds: entity.saleZoneSelectorAPI.getSelectedIds()
						};
					}
				}
				return data;
			};

			if (ctrl.onReady != null) {
				ctrl.onReady(api);
			}
		}

		function extendBulkActionContext() {
			if (bulkActionContext == undefined)
				return;
			bulkActionContext.onBulkActionChanged = function () {
				$scope.scopeModel.gridDataSource.length = 0;
				$scope.scopeModel.isLoading = true;
				UtilsService.waitMultipleAsyncOperations([loadCountrySelector, loadSaleZoneSelector]).finally(function () {
					$scope.scopeModel.isLoading = false;
				});
			};
		}

		function loadCountrySelector() {
			var countrySelectorLoadDeferred = UtilsService.createPromiseDeferred();

			var countrySelectorPayload = {
				filter: {
					ExcludedCountryIds: getExcludedCountryIds(),
					Filters: getCountrySelectorFilters()
				}
			};
			VRUIUtilsService.callDirectiveLoad(countrySelectorAPI, countrySelectorPayload, countrySelectorLoadDeferred);

			return countrySelectorLoadDeferred.promise;
		}
		function getExcludedCountryIds() {
			var excludedCountryIds = [];
			for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
				var entity = $scope.scopeModel.gridDataSource[i].Entity;
				if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Country.value)
					excludedCountryIds.push(entity.entityId);
			}
			return excludedCountryIds;
		}
		function getCountrySelectorFilters() {
			var countrySelectorFilters = [];

			var ownerType;
			var ownerId;

			if (bulkActionContext != undefined) {
				ownerType = bulkActionContext.ownerType;
				ownerId = bulkActionContext.ownerId;
			}

			if (ownerType === WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
				var countrySoldToCustomerFilter = {
					$type: 'TOne.WhS.Sales.Business.CountrySoldToCustomerFilter, TOne.WhS.Sales.Business',
					CustomerId: bulkActionContext.ownerId,
					EffectiveOn: UtilsService.getDateFromDateTime(new Date()),
					IsEffectiveInFuture: false
				};
				countrySelectorFilters.push(countrySoldToCustomerFilter);
			}

			return countrySelectorFilters;
		}

		function loadSaleZoneSelector() {
			var saleZoneSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			var saleZoneSelectorPayload = getSaleZoneSelectorPayload();
			VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, saleZoneSelectorPayload, saleZoneSelectorLoadDeferred);

			return saleZoneSelectorLoadDeferred.promise;
		}
		function getSaleZoneSelectorPayload() {
			var saleZoneSelectorPayload = {
				filter: {}
			};

			if (bulkActionContext != undefined) {
				saleZoneSelectorPayload.sellingNumberPlanId = bulkActionContext.ownerSellingNumberPlanId;
			}

			var countryId = countrySelectorAPI.getSelectedIds();
			if (countryId != undefined) {
				saleZoneSelectorPayload.filter.CountryIds = [];
				saleZoneSelectorPayload.filter.CountryIds.push(countryId);
			}

			saleZoneSelectorPayload.filter.ExcludedZoneIds = getExcludedSaleZoneIds();
			saleZoneSelectorPayload.filter.Filters = getSaleZoneSelectorFilters();

			return saleZoneSelectorPayload;
		}
		function getExcludedSaleZoneIds() {
			var excludedSaleZoneIds = [];
			for (var i = 0; i < $scope.scopeModel.gridDataSource.length; i++) {
				var entity = $scope.scopeModel.gridDataSource[i].Entity;
				if (entity.entityTypeValue == WhS_Sales_SpecificApplicableZoneEntityTypeEnum.Zone.value)
					excludedSaleZoneIds.push(entity.entityId);
				else {
					var selectedSaleZoneIds = entity.saleZoneSelectorAPI.getSelectedIds();
					if (selectedSaleZoneIds != undefined) {
						for (var j = 0; j < selectedSaleZoneIds.length; j++)
							excludedSaleZoneIds.push(selectedSaleZoneIds[j]);
					}
				}
			}
			return excludedSaleZoneIds;
		}
		function getSaleZoneSelectorFilters() {
			var saleZoneSelectorFilters = [];

			var ownerType;
			var ownerId;
			var bulkAction;

			if (bulkActionContext != undefined) {
				ownerType = bulkActionContext.ownerType;
				ownerId = bulkActionContext.ownerId;
				if (bulkActionContext.getSelectedBulkAction != undefined)
					bulkAction = bulkActionContext.getSelectedBulkAction();
			}

			var applicableSaleZoneFilter = {
				$type: 'TOne.WhS.Sales.Business.ApplicableSaleZoneFilter, TOne.WhS.Sales.Business',
				OwnerType: ownerType,
				OwnerId: ownerId,
				ActionType: bulkAction
			};
			saleZoneSelectorFilters.push(applicableSaleZoneFilter);

			if (ownerType != undefined && ownerType == WhS_BE_SalePriceListOwnerTypeEnum.Customer.value) {
				var countrySoldToCustomerFilter = {
					$type: 'TOne.WhS.Sales.Business.SaleZoneCountrySoldToCustomerFilter, TOne.WhS.Sales.Business',
					CustomerId: ownerId,
					EffectiveOn: UtilsService.getDateFromDateTime(new Date()),
					IsEffectiveInFuture: false
				};
				saleZoneSelectorFilters.push(countrySoldToCustomerFilter);
			}

			return saleZoneSelectorFilters;
		}
	}
}]);