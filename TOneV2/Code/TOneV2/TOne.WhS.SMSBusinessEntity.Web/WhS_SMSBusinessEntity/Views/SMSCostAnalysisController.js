(function (appControllers) {

    "use strict";

    SMSRatePlanController.$inject = ["$scope", "VRNotificationService", "UtilsService", "VRUIUtilsService", "VRDateTimeService", "WhS_SMSBusinessEntity_SupplierSMSRateAPIService"];

    function SMSRatePlanController($scope, VRNotificationService, UtilsService, VRUIUtilsService, VRDateTimeService, WhS_SMSBusinessEntity_SupplierSMSRateAPIService) {

        var hasMobileCountryValue = false;

        var mobileNetworkSelectorAPI;
        var mobileNetworkSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var mobileCountrySelectorAPI;
        var mobileCountrySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isLoading = true;
            $scope.scopeModel.isLoadingMobileNetworkSelector = false;

            $scope.scopeModel.loadSMSCostAnalysisGrid = function () {
                $scope.scopeModel.isLoading = true;
                var promises = [];

                var payload = { query: getSMSCostQuery() };

                var gridLoadedPromise = gridAPI.load(payload);
                promises.push(gridLoadedPromise);

                return UtilsService.waitMultiplePromises(promises).catch(function () {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.hasViewSMSCostAnalysisPermission = function () {
                return WhS_SMSBusinessEntity_SupplierSMSRateAPIService.HasViewSMSCostAnalysisPermission();
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

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridReadyDeferred.resolve();
            };
        }

        function load() {

            function loadAllControls() {

                $scope.scopeModel.effectiveDate = VRDateTimeService.getNowDateTime();
                $scope.scopeModel.numberOfOptions = 3;
                $scope.scopeModel.limitResult = 1000;

                var rootPromiseNode = {
                    promises: [loadMobileCountrySelector(), loadMobileNetworkSelector()],
                    getChildNode: function () {
                        return {
                            promises: []
                        };
                    }
                };

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

                return UtilsService.waitPromiseNode(rootPromiseNode).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }

            loadAllControls();
        }

        function getSMSCostQuery() {
            return {
                EffectiveDate: $scope.scopeModel.effectiveDate,
                MobileCountryIds: mobileCountrySelectorAPI.getSelectedIds(),
                MobileNetworkIds: mobileNetworkSelectorAPI.getSelectedIds(),
                NumberOfOptions: $scope.scopeModel.numberOfOptions,
                LimitResult: $scope.scopeModel.limitResult
            };
        }
    }

    appControllers.controller("WhS_SMSBusinessEntity_SMSCostAnalysisController", SMSRatePlanController);

})(appControllers);