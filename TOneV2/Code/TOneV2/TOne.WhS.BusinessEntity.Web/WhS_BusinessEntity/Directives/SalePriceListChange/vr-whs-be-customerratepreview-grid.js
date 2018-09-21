'use strict';

app.directive('vrWhsBeCustomerratepreviewGrid', ['WhS_BE_SalePriceListChangeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_RateChangeTypeEnum', function (WhS_BE_SalePriceListChangeAPIService, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_RateChangeTypeEnum) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var customerRatePreviewGrid = new CustomerRatePreviewGrid($scope, ctrl, $attrs);
			customerRatePreviewGrid.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/CustomerRatePreviewGrid.html'
	};

	function CustomerRatePreviewGrid($scope, ctrl, $attrs) {
		this.initializeController = initializeController;

		var gridAPI;
		var gridDrillDownTabs;
		var processInstanceId;
		var customersIds;

		function initializeController() {
			$scope.scopeModel = {};
			$scope.scopeModel.ratePreviews = [];
			$scope.scopeModel.menuActions = [];

			$scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
				return WhS_BE_SalePriceListChangeAPIService.GetFilteredCustomerRatePreviews(dataRetrievalInput).then(function (response) {
					if (response != null && response.Data != null) {
						for (var i = 0; i < response.Data.length; i++)
							extendDataItem(response.Data[i]);
					}
					onResponseReady(response);
				}).catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				});
			};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				var drillDownDefinitions = getDrillDownDefinitions();
				gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, null);

				defineAPI();
			};


		}

		function defineAPI() {
			var api = {};

			api.load = function (query) {
				processInstanceId = query.ProcessInstanceId;
				if (query != null) {
					$scope.scopeModel.showCustomerName = query.ShowCustomerName;
				}
				return gridAPI.retrieveData(query);
			};
			api.gridHasData = function () {
				return ($scope.scopeModel.ratePreviews.length != 0) ? true : false;
			};
			api.cleanGrid = function () {
				$scope.scopeModel.ratePreviews.length = 0;
			};
			if (ctrl.onReady != null)
				ctrl.onReady(api);
		}

		function getDrillDownDefinitions() {
			return [{
				title: "Other Rates",
				directive: "vr-whs-be-otherratespreview-grid",
				loadDirective: function (otherRatePreviewGridAPI, dataItem) {
					dataItem.otherRatePreviewGridAPI = otherRatePreviewGridAPI;
					var queryHandler = {
						$type: "TOne.WhS.Sales.Business.RPOtherRatesPreviewHandler, TOne.WhS.Sales.Business"
					};
					queryHandler.Query = {
						ProcessInstanceId: processInstanceId,
						ZoneName: dataItem.ZoneName,
						CustomerId: dataItem.CustomerId
					};

					return otherRatePreviewGridAPI.load(queryHandler);
				}
			}];
		}

		function extendDataItem(dataItem) {
			gridDrillDownTabs.setDrillDownExtensionObject(dataItem);

			var rateChangeType = UtilsService.getEnum(WhS_BE_RateChangeTypeEnum, 'value', dataItem.ChangeType);

			if (rateChangeType != undefined) {
				dataItem.RateChangeTypeIconType = rateChangeType.iconType;
				dataItem.RateChangeTypeIconUrl = rateChangeType.iconUrl;
				dataItem.RateChangeTypeIconTooltip = rateChangeType.description;
			}
			if (dataItem.RecentCurrencyId != undefined && dataItem.CurrencyId != dataItem.RecentCurrencyId) {
				dataItem.RecentCurrencyIconType = 'exchange';
				dataItem.RecentCurrencyIconTooltip = 'The real currency for current rate is ' + dataItem.RecentCurrencySymbol;
			}
		}
	}
}]);