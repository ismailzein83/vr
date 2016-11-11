'use strict';

app.directive('vrWhsDealSwapdealanalysisInboundManagement', ['WhS_Deal_SwapDealAnalysisService', 'UtilsService', function (WhS_Deal_SwapDealAnalysisService, UtilsService) {
	return {
		restrict: 'E',
		scope: {
			onReady: '='
		},
		controller: function ($scope, $element, $attrs) {
			var ctrl = this;
			var swapDealAnalysisInboundMangement = new SwapDealAnalysisInboundMangement($scope, ctrl, $attrs);
			swapDealAnalysisInboundMangement.initializeController();
		},
		controllerAs: 'ctrl',
		bindToController: true,
		templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/Templates/SwapDealAnalysisInboundManagementTemplate.html'
	};

	function SwapDealAnalysisInboundMangement($scope, ctrl, $attrs) {

		this.initializeController = initializeController;

		var gridAPI;
		var context;

		var settings;
		var sellingNumberPlanId;

		function initializeController() {
			$scope.scopeModel = {};

			$scope.scopeModel.inbounds = [];

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				defineAPI();
			};

			$scope.scopeModel.addInbound = function () {
				addInbound();
			};

			$scope.scopeModel.removeInbound = function (inbound) {
				$scope.scopeModel.inbounds.splice($scope.scopeModel.inbounds.indexOf(inbound), 1);
			};

			$scope.scopeModel.validateInbounds = function () {
				if ($scope.scopeModel.inbounds.length == 0)
					return 'Add at least 1 inbound';
				return null;
			};

			defineMenuActions();
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload) {

				var inbounds;
				var summary;

				if (payload != undefined) {
					context = payload.context;
					settings = payload.settings;
					inbounds = payload.Inbounds;
					summary = payload.Summary;
				}

				if (inbounds != undefined) {

					var entities = getInboundEntities();

					for (var i = 0; i < inbounds.length; i++) {

						var index = UtilsService.getItemIndexByVal(entities, inbounds[i].Name, 'Name');
						if (index == -1)
							continue;

						var existingRecord = $scope.scopeModel.inbounds[index];
						if (existingRecord == undefined)
							continue;

						var updatedEntity = UtilsService.cloneObject(existingRecord.Entity);

						updatedEntity.DailyVolume = inbounds[i].DailyVolume;
						updatedEntity.CurrentRate = inbounds[i].CurrentRate;
						updatedEntity.RateProfit = inbounds[i].RateProfit;
						updatedEntity.Profit = inbounds[i].Profit;
						updatedEntity.Revenue = inbounds[i].Revenue;
						
						$scope.scopeModel.inbounds[index] = { Entity: updatedEntity };
					}
				}

				if (summary != undefined) {
					gridAPI.setSummary({
						TotalSaleMargin: summary.TotalSaleMargin,
						TotalSaleRevenue: summary.TotalSaleRevenue
					});
				}
			};

			api.getData = function () {
				return {
					Inbounds: getInboundEntities()
				};
			};

			api.setSellingNumberPlanId = function (carrierSellingNumberPlanId) {
				if (carrierSellingNumberPlanId != undefined)
					sellingNumberPlanId = carrierSellingNumberPlanId;
			};

			api.clear = function () {
				clearSummary();
				$scope.scopeModel.inbounds.length = 0;
			};

			if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
				ctrl.onReady(api);
		}

		function defineMenuActions() {
			$scope.scopeModel.menuActions = [{
				name: 'Edit',
				clicked: editInbound
			}];
		}
		function addInbound() {
			var carrierAccountId = context.settingsAPI.getCarrierAccountId();
			var onInboundAdded = function (addedInbound) {
				var obj = { Entity: addedInbound };
				$scope.scopeModel.inbounds.push(obj);
				clearCalclulatedFields();
			};
			WhS_Deal_SwapDealAnalysisService.addInbound(settings, carrierAccountId, sellingNumberPlanId, onInboundAdded);
		}
		function editInbound(inboundEntity) {
			var carrierAccountId = context.settingsAPI.getCarrierAccountId();
			var onInboundUpdated = function (updatedInbound) {
				var obj = { Entity: updatedInbound };
				$scope.scopeModel.inbounds[$scope.scopeModel.inbounds.indexOf(inboundEntity)] = obj;
				clearCalclulatedFields();
			};
			WhS_Deal_SwapDealAnalysisService.editInbound(settings, carrierAccountId, sellingNumberPlanId, inboundEntity.Entity, onInboundUpdated);
		}

		function getInboundEntities() {
			return UtilsService.getPropValuesFromArray($scope.scopeModel.inbounds, 'Entity');
		}
		function clearCalclulatedFields() {
			for (var i = 0; i < $scope.scopeModel.inbounds.length; i++) {
				var updatedEntity = UtilsService.cloneObject($scope.scopeModel.inbounds[i].Entity);
				updatedEntity.DailyVolume = undefined;
				updatedEntity.CurrentRate = undefined;
				updatedEntity.RateProfit = undefined;
				updatedEntity.Profit = undefined;
				updatedEntity.Revenue = undefined;
				$scope.scopeModel.inbounds[i] = { Entity: updatedEntity };
			}
			clearSummary();
			context.clearResult();
		}
		function clearSummary() {
			gridAPI.clearSummary();
		}
	}
}]);