(function (appControllers) {

    "use strict";

    supplierManagementController.$inject = ['$scope', 'QM_BE_SupplierService'];

    function supplierManagementController($scope, QM_BE_SupplierService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            $scope.searchClicked = function () {
                if (gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewSupplier = AddNewSupplier;

            function getFilterObject() {
                var query = {
                    Name: $scope.name
                };
                return query;
            }
        }

        function load() {
        }


        function AddNewSupplier() {
            var onSupplierAdded = function (addedItem) {
                gridAPI.onSupplierAdded(addedItem);
            };

            QM_BE_SupplierService.addSupplier(onSupplierAdded);
        }
    }

    appControllers.controller('QM_BE_SupplierManagementController', supplierManagementController);
})(appControllers);