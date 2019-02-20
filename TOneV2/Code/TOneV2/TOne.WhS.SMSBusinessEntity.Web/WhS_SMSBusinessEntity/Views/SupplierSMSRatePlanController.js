(function (appControllers) {

    "use strict";

    SMSRatePlanController.$inject = ["$scope", "VRNotificationService", "UtilsService", "WhS_SMSBusinessEntity_SupplierRatePlanService", "VRUIUtilsService", "VRDateTimeService", "BusinessProcess_BPInstanceService", "WhS_BP_SMSSupplierRateBPDefinition", "WhS_SMSBusinessEntity_SupplierSMSRateAPIService"];

    function SMSRatePlanController($scope, VRNotificationService, UtilsService, WhS_SMSBusinessEntity_SupplierRatePlanService, VRUIUtilsService, VRDateTimeService, BusinessProcess_BPInstanceService, WhS_BP_SMSSupplierRateBPDefinition, WhS_SMSBusinessEntity_SupplierSMSRateAPIService) {

        var selectedSupplier;
        var processDraftID;
        var hasMobileCountryValue = false;

        var mobileNetworkSelectorAPI;
        var mobileNetworkSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var mobileCountrySelectorAPI;
        var mobileCountrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var supplierSelectorAPI;
        var supplierSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.searchSupplierSMSRates = function () {
                $scope.scopeModel.isLoading = true;
                var promises = [];

                var payload = { query: getSupplierSMSRateQuery() };

                var gridLoadedPromise = gridAPI.load(payload).then(function () {
                    $scope.scopeModel.isSupplierSMSRateLoaded = true;
                });
                promises.push(gridLoadedPromise);

                return UtilsService.waitMultiplePromises(promises).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.addSupplierRates = function () {
                return hasRunningProcessesForSupplier().then(function (response) {
                    if (!response.hasRunningProcesses) {

                        var onSupplierSMSRatesApplied = function () {
                            resetSupplierSMSRates();
                        };

                        WhS_SMSBusinessEntity_SupplierRatePlanService.addSMSRates(selectedSupplier, onSupplierSMSRatesApplied);
                    }
                });
            };

            $scope.scopeModel.onSupplierChanged = function (supplier) {
                if (supplier != undefined) {
                    selectedSupplier = supplier;

                    var currencyPayload = {
                        selectedIds: selectedSupplier.CurrencyId
                    };

                    var setLoader = function (value) {
                        $scope.scopeModel.isLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, currencySelectorAPI, currencyPayload, setLoader, undefined);

                    resetSupplierSMSRates();
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

            $scope.scopeModel.onSupplierSelectorReady = function (api) {
                supplierSelectorAPI = api;
                supplierSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };

            $scope.scopeModel.hasSearchRatesPermission = function () {
                return WhS_SMSBusinessEntity_SupplierSMSRateAPIService.HasSearchRatesPermission();
            };

            $scope.scopeModel.hasAddDraftPermission = function () {
                return WhS_SMSBusinessEntity_SupplierSMSRateAPIService.HasAddDraftPermission();
            };
        }

        function load() {

            function loadAllControls() {

                function loadStaticData() {
                    $scope.scopeModel.effectiveDate = VRDateTimeService.getNowDateTime();
                }

                function loadSupplierSelector() {
                    var supplierSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    supplierSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(supplierSelectorAPI, undefined, supplierSelectorLoadDeferred);
                    });
                    return supplierSelectorLoadDeferred.promise;
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

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadSupplierSelector, loadMobileCountrySelector, loadMobileNetworkSelector]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            loadAllControls();
        }

        function getSupplierSMSRateQuery() {
            return {
                SupplierID: selectedSupplier.CarrierAccountId,
                EffectiveDate: $scope.scopeModel.effectiveDate,
                MobileCountryIds: mobileCountrySelectorAPI.getSelectedIds(),
                MobileNetworkIds: mobileNetworkSelectorAPI.getSelectedIds()
            };
        }

        function resetSupplierSMSRates() {
            gridAPI.cleanGrid();
            $scope.scopeModel.isSupplierSMSRateDraftExist = false;
            $scope.scopeModel.isSupplierSMSRateLoaded = false;
        }

        function hasRunningProcessesForSupplier() {
            var editorMessage = "Other SMS rate processes are still pending for supplier '" + selectedSupplier.Name + "'";
            var runningInstanceEditorSettings = { message: editorMessage };
            var entityId = "SupplierId_" + selectedSupplier.CarrierAccountId;
            return BusinessProcess_BPInstanceService.displayRunningInstancesIfExist(WhS_BP_SMSSupplierRateBPDefinition.BPDefinitionId.value, [entityId], runningInstanceEditorSettings);
        }
    }

    appControllers.controller("WhS_SMSBusinessEntity_SupplierSMSRatePlanController", SMSRatePlanController);

})(appControllers);