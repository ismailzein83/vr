(function (appControllers) {

    "use strict";

    supplierUploaderController.$inject = ['$scope', 'QM_BE_SupplierService', 'QM_BE_SupplierAPIService'];

    function supplierUploaderController($scope, QM_BE_SupplierService, QM_BE_SupplierAPIService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            };



            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewSupplier = AddNewSupplier;
            $scope.hasAddSupplierPermission = function () {
                return QM_BE_SupplierAPIService.HasAddSupplierPermission();
            };

            $scope.UploadNewSuppliers = UploadNewSuppliers;
            $scope.hasUploadSupplierPermission = function () {
                return QM_BE_SupplierAPIService.HasUploadSupplierPermission();
            };

            function getFilterObject() {
                var query = {
                    Name: $scope.name
                };
                return query;
            }
        }

        function load() {
        }

        function UploadNewSuppliers() {
            QM_BE_SupplierService.uploadNewSuppliers();
        }

        function AddNewSupplier() {
            var onSupplierAdded = function (addedItem) {
                gridAPI.onSupplierAdded(addedItem);
            };

            QM_BE_SupplierService.addSupplier(onSupplierAdded);
        }
    }

    appControllers.controller('QM_BE_SupplierManagementController', supplierUploaderController);
})(appControllers);