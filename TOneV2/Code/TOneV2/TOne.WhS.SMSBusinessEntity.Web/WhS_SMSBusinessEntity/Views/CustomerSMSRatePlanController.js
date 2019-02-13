(function (appControllers) {

    "use strict";

    SMSRatePlanController.$inject = ["$scope", "WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService", "VRNotificationService", "UtilsService", "WhS_SMSBusinessEntity_CustomerRatePlanService", "WhS_SMSBuisenessProcess_SMSRatePlanStatusEnum", "VRUIUtilsService", "VRDateTimeService", "BusinessProcess_BPInstanceService", "WhS_BP_SMSSaleRateDefinitionEnum", "BusinessProcess_BPInstanceAPIService", "WhS_BP_CreateProcessResultEnum"];

    function SMSRatePlanController($scope, WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService, VRNotificationService, UtilsService, WhS_SMSBusinessEntity_CustomerRatePlanService, WhS_SMSBuisenessProcess_SMSRatePlanStatusEnum, VRUIUtilsService, VRDateTimeService, BusinessProcess_BPInstanceService, WhS_BP_SMSSaleRateDefinitionEnum, BusinessProcess_BPInstanceAPIService, WhS_BP_CreateProcessResultEnum) {

        var selectedCustomer;
        var processDraftID;
        var hasMobileCountryValue = false;

        var mobileNetworkSelectorAPI;
        var mobileNetworkSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var mobileCountrySelectorAPI;
        var mobileCountrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

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
            $scope.scopeModel.isLoadingMobileNetworkSelector = false;

            $scope.scopeModel.applyChanges = function () {
                $scope.scopeModel.isLoading = true;
                hasRunningProcessesForCustomer().then(function (response) {
                    if (!response.hasRunningProcesses) {
                        var inputArguments = {
                            $type: "TOne.WhS.SMSBusinessEntity.BP.Arguments.SMSSaleRateInput, TOne.WhS.SMSBusinessEntity.BP.Arguments",
                            CustomerID: selectedCustomer.CarrierAccountId,
                            ProcessDraftID: processDraftID
                        };

                        var input = {
                            InputArguments: inputArguments
                        };

                        BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                            if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value) {
                                var processTrackingContext = {
                                    onClose: function (bpInstanceClosureContext) {
                                        //if (bpInstanceClosureContext != undefined && bpInstanceClosureContext.bpInstanceStatusValue === BPInstanceStatusEnum.Completed.value) 
                                        resetCustomerSMSRates();
                                    }
                                };

                                BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId, processTrackingContext);
                            }

                        }).finally(function () {
                            $scope.scopeModel.isLoading = false;
                        });
                    }
                    else {
                        $scope.scopeModel.isLoading = false;
                    }
                });
            };

            //$scope.scopeModel.hasApplyChangesPermission = function () {
            //    return WhS_SMSBusinessEntity_CustomerRatePlanService.HasApplyChangesPermission();
            //};

            $scope.scopeModel.loadCustomerSMSRates = function () {
                $scope.scopeModel.isLoading = true;
                var promises = [];

                var payload = { query: getCustomerSMSRateQuery() };

                var gridLoadedPromise = gridAPI.load(payload).then(function () {
                    $scope.scopeModel.isCustomerSMSRateLoaded = true;
                });
                promises.push(gridLoadedPromise);

                var checkedIfDraftExistPromise = WhS_SMSBusinessEntity_CustomerSMSRateChangesAPIService.CheckIfDraftExist(selectedCustomer.CarrierAccountId).then(function (response) {
                    if (response) {
                        processDraftID = response.ProcessDraftID;
                        $scope.scopeModel.isCustomerSMSRateDraftExist = (processDraftID != 0);
                    }
                });
                promises.push(checkedIfDraftExistPromise);

                return UtilsService.waitMultiplePromises(promises).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.addCustomerRates = function () {
                hasRunningProcessesForCustomer().then(function (response) {
                    if (!response.hasRunningProcesses) {
                        var onDraftSaved = function (processID) {
                            processDraftID = processID;
                            $scope.scopeModel.isCustomerSMSRateDraftExist = true;
                        };

                        var onSaleSMSRatesApplied = function () {
                            resetCustomerSMSRates();
                        };

                        

                        WhS_SMSBusinessEntity_CustomerRatePlanService.addSMSRates(selectedCustomer, processDraftID, $scope.scopeModel.isCustomerSMSRateDraftExist,  onDraftSaved, onSaleSMSRatesApplied);
                    }
                });
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

                    resetCustomerSMSRates();
                }
            };

            $scope.scopeModel.onMobileCountrySelectionChanged = function (selectedMobileCountry) {
                if (!selectedMobileCountry && !hasMobileCountryValue)
                    return;

                hasMobileCountryValue = (selectedMobileCountry != undefined);

                var payload = {
                    filter: { MobileCountryIds: mobileCountrySelectorAPI.getSelectedIds() },
                    selectedIds: mobileNetworkSelectorAPI.getSelectedIds()
                };

                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingMobileNetworkSelector = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, mobileNetworkSelectorAPI, payload, setLoader, undefined);
            };

            $scope.scopeModel.onMobileCountrySelectorReady = function (api) {
                mobileCountrySelectorAPI = api;
                mobileCountrySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onMobileNetworkSelectorReady = function (api) {
                mobileNetworkSelectorAPI = api;
                mobileNetworkSelectorReadyDeferred.resolve();
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

                function loadMobileCountrySelector() {
                    var mobileCountryLoadDeferred = UtilsService.createPromiseDeferred();
                    mobileCountrySelectorReadyDeferred.promise.then(function () {
                        var payload;
                        VRUIUtilsService.callDirectiveLoad(mobileCountrySelectorAPI, payload, mobileCountryLoadDeferred);
                    });
                    return mobileCountryLoadDeferred.promise;
                }

                function loadMobileNetworkSelector() {
                    var mobileNetworkLoadDeferred = UtilsService.createPromiseDeferred();
                    mobileNetworkSelectorReadyDeferred.promise.then(function () {
                        var payload;
                        VRUIUtilsService.callDirectiveLoad(mobileNetworkSelectorAPI, payload, mobileNetworkLoadDeferred);
                    });
                    return mobileNetworkLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadCustomerSelector, loadMobileCountrySelector, loadMobileNetworkSelector]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            loadAllControls();
        }

        function getCustomerSMSRateQuery() {
            return {
                CustomerID: selectedCustomer.CarrierAccountId,
                EffectiveDate: $scope.scopeModel.effectiveDate,
                MobileCountryIds: mobileCountrySelectorAPI.getSelectedIds(),
                MobileNetworkIds: mobileNetworkSelectorAPI.getSelectedIds()
            };
        }

        function getCustomerSMSRateStatusToUpdate(processDraftID, newStatus) {
            return {
                ProcessDraftID: processDraftID,
                NewStatus: newStatus
            };
        }

        function resetCustomerSMSRates() {
            gridAPI.cleanGrid();
            $scope.scopeModel.isCustomerSMSRateDraftExist = false;
            $scope.scopeModel.isCustomerSMSRateLoaded = false;
        }

        function hasRunningProcessesForCustomer() {
            var editorMessage = "Other SMS rate processes are still pending for customer '" + selectedCustomer.Name + "'";
            var runningInstanceEditorSettings = { message: editorMessage };
            var entityId = "CustomerId_" + selectedCustomer.CarrierAccountId;
            return BusinessProcess_BPInstanceService.displayRunningInstancesIfExist(WhS_BP_SMSSaleRateDefinitionEnum.BPDefinitionId.value, [entityId], runningInstanceEditorSettings);
        }
    }

    appControllers.controller("WhS_SMSBusinessEntity_CustomerSMSRatePlanController", SMSRatePlanController);

})(appControllers);