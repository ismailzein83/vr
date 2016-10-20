'use strict';

app.directive('vrWhsDealSwapdealanalysisSettings', ['WhS_Deal_SwapDealAnalysisTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_Deal_SwapDealAnalysisTypeEnum, UtilsService, VRUIUtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisSettings = new SwapDealAnalysisSettings($scope, ctrl, $attrs);
			swapDealAnalysisSettings.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisSettingsTemplate.html'
	};

	function SwapDealAnalysisSettings($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var carrierAccountSelectorAPI;
		var carrierAccountReadyDeferred = UtilsService.createPromiseDeferred();

		var analysisTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.swapDealAnalysisTypes = UtilsService.getArrayEnum(WhS_Deal_SwapDealAnalysisTypeEnum);

			$scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
				carrierAccountSelectorAPI = api;
				carrierAccountReadyDeferred.resolve();
			};

			$scope.scopeModel.onAnalysisTypeSelectorReady = function (api) {
				analysisTypeSelectorReadyDeferred.resolve();
			};

			UtilsService.waitMultiplePromises([carrierAccountReadyDeferred.promise, analysisTypeSelectorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload)
			{
				var carrierAccountId;

				if (payload != undefined)
				{
					$scope.scopeModel.name = payload.Name;
					carrierAccountId = payload.CarrierAccountId;
					$scope.scopeModel.selectedSwapDealAnalysisType = UtilsService.getItemByVal($scope.swapDealAnalysisTypes, payload.AnalysisType, 'value');
					$scope.scopeModel.fromDate = payload.FromDate;
					$scope.scopeModel.toDate = payload.ToDate;
				}

				return loadCarrierAccountSelector(carrierAccountId);
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
		function loadCarrierAccountSelector(carrierAccountId) {
			var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

			carrierAccountReadyDeferred.promise.then(function () {
				var carrierAccountSelectorPayload = {
					selectedIds: carrierAccountId
				};
				VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, carrierAccountSelectorPayload, carrierAccountSelectorLoadDeferred);
			});

			return carrierAccountSelectorLoadDeferred.promise;
		}
	}
}]);