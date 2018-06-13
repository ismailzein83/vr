(function (appControllers) {

	"use strict";

	customerRouteManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRNavigationService'];

	function customerRouteManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, VRNavigationService) {

		var gridAPI;
		var gridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var routingDatabaseSelectorAPI;
		var routingDatabaseReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var saleZoneSelectorAPI;
		var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var carrierAccountDirectiveAPI;
		var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var routeStatusSelectorAPI;
		var routeStatusSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var parametersCustomersIds;
		var parametersZoneIds;

		loadParameters();
		defineScope();
		load();

		function loadParameters() {
			var parameters = VRNavigationService.getParameters($scope);

			if (parameters != null) {
				parametersCustomersIds = parameters.CustomersIds;
				parametersZoneIds = parameters.ZoneIds;
			}
		}

		function defineScope() {
			$scope.legendHeader = "Legend";
			$scope.legendContent = getLegendContent();

			$scope.onGridReady = function (api) {
				gridAPI = api;
				gridReadyPromiseDeferred.resolve();
			};

			$scope.onRoutingDatabaseSelectorReady = function (api) {
				routingDatabaseSelectorAPI = api;
				routingDatabaseReadyPromiseDeferred.resolve();
			};

			$scope.onSaleZoneSelectorReady = function (api) {
				saleZoneSelectorAPI = api;
				saleZoneReadyPromiseDeferred.resolve();
			};

			$scope.onCarrierAccountDirectiveReady = function (api) {
				carrierAccountDirectiveAPI = api;
				carrierAccountReadyPromiseDeferred.resolve();
			};

			$scope.onRouteStatusDirectiveReady = function (api) {
				routeStatusSelectorAPI = api;
				routeStatusSelectorReadyPromiseDeferred.resolve();
			};

			$scope.searchClicked = function () {
				if (gridAPI != undefined)
					return gridAPI.loadGrid(getFilterObject());
			};

			function getLegendContent() {
				return '<div style="font-size:12px; margin:10px">' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FF0000; margin: 0px 3px"></div> Blocked </div>' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #FFA500; margin: 0px 3px"></div> Lossy </div>' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #0000FF; margin: 0px 3px"></div> Forced </div>' +
					'<div><div style="display: inline-block; width: 20px; height: 10px; background-color: #28A744; margin: 0px 3px"></div> Market Price </div>' +
					'</div>';
			}

			function getFilterObject() {

				var query = {
					isDatabaseTypeCurrent: routingDatabaseSelectorAPI.isDatabaseTypeCurrent(),
					RoutingDatabaseId: routingDatabaseSelectorAPI.getSelectedIds(),
					SaleZoneIds: saleZoneSelectorAPI.getSelectedIds(),
					Code: $scope.code,
					CustomerIds: carrierAccountDirectiveAPI.getSelectedIds(),
					RouteStatus: routeStatusSelectorAPI.getSelectedIds(),
					LimitResult: $scope.limit,
					IncludeBlockedSuppliers: $scope.includeBlockedSuppliers
				};
				return query;
			}
		}
		function load() {
			$scope.isLoadingFilterData = true;
			$scope.limit = 1000;
			$scope.includeBlockedSuppliers = true;
			var promiseDeferred = UtilsService.createPromiseDeferred();
			UtilsService.waitMultipleAsyncOperations([loadRoutingDatabaseSelector, loadSaleZoneSection, loadCustomersSection, loadRouteStatusSelector])
				.then(function () {
					gridReadyPromiseDeferred.promise.then(function () { $scope.searchClicked(); promiseDeferred.resolve(); });
				})
				.catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});

			return promiseDeferred.promise.then(function () {
				$scope.isLoadingFilterData = false;
			});
		}

		function loadRoutingDatabaseSelector() {
			var loadRoutingDatabasePromiseDeferred = UtilsService.createPromiseDeferred();

			routingDatabaseReadyPromiseDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(routingDatabaseSelectorAPI, undefined, loadRoutingDatabasePromiseDeferred);
			});

			return loadRoutingDatabasePromiseDeferred.promise;
		}
		function loadSaleZoneSection() {
			var loadSaleZonePromiseDeferred = UtilsService.createPromiseDeferred();

			var payload;
			if (parametersZoneIds != null)
				payload = { selectedIds: parametersZoneIds };

			saleZoneReadyPromiseDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(saleZoneSelectorAPI, payload, loadSaleZonePromiseDeferred);
			});

			return loadSaleZonePromiseDeferred.promise;
		}
		function loadCustomersSection() {
			var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

			var payload;
			if (parametersCustomersIds != null)
				payload = { selectedIds: parametersCustomersIds };

			carrierAccountReadyPromiseDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, payload, loadCarrierAccountPromiseDeferred);
			});

			return loadCarrierAccountPromiseDeferred.promise;
		}
		function loadRouteStatusSelector() {
			var loadRouteStatusPromiseDeferred = UtilsService.createPromiseDeferred();

			routeStatusSelectorReadyPromiseDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(routeStatusSelectorAPI, undefined, loadRouteStatusPromiseDeferred);
			});

			return loadRouteStatusPromiseDeferred.promise;
		}
	}

	appControllers.controller('WhS_Routing_CustomerRouteManagementController', customerRouteManagementController);

})(appControllers);