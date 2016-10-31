'use strict';

app.directive('vrWhsDealSwapdealanalysisSettingsInboundEditor', ['WhS_Deal_SwapDealAnalysisSettingsService', 'UtilsService', function (WhS_Deal_SwapDealAnalysisSettingsService, UtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '=',
			normalColNum: '@'
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisInboundSettingsEditor = new SwapDealAnalysisInboundSettingsEditor($scope, ctrl, $attrs);
			swapDealAnalysisInboundSettingsEditor.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/Settings/SwapDealSettings/Templates/SwapDealAnalysisInboundSettingsEditorTemplate.html'
	};

	function SwapDealAnalysisInboundSettingsEditor($scope, ctrl, $attrs) {

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

			$scope.scopeModel.validateRateCalcMethods = function () {
				if ($scope.scopeModel.rateCalcMethods.length == 0)
					return 'Add at least 1 rate calc method';
				// Make sure that a default rate calc method is selected
				var counter = 0;
				for (var i = 0; i < $scope.scopeModel.rateCalcMethods.length; i++) {
					if ($scope.scopeModel.rateCalcMethods[i].isSelected === true)
						counter++;
				}
				if (counter == 0)
					return 'Select a default rate calc method';
				if (counter > 1)
					return 'You can only select 1 rate calc method';
				return null;
			};

			defineMenuActions();
		}
		function defineAPI() {
			var api = {};

			api.load = function (payload) {
				if (payload != undefined && payload.outboundCalculationMethods != undefined) {
					for (var key in payload.outboundCalculationMethods) {
						if (key == '$type')
							continue;
						var rateCalcMethod = { Entity: payload.outboundCalculationMethods[key] }
						if (payload.outboundCalculationMethods[key].CalculationMethodId == payload.defaultCalculationMethodId)
							rateCalcMethod.isSelected = true;
						$scope.scopeModel.rateCalcMethods.push(rateCalcMethod);
					}
				}
			};

			api.getData = function () {

				var data = {
					DefaultCalculationMethodId: UtilsService.getItemByVal($scope.scopeModel.rateCalcMethods, true, 'isSelected').Entity.CalculationMethodId
				};

				var rateCalcMethods = getRateCalcMethods();

				if (rateCalcMethods != undefined) {
					data.OutboundCalculationMethods = {};
					for (var i = 0; i < rateCalcMethods.length; i++)
						data.OutboundCalculationMethods[rateCalcMethods[i].CalculationMethodId] = rateCalcMethods[i];
				}

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
		function addRateCalcMethod() {
			var onRateCalcMethodAdded = function (addedRateCalcMethod) {
				if (addedRateCalcMethod != undefined) {
					var obj = {};
					obj.Entity = addedRateCalcMethod;
					obj.Entity.CalculationMethodId = UtilsService.guid();
					$scope.scopeModel.rateCalcMethods.push(obj);
				}
			};
			WhS_Deal_SwapDealAnalysisSettingsService.addOutboundRateCalcMethod(onRateCalcMethodAdded);
		}
		function editRateCalcMethod(rateCalcMethod) {
			var onRateCalcMethodUpdated = function (updatedRateCalcMethod) {
				if (updatedRateCalcMethod != undefined) {
					var rateCalcMethods = getRateCalcMethods();
					var updatedRateCalcMethodIndex = UtilsService.getItemIndexByVal(rateCalcMethods, updatedRateCalcMethod.CalculationMethodId, 'CalculationMethodId');
					$scope.scopeModel.rateCalcMethods[updatedRateCalcMethodIndex] = {
						isSelected: rateCalcMethod.isSelected,
						Entity: updatedRateCalcMethod
					};
				}
			};
			WhS_Deal_SwapDealAnalysisSettingsService.editOutboundRateCalcMethod(rateCalcMethod.Entity.CalculationMethodId, rateCalcMethod.Entity, onRateCalcMethodUpdated);
		}

		function getRateCalcMethods() {
			return UtilsService.getPropValuesFromArray($scope.scopeModel.rateCalcMethods, 'Entity');
		}
	}
}]);