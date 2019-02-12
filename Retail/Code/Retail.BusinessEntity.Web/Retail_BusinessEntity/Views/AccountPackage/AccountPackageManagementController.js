(function (appControllers) {

	"use strict";

	AccountPackageManagement.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_AccountBEDefinitionAPIService'];

	function AccountPackageManagement($scope, UtilsService, VRUIUtilsService, Retail_BE_AccountBEDefinitionAPIService) {

		var gridAPI;
		var gridDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var packageDefinitionSelectorAPI;
		var packageDefinitionSelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

		var currencySelectorAPI;
		var currencySelectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        defineAPI();
		load();


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

			currencySelectorDirectiveReadyDeferred.promise.then(function () {
				var selectorPayload;
				VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, selectorPayload, undefined);
			});

		}

		function getFilter() {
			return {
				packageName: $scope.scopeModel.packageName,
				packageTypes: packageDefinitionSelectorAPI.getSelectedIds(),
				currencyIds: currencySelectorAPI.getSelectedIds(),
				bed:$scope.scopeModel.bed,
				eed: $scope.scopeModel.eed
			};
		}
	}


	appControllers.controller("Retail_BE_AccountPackageManagementController", AccountPackageManagement);
})(appControllers);