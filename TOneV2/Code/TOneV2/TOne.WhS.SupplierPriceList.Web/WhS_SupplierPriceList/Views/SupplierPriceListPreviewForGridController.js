(function (appControllers) {

    "use strict";

    supplierPriceListPreviewForGridController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewForGridController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var processInstanceId;

        var SupplierPriceListPreviewDirectiveApi;
        var SupplierPriceListPreviewDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                processInstanceId = parameters.processInstanceId;
            }
        }

        function defineScope() {

            $scope.scopeModal = {};

            $scope.onSupplierPriceListPreviewDirectiveReady = function (api) {
                SupplierPriceListPreviewDirectiveApi = api;
                SupplierPriceListPreviewDirectiveReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSupplierPriceListPreviewDirective])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });
        }

        function loadSupplierPriceListPreviewDirective() {
            var loadSupplierPriceListPreviewDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            SupplierPriceListPreviewDirectiveReadyPromiseDeferred.promise.then(function () {
                var SupplierPriceListPreviewDirectivePayload = {
                    processInstanceId: processInstanceId,
                };
                VRUIUtilsService.callDirectiveLoad(SupplierPriceListPreviewDirectiveApi, SupplierPriceListPreviewDirectivePayload, loadSupplierPriceListPreviewDirectivePromiseDeferred);
            });
            return loadSupplierPriceListPreviewDirectivePromiseDeferred.promise;
        }
    }
    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewForGridController', supplierPriceListPreviewForGridController);
})(appControllers);