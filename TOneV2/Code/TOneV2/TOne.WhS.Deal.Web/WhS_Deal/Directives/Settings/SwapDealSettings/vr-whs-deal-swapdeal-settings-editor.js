'use strict';

app.directive('vrWhsDealSwapdealSettingsEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisSettingsEditor = new SwapDealAnalysisSettingsEditor($scope, ctrl, $attrs);
			swapDealAnalysisSettingsEditor.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Settings/SwapDealSettings/Templates/SwapDealSettingsEditorTemplate.html'
	};

	function SwapDealAnalysisSettingsEditor($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var inboundSettingsEditorAPI;
		var inboundSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

		var outboundSettingsEditorAPI;
		var outboundSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.onInboundSettingsEditorReady = function (api) {
				inboundSettingsEditorAPI = api;
				inboundSettingsEditorReadyDeferred.resolve();
			};

			$scope.scopeModel.onOutboundSettingsEditorReady = function (api) {
				outboundSettingsEditorAPI = api;
				outboundSettingsEditorReadyDeferred.resolve();
			};

			UtilsService.waitMultiplePromises([inboundSettingsEditorReadyDeferred.promise, outboundSettingsEditorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {

			var api = {};

			api.load = function (payload) {

				var promises = [];

				if (payload != undefined && payload.data != undefined) {

					var inboundSettingsLoadDeferred = UtilsService.createPromiseDeferred();
					promises.push(inboundSettingsLoadDeferred.promise);

					var inboundSettingsPayload = {
						defaultCalculationMethodId: payload.data.DefaultInboundRateCalcMethodId,
						inboundCalculationMethods: payload.data.InboundCalculationMethods
					};
					VRUIUtilsService.callDirectiveLoad(inboundSettingsEditorAPI, inboundSettingsPayload, inboundSettingsLoadDeferred);

					var outboundSettingsLoadDeferred = UtilsService.createPromiseDeferred();
					promises.push(outboundSettingsLoadDeferred.promise);

					var outboundSettingsPayload = {
						defaultCalculationMethodId: payload.data.DefaultCalculationMethodId,
						outboundCalculationMethods: payload.data.OutboundCalculationMethods
					};
					VRUIUtilsService.callDirectiveLoad(outboundSettingsEditorAPI, outboundSettingsPayload, outboundSettingsLoadDeferred);

					$scope.scopeModel.gracePeriod = payload.data.GracePeriod;
				}

				return UtilsService.waitMultiplePromises(promises);
			};

			api.getData = function () {
				var data = {};
				var inboundSettingsData = inboundSettingsEditorAPI.getData();
				var outboundSettingsData = outboundSettingsEditorAPI.getData();
				var data = {
					$type: 'TOne.WhS.Deal.Entities.Settings.SwapDealSettingData, TOne.WhS.Deal.Entities',
					DefaultCalculationMethodId: outboundSettingsData.DefaultCalculationMethodId,
					DefaultInboundRateCalcMethodId: inboundSettingsData.DefaultInboundRateCalcMethodId,
					OutboundCalculationMethods: outboundSettingsData.OutboundCalculationMethods,
					InboundCalculationMethods: inboundSettingsData.InboundCalculationMethods,
					GracePeriod: $scope.scopeModel.gracePeriod
				};
				return data;
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);