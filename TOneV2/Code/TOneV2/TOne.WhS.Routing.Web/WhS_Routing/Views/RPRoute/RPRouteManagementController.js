(function (appControllers) {

	"use strict";

	rpRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_Routing_RoutingProductFilterEnum', 'WhS_Routing_RouteFilterEnum'];

	function rpRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Routing_RoutingProductFilterEnum, WhS_Routing_RouteFilterEnum) {

		var filterSettingsData;
		//var gridSettingsData;
		//var gridPersonalizationSettings;

		var actionBarAPI;
		var actionBarReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var currencyId;

		var gridAPI;
		var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var routingDatabaseSelectorAPI;
		var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var rpRoutePolicyAPI;
		var rpRoutePolicyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var saleZoneSelectorAPI;
		var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var singleRoutingProductSelectorAPI;
		var singleRoutingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var multipleRoutingProductSelectorAPI;
		var multipleRoutingProductReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var routeStatusSelectorAPI;
		var routeStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var customerSelectorAPI;
		var customerSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var loadRoutePolicyDataPromiseDeferred;

		defineScope();
		load();

		function defineScope() {
			$scope.showRPRouteGrid = true;
			$scope.numberOfOptions = 3;
			$scope.legendHeader = "Legend";
			$scope.legendContent = getLegendContent();

			$scope.onActionBarSettingsReady = function (api) {
				actionBarAPI = api;
				actionBarReadyPromiseDeferred.resolve();
			};

			$scope.onGridReady = function (api) {
				gridAPI = api;
				gridReadyPromiseDeferred.resolve();
			};

			$scope.onGridByCodeReady = function (api) {
				gridAPI = api;
				gridReadyPromiseDeferred.resolve();
			};

			$scope.onRoutingDatabaseSelectorReady = function (api) {
				routingDatabaseSelectorAPI = api;
				routingDatabaseReadyPromiseDeferred.resolve();
			};

			$scope.onRPRoutePolicySelectorReady = function (api) {
				rpRoutePolicyAPI = api;
				rpRoutePolicyReadyPromiseDeferred.resolve();
			};

			$scope.onSaleZoneSelectorReady = function (api) {
				saleZoneSelectorAPI = api;
				saleZoneReadyPromiseDeferred.resolve();
			};

			$scope.onMultipleRoutingProductSelectorReady = function (api) {
				multipleRoutingProductSelectorAPI = api;
				multipleRoutingProductReadyPromiseDeferred.resolve();
			};

			$scope.onSingleRoutingProductSelectorReady = function (api) {
				singleRoutingProductSelectorAPI = api;
				singleRoutingProductReadyPromiseDeferred.resolve();
			};

			$scope.onRouteStatusDirectiveReady = function (api) {
				routeStatusSelectorAPI = api;
				routeStatusSelectorReadyPromiseDeferred.resolve();
			};

			$scope.onCustomerSelectorReady = function (api) {
				customerSelectorAPI = api;
				customerSelectorReadyPromiseDeferred.resolve();
			};

			$scope.onRoutingProductFilterSelectorReady = function (api) {
			};

			$scope.onRouteFilterSelectorReady = function (api) {
			};

			$scope.onRoutingDatabaseSelectorChange = function () {
				var selectedId = routingDatabaseSelectorAPI.getSelectedIds();

				if (selectedId == undefined)
					return;
				loadRoutePolicyDataPromiseDeferred = undefined;
				loadRoutePolicyDataPromiseDeferred = UtilsService.createPromiseDeferred();
				var policySelectorPayload = {
					filter: {
						RoutingDatabaseId: selectedId
					},
					selectDefaultPolicy: true
				};

				var setLoader = function (value) {
					$scope.isLoadingFilterData = value;
				};
				//VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, rpRoutePolicyAPI, policySelectorPayload, setLoader, undefined);
				$scope.isLoadingFilterData = true;
				VRUIUtilsService.callDirectiveLoad(rpRoutePolicyAPI, policySelectorPayload, loadRoutePolicyDataPromiseDeferred);
				loadRoutePolicyDataPromiseDeferred.promise.then(function () { $scope.isLoadingFilterData = false; });
			};

			$scope.searchClicked = function () {
				gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();
				var oldShowRPRouteGrid = $scope.showRPRouteGrid;
				$scope.showRPRouteGrid = $scope.selectedRouteFilter == WhS_Routing_RouteFilterEnum.Zone;
				if (oldShowRPRouteGrid != $scope.showRPRouteGrid) {
					//   gridPersonalizationSettings = gridAPI.getPersonalizationItem();
					gridAPI = undefined;
				}

				if (gridAPI != undefined) {
					//if (gridSettingsData != undefined) {
					//    gridAPI.setPersonalizationItem(gridSettingsData);
					//}
					return gridAPI.loadGrid(getFilterObject());
				}
				else {
					var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
					gridReadyPromiseDeferred.promise.then(function () {
						//if (gridSettingsData != undefined) {
						//    gridAPI.setPersonalizationItem(gridSettingsData);
						//}
						gridAPI.loadGrid(getFilterObject()).then(function () {
							loadGridPromiseDeferred.resolve();
						});
					});
					return loadGridPromiseDeferred.promise;
				}
			};

			function getLegendContent() {
				return '<div style="font-size:12px; margin:10px">' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FF0000; margin: 0px 3px"></div> Blocked </div>' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FFA500; margin: 0px 3px"></div> Lossy </div>' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #0000FF; margin: 0px 3px"></div> Forced </div>' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #28A744; margin: 0px 3px"></div> Market Price </div>' +
					'</div>';
			}

		}

		function load() {
			$scope.isLoading = true;
			loadAllControls()
				.catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				})
				.finally(function () {
					$scope.isLoading = false;
				});
		}

		function loadAllControls() {
			//var intialGridPersonalizationSettingsPromiseDeferred = UtilsService.createPromiseDeferred();
			var rootPromiseNode = {
				promises: [actionBarReadyPromiseDeferred.promise],
				getChildNode: function () {
					var loadActionBarPromise = actionBarAPI.load(buildactionBarPayload()).then(function (settingsData) {
						filterSettingsData = settingsData.getItemByUniqueName("WhSRPRouteFilter");
						if (filterSettingsData != undefined && filterSettingsData.RouteFilter == WhS_Routing_RouteFilterEnum.Code.value)
							$scope.showRPRouteGrid = false;
						//gridSettingsData = settingsData.getItemByUniqueName("WhSRPRouteGrid");

						//if (filterSettingsData != undefined && gridSettingsData != undefined) {
						//    if (filterSettingsData.RouteFilter == WhS_Routing_RouteFilterEnum.Zone.value) {
						//        gridPersonalizationSettings = gridSettingsData.CodeGridPersonalization;
						//    }
						//    else {
						//        gridPersonalizationSettings = gridSettingsData.ZoneGridPersonalization;
						//        $scope.showRPRouteGrid = false;
						//    }
						//}
						//intialGridPersonalizationSettingsPromiseDeferred.resolve();
					});
					return {
						promises: [loadActionBarPromise],
						getChildNode: function () {
							var loadFilterPromise = UtilsService.waitMultiplePromises([rpRoutePolicyReadyPromiseDeferred.promise]);//, intialGridPersonalizationSettingsPromiseDeferred.promise]);
							return {
								promises: [loadFilterPromise],
								getChildNode: function () {
									var loadStaticDataPromise = UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadStaticData, loadRoutingProductFilters, loadSingleRoutingProductSelector, loadMultipleRoutingProductSelector, loadSaleZoneSection, loadRouteStatusSelector, loadCustomerSelector]);
									return {
										promises: [loadStaticDataPromise, gridReadyPromiseDeferred.promise],
										getChildNode: function () {
											//gridAPI.setPersonalizationItem(gridSettingsData);
											//var loadGridPromise = gridAPI.loadGrid(getFilterObject());

											var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
											if (loadRoutePolicyDataPromiseDeferred != undefined)
												loadRoutePolicyDataPromiseDeferred.promise.then(function () { gridAPI.loadGrid(getFilterObject()).then(function () { rodiLoadPromise.resolve(); }) });
											else gridAPI.loadGrid(getFilterObject()).then(function () { loadGridPromiseDeferred.resolve(); });

											return {
												//promises: [loadGridPromise]
												promises: [loadGridPromiseDeferred.promise]
											};
										}
									};
								}
							};
						}
					};
				}
			};
			return UtilsService.waitPromiseNode(rootPromiseNode);

		}

		function loadRoutingDatabaseSelector() {

			var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

			routingDatabaseReadyPromiseDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(routingDatabaseSelectorAPI, undefined, loadRoutingDatabasePromiseDeferred);
			});

			return loadRoutingDatabasePromiseDeferred.promise;
		}

		function loadRouteFilters() {

			$scope.routeFilters = UtilsService.getArrayEnum(WhS_Routing_RouteFilterEnum);

			if (filterSettingsData == undefined) {
				$scope.selectedRouteFilter = $scope.routeFilters[0];
				return;
			}
			$scope.selectedRouteFilter = UtilsService.getItemByVal($scope.routeFilters, filterSettingsData.RouteFilter, 'value');
		}

		function loadStaticData() {
			loadRouteFilters();
			if (filterSettingsData == undefined) {
				$scope.limit = 1000;
				$scope.includeBlockedSuppliers = true;
				return;
			}

			$scope.limit = filterSettingsData.LimitResult;
			$scope.numberOfOptions = filterSettingsData.NumberOfOptions;

			if (filterSettingsData.MaxSupplierRate != undefined) {
				$scope.maxSupplierRate = filterSettingsData.MaxSupplierRate;
			}

			$scope.includeBlockedSuppliers = filterSettingsData.IncludeBlockedSuppliers;
			$scope.codePrefix = filterSettingsData.CodePrefix;
			$scope.showInSystemCurrency = filterSettingsData.ShowInSystemCurrency;
		}
		function loadRoutingProductFilters() {

			$scope.routingProductFilters = UtilsService.getArrayEnum(WhS_Routing_RoutingProductFilterEnum);

			if (filterSettingsData == undefined) {
				$scope.selectedRoutingProductFilter = $scope.routingProductFilters[0];
				return;
			}
			$scope.selectedRoutingProductFilter = UtilsService.getItemByVal($scope.routingProductFilters, filterSettingsData.RoutingProductFilter, 'value');
		}
		function loadSingleRoutingProductSelector() {

			var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

			singleRoutingProductReadyPromiseDeferred.promise.then(function () {

				var payload;
				if (filterSettingsData != undefined) {
					var routingProductFilter = UtilsService.getItemByVal($scope.routingProductFilters, filterSettingsData.RoutingProductFilter, 'value');
					payload = {
						filter: routingProductFilter
					};
					if (filterSettingsData.SimulatedRoutingProductId != undefined) {
						payload.selectedIds = filterSettingsData.SimulatedRoutingProductId;
					}
				}
				VRUIUtilsService.callDirectiveLoad(singleRoutingProductSelectorAPI, payload, loadRoutingProductPromiseDeferred);
			});

			return loadRoutingProductPromiseDeferred.promise;
		}
		function loadMultipleRoutingProductSelector() {

			var loadRoutingProductPromiseDeferred = UtilsService.createPromiseDeferred();

			multipleRoutingProductReadyPromiseDeferred.promise.then(function () {

				var payload;
				if (filterSettingsData != undefined) {
					var routingProductFilter = UtilsService.getItemByVal($scope.routingProductFilters, filterSettingsData.RoutingProductFilter, 'value');
					payload = {
						selectedIds: filterSettingsData.RoutingProductIds,
						filter: routingProductFilter
					};
				}
				VRUIUtilsService.callDirectiveLoad(multipleRoutingProductSelectorAPI, payload, loadRoutingProductPromiseDeferred);
			});

			return loadRoutingProductPromiseDeferred.promise;
		}
		function loadSaleZoneSection() {

			var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

			saleZoneReadyPromiseDeferred.promise.then(function () {

				var payload;
				if (filterSettingsData != undefined)
					payload = {
						selectedIds: filterSettingsData.SaleZoneIds
					};
				VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, payload, loadSaleZonePromiseDeferred);
			});

			return loadSaleZonePromiseDeferred.promise;
		}
		function loadRouteStatusSelector() {

			var loadRouteStatusPromiseDeferred = UtilsService.createPromiseDeferred();

			routeStatusSelectorReadyPromiseDeferred.promise.then(function () {

				var payload;
				if (filterSettingsData != undefined && filterSettingsData.RouteStatus != undefined)
					payload = {
						selectedIds: filterSettingsData.RouteStatus
					};
				VRUIUtilsService.callDirectiveLoad(routeStatusSelectorAPI, payload, loadRouteStatusPromiseDeferred);
			});

			return loadRouteStatusPromiseDeferred.promise;
		}
		function loadCustomerSelector() {

			var loadCustomerPromiseDeferred = UtilsService.createPromiseDeferred();

			customerSelectorReadyPromiseDeferred.promise.then(function () {

				var payload;
				if (filterSettingsData != undefined && filterSettingsData.CustomerId != undefined) {
					payload = {
						selectedIds: filterSettingsData.CustomerId
					};
				}
				VRUIUtilsService.callDirectiveLoad(customerSelectorAPI, payload, loadCustomerPromiseDeferred);
			});

			return loadCustomerPromiseDeferred.promise;
		}

		function getFilterObject() {
			currencyId = null;

			if (!$scope.showInSystemCurrency && $scope.selectedCustomer != undefined)
				currencyId = $scope.selectedCustomer.CurrencyId;

			var query = {
				RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
				PolicyConfigId: rpRoutePolicyAPI.getSelectedIds(),
				NumberOfOptions: $scope.numberOfOptions,
				RoutingProductIds: $scope.selectedRoutingProductFilter.value == 0 || $scope.selectedCustomer == undefined ? multipleRoutingProductSelectorAPI.getSelectedIds() : undefined,
				SimulatedRoutingProductId: $scope.selectedRoutingProductFilter.value == 1 && $scope.selectedCustomer != undefined ? singleRoutingProductSelectorAPI.getSelectedIds() : undefined,
				SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
				FilteredPolicies: rpRoutePolicyAPI.getFilteredPoliciesIds(),
				RouteStatus: routeStatusSelectorAPI.getSelectedIds(),
				LimitResult: $scope.limit,
				CustomerId: customerSelectorAPI.getSelectedIds(),
				ShowInSystemCurrency: $scope.showInSystemCurrency,
				CurrencyId: currencyId,
				IncludeBlockedSuppliers: $scope.includeBlockedSuppliers,
				MaxSupplierRate: $scope.maxSupplierRate,
				CodePrefix: $scope.codePrefix
			};
			return query;
		}

		function buildactionBarPayload() {
			var uniqueKeys = [{
				EntityUniqueName: "WhSRPRouteFilter",
				DisplayName: "Filter"
			}, {
				EntityUniqueName: "WhSRPRouteGrid",
				DisplayName: "Grid"
			}];
			var context = {
				getPersonalizationItems: function () {
					var items = [];
					var selectedRouteFilter = $scope.selectedRouteFilter;
					var selectedRoutingProductFilter = $scope.selectedRoutingProductFilter;
					var multipleRoutingProductSelectedIds = multipleRoutingProductSelectorAPI.getSelectedIds();
					var singleRoutingProductSelectedIds = singleRoutingProductSelectorAPI.getSelectedIds();
					var saleZoneSelectedIds = saleZoneSelectorAPI.getSelectedIds();
					var routeStatusSelectedIds = routeStatusSelectorAPI.getSelectedIds();
					var customerSelectedIds = customerSelectorAPI.getSelectedIds();

					if (selectedRouteFilter != undefined || $scope.numberOfOptions || selectedRoutingProductFilter != undefined
						|| multipleRoutingProductSelectedIds != undefined || singleRoutingProductSelectedIds != undefined
						|| saleZoneSelectedIds != undefined || routeStatusSelectedIds != undefined || $scope.limit
						|| customerSelectedIds != undefined || $scope.showInSystemCurrency || $scope.includeBlockedSuppliers
						|| $scope.maxSupplierRate || $scope.codePrefix) {

						items.push({
							EntityUniqueName: "WhSRPRouteFilter",
							ExtendedSetting: {
								"$type": "TOne.WhS.Routing.Business.RPRouteFilterPersonalizationExtendedSetting, TOne.WhS.Routing.Business",
								RouteFilter: selectedRouteFilter.value,
								NumberOfOptions: $scope.numberOfOptions,
								RoutingProductFilter: selectedRoutingProductFilter.value,
								RoutingProductIds: multipleRoutingProductSelectedIds,
								SimulatedRoutingProductId: singleRoutingProductSelectedIds,
								SaleZoneIds: saleZoneSelectedIds,
								RouteStatus: routeStatusSelectedIds,
								LimitResult: $scope.limit,
								CustomerId: customerSelectedIds,
								ShowInSystemCurrency: $scope.showInSystemCurrency,
								IncludeBlockedSuppliers: $scope.includeBlockedSuppliers,
								MaxSupplierRate: $scope.maxSupplierRate,
								CodePrefix: $scope.codePrefix
							}
						});
					}
					//if (gridAPI && gridAPI.getPersonalizationItem() != null) {
					//    var zoneGridPersonalization;
					//    var codeGridPersonalization;

					//    if ($scope.showRPRouteGrid) {
					//        zoneGridPersonalization = gridAPI.getPersonalizationItem();
					//        codeGridPersonalization = gridPersonalizationSettings;
					//    }
					//    else {
					//        zoneGridPersonalization = gridPersonalizationSettings;
					//        codeGridPersonalization = gridAPI.getPersonalizationItem();
					//    }

					//    items.push({
					//        EntityUniqueName: "WhSRPRouteGrid",
					//        ExtendedSetting: {
					//            "$type": "TOne.WhS.Routing.Business.RPRouteGridPersonalizationExtendedSetting, TOne.WhS.Routing.Business",
					//            ZoneGridPersonalization: zoneGridPersonalization,
					//            CodeGridPersonalization: codeGridPersonalization
					//        }
					//    });

					//}
					return items;
				}
			};
			return {
				uniqueKeys: uniqueKeys,
				context: context
			};
		}
	}

	appControllers.controller('WhS_Routing_RPRouteManagementController', rpRouteManagementController);
})(appControllers);