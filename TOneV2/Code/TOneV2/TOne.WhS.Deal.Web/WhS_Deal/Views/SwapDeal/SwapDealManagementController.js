(function (appControllers) {

	'use strict';

	DealAnalysisManagementController.$inject = ['$scope', 'WhS_Deal_SwapDealAPIService', 'WhS_Deal_SwapDealService', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

	function DealAnalysisManagementController($scope, WhS_Deal_SwapDealAPIService, WhS_Deal_SwapDealService, UtilsService, VRNotificationService, VRUIUtilsService) {

	    var carrierAccountSelectorAPI;
	    var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
		var gridAPI;

		defineScope();
		load();

		function defineScope()
		{
		    $scope.scopeModel = {};

			$scope.scopeModel.onGridReady = function (api) {
			    gridAPI = api;
			    api.load(getFilterObject());
			};
			$scope.scopeModel.search = function () {
			    return gridAPI.load(getFilterObject());
			};
			$scope.scopeModel.hasAddSwapDealPermission = function () {
			    return WhS_Deal_SwapDealAPIService.HasAddDealPermission();
			}
			$scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
			    carrierAccountSelectorAPI = api;
			    carrierAccountSelectorReadyDeferred.resolve();
			};
			$scope.scopeModel.analyze = function () {
				var onSwapDealAnalyzed = function () { };
				WhS_Deal_SwapDealService.analyzeSwapDeal(onSwapDealAnalyzed);
			};			
			$scope.scopeModel.add = function () {          
			    var onSwapDealAdded = function (swapDealObj) {
			        gridAPI.onSwapDealAdded(swapDealObj);
			    };
			    WhS_Deal_SwapDealService.addSwapDeal(onSwapDealAdded);
			};
		}

		function load() {
			$scope.scopeModel.isLoading = true;
			UtilsService.waitMultipleAsyncOperations([loadCarrierAccountSelector]).catch(function (error) {
			    VRNotificationService.notifyException(error, $scope);
			}).finally(function () {
			    $scope.scopeModel.isLoading = false;
			});
		}

		

		function loadCarrierAccountSelector() {
		    var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
		    carrierAccountSelectorReadyDeferred.promise.then(function () {		        
		        VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, undefined, carrierAccountSelectorLoadDeferred);
		    });
		    return carrierAccountSelectorLoadDeferred.promise;
		}
		function getFilterObject() {
		    return {
		        CarrierAccountIds: carrierAccountSelectorAPI.getSelectedIds(),
		        Name: $scope.scopeModel.description
		    };
		}
	}

	appControllers.controller('WhS_Deal_SwapDealManagementController', DealAnalysisManagementController);

})(appControllers);