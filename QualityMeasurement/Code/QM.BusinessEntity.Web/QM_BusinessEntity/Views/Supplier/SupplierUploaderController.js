(function (appControllers) {
    "use strict";

    supplierUploaderController.$inject = ['$scope', 'QM_BE_SupplierAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

    function supplierUploaderController($scope, QM_BE_SupplierAPIService, VRNotificationService, VRNavigationService, UtilsService) {

        defineScope();
        load();

        function defineScope() {
            $scope.uploadSuppliers = UploadSuppliers;
            $scope.downloadTemplate = DownloadTemplate;

        }

        function load() {
        }

        function UploadSuppliers() {
            return QM_BE_SupplierAPIService.UploadSuppliers($scope.uploadedFile.fileId).then(function (response) {
                VRNotificationService.showInformation(response)
            });
        }

        function DownloadTemplate() {
            return QM_BE_SupplierAPIService.DownloadImportSupplierTemplate().then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        }
    }

    appControllers.controller('QM_BE_SupplierUploaderController', supplierUploaderController);
})(appControllers);









