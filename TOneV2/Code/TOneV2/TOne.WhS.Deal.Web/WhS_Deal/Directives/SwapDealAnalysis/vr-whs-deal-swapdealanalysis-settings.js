'use strict';

app.directive('vrWhsDealSwapdealanalysisSettings', ['UtilsService', 'VRUIUtilsService', 'VRValidationService', function (UtilsService, VRUIUtilsService, VRValidationService) {
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

		var context;

		var carrierAccountSelectorAPI;
		var carrierAccountReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
				carrierAccountSelectorAPI = api;
				carrierAccountReadyDeferred.resolve();
			};

			$scope.scopeModel.onCarrierAccountSelectionChanged = function (selectedCarrierAccount) {

				var carrierAccountId = carrierAccountSelectorAPI.getSelectedIds()
				if (carrierAccountId == undefined)
					return;

				context.setSellingNumberPlanId(selectedCarrierAccount.SellingNumberPlanId);
				context.clearAnalysis();
			};

			$scope.scopeModel.validateTimeRange = function () {
				return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
			};

			UtilsService.waitMultiplePromises([carrierAccountReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload)
			{
				var carrierAccountId;
				var settings;

				if (payload != undefined)
				{
					context = payload.context;
					settings = payload.Settings;
				}

				if (settings != undefined) {
					$scope.scopeModel.name = payload.Name;
					carrierAccountId = payload.CarrierAccountId;
					$scope.scopeModel.fromDate = payload.FromDate;
					$scope.scopeModel.toDate = payload.ToDate;
				}

				return loadCarrierAccountSelector(carrierAccountId);
			};

			api.getData = function () {
				return {
					CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
					FromDate: $scope.scopeModel.fromDate,
					ToDate: $scope.scopeModel.toDate
				};
			};

			api.getCarrierAccountId = function () {
				return carrierAccountSelectorAPI.getSelectedIds();
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