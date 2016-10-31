'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodOutboundCustomersItemeditor', ['WhS_Deal_CarrierFilterTypeEnum', 'WhS_Deal_ZoneRateEvaluationTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_Deal_CarrierFilterTypeEnum, WhS_Deal_ZoneRateEvaluationTypeEnum, UtilsService, VRUIUtilsService) {
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
		controllerAs: 'customersCtrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Outbound/RateCalcMethod/Templates/CustomersOutboundRateCalcMethodItemEditorTemplate.html'
	};

	function CustomersOutboundRateCalcMethodItem($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var carrierFilterSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var customerSelectorAPI;
		var customerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var zoneRateEvalSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var directiveAPI;
		var directiveReadyDeferred;

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.carrierFilters = UtilsService.getArrayEnum(WhS_Deal_CarrierFilterTypeEnum);
			$scope.scopeModel.zoneRateEvals = UtilsService.getArrayEnum(WhS_Deal_ZoneRateEvaluationTypeEnum);

			$scope.scopeModel.onCarrierFilterSelectorReady = function (api) {
				carrierFilterSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onCustomerSelectorReady = function (api) {
				customerSelectorAPI = api;
				customerSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onZoneRateEvalSelectorReady = function (api) {
				zoneRateEvalSelectorReadyDeferred.resolve();
			};

			$scope.scopeModel.onDirectiveReady = function (api) {
				directiveAPI = api;
				var setLoader = function (value) {
					$scope.scopeModel.isLoading = value;
				};
				VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
			};

			$scope.scopeModel.areCustomersRequired = function () {
				if ($scope.scopeModel.selectedCarrierFilter == undefined)
					return false;
				return ($scope.scopeModel.selectedCarrierFilter.value != WhS_Deal_CarrierFilterTypeEnum.All.value);
			};

			UtilsService.waitMultiplePromises([carrierFilterSelectorReadyDeferred.promise, customerSelectorReadyDeferred.promise, zoneRateEvalSelectorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				var promises = [];

				var customerIds;
				var zoneRateEval;
				var zoneRateEvalData;

				if (payload != undefined) {
					$scope.scopeModel.selectedCarrierFilter = UtilsService.getItemByVal($scope.scopeModel.carrierFilters, payload.CarrierFilter, 'value');
					customerIds = payload.CustomerIds;
					zoneRateEval = payload.ZoneRateEval;
					zoneRateEvalData = payload.ZoneRateEvalData;
				}
				
				var customerSelectorPayload = { selectedIds: customerIds };
				promises.push(customerSelectorAPI.load(customerSelectorPayload));

				if (zoneRateEval != undefined) {
					directiveReadyDeferred = UtilsService.createPromiseDeferred();
					$scope.scopeModel.selectedZoneRateEval = UtilsService.getItemByVal($scope.scopeModel.zoneRateEvals, zoneRateEval, 'value');

					var directiveLoadDeferred = UtilsService.createPromiseDeferred();
					promises.push(directiveLoadDeferred.promise);

					directiveReadyDeferred.promise.then(function () {
						VRUIUtilsService.callDirectiveLoad(directiveAPI, zoneRateEvalData, directiveLoadDeferred);
					});
				}

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisOutboundItemRateCustomers, TOne.WhS.Deal.MainExtensions',
					CarrierFilter: $scope.scopeModel.selectedCarrierFilter.value,
					CustomerIds: ($scope.scopeModel.selectedCarrierFilter.value != WhS_Deal_CarrierFilterTypeEnum.All.value) ? customerSelectorAPI.getSelectedIds() : null,
					ZoneRateEval: $scope.scopeModel.selectedZoneRateEval.value,
					ZoneRateEvalData: directiveAPI.getData()
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}q
}]);