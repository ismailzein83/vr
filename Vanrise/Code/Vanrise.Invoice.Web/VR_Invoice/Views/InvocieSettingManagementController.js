(function (appControllers) {

    "use strict";

    invoiceSettingManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceSettingService', 'VR_Invoice_InvoiceSettingAPIService', 'VRNotificationService','VR_Invoice_InvoiceTypeAPIService'];

    function invoiceSettingManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceSettingService, VR_Invoice_InvoiceSettingAPIService, VRNotificationService, VR_Invoice_InvoiceTypeAPIService) {
        var gridAPI;

        var invoiceTypeSelectorAPI;
        var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var invoiceTypeEntity;
        var partnerInvoiceSettingFilterFQTN;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountStatusSelectionChanged = function (value) {
                if (value != undefined) {
                    if (partnerSelectorAPI != undefined) {
                        var setLoader = function (value) {
                            $scope.scopeModel.isLoadingDirective = value;
                        };
                        var partnerSelectorPayload = {
                            extendedSettings: invoiceTypeEntity.Settings.ExtendedSettings,
                            invoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                            filter: accountStatusSelectorAPI.getData()
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerSelectorAPI, partnerSelectorPayload, setLoader);
                    }
                }
            };

            $scope.scopeModel.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
               
            };

            $scope.scopeModel.hasAddAccess = false;

            $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                invoiceTypeSelectorAPI = api;
                invoiceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.scopeModel.hasAddInvoiceSettingPermission = function () {
               
            };

            $scope.scopeModel.onInvoiceTypeSelectionChanged = function (value) {
                if(invoiceTypeSelectorAPI.getSelectedIds() != undefined)
                {
                    UtilsService.waitMultipleAsyncOperations([checkHasAddInvoiceSettingPermission, getInvoicePartnerSelector, getInvoiceType]).then(function () {
                        if($scope.scopeModel.partnerInvoiceSelector != undefined)
                        {
                            partnerSelectorReadyDeferred.promise.then(function () {
                                partnerSelectorReadyDeferred = undefined;
                                var setLoader = function (value) {
                                    $scope.scopeModel.isLoadingDirective = value;
                                };
                                var partnerSelectorPayload = {
                                    extendedSettings: invoiceTypeEntity.Settings.ExtendedSettings,
                                    invoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                                    filter: accountStatusSelectorAPI.getData()
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, partnerSelectorAPI, partnerSelectorPayload, setLoader);
                            });
                        }
                        gridAPI.loadGrid(getFilterObject());
                    });
                }
                else {
                    $scope.scopeModel.hasAddAccess = false;
                }
            };
            $scope.scopeModel.addInvoiceSetting = addInvoiceSetting;
        }
        function load() {
            $scope.scopeModel.isLoadingFilters = true;
            loadAllControls();
        }

      
        function getFilterObject() {
            var partnerIds;
            if (partnerSelectorAPI != undefined) {
                var partnerObj = partnerSelectorAPI.getData();
                if (partnerObj != undefined)
                    partnerIds = partnerObj.selectedIds;
            }
            var payload = {
                showAccountSelector: partnerIds != undefined && partnerIds.length > 0 ? false : true,
                partnerIds:partnerIds,
                query: {
                    Name: $scope.name,
                    InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                    PartnerIds: partnerIds
                }
            };
            return payload;
        }
        function checkHasAddInvoiceSettingPermission() {
            return VR_Invoice_InvoiceSettingAPIService.HasAddInvoiceSettingPermission(invoiceTypeSelectorAPI.getSelectedIds()).then(function (response) {
                $scope.scopeModel.hasAddAccess = response;
            });
        }
        function getInvoicePartnerSelector()
        {
            partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            return VR_Invoice_InvoiceTypeAPIService.GetInvoicePartnerSelector(invoiceTypeSelectorAPI.getSelectedIds()).then(function(response){
                $scope.scopeModel.partnerInvoiceSelector = response;
            });
        }

        function getInvoiceType()
        {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeSelectorAPI.getSelectedIds()).then(function(response){
                invoiceTypeEntity = response;
            });
        }
        function loadAllControls() {

            function loadAccountStatusSelectorDirective() {
                var accountStatusSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                accountStatusSelectorReadyDeferred.promise.then(function () {
                    var accountStatusSelectorPayload = { selectFirstItem: true };
                    VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, accountStatusSelectorPayloadLoadDeferred);
                });
                return accountStatusSelectorPayloadLoadDeferred.promise;
            }
            function loadInvoiceTypeSelectorDirective() {
                var invoiceTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                invoiceTypeSelectorReadyDeferred.promise.then(function () {
                    var invoiceTypeSelectorPayload;
                    VRUIUtilsService.callDirectiveLoad(invoiceTypeSelectorAPI, invoiceTypeSelectorPayload, invoiceTypeSelectorPayloadLoadDeferred);
                });
                return invoiceTypeSelectorPayloadLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadInvoiceTypeSelectorDirective, loadAccountStatusSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoadingFilters = false;
              });
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