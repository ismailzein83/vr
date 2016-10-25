'use strict';

app.directive('vrWhsDealSwapdealanalysisSettingsEditor', ['UtilsService', function (UtilsService) {
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
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Settings/SwapDealAnalysis/Templates/SwapDealAnalysisSettingsEditorTemplate.html'
	};

	function SwapDealAnalysisSettingsEditor($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var outboundSettingsEditorAPI;
		var outboundSettingsEditorReadyDeferred = UtilsService.createPromiseDeferred();

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.onOutboundSettingsEditorReady = function (api) {
				outboundSettingsEditorAPI = api;
				outboundSettingsEditorReadyDeferred.resolve();
			};

			UtilsService.waitMultiplePromises([outboundSettingsEditorReadyDeferred.promise]).then(function () {
				defineAPI();
			});
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {
				
			};

			api.getData = function () {
				console.log({
					$type: 'TOne.WhS.Deal.Entities.Settings.SwapDealAnalysisSettingData, TOne.WhS.Deal.Entities',
					OutboundCalculationMethods: outboundSettingsEditorAPI.getData()
				});
				return {
					$type: 'TOne.WhS.Deal.Entities.Settings.SwapDealAnalysisSettingData, TOne.WhS.Deal.Entities',
					OutboundCalculationMethods: outboundSettingsEditorAPI.getData()
				};
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}
	}
}]);