(function (appControllers) {

    "use strict";

    genericInvoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VR_Invoice_InvoiceService', 'VR_Invoice_InvoiceTypeAPIService','VRNotificationService'];

    function genericInvoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_InvoiceService, VR_Invoice_InvoiceTypeAPIService, VRNotificationService) {
        var invoiceTypeId;
        $scope.invoiceTypeEntity;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {
            var date = new Date();
            $scope.fromDate = new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0, 0);
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());

            };
            $scope.onPartnerSelectorReady = function(api)
            {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            }
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());

            };
            $scope.generateInvoice = generateInvoice;
        }

        function load() {
            $scope.isLoadingFilters = true;
            getInvoiceTypeRuntime().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoadingFilters = false;
            });
           
        }
        function getFilterObject() {
            var partnerObject = partnerSelectorAPI.getData();
            var filter = {
                mainGridColumns: $scope.invoiceTypeEntity.MainGridRuntimeColumns,
                subSections: $scope.invoiceTypeEntity.InvoiceType.Settings.UISettings.SubSections,
                invoiceGridActions: $scope.invoiceTypeEntity.InvoiceType.Settings.UISettings.InvoiceGridActions,
                InvoiceTypeId:invoiceTypeId,
                query: {
                    FromTime: $scope.fromDate,
                    ToTime: $scope.toDate,
                    PartnerIds: partnerObject != undefined ? partnerObject.selectedIds : undefined,
                    PartnerPrefix:partnerObject != undefined?partnerObject.partnerPrefix:undefined,
                    InvoiceTypeId: invoiceTypeId,
                    IssueDate: $scope.issueDate
                }
            };
            return filter;
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadPartnerSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }
        function getInvoiceTypeRuntime()
        {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypeRuntime(invoiceTypeId).then(function (response) {
               $scope.invoiceTypeEntity = response;
            });
        }

        function generateInvoice() {
            var onGenerateInvoice = function (invoice) {
                gridAPI.onGenerateInvoice(invoice);
            };

            VR_Invoice_InvoiceService.generateInvoice(onGenerateInvoice, invoiceTypeId);
        }
        function loadPartnerSelectorDirective() {
            var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            partnerSelectorReadyDeferred.promise.then(function () {
                var partnerSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
            });
            return partnerSelectorPayloadLoadDeferred.promise;
        }
    }

    appControllers.controller('VR_Invoice_GenericInvoiceManagementController', genericInvoiceManagementController);
})(appControllers);