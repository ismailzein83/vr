(
    function (appControllers) {
        "use strict";

        function supplierManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, supplierService) {
            var gridAPI;

            function defineScope() {

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid();
                }

                $scope.AddSupplier = function () {
                    supplierService.addSupplier();
                }
            }

            defineScope();

        }

        supplierManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierService'];
        appControllers.controller('CP_SupplierPriceList_SupplierManagementController', supplierManagementController);
    }
)(appControllers);