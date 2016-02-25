(
    function (appControllers) {
        "use strict";

        function customerManagementController($scope, utilsService, vrNotificationService, vruiUtilsService, customerManagmentApiService, customerService) {
            var gridAPI;

            function defineScope() {

                $scope.onGridReady = function (api) {
                    gridAPI = api;
                    api.loadGrid({});
                }

                $scope.searchClicked = function () {
                    return gridAPI.loadGrid(getFilterObject());
                };
                $scope.AddCustomer = function () {
                    var onCustomerAdded = function (customerObject) {
                        gridAPI.onCustomerAdded(customerObject);
                    }
                    customerService.addCustomer(onCustomerAdded);
                }
            }

            defineScope();
            function getFilterObject() {
                var data = {
                    Name: $scope.CustomerName
                };
                return data;
            }
        }

        customerManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'CP_SupplierPricelist_CustomerManagmentAPIService', 'CP_SupplierPricelist_CustomerService'];
        appControllers.controller('CP_SupplierPriceList_CustomerManagementController', customerManagementController);
    }
)(appControllers);