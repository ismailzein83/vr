"use strict";

app.directive("vrWhsBeSalezoneGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SaleZoneAPIService", "WhS_BE_SaleZoneService", "VRUIUtilsService", function (UtilsService, VRNotificationService, WhS_BE_SaleZoneAPIService, WhS_BE_SaleZoneService, VRUIUtilsService) {

	return {
		restrict: "E",
		scope: {
			onReady: "="
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var saleZoneGrid = new SaleZoneGrid($scope, ctrl, $attrs);
			saleZoneGrid.initializeController();
		},
		controllerAs: "ctrl",
		bindToController: true,
		templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SaleZone/Templates/SaleZoneGridTemplate.html"
	};

	function SaleZoneGrid($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var gridAPI;
		var gridDrillDownTabsObj;

		var effectiveOn;
		var getEffectiveAfter;

		function initializeController() {

			$scope.salezones = [];
			$scope.showGrid = false;

			$scope.onGridReady = function (api) {
				gridAPI = api;
				var drillDownDefinitions = getDrillDownDefinitions();
				gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
				defineAPI();
			};
			$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
				return WhS_BE_SaleZoneAPIService.GetFilteredSaleZones(dataRetrievalInput).then(function (response) {
					$scope.showGrid = true;
					if (response && response.Data) {
						for (var i = 0; i < response.Data.length; i++)
							gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
					}
					onResponseReady(response);
				}).catch(function (error) {
					VRNotificationService.notifyException(error, $scope);
				});
			};
		}

		function defineAPI() {
			var api = {};

			api.loadGrid = function (query) {
				effectiveOn = query.EffectiveOn;
				getEffectiveAfter = query.GetEffectiveAfter;
				return gridAPI.retrieveData(query);
			};

			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function getDrillDownDefinitions() {

			var drillDownDefinitions = [];

			var saleCodeDrillDownDefinition = {
				title: "Sale Codes",
				directive: 'vr-whs-be-salecode-grid',
				loadDirective: function (saleCodeGridAPI, saleZoneDataItem) {
					saleZoneDataItem.saleCodeGridAPI = saleCodeGridAPI;
					var queryHandler = {
						$type: "TOne.WhS.BusinessEntity.Business.SaleCodeQueryHandler, TOne.WhS.BusinessEntity.Business"
					};
					queryHandler.Query = {
						ZonesIds: [saleZoneDataItem.Entity.SaleZoneId],
						EffectiveOn: effectiveOn,
						GetEffectiveAfter: getEffectiveAfter
					};
					var saleCodeGridPayload = {
						queryHandler: queryHandler,
						hidesalezonecolumn: true
					};
					return saleZoneDataItem.saleCodeGridAPI.loadGrid(saleCodeGridPayload);
				}
			};

			drillDownDefinitions.push(saleCodeDrillDownDefinition);
			return drillDownDefinitions;
		}
	}
}]);
