'use strict';

app.directive('vrWhsDealSwapdealanalysisRatecalcmethodInboundSuppliersItemeditor', ['WhS_Deal_CarrierFilterTypeEnum', 'WhS_Deal_ZoneRateEvaluationTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_Deal_CarrierFilterTypeEnum, WhS_Deal_ZoneRateEvaluationTypeEnum, UtilsService, VRUIUtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var suppliersInboundRateCalcMethodItem = new SuppliersInboundRateCalcMethodItem($scope, ctrl, $attrs);
			suppliersInboundRateCalcMethodItem.initializeController();
		},
		controllerAs: 'suppliersCtrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Extensions/SwapDealAnalysis/Inbound/RateCalcMethod/Templates/SuppliersInboundRateCalcMethodItemEditorTemplate.html'
	};

	function SuppliersInboundRateCalcMethodItem($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var carrierFilterSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		var supplierSelectorAPI;
		var supplierSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

			$scope.scopeModel.onSupplierSelectorReady = function (api) {
				supplierSelectorAPI = api;
				supplierSelectorReadyDeferred.resolve();
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

			$scope.scopeModel.areSuppliersRequired = function () {
				if ($scope.scopeModel.selectedCarrierFilter == undefined)
					return false;
				return ($scope.scopeModel.selectedCarrierFilter.value != WhS_Deal_CarrierFilterTypeEnum.All.value);
			};

			UtilsService.waitMultiplePromises([carrierFilterSelectorReadyDeferred.promise, supplierSelectorReadyDeferred.promise, zoneRateEvalSelectorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {
				var promises = [];

				var supplierIds;
				var rateEvaluationType;
				var rateEvaluationData;

				if (payload != undefined) {
					$scope.scopeModel.selectedCarrierFilter = UtilsService.getItemByVal($scope.scopeModel.carrierFilters, payload.SupplierFilterType, 'value');
					supplierIds = payload.SupplierIds;
					rateEvaluationType = payload.RateEvaluationType;
					rateEvaluationData = payload.RateEvaluationData;
				}

				var supplierSelectorPayload = { selectedIds: supplierIds };
				promises.push(supplierSelectorAPI.load(supplierSelectorPayload));

				if (rateEvaluationType != undefined) {
					directiveReadyDeferred = UtilsService.createPromiseDeferred();
					$scope.scopeModel.selectedZoneRateEval = UtilsService.getItemByVal($scope.scopeModel.zoneRateEvals, rateEvaluationType, 'value');

					var directiveLoadDeferred = UtilsService.createPromiseDeferred();
					promises.push(directiveLoadDeferred.promise);

					directiveReadyDeferred.promise.then(function () {
						VRUIUtilsService.callDirectiveLoad(directiveAPI, rateEvaluationData, directiveLoadDeferred);
					});
				}

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function () {
				return {
					$type: 'TOne.WhS.Deal.MainExtensions.SwapDealAnalysisInboundItemRateSuppliers, TOne.WhS.Deal.MainExtensions',
					SupplierFilterType: $scope.scopeModel.selectedCarrierFilter.value,
					SupplierIds: ($scope.scopeModel.selectedCarrierFilter.value != WhS_Deal_CarrierFilterTypeEnum.All.value) ? supplierSelectorAPI.getSelectedIds() : null,
					RateEvaluationType: $scope.scopeModel.selectedZoneRateEval.value,
					RateEvaluationData: directiveAPI.getData()
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);