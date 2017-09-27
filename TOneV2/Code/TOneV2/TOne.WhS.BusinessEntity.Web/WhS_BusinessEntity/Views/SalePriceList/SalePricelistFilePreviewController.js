(function (appControllers) {
    "use strict";

    salePricelistFilePreviewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_BE_SalePriceListChangeAPIService', 'VRModalService'];

    function salePricelistFilePreviewController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, whSBeSalePriceListChangeApiService, VRModalService) {

        var gridApi;
        var readyPromiseDeferred = utilsService.createPromiseDeferred();
        var vrFiles = {};

        loadParameters();
        defineScope();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.onGridReady = function (api) {
                gridApi = api;
                gridApi.loadGrid(vrFiles);
            };
        }
        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters) {
                vrFiles = parameters.vrFiles;
            }
        }
    }
    appControllers.controller('WhS_BE_SalePricelistFilePreviewController', salePricelistFilePreviewController);
})(appControllers);