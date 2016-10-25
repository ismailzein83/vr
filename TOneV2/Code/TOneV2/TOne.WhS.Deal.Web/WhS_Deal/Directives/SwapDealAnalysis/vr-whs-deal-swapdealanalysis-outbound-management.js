'use strict';

app.directive('vrWhsDealSwapdealanalysisOutboundManagement', ['WhS_Deal_SwapDealAnalysisService', function (WhS_Deal_SwapDealAnalysisService) {
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
				var carrierAccountId = context.settingsAPI.getCarrierAccountId();
				if (carrierAccountId == undefined) {
					console.log('Select a carrier');
					return;
				}
				var onOutboundAdded = function (addedOutbound) {
					if (addedOutbound != undefined) {
						$scope.scopeModel.outbounds.push(addedOutbound);
					}
				};
				WhS_Deal_SwapDealAnalysisService.addOutbound(settings, carrierAccountId, onOutboundAdded);
			};

			$scope.scopeModel.removeOutbound = function (outbound) {
				$scope.scopeModel.outbounds.splice($scope.scopeModel.outbounds.indexOf(outbound), 1);
			};

			defineMenuActions();
		}

		function defineAPI() {
			var api = {};

			api.load = function (payload)
			{
				if (payload != undefined) {
					context = payload.context;
					settings = payload.settings;
				}
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
		function editOutbound(outboundEntity) {
			var carrierAccountId = context.settingsAPI.getCarrierAccountId();
			if (carrierAccountId == undefined) {
				console.log('Select a carrier');
				return;
			}
			var onOutboundUpdated = function (updatedOutbound) {
				if (updatedOutbound != undefined) {
					$scope.scopeModel.outbounds[$scope.scopeModel.outbounds.indexOf(outboundEntity)] = updatedOutbound;
				}
			};
			WhS_Deal_SwapDealAnalysisService.editOutbound(settings, carrierAccountId, outboundEntity, onOutboundUpdated);
		}
	}
}]);