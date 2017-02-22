(function (appControllers) {

    "use strict";

    invoiceSettingManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceSettingService', 'VR_Invoice_InvoiceSettingAPIService', 'VRNotificationService'];

    function invoiceSettingManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceSettingService, VR_Invoice_InvoiceSettingAPIService, VRNotificationService) {
        var gridAPI;

        var invoiceTypeSelectorAPI;
        var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
            }
        }
        function defineScope() {
            $scope.hasAddAccess = false;
            $scope.onInvoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorReadyDeferred.resolve();
            };
            $scope.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
            $scope.hasAddInvoiceSettingPermission = function () {
               
            };
            $scope.onInvoiceTypeSelectionChanged = function () {
                if( invoiceTypeSelectorAPI.getSelectedIds() != undefined)
                {
                    gridAPI.loadGrid(getFilterObject());
                    checkHasAddInvoiceSettingPermission();
                }
                else {
                    $scope.hasAddAccess = false;
                }
            };
            $scope.addInvoiceSetting = addInvoiceSetting;
        }
        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }
        function getFilterObject() {
            var query = {
                Name: $scope.name,
                InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds()
            };
            return query;
        }
        function checkHasAddInvoiceSettingPermission() {
            return VR_Invoice_InvoiceSettingAPIService.HasAddInvoiceSettingPermission(invoiceTypeSelectorAPI.getSelectedIds()).then(function (response) {
                $scope.hasAddAccess = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadInvoiceTypeSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function loadInvoiceTypeSelectorDirective() {
            var invoiceTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            invoiceTypeSelectorReadyDeferred.promise.then(function () {
                var invoiceTypeSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorPayloadLoadDeferred);
            });
            return invoiceTypeSelectorPayloadLoadDeferred.promise;
        }
        function addInvoiceSetting() {
            var onInvoiceSettingAdded = function (invoiceSetting) {
                gridAPI.onInvoiceSettingAdded(invoiceSetting);
            };
            VR_Invoice_InvoiceSettingService.addInvoiceSetting(onInvoiceSettingAdded, invoiceTypeSelectorAPI.getSelectedIds());
        }
    }

    appControllers.controller('VR_Invoice_InvoiceSettingManagementController', invoiceSettingManagementController);
})(appControllers);