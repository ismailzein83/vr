(function (appControllers) {

    "use strict";

    invoiceTypeManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceTypeService', 'VR_Invoice_InvoiceTypeAPIService', 'VRNotificationService'];

    function invoiceTypeManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceTypeService, VR_Invoice_InvoiceTypeAPIService, VRNotificationService) {
        var gridAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
            }
        }
        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
            $scope.hasAddInvoiceTypePermission = function () {
                return VR_Invoice_InvoiceTypeAPIService.HasAddInvoiceTypePermission();
            };
            $scope.addInvoiceType = addInvoiceType;
        }
        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }
        function getFilterObject() {
            var query = {
                Name: $scope.name,
            };
            return query;
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function addInvoiceType() {
            var onInvoiceTypeAdded = function (invoice) {
            };
            VR_Invoice_InvoiceTypeService.addInvoiceType(onInvoiceTypeAdded);
        }
    }

    appControllers.controller('VR_Invoice_InvoiceTypeManagementController', invoiceTypeManagementController);
})(appControllers);