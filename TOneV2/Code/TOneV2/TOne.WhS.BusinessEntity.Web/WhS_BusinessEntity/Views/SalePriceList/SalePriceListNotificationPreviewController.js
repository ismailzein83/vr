(function (appControllers) {
    "use strict";

    salePriceListNotificationPreview.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'WhS_BE_SalePriceListChangeAPIService'];

    function salePriceListNotificationPreview($scope, utilsService, vrNotificationService, vrNavigationService, WhS_BE_SalePriceListChangeAPIService) {
        var pricelistId;
        var customerId;
        var ownerName;

        loadParameters();
        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.notififcationsDataSource = [];
            $scope.downloadPricelist = function (dataItem) {
                WhS_BE_SalePriceListChangeAPIService.DownloadSalePriceList(dataItem.FileId).then(function (bufferArrayRespone) {
                    utilsService.downloadFile(bufferArrayRespone.data, bufferArrayRespone.headers);
                });
            };
            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            WhS_BE_SalePriceListChangeAPIService.getSalePricelistNotifictaion(pricelistId).then(function (response) {
                $scope.scopeModel.notififcationsDataSource = response;
            });

        }

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined) {
                pricelistId = parameters.pricelistId;
                customerId = parameters.customerId;
            }
        }

        function setTitle() {
            $scope.title = 'Email Notification For ' + ownerName;
        }
        function GetCustomerName() {
            return WhS_BE_SalePriceListChangeAPIService.GetCustomerName(customerId)
                .then(function (response) {
                    ownerName = response;
                });
        }
        function loadAllControls() {
            $scope.isLoadingFilter = true;
            return utilsService.waitMultipleAsyncOperations([GetCustomerName])
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
    appControllers.controller('WhS_BE_SalePriceListNotificationPreviewController', salePriceListNotificationPreview);
})(appControllers);