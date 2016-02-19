(
    function (appControllers) {
        "use strict";

        function customerManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, customerManagmentApiService) {
            var gridAPI;

            function defineScope() {

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid();
                }
            }

            defineScope();

        }

        customerManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPricelist_CustomerManagmentAPIService'];
        appControllers.controller('CP_SupplierPriceList_CustomerManagementController', customerManagementController);
    }
)(appControllers);