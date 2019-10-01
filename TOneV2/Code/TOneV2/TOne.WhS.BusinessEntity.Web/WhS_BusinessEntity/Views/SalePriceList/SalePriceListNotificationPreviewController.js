(function (appControllers) {
    "use strict";

    salePriceListNotificationPreview.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService'];

    function salePriceListNotificationPreview($scope, utilsService, vrNotificationService, vrNavigationService) {
        var pricelistId;


        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.downloadPricelist = function (dataItem) {
                WhS_BE_SalePriceListChangeAPIService.DownloadSalePriceList(dataItem.FileId).then(function (bufferArrayRespone) {
                    utilsService.downloadFile(bufferArrayRespone.data, bufferArrayRespone.headers);
                });
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                pricelistId = param.pricelistId;
            }
        }

        function setTitle() {
            $scope.title = 'Sale Pricelist Notififcation For PricelistId ' + pricelistId;
        }

        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([])
                .then(function () {
                    setTitle();
                })
                .catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoadingFilter = false;
                });
        }
    }
    appControllers.controller('WhS_BE_SalePriceListNotificationPreview', salePriceListNotificationPreview);
})(appControllers);