'use strict';
       
app.directive('vrAccountbalanceFinancialaccountbalanceNotificationType', ['UtilsService', 'VRUIUtilsService', 'VR_AccountBalance_AccountBalanceNotificationTypeAPIService',
	function (UtilsService, VRUIUtilsService, VR_AccountBalance_AccountBalanceNotificationTypeAPIService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '=',
				normalColNum: '@'
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new FinancialAccountBalanceNotificationTypeSettings($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: "/Client/Modules/VR_AccountBalance/Directives/AccountBalanceNotification/Templates/AccountBalanceFinancialAccountBalanceNotificationTypeTemplate.html"
		};

		function FinancialAccountBalanceNotificationTypeSettings($scope, ctrl, $attrs) {
			this.initializeController = initializeController;

			var beDefinitionSelectorAPI;
			var beDefinitionSelectorDeferredReady = UtilsService.createPromiseDeferred();
			
			function initializeController() {
				$scope.scopeModel = {};
				$scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
					beDefinitionSelectorAPI = api;
					beDefinitionSelectorDeferredReady.resolve();
				};
				defineAPI();
			}

			function defineAPI() {
				var api = {};
				api.load = function (payload) {
					var beDefinitionId;
					var promises = [];
					if (payload != undefined && payload.accountBalanceNotificationTypeExtendedSettings != undefined)
						beDefinitionId = payload.accountBalanceNotificationTypeExtendedSettings.FinancialAccountBEDefinitionId;
					promises.push(loadSelector());

					function loadSelector() {
						var selectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
						beDefinitionSelectorDeferredReady.promise.then(function () {
							var selectorPayload = { selectedIds: beDefinitionId };
							VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, selectorPayload, selectorLoadPromiseDeferred);
						});
						return selectorLoadPromiseDeferred.promise;
					}
					return UtilsService.waitMultiplePromises(promises);
				};
				api.getData = function () {
					return {
						$type: "Vanrise.AccountBalance.MainExtensions.GenericFinancialAccountBalanceNotificationTypeSettings,Vanrise.AccountBalance.MainExtensions",
						FinancialAccountBEDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
					};
				};
				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
	}]);