(function (appControllers) {

    "use strict";

    SMSRatePlanController.$inject = ["$scope", "WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService", "VRNotificationService", "UtilsService", "WhS_SMSBusinessEntity_CustomerRatePlanService", "WhS_SMSBuisenessProcess_SMSRatePlanStatusEnum", "VRUIUtilsService", "VRDateTimeService"];

    function SMSRatePlanController($scope, WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService, VRNotificationService, UtilsService, WhS_SMSBusinessEntity_CustomerRatePlanService, WhS_SMSBuisenessProcess_SMSRatePlanStatusEnum, VRUIUtilsService, VRDateTimeService) {

        var selectedCustomer;
        var processDraftID;

        var customerSelectorAPI;
        var customerSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isLoading = true;

            $scope.scopeModel.loadCustomerSMSRates = function () {
                var payload = {
                    query: getCustomerSMSRateQuery(selectedCustomer.CarrierAccountId, $scope.scopeModel.effectiveDate)
                };

                gridAPI.load(payload);

                $scope.scopeModel.isLoading = true;
                WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.CheckIfDraftExist(selectedCustomer.CarrierAccountId).then(function (response) {
                    if (response) {
                        processDraftID = response.ProcessDraftID;
                        $scope.scopeModel.isCustomerSMSRateDraftExist = true;
                    }
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.addCustomerRates = function () {
                var onDraftSaved = function (processID) {
                    processDraftID = processID;
                    $scope.scopeModel.isCustomerSMSRateDraftExist = true;
                };

                WhS_SMSBusinessEntity_CustomerRatePlanService.addSMSRates(selectedCustomer, onDraftSaved);
            };

            $scope.scopeModel.cancelCustomerDraft = function () {
                var confirmationPromise = VRNotificationService.showConfirmation("Are you sure that you want to delete the entire draft?");

                confirmationPromise.then(function (isConfirmed) {
                    if (isConfirmed) {
                        $scope.scopeModel.isLoading = true;
                        WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.UpdateSMSRateChangesStatus(getCustomerSMSRateStatusToUpdate(processDraftID, WhS_SMSBuisenessProcess_SMSRatePlanStatusEnum.Cancelled.value)).then(function (response) {
                            if (response) {
                                VRNotificationService.showSuccess("Draft Cancelled Successfully");
                                $scope.scopeModel.isCustomerSMSRateDraftExist = false;
                            }
                            else {
                                VRNotificationService.showWarning("Draft Cannot be Cancelled");
                            }
                        }).catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }
                });
            };

            $scope.scopeModel.onCustomerChanged = function (customer) {
                if (customer != undefined) {
                    selectedCustomer = customer;

                    var currencyPayload = {
                        selectedIds: selectedCustomer.CurrencyId
                    };

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencySelectorAPI, currencyPayload, setLoader, undefined);

                    gridAPI.cleanGrid();
                    $scope.scopeModel.isCustomerSMSRateDraftExist = false;
                }
            };

            $scope.scopeModel.onCustomerSelectorReady = function (api) {
                customerSelectorAPI = api;
                customerSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

        }

        function load() {

            function loadAllControls() {

                function loadStaticData() {
                    $scope.scopeModel.effectiveDate = VRDateTimeService.getNowDateTime();
                }

                function loadCustomerSelector() {
                    var customerSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    customerSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(customerSelectorAPI, undefined, customerSelectorLoadDeferred);
                    });
                    return customerSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadCustomerSelector]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            loadAllControls();
        }

        function getCustomerSMSRateQuery(customerID, effectiveDate) {
            return {
                CustomerID: customerID,
                EffectiveDate: effectiveDate
            };
        }

        function getCustomerSMSRateStatusToUpdate(processDraftID, newStatus) {
            return {
                Query: {
                    ProcessDraftID: processDraftID,
                    NewStatus: newStatus
                }
            };
        }
    }

    appControllers.controller("WhS_SMSBusinessEntity_CustomerSMSRatePlanController", SMSRatePlanController);

})(appControllers);