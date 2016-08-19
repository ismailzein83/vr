﻿(function (appControllers) {

    "use strict";

    genericInvoiceEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService'];

    function genericInvoiceEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService) {
        var invoiceTypeId;
        $scope.invoiceTypeEntity;
        var partnerSelectorAPI;
        var partnerSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        loadParameters();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }
        function defineScope() {

            $scope.onPartnerSelectorReady = function (api) {
                partnerSelectorAPI = api;
                partnerSelectorReadyDeferred.resolve();
            }

            $scope.generateInvoice = function () {
                return generateInvoice();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;
            getInvoiceType().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerSelectorDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        function setTitle() {
            $scope.title = "Generate Invoice";
        }
        function getInvoiceType() {
            return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                $scope.invoiceTypeEntity = response;
            });
        }
        function loadPartnerSelectorDirective() {
            var partnerSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
            partnerSelectorReadyDeferred.promise.then(function () {
                var partnerSelectorPayload;
                VRUIUtilsService.callDirectiveLoad(partnerSelectorAPI, partnerSelectorPayload, partnerSelectorPayloadLoadDeferred);
            });
            return partnerSelectorPayloadLoadDeferred.promise;
        }
        function loadStaticData() {
        }
        function buildInvoiceObjFromScope() {
            var obj = {
                InvoiceTypeId: invoiceTypeId,
                PartnerId:partnerSelectorAPI.getSelectedIds(),
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate
            };
            return obj;
        }
        function generateInvoice() {
            var incvoiceObject = buildInvoiceObjFromScope();
            return VR_Invoice_InvoiceAPIService.GenerateInvoice(incvoiceObject)
           .then(function (response) {
               $scope.modalContext.closeModal();
           })
           .catch(function (error) {
               VRNotificationService.notifyException(error, $scope);
           }).finally(function () {
               $scope.scopeModal.isLoading = false;
           });
        }
    }

    appControllers.controller('VR_Invoice_GenericInvoiceEditorController', genericInvoiceEditorController);
})(appControllers);
