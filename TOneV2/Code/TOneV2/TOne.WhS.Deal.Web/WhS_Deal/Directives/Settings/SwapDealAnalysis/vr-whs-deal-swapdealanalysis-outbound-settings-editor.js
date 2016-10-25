'use strict';

app.directive('vrWhsDealSwapdealanalysisOutboundSettingsEditor', ['WhS_Deal_SwapDealAnalysisSettingsService', 'UtilsService', function (WhS_Deal_SwapDealAnalysisSettingsService, UtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisOutboundSettingsEditor = new SwapDealAnalysisOutboundSettingsEditor($scope, ctrl, $attrs);
			swapDealAnalysisOutboundSettingsEditor.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Settings/SwapDealAnalysis/Templates/SwapDealAnalysisOutboundSettingsEditorTemplate.html'
	};

	function SwapDealAnalysisOutboundSettingsEditor($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		function initializeController() {

			$scope.scopeModel = {};

			$scope.scopeModel.rateCalcMethods = [];

			$scope.scopeModel.onGridReady = function (api) {
				defineAPI();
			};

			$scope.scopeModel.addRateCalcMethod = function () {
				addRateCalcMethod();
			};

			$scope.scopeModel.removeRateCalcMethod = function (rateCalcMethod) {
				$scope.scopeModel.rateCalcMethods.splice($scope.scopeModel.rateCalcMethods.indexOf(rateCalcMethod), 1);
			};

			defineMenuActions();
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {

			};

			api.getData = function () {
				var data = {};
				var rateCalcMethods = getRateCalcMethods();
				if (rateCalcMethods != undefined)
					for (var i = 0; i < rateCalcMethods.length; i++)
						data[rateCalcMethods[i].CalculationMethodId] = rateCalcMethods[i];
				return data;
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}

		function defineMenuActions() {
			$scope.scopeModel.menuActions = [{
				name: 'Edit',
				clicked: editRateCalcMethod
			}];
		}
		function addRateCalcMethod()
		{
			var onRateCalcMethodAdded = function (addedRateCalcMethod) {
				if (addedRateCalcMethod != undefined) {
					addedRateCalcMethod.CalculationMethodId = UtilsService.guid();
					var dataItem = {
						rateCalcMethod: addedRateCalcMethod
					};
					$scope.scopeModel.rateCalcMethods.push(dataItem);
				}
			};
			WhS_Deal_SwapDealAnalysisSettingsService.addOutboundRateCalcMethod(onRateCalcMethodAdded);
		}
		function editRateCalcMethod(dataItem)
		{
			var onRateCalcMethodUpdated = function (updatedRateCalcMethod) {
				if (updatedRateCalcMethod != undefined) {
					var rateCalcMethods = getRateCalcMethods();
					var index = UtilsService.getItemIndexByVal(rateCalcMethods, updatedRateCalcMethod.CalculationMethodId, 'CalculationMethodId');
					$scope.scopeModel.rateCalcMethods[index] = {
						rateCalcMethod: updatedRateCalcMethod
					};
				}
			};
			WhS_Deal_SwapDealAnalysisSettingsService.editOutboundRateCalcMethod(dataItem.rateCalcMethod.CalculationMethodId, dataItem.rateCalcMethod, onRateCalcMethodUpdated);
		}

		function getRateCalcMethods() {
			return UtilsService.getPropValuesFromArray($scope.scopeModel.rateCalcMethods, 'rateCalcMethod');
		}
	}
}]);