(
    function (appControllers) {
        "use strict";

        function supplierManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, supplierMappingService) {
            var gridAPI;

            function defineScope() {

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid();
                }

                $scope.AddSupplierMapping = function () {
                    supplierMappingService.addSupplierMapping();
                }
            }

            defineScope();

        }

        supplierManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPricelist_SupplierMappingService'];
        appControllers.controller('CP_SupplierPriceList_SupplierManagementController', supplierManagementController);
    }
)(appControllers);