(function (appControllers) {

    "use strict";

    invoiceGenerationProcessEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRDateTimeService', 'VR_Invoice_InvoiceGenerationPeriodEnum', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService'];

    function invoiceGenerationProcessEditorController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRValidationService, VRDateTimeService, VR_Invoice_InvoiceGenerationPeriodEnum, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService) {
        var invoiceTypeId;
        var invoiceType;

        var invoicePartnerGroupAPI;
        var invoicePartnerGroupReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceGenerationPeriodAPI;

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceGenerationDraftGridAPI;
        var invoiceGenerationDraftGridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                invoiceTypeId = parameters.invoiceTypeId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onInvoicePartnerGroupReady = function (api) {
                invoicePartnerGroupAPI = api;
                invoicePartnerGroupReadyDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceGenerationPeriodReady = function (api) {
                invoiceGenerationPeriodAPI = api;
            };

            $scope.scopeModel.validateDates = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
            };

            $scope.scopeModel.areDatesRequired = function () {
                if ($scope.scopeModel.selectedInvoiceGenerationPeriod != undefined && $scope.scopeModel.selectedInvoiceGenerationPeriod.datesRequired)
                    return true;
                return false;
            };

            $scope.scopeModel.onAccountStatusSelectorReady = function (api) {
                accountStatusSelectorAPI = api;
                accountStatusSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountStatusSelectionChanged = function (selectedItem) {
                if (invoicePartnerGroupAPI != undefined) {
                    var accountStatusData = accountStatusSelectorAPI.getData();
                    invoicePartnerGroupAPI.accountStatusChanged(accountStatusData);
                }
            };

            $scope.scopeModel.onInvoiceGenerationDraftReady = function (api) {
                invoiceGenerationDraftGridAPI = api;
                invoiceGenerationDraftGridReadyDeferred.resolve();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.evaluate = function () {
                $scope.scopeModel.errorMessage = undefined;
                $scope.scopeModel.isLoading = true;
                var accountStatusData = accountStatusSelectorAPI.getData();
                var query = {
                    InvoiceTypeId: invoiceTypeId,
                    EffectiveDate: accountStatusData.EffectiveDate,
                    IsEffectiveInFuture: accountStatusData.IsEffectiveInFuture,
                    Status: accountStatusData.Status,
                    Period: $scope.scopeModel.selectedInvoiceGenerationPeriod.value,
                    FromDate: $scope.scopeModel.fromDate,
                    ToDate: $scope.scopeModel.toDate,
                    IssueDate: $scope.scopeModel.issueDate,
                    PartnerGroup: invoicePartnerGroupAPI.getData()
                };

                VR_Invoice_InvoiceAPIService.GenerateFilteredInvoiceGenerationDrafts(query).then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    console.log(response);
                    if (response.Result == 0) {
                        var gridPayload = { query: query, customPayloadDirective: invoiceType.Settings.ExtendedSettings.GenerationCustomSection.GenerationCustomSectionDirective };
                        invoiceGenerationDraftGridAPI.loadGrid(gridPayload);
                    }
                    else {
                        $scope.scopeModel.errorMessage = response.Message;
                    }
                });

            };

            UtilsService.waitMultiplePromises([invoiceGenerationDraftGridReadyDeferred.promise]).then(function () {
                load();
            });
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            getInvoiceType().then(function () {
                loadAllControls();
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            });

            function getInvoiceType() {
                return VR_Invoice_InvoiceTypeAPIService.GetInvoiceType(invoiceTypeId).then(function (response) {
                    invoiceType = response;
                });
            }

        }

        function loadAllControls() {

            function setTitle() {
                $scope.title = "Generate Invoices";
            }
            function loadPartnerGroupSelector() {
                var partnerGroupSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                var promises = [];
                promises.push(invoicePartnerGroupReadyDeferred.promise);
                promises.push(loadAccountStatusSelectorDirective());

                UtilsService.waitMultiplePromises(promises).then(function () {
                    var partnerGroupSelectorPaylod = { invoiceTypeId: invoiceTypeId, accountStatus: accountStatusSelectorAPI.getData() };
                    VRUIUtilsService.callDirectiveLoad(invoicePartnerGroupAPI, partnerGroupSelectorPaylod, partnerGroupSelectorLoadDeferred);
                });

                return partnerGroupSelectorLoadDeferred.promise;
            }

            function loadAccountStatusSelectorDirective() {
                var accountStatusSelectorPayloadLoadDeferred = UtilsService.createPromiseDeferred();
                accountStatusSelectorReadyDeferred.promise.then(function () {
                    var accountStatusSelectorPayload = {
                        selectFirstItem: true,
                        dontShowInActive: true
                    };
                    if ($scope.scopeModel.isEditMode)
                        accountStatusSelectorPayload.selectedIds = VR_Invoice_InvoiceAccountStatusEnum.ActiveAndExpired.value;
                    VRUIUtilsService.callDirectiveLoad(accountStatusSelectorAPI, accountStatusSelectorPayload, accountStatusSelectorPayloadLoadDeferred);
                });
                return accountStatusSelectorPayloadLoadDeferred.promise;
            }

            function loadStaticData() {
                $scope.scopeModel.invoiceGenerationPeriods = UtilsService.getArrayEnum(VR_Invoice_InvoiceGenerationPeriodEnum);
                $scope.scopeModel.selectedInvoiceGenerationPeriod = UtilsService.getEnum(VR_Invoice_InvoiceGenerationPeriodEnum, 'value', VR_Invoice_InvoiceGenerationPeriodEnum.FollowBillingCycle.value);
                $scope.scopeModel.issueDate = VRDateTimeService.getNowDateTime();
            }


            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerGroupSelector]).then(function () {
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isLoading = false;
            })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });
        }
    }

    appControllers.controller('VR_Invoice_InvoiceGenerationProcessEditorController', invoiceGenerationProcessEditorController);
})(appControllers);
