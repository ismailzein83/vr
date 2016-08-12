(function (appControllers) {

    "use strict";

    genericInvoiceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService','VRNavigationService','VR_Invoice_GenericInvoiceService','VR_Invoice_InvoiceTypeService'];

    function genericInvoiceManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VR_Invoice_GenericInvoiceService, VR_Invoice_InvoiceTypeService) {
        var invoiceTypeId;
        $scope.invoiceTypeEntity;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
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

            $scope.onPartnerSelectorReady = function(api)
            {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            }

            $scope.searchClicked = function () {
            };
            $scope.generateInvoice = generateInvoice;
        }

        function load() {
            $scope.isLoadingFilters = true;
            getInvoiceType().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
           
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
        function getInvoiceType()
        {
           return VR_Invoice_InvoiceTypeService.GetInvoiceType(invoiceTypeId).then(function (response) {
               $scope.invoiceTypeEntity = response;
            });
        }
        function generateInvoice() {
            var onGenerateInvoice = function (invoice) {
            };

            VR_Invoice_GenericInvoiceService.generateInvoice(onGenerateInvoice, invoiceTypeId);
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