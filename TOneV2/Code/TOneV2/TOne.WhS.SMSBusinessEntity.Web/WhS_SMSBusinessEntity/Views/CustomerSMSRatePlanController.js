(function (appControllers) {

    "use strict";

    SMSRatePlanController.$inject = ["$scope", "VRNotificationService", "UtilsService", "WhS_SMSBusinessEntity_CustomerRatePlanService", "VRUIUtilsService", "VRDateTimeService", "BusinessProcess_BPInstanceService", "WhS_BP_SMSSaleRateDefinitionEnum"];

    function SMSRatePlanController($scope, VRNotificationService, UtilsService, WhS_SMSBusinessEntity_CustomerRatePlanService, VRUIUtilsService, VRDateTimeService, BusinessProcess_BPInstanceService, WhS_BP_SMSSaleRateDefinitionEnum) {

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

            $scope.scopeModel.searchCustomerSMSRates = function () {
                $scope.scopeModel.isLoading = true;
                var promises = [];

                var payload = { query: getCustomerSMSRateQuery() };

                var gridLoadedPromise = gridAPI.load(payload).then(function () {
                    $scope.scopeModel.isCustomerSMSRateLoaded = true;
                });
                promises.push(gridLoadedPromise);

                return UtilsService.waitMultiplePromises(promises).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.addCustomerRates = function () {
                return hasRunningProcessesForCustomer().then(function (response) {
                    if (!response.hasRunningProcesses) {
                        var onDraftSaved = function (processID) {
                            processDraftID = processID;
                            $scope.scopeModel.isCustomerSMSRateDraftExist = true;
                        };

                        var onSaleSMSRatesApplied = function () {
                            resetCustomerSMSRates();
                        };

                        WhS_SMSBusinessEntity_CustomerRatePlanService.addSMSRates(selectedCustomer, onDraftSaved, onSaleSMSRatesApplied);
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