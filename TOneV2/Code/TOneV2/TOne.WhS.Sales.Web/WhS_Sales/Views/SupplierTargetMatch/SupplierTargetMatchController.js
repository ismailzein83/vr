(function (appControllers) {

    "use strict";

	supplierTargetMatchController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService','WhS_Sales_SupplierTargetMatchService'];

	function supplierTargetMatchController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_Sales_SupplierTargetMatchService) {
        var gridAPI;

        var targetMatchFilterDirectiveAPI;
        var targetMatchFilterReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
		load();
        
        function defineScope() {
            $scope.scopeModel = {};
            $scope.loadClicked = function () {
				return gridAPI.load(getFilter()).then(function () {
					$scope.showExportButton = true;
				});
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.onTargetMatchFilterDirectiveReady = function (api) {
                targetMatchFilterDirectiveAPI = api;
                targetMatchFilterReadyPromiseDeferred.resolve();
			};

			$scope.scopeModel.exportData = function () {
				var onExportSupplierTargetMatch = function (exportSettings) {
					var query = getFilter();
					query.DefaultACD = exportSettings.DefaultACD;
					query.DefaultASR = exportSettings.DefaultASR;
					query.DefaultVolume = exportSettings.DefaultVolume;
					query.IncludeACD_ASR = exportSettings.IncludeACD_ASR;
					return gridAPI.export(query);
				};
				WhS_Sales_SupplierTargetMatchService.exportSupplierTargetMatch(onExportSupplierTargetMatch);
			};

        }

        function load() {
            $scope.isLoadingFilter = true;

            UtilsService.waitMultipleAsyncOperations([loadTargetMatchFilter]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilter = false;
            });
        }



        function loadTargetMatchFilter() {
            var loadTargetMatchFilterDeferred = UtilsService.createPromiseDeferred();
            targetMatchFilterReadyPromiseDeferred.promise.then(function () {

                VRUIUtilsService.callDirectiveLoad(targetMatchFilterDirectiveAPI, undefined, loadTargetMatchFilterDeferred);
            });

            return loadTargetMatchFilterDeferred.promise;
        }

        function getFilter() {
            return targetMatchFilterDirectiveAPI.getData();
        }
    }

    appControllers.controller('WhS_Sales_SupplierTargetMatchController', supplierTargetMatchController);
})(appControllers);