(function (appControllers) {

	"use strict";

	AccountPackageManagement.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountBEDefinitionAPIService'];

	function AccountPackageManagement($scope, UtilsService, VRUIUtilsService, Retail_BE_AccountBEDefinitionAPIService) {

		var gridAPI;
		var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var packageDefinitionSelectorAPI;
		var packageDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var accountSelectorAPI;
		var accountSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var currencySelectorAPI;
		var currencySelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var statusDefinitionSelectorAPI;
		var statusDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		defineAPI();
		load();

		var accountBEDefinitionId = "9a427357-cf55-4f33-99f7-745206dee7cd";

		function defineAPI() {
			$scope.scopeModel = {};

			$scope.scopeModel.onGridReady = function (api) {
				gridAPI = api;
				gridDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.onPackageDefinitionSelectorReady = function (api) {
				packageDefinitionSelectorAPI = api;
				packageDefinitionSelectorDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.onCurrencySelectorReady = function (api) {
				currencySelectorAPI = api;
				currencySelectorDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
				statusDefinitionSelectorAPI = api;
				statusDefinitionSelectorDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.onAccountSelectorReady = function (api) {
				accountSelectorAPI = api;
				accountSelectorDirectiveReadyDeferred.resolve();
			};

			$scope.scopeModel.search = function () {
				var gridPayload = getFilter();
				VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, undefined);
			};
		}

		function load() {

			gridDirectiveReadyDeferred.promise.then(function () {
				var gridPayload = getFilter();
				VRUIUtilsService.callDirectiveLoad(gridAPI, gridPayload, undefined);
			});

			packageDefinitionSelectorDirectiveReadyDeferred.promise.then(function () {
				var selectorPayload;
				VRUIUtilsService.callDirectiveLoad(packageDefinitionSelectorAPI, selectorPayload, undefined);
			});

			accountSelectorDirectiveReadyDeferred.promise.then(function () {
				var selectorPayload = {
					AccountBEDefinitionId: accountBEDefinitionId
				};
				VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, selectorPayload, undefined);
			});

			currencySelectorDirectiveReadyDeferred.promise.then(function () {
				var selectorPayload;
				VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, selectorPayload, undefined);
			});

			statusDefinitionSelectorDirectiveReadyDeferred.promise.then(function () {
				Retail_BE_AccountBEDefinitionAPIService.GetAccountBEStatusDefinitionId(accountBEDefinitionId).then(function (response) {

					var selectorPayload = {
						businessEntityDefinitionId: response
					};
					VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, selectorPayload, undefined);
				});
			});
		}

		function getFilter() {
			return {
				accountBEDefinitionId: accountBEDefinitionId,
				packageName: $scope.scopeModel.packageName,
				packageTypes: packageDefinitionSelectorAPI.getSelectedIds(),
				accountIds: accountSelectorAPI.getSelectedIds(),
				statusIds: statusDefinitionSelectorAPI.getSelectedIds(),
				currencyIds: currencySelectorAPI.getSelectedIds(),
				bed:$scope.scopeModel.bed,
				eed: $scope.scopeModel.eed
			};
		}
	}


	appControllers.controller("Retail_BE_AccountPackageManagementController", AccountPackageManagement);
})(appControllers);