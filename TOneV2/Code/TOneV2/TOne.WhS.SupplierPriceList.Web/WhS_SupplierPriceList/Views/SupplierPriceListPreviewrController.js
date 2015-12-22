(function (appControllers) {

    "use strict";

    supplierPriceListPreviewController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
        var priceListId;
        var zonePreviewDirectiveAPI;
        var zonePreviewReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var codePreviewDirectiveAPI;
        var codePreviewReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        var ratePreviewDirectiveAPI;
        var ratePreviewReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        function defineScope() {

            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                priceListId = parameters.PriceListId;
            }
           
            $scope.onZonePreviewDirectiveReady = function (api) {
                zonePreviewDirectiveAPI = api;
                zonePreviewReadyPromiseDeferred.resolve();
            }
            $scope.onCodePreviewDirectiveReady = function (api) {
                codePreviewDirectiveAPI = api;
                codePreviewReadyPromiseDeferred.resolve();
            }
            $scope.onRatePreviewDirectiveReady = function (api) {
                ratePreviewDirectiveAPI = api;
                ratePreviewReadyPromiseDeferred.resolve();
            }
        }

        function load() {
           
               
          
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls()
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadZonePreview, loadCodePreview, loadRatePreview])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadZonePreview() {
            var loadZonePreviewPromiseDeferred = UtilsService.createPromiseDeferred();
            zonePreviewReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    priceListId: priceListId
                };

                VRUIUtilsService.callDirectiveLoad(zonePreviewDirectiveAPI, payload, loadZonePreviewPromiseDeferred);
            });

            return loadZonePreviewPromiseDeferred.promise;
        }
        function loadCodePreview() {
            var loadCodePreviewPromiseDeferred = UtilsService.createPromiseDeferred();
            codePreviewReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    priceListId: priceListId
                };

                VRUIUtilsService.callDirectiveLoad(codePreviewDirectiveAPI, payload, loadCodePreviewPromiseDeferred);
            });

            return loadCodePreviewPromiseDeferred.promise;
        }

        function loadRatePreview() {
            var loadRatePreviewPromiseDeferred = UtilsService.createPromiseDeferred();
            ratePreviewReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    priceListId: priceListId
                };

                VRUIUtilsService.callDirectiveLoad(ratePreviewDirectiveAPI, payload, loadRatePreviewPromiseDeferred);
            });

            return loadRatePreviewPromiseDeferred.promise;
        }

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
        
    }

    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);
