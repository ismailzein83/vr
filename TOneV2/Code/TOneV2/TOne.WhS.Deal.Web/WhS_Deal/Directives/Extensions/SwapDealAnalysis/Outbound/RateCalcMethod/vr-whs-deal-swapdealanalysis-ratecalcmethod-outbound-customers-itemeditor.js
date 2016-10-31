'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodOutboundCustomersItemeditor', ['WhS_Deal_CarrierFilterTypeEnum', 'UtilsService', function (WhS_Deal_CarrierFilterTypeEnum, UtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var customersOutboundRateCalcMethodItem = new CustomersOutboundRateCalcMethodItem($scope, ctrl, $attrs);
			customersOutboundRateCalcMethodItem.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Outbound/RateCalcMethod/Templates/CustomersOutboundRateCalcMethodItemEditorTemplate.html'
	};

	function CustomersOutboundRateCalcMethodItem($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var carrierFilterSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var customerSelectorAPI;
		var customerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.carrierFilters = UtilsService.getArrayEnum(WhS_Deal_CarrierFilterTypeEnum);

			$scope.scopeModel.onCarrierFilterSelectorReady = function (api) {
				carrierFilterSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onCustomerSelectorReady = function (api) {
				customerSelectorAPI = api;
				customerSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.areCustomersRequired = function () {
				if ($scope.scopeModel.selectedCarrierFilter == undefined)
					return false;
				return ($scope.scopeModel.selectedCarrierFilter.value != WhS_Deal_CarrierFilterTypeEnum.All.value);
			};

			UtilsService.waitMultiplePromises([carrierFilterSelectorReadyDeferred.promise, customerSelectorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				var promises = [];
				var customerIds;

				if (payload != undefined) {
					$scope.scopeModel.selectedCarrierFilter = UtilsService.getItemByVal($scope.scopeModel.carrierFilters, payload.CarrierFilter, 'value');
					customerIds = payload.CustomerIds;
				}

				var customerSelectorPayload = { selectedIds: customerIds };
				promises.push(customerSelectorAPI.load(customerSelectorPayload));

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundItemRateCustomers, TOne.WhS.Deal.MainExtensions'
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}q
}]);