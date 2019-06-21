'use strict';
app.directive('vrAccountbalanceFinancialaccountbalancenotificationtypeSearcheditor', ['UtilsService', 'VRUIUtilsService',
	function (UtilsService, VRUIUtilsService) {
		return {
			restrict: 'E',
			scope: {
				onReady: '='
			},
			controller: function ($scope, $element, $attrs) {
				var ctrl = this;
				var ctor = new SearchEditorCtor($scope, ctrl, $attrs);
				ctor.initializeController();
			},
			controllerAs: 'ctrl',
			bindToController: true,
			templateUrl: '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/AccountBalanceNotification/Templates/FinancialAccountBalanceNotificationSearchEditor.html'
		};

		function SearchEditorCtor($scope, ctrl, $attrs) {
			this.initializeController = initializeController;
			var accountSelectorDirectiveAPI;
			var accountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

			function initializeController() {
				$scope.scopeModel = {};

				$scope.scopeModel.onAccountSelectorReady = function (api) {
					accountSelectorDirectiveAPI = api;
					accountSelectorReadyPromiseDeferred.resolve();
				};

				defineAPI();
			}

			function defineAPI() {
				var api = {};

				api.load = function (payload) {
					var promises = [];

					var accountBalanceNotificationTypeExtendedSettings;

					if (payload != undefined) {
						accountBalanceNotificationTypeExtendedSettings = payload.accountBalanceNotificationTypeExtendedSettings;
					}

					var accountSelectorLoadPromise = getAccountSelectorLoadPromise();
					promises.push(accountSelectorLoadPromise);


					function getAccountSelectorLoadPromise() {
						var accountSelectorLoadPromise = UtilsService.createPromiseDeferred();

						accountSelectorReadyPromiseDeferred.promise.then(function () {
							var selectorPayload;
							if (accountBalanceNotificationTypeExtendedSettings != undefined) {
								selectorPayload = {
									businessEntityDefinitionId: accountBalanceNotificationTypeExtendedSettings.FinancialAccountBEDefinitionId
								};
							}
							VRUIUtilsService.callDirectiveLoad(accountSelectorDirectiveAPI, selectorPayload, accountSelectorLoadPromise);
						});

						return accountSelectorLoadPromise.promise;
					}

					var rootPromiseNode = {
						promises: promises
					};
					return UtilsService.waitPromiseNode(rootPromiseNode);
				};

				api.getData = function () {
					return {
						$type: "Vanrise.AccountBalance.MainExtensions.GenericFinanciaAccountBalanceNotificationExtendedQuery,Vanrise.AccountBalance.MainExtensions",
						AccountIds: accountSelectorDirectiveAPI.getSelectedIds()
					};
				};

				if (ctrl.onReady != null)
					ctrl.onReady(api);
			}
		}
	}]);
