
(function (appControllers) {

    "use strict";

    invoiceSettingEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Invoice_InvoiceSettingService'];

    function invoiceSettingEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Invoice_InvoiceSettingService) {

        var isEditMode;
        var invoiceSettingEntity;
        var customerInvoiceMailTemplateReadyAPI;
        var customerInvoiceMailTemplateReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var serialNumberPatternAPI;
        var serialNumberPatternReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var billingPeriodAPI;
        var billingPeriodReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceSettingEntity = parameters.invoiceSettingEntity;
            }
            isEditMode = (invoiceSettingEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.saveInvoiceSetting = function () {
                if (isEditMode)
                    return updateInvoiceSettings();
                else
                    return insertInvoiceSettings();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onCustomerInvoiceMailTemplateSelectorReady = function (api) {
                customerInvoiceMailTemplateReadyAPI = api;
                customerInvoiceMailTemplateReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onSerialNumberPatternReady = function (api) {
                serialNumberPatternAPI = api;
                serialNumberPatternReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.onBillingPeriodReady = function (api) {
                billingPeriodAPI = api;
                billingPeriodReadyPromiseDeferred.resolve();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();

            loadMailMsgTemplateSelector();
            loadSerialNumberPattern();
            loadBillingPeriod();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }

        function setTitle() {
            if (isEditMode && invoiceSettingEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(invoiceSettingEntity.Title, "Invoice Setting");
            else
                $scope.title = UtilsService.buildTitleForAddEditor("Invoice Setting");
        }

        function loadStaticData() {
            if (invoiceSettingEntity == undefined)
                return;
            $scope.scopeModel.title = invoiceSettingEntity.Title;
            $scope.scopeModel.duePeriod = invoiceSettingEntity.DuePeriod;
            $scope.scopeModel.isFollow = invoiceSettingEntity.IsFollow;
            $scope.scopeModel.isDefault = invoiceSettingEntity.IsDefault;
        }

        function loadMailMsgTemplateSelector() {
            var mailMsgTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            WhS_Invoice_InvoiceSettingService.getCustomerInvoiceMailType().then(function(response) {
                customerInvoiceMailTemplateReadyPromiseDeferred.promise.then(function() {

                    var selectorPayload = { filter: { VRMailMessageTypeId: response } };
                    if (invoiceSettingEntity != undefined) {
                        selectorPayload.selectedIds = invoiceSettingEntity.DefaultEmailId;
                    }
                    VRUIUtilsService.callDirectiveLoad(customerInvoiceMailTemplateReadyAPI, selectorPayload, mailMsgTemplateSelectorLoadDeferred);
                });
            });


            return mailMsgTemplateSelectorLoadDeferred.promise;
        }

        function loadSerialNumberPattern() {
            var serialNumberPatternDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            serialNumberPatternReadyPromiseDeferred.promise.then(function () {
                var serialNumberPatternDirectivePayload = { invoiceTypeId: "EADC10C8-FFD7-4EE3-9501-0B2CE09029AD" };
                if (invoiceSettingEntity != undefined)
                    serialNumberPatternDirectivePayload.serialNumberPattern = invoiceSettingEntity.SerialNumberPattern;
                VRUIUtilsService.callDirectiveLoad(serialNumberPatternAPI, serialNumberPatternDirectivePayload, serialNumberPatternDeferredLoadPromiseDeferred);
            });
            return serialNumberPatternDeferredLoadPromiseDeferred.promise;
        }

        function loadBillingPeriod() {
            var billingPeriodDeferredLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            billingPeriodReadyPromiseDeferred.promise.then(function () {
                var billingPeriodDirectivePayload = {};
                if (invoiceSettingEntity != undefined)
                    billingPeriodDirectivePayload.billingPeriodEntity = invoiceSettingEntity.BillingPeriod;
                VRUIUtilsService.callDirectiveLoad(billingPeriodAPI, billingPeriodDirectivePayload, billingPeriodDeferredLoadPromiseDeferred);
            });
            return billingPeriodDeferredLoadPromiseDeferred.promise;
        }

        function buildInvoiceSettingsObjFromScope() {
            var obj = {
                Title: $scope.scopeModel.title,
                DuePeriod: $scope.scopeModel.duePeriod,
                DefaultEmailId: customerInvoiceMailTemplateReadyAPI.getSelectedIds(),
                IsFollow: $scope.scopeModel.isFollow,
                IsDefault: $scope.scopeModel.isDefault,
                BillingPeriod: billingPeriodAPI.getData(),
                SerialNumberPattern: serialNumberPatternAPI.getData()
            };
            return obj;
        }

        function insertInvoiceSettings() {
            var invoiceSettingsObject = buildInvoiceSettingsObjFromScope();
            if ($scope.onInvoiceSettingsAdded != undefined)
                $scope.onInvoiceSettingsAdded(invoiceSettingsObject);
            $scope.modalContext.closeModal();
        }
        function updateInvoiceSettings() {
            var invoiceSettingsObject = buildInvoiceSettingsObjFromScope();
            if ($scope.onInvoiceSettingsUpdated != undefined)
                $scope.onInvoiceSettingsUpdated(invoiceSettingsObject);
            $scope.modalContext.closeModal();
        }
    }

    appControllers.controller('WhS_Invoice_InvoiceSettingEditorController', invoiceSettingEditorController);
})(appControllers);
