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
                var payload = {
                    priceListId: priceListId
                };
                zonePreviewDirectiveAPI.load(payload);
            }
            $scope.onCodePreviewDirectiveReady = function (api) {
                codePreviewDirectiveAPI = api;
                var payload = {
                    priceListId: priceListId
                };
                codePreviewDirectiveAPI.load(payload);
            }
            $scope.onRatePreviewDirectiveReady = function (api) {
                ratePreviewDirectiveAPI = api;
                var payload = {
                    priceListId: priceListId
                };
                ratePreviewDirectiveAPI.load(payload);
            }
        }

        function load() {
        }
       

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
        
    }

    appControllers.controller('WhS_SupPL_SupplierPriceListPreviewController', supplierPriceListPreviewController);
})(appControllers);
