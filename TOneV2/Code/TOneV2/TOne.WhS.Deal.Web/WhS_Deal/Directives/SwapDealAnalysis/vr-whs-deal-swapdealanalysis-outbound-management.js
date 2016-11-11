'use strict';

app.directive('vrWhsDealSwapdealanalysisOutboundManagement', ['WhS_Deal_SwapDealAnalysisService', 'UtilsService', function (WhS_Deal_SwapDealAnalysisService, UtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisOutboundMangement = new SwapDealAnalysisOutboundMangement($scope, ctrl, $attrs);
			swapDealAnalysisOutboundMangement.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisOutboundManagementTemplate.html'
	};

	function SwapDealAnalysisOutboundMangement($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var gridAPI;
		var context;

		var settings;

		function initializeController()
		{
			$scope.scopeModel = {};

			$scope.scopeModel.outbounds = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};

			$scope.scopeModel.addOutbound = function () {
				addOutbound();
			};

			$scope.scopeModel.removeOutbound = function (outbound) {
				$scope.scopeModel.outbounds.splice($scope.scopeModel.outbounds.indexOf(outbound), 1);
			};

			$scope.scopeModel.validateOutbounds = function () {
				if ($scope.scopeModel.outbounds.length == 0)
					return 'Add at least 1 outbound';
				return null;
			};

			defineMenuActions();
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {

				var outbounds;
				var summary;

				if (payload != undefined) {
					context = payload.context;
					settings = payload.settings;
					outbounds = payload.Outbounds;
					summary = payload.Summary;
				}

				if (outbounds != undefined) {

					var entities = getOutboundEntities();

					for (var i = 0; i < outbounds.length; i++) {

						var index = UtilsService.getItemIndexByVal(entities, outbounds[i].Name, 'Name');
						if (index == -1)
							continue;

						var existingRecord = $scope.scopeModel.outbounds[index];
						if (existingRecord == undefined)
							continue;

						var updatedEntity = UtilsService.cloneObject($scope.scopeModel.outbounds[index].Entity);

						updatedEntity.DailyVolume = outbounds[i].DailyVolume;
						updatedEntity.CurrentRate = outbounds[i].CurrentRate;
						updatedEntity.RateSavings = outbounds[i].RateSavings;
						updatedEntity.Savings = outbounds[i].Savings;
						updatedEntity.Revenue = outbounds[i].Revenue;

						$scope.scopeModel.outbounds[index] = { Entity: updatedEntity };
					}
				}

				if (summary != undefined) {
					gridAPI.setSummary({
						TotalCostMargin: summary.TotalCostMargin,
						TotalCostRevenue: summary.TotalCostRevenue
					});
				}
			};

			api.getData = function () {
				return {
					Outbounds: getOutboundEntities()
				};
			};

			api.clear = function () {
				clearSummary();
				$scope.scopeModel.outbounds.length = 0;
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}

		function defineMenuActions() {
			$scope.scopeModel.menuActions = [{
				name: 'Edit',
				clicked: editOutbound
			}];
		}
		function addOutbound() {
			var carrierAccountId = context.settingsAPI.getCarrierAccountId();
			var onOutboundAdded = function (addedOutbound) {
				var obj = { Entity: addedOutbound };
				$scope.scopeModel.outbounds.push(obj);
				clearCalclulatedFields();
			};
			WhS_Deal_SwapDealAnalysisService.addOutbound(settings, carrierAccountId, onOutboundAdded);
		}
		function editOutbound(outboundEntity) {
			var carrierAccountId = context.settingsAPI.getCarrierAccountId();
			var onOutboundUpdated = function (updatedOutbound) {
				var obj = { Entity: updatedOutbound };
				$scope.scopeModel.outbounds[$scope.scopeModel.outbounds.indexOf(outboundEntity)] = obj;
				clearCalclulatedFields();
			};
			WhS_Deal_SwapDealAnalysisService.editOutbound(settings, carrierAccountId, outboundEntity.Entity, onOutboundUpdated);
		}

		function getOutboundEntities() {
			return UtilsService.getPropValuesFromArray($scope.scopeModel.outbounds, 'Entity');
		}
		function clearCalclulatedFields() {
			for (var i = 0; i < $scope.scopeModel.outbounds.length; i++) {
				var updatedEntity = UtilsService.cloneObject($scope.scopeModel.outbounds[i].Entity);
				updatedEntity.DailyVolume = undefined;
				updatedEntity.CurrentRate = undefined;
				updatedEntity.RateSavings = undefined;
				updatedEntity.Savings = undefined;
				updatedEntity.Revenue = undefined;
				$scope.scopeModel.outbounds[i] = { Entity: updatedEntity };
			}
			clearSummary();
			context.clearResult();
		}
		function clearSummary() {
			gridAPI.clearSummary();
		}
	}
}]);