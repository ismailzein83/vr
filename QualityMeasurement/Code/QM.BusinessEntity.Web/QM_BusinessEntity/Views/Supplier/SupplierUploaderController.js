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
            return QM_BE_SupplierAPIService.UploadSuppliersList($scope.uploadedFile.fileId, $scope.chkAllowUpdate).then(function (response) {
                VRNotificationService.showInformation(response)
            });
        }

        function DownloadTemplate() {
            return QM_BE_SupplierAPIService.DownloadImportSupplierTemplate().then(function (response) {
                console.log(response)
                UtilsService.downloadFile(response.data, response.headers);
            });
        }
    }

    appControllers.controller('QM_BE_SupplierUploaderController', supplierUploaderController);
})(appControllers);









