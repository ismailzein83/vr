(function (appControllers) {

	"use strict";

	receivedSupplierPricelistController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService'];

	function receivedSupplierPricelistController($scope, VRNotificationService, UtilsService, VRUIUtilsService) {

		var carrierAccountDirectiveAPI;
		var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

		var statusSelectorAPI;
		var statusSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

		var filter = {};
		var gridPromiseDeferred = UtilsService.createPromiseDeferred();
		var gridAPI;

		defineScope();
		load();

		function defineScope() {
			$scope.scopeModel = {};
			$scope.scopeModel.top = 100;


			$scope.scopeModel.onGridReady = function (api) {
				gridPromiseDeferred.resolve();
				gridAPI = api;
			};

			$scope.scopeModel.onStatusSelectorReady = function (api) {
				statusSelectorAPI = api;
				statusSelectorPromiseDeferred.resolve();
			};

			$scope.scopeModel.load = function () {
				if ($scope.scopeModel.top > 0) {
					getFilterObject();
					return gridAPI.loadGrid(filter);
				}
			};

			$scope.scopeModel.onCarrierAccountDirectiveReady = function (api) {
				carrierAccountDirectiveAPI = api;
				carrierAccountReadyPromiseDeferred.resolve();
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			loadAllControls();
		}

		function loadAllControls() {
			return UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector, loadGrid])
				.catch(function (error) {
					VRNotificationService.notifyExceptionWithClose(error, $scope);
				})
				.finally(function () {
					$scope.scopeModel.isLoading = false;
				});
		}

		function loadCarrierAccountSelector() {
			var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

			carrierAccountReadyPromiseDeferred.promise.then(function () {
				VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, undefined, loadCarrierAccountPromiseDeferred);
			});
			return loadCarrierAccountPromiseDeferred.promise;
		}

		function loadGrid() {
			var loadGridPromiseDeferred = UtilsService.createPromiseDeferred();
			var promises = [];
			promises.push(carrierAccountReadyPromiseDeferred.promise);
			promises.push(statusSelectorPromiseDeferred.promise)
			UtilsService.waitMultiplePromises(promises).then(function () {
				getFilterObject();
				gridPromiseDeferred.promise.then(function () {
					gridAPI.loadGrid(filter);
					loadGridPromiseDeferred.resolve();
				});
			});
			return loadGridPromiseDeferred.promise;
		}

		function getFilterObject() {

			filter = {
				SupplierIds: carrierAccountDirectiveAPI.getSelectedIds(),
				Status: statusSelectorAPI.getSelectedIds(),
				Top: $scope.scopeModel.top
			};
		}
	}
	appControllers.controller('WhS_SupPL_ReceivedSupplierPricelistsController', receivedSupplierPricelistController);
})(appControllers);