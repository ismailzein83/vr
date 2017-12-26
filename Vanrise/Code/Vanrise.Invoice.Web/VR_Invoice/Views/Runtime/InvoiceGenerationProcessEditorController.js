(function (appControllers) {

    "use strict";

    invoiceGenerationProcessEditorController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'VRDateTimeService', 'VR_Invoice_InvoiceGenerationPeriodEnum', 'VR_Invoice_InvoiceTypeAPIService', 'VR_Invoice_InvoiceAPIService', 'VRNotificationService', 'BusinessProcess_BPInstanceService'];

    function invoiceGenerationProcessEditorController($scope, VRNavigationService, UtilsService, VRUIUtilsService, VRValidationService, VRDateTimeService, VR_Invoice_InvoiceGenerationPeriodEnum, VR_Invoice_InvoiceTypeAPIService, VR_Invoice_InvoiceAPIService, VRNotificationService, BusinessProcess_BPInstanceService) {
        var invoiceTypeId;
        var invoiceType;

        var issueDate;

        var invoicePartnerGroupAPI;
        var invoicePartnerGroupReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceGenerationPeriodAPI;

        var accountStatusSelectorAPI;
        var accountStatusSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var invoiceGenerationDraftGridAPI;
        var invoiceGenerationDraftGridReadyDeferred = UtilsService.createPromiseDeferred();

        var invoicePeriodGapActionReadyDeferred = UtilsService.createPromiseDeferred();
        var invoicePeriodGapActionAPI;

        var invoiceGenerationIdentifier;

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


            $scope.scopeModel.onInvoicePeriodGapActionReady = function (api) {
                invoicePeriodGapActionAPI = api;
                invoicePeriodGapActionReadyDeferred.resolve();
            };
            $scope.scopeModel.onInvoicePartnerGroupReady = function (api) {
                invoicePartnerGroupAPI = api;
                invoicePartnerGroupReadyDeferred.resolve();
            };

            $scope.scopeModel.onInvoiceGenerationPeriodReady = function (api) {
                invoiceGenerationPeriodAPI = api;
            };

            $scope.scopeModel.validateFromDate = function () {
                return VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
            };

            $scope.scopeModel.validateToDate = function () {
                var result = VRValidationService.validateTimeRange($scope.scopeModel.fromDate, $scope.scopeModel.toDate);
                if (result != null)
                    return result;

                if ($scope.scopeModel.toDate != undefined && $scope.scopeModel.toDate >= VRDateTimeService.getTodayDate()) {
                    return 'To should be less than today date';
                }
                return null;
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
                issueDate = $scope.scopeModel.issueDate;

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
                    IssueDate: issueDate,
                    PartnerGroup: invoicePartnerGroupAPI.getData(),
                    InvoiceGenerationIdentifier: invoiceGenerationIdentifier
                };


                VR_Invoice_InvoiceAPIService.GenerateFilteredInvoiceGenerationDrafts(query).then(function (response) {
                    $scope.scopeModel.isLoading = false;
                    if (response.Result == 0) {
                        var gridPayload = {
                            query: query,
                            customPayloadDirective: invoiceType.Settings.ExtendedSettings.GenerationCustomSection != undefined ? invoiceType.Settings.ExtendedSettings.GenerationCustomSection.GenerationCustomSectionDirective : undefined,
                            invoiceTypeId: invoiceTypeId,
                            issueDate: issueDate
                        };
                        invoiceGenerationDraftGridAPI.loadGrid(gridPayload);
                    }
                    else {
                        invoiceGenerationDraftGridAPI.clearDataSource();
                        VRNotificationService.showError(response.Message, $scope);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });


                $scope.scopeModel.generateInvoice = function () {
                    var invoiceGenerationData = invoiceGenerationDraftGridAPI.getData();
                    var changedItems = invoiceGenerationData.changedItems;

                    var allItemsExcluded = invoiceGenerationData.allItemsExcluded;
                    if (allItemsExcluded) {
                        VRNotificationService.showError('At least one invoice should be selected for generation process', $scope);
                    }
                    else {
                        var input = { InvoiceGenerationIdentifier: invoiceGenerationIdentifier, IssueDate: issueDate, ChangedItems: changedItems, InvoiceTypeId: invoiceTypeId, InvoiceGapAction: invoicePeriodGapActionAPI.getSelectedIds() };

                        return VR_Invoice_InvoiceAPIService.GenerateInvoices(input).then(function (response) {
                            if (response.Succeed) {
                                $scope.modalContext.closeModal();
                                BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                            }
                            else {
                                VRNotificationService.showError(response.OutputMessage, $scope);
                            }
                        });
                    }
                };
            };

            UtilsService.waitMultiplePromises([invoiceGenerationDraftGridReadyDeferred.promise]).then(function () {
                load();
            });
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            invoiceGenerationIdentifier = UtilsService.guid();

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
                $scope.scopeModel.issueDate = VRDateTimeService.getTodayDate();
            }

            function loadInvoicePeriodGapActionSelector() {
                var invoicePeriodGapActionLoadDeferred = UtilsService.createPromiseDeferred();
                invoicePeriodGapActionReadyDeferred.promise.then(function () {
                    var payloadInvoicePeriodGapAction = {
                        selectFirstItem: true
                    };
                    VRUIUtilsService.callDirectiveLoad(invoicePeriodGapActionAPI, payloadInvoicePeriodGapAction, invoicePeriodGapActionLoadDeferred);
                });

                return invoicePeriodGapActionLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPartnerGroupSelector, loadInvoicePeriodGapActionSelector]).then(function () {
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
