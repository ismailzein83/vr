﻿(function (appControllers) {

    "use strict";

    invoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService',  'VRNotificationService','PartnerPortal_Invoice_InvoiceViewerTypeAPIService'];

    function invoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, PartnerPortal_Invoice_InvoiceViewerTypeAPIService) {
        var invoiceViewerTypeSelectorAPI;
        var invoiceViewerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceAccountsSelectorAPI;
        var invoiceAccountsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var viewId;

        var invoiceViewerTypeRuntimeEntity;
        var gridAPI;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {
            var date = new Date();
            $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0, 0);
            $scope.onGridReady = function (api) {
                gridAPI = api;
            };
            $scope.onInvoiceViewerTypeSelectorReady = function (api) {
                invoiceViewerTypeSelectorAPI = api;
                invoiceViewerTypeSelectorReadyDeferred.resolve();
            };
            $scope.onInvoiceAccountsSelectorReady = function (api) {
                invoiceAccountsSelectorAPI = api;
                invoiceAccountsSelectorReadyDeferred.resolve();
            };
            $scope.onInvoiceViewerTypeSelectorSelectionChanged = function () {
                var invoiceViewerTypeId = invoiceViewerTypeSelectorAPI.getSelectedIds();
                if (invoiceViewerTypeId != undefined)
                {
                    var promises = [];
                    promises.push(getInvoiceViewerType(invoiceViewerTypeId));
                    promises.push(loadInvoiceAccountsSelectorDirective());
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        return gridAPI.loadGrid(getFilterObject());
                    });
                }
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());

            };
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }
        function getFilterObject() {
            var invoiceViewerTypeId = invoiceViewerTypeSelectorAPI.getSelectedIds();
            var filter = {
                gridColumns: invoiceViewerTypeRuntimeEntity.RuntimeGridColumns,
                gridActions: invoiceViewerTypeRuntimeEntity.InvoiceGridActions,
                invoiceTypeId: invoiceViewerTypeRuntimeEntity.InvoiceTypeId,
                invoiceViewerTypeId: invoiceViewerTypeId,
                query: {
                    FromTime: $scope.fromDate,
                    ToTime: $scope.toDate,
                    InvoiceViewerTypeId: invoiceViewerTypeId,
                    PartnerIds: invoiceAccountsSelectorAPI.getSelectedIds()
                }
            };
            return filter;
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadInvoiceViewerTypeSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function getInvoiceViewerType(invoiceViewerTypeId) {
            return PartnerPortal_Invoice_InvoiceViewerTypeAPIService.GetInvoiceViewerTypeRuntime(invoiceViewerTypeId).then(function (response) {
                invoiceViewerTypeRuntimeEntity = response;
            });
        }
        function loadInvoiceViewerTypeSelectorDirective() {
            var invoiceViewerTypeSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            invoiceViewerTypeSelectorReadyDeferred.promise.then(function () {
                var invoiceViewerTypeSelectorPayload = {
                    filter: {
                        Filters: [{
                            $type:"PartnerPortal.Invoice.Business.InvoiceViewerTypeViewFilter,PartnerPortal.Invoice.Business",
                             ViewId:viewId
                        }]
                    },
                };
                VRUIUtilsService.callDirectiveLoad(invoiceViewerTypeSelectorAPI, invoiceViewerTypeSelectorPayload, invoiceViewerTypeSelectorPayloadLoadDeferred);
            });
            return invoiceViewerTypeSelectorPayloadLoadDeferred.promise;
        }
        function loadInvoiceAccountsSelectorDirective() {
            var invoiceAccountsSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            invoiceAccountsSelectorReadyDeferred.promise.then(function () {
                var invoiceAccountsSelectorPayload = {
                    invoiceViewerTypeId: invoiceViewerTypeSelectorAPI.getSelectedIds(),
                };
                VRUIUtilsService.callDirectiveLoad(invoiceAccountsSelectorAPI, invoiceAccountsSelectorPayload, invoiceAccountsSelectorPayloadLoadDeferred);
            });
            return invoiceAccountsSelectorPayloadLoadDeferred.promise;
        }
    }

    appControllers.controller('PartnerPortal_Invoice_InvoiceManagementController', invoiceManagementController);
})(appControllers);