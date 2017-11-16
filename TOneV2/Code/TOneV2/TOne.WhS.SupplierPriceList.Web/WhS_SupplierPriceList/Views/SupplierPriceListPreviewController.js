(function (appControllers) {

    "use strict";

    supplierPriceListPreviewController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function supplierPriceListPreviewController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var processInstanceId;

        var SupplierPriceListPreviewSectionApi;
        var SupplierPriceListPreviewSectionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            $scope.onSupplierPriceListPreviewSectionReady = function (api) {
                SupplierPriceListPreviewSectionApi = api;
                SupplierPriceListPreviewSectionReadyPromiseDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModal.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSupplierPriceListPreviewSection])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });
        }

        function loadSupplierPriceListPreviewSection() {
            var loadSupplierPriceListPreviewSectionPromiseDeferred = UtilsService.createPromiseDeferred();
            SupplierPriceListPreviewSectionReadyPromiseDeferred.promise.then(function () {
                var SupplierPriceListPreviewSectionPayload = {
                    processInstanceId: processInstanceId,
                    requireWarningConfirmation : false,
                };
                VRUIUtilsService.callDirectiveLoad(SupplierPriceListPreviewSectionApi, SupplierPriceListPreviewSectionPayload, loadSupplierPriceListPreviewSectionPromiseDeferred);
            });
            return loadSupplierPriceListPreviewSectionPromiseDeferred.promise;
        }
    }
    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);