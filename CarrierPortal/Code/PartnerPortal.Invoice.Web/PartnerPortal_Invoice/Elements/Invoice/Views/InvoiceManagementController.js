(function (appControllers) {

    "use strict";

    invoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'PartnerPortal_Invoice_InvoiceViewerTypeAPIService', 'VRDateTimeService'];

    function invoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, PartnerPortal_Invoice_InvoiceViewerTypeAPIService, VRDateTimeService) {
        var invoiceViewerTypeSelectorAPI;
        var invoiceViewerTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var selectedViewerTypePromiseDeferred = UtilsService.createPromiseDeferred();
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
            var date = VRDateTimeService.getNowDateTime();
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
            $scope.onInvoiceViewerTypeSelectorSelectionChanged = function (value) {

                var invoiceViewerTypeId = invoiceViewerTypeSelectorAPI.getSelectedIds();
                if (invoiceViewerTypeId != undefined) {
                    if (selectedViewerTypePromiseDeferred != undefined) {
                        selectedViewerTypePromiseDeferred.resolve();
                    } else {
                        var promises = [];
                        promises.push(getInvoiceViewerType(invoiceViewerTypeId));
                        promises.push(loadInvoiceAccountsSelectorDirective());
                        UtilsService.waitMultiplePromises(promises).then(function () {
                            return gridAPI.loadGrid(getFilterObject());
                        });
                    }
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
            return UtilsService.waitMultipleAsyncOperations([loadInvoiceViewerTypeSelectorDirective, loadAccountSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function loadAccountSelector() {
            var loadAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            selectedViewerTypePromiseDeferred.promise.then(function () {
                selectedViewerTypePromiseDeferred = undefined;
                var promises = [];
                var invoiceViewerTypeId = invoiceViewerTypeSelectorAPI.getSelectedIds();
                promises.push(getInvoiceViewerType(invoiceViewerTypeId));
                promises.push(loadInvoiceAccountsSelectorDirective());
                UtilsService.waitMultiplePromises(promises).then(function () {
                    loadAccountSelectorLoadDeferred.resolve();
                    gridAPI.loadGrid(getFilterObject());
                }).catch(function (error) {
                    loadAccountSelectorLoadDeferred.reject(error);
                });
            }).catch(function (error) {
                loadAccountSelectorLoadDeferred.reject(error);
            });
            return loadAccountSelectorLoadDeferred.promise;
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
                            $type: "PartnerPortal.Invoice.Business.InvoiceViewerTypeViewFilter,PartnerPortal.Invoice.Business",
                            ViewId: viewId
                        }]
                    },
                    selectFirstItem: true
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