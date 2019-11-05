(function (appControllers) {

    "use strict";

    invoiceTypeManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceTypeService', 'VR_Invoice_InvoiceTypeAPIService', 'VRNotificationService'];

    function invoiceTypeManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceTypeService, VR_Invoice_InvoiceTypeAPIService, VRNotificationService) {
        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
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
                DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
            };
            return query;
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDevProjectSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function addInvoiceType() {
            var onInvoiceTypeAdded = function (invoice) {
                gridAPI.onInvoiceTypeAdded(invoice);
            };
            VR_Invoice_InvoiceTypeService.addInvoiceType(onInvoiceTypeAdded);
        }
    }

    appControllers.controller('VR_Invoice_InvoiceTypeManagementController', invoiceTypeManagementController);
})(appControllers);