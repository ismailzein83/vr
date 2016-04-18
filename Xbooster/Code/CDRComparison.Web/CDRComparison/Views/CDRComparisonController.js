(function (appControllers) {

    'use strict';

    CDRComparisonController.$inject = ['$scope', 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'CDRComparison_CDRSourceConfigAPIService', 'WhS_BP_CreateProcessResultEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function CDRComparisonController($scope, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, CDRComparison_CDRSourceConfigAPIService, WhS_BP_CreateProcessResultEnum, UtilsService, VRUIUtilsService, VRNotificationService) {

        var systemConfigSelectorAPI;
        var systemConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var systemSelectiveAPI;
        var systemSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerConfigSelectorAPI;
        var partnerConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectiveAPI;
        var partnerSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.showSystemSelective = true;
            $scope.showPartnerSelective = true;

            $scope.onSystemConfigSelectorReady = function (api) {
                systemConfigSelectorAPI = api;
                systemConfigSelectorReadyDeferred.resolve();
            };

            $scope.onPartnerConfigSelectorReady = function (api) {
                partnerConfigSelectorAPI = api;
                partnerConfigSelectorReadyDeferred.resolve();
            };

            $scope.onSystemConfigSelected = function (selectedConfig) {
                if (selectedConfig != undefined) {
                    $scope.showSystemSelective = false;
                    systemSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

                    systemSelectiveReadyDeferred.promise.then(function () {
                        var loadSystemConfigFunction = function () {
                            var systemSelectivePayload = { cdrSourceConfigId: selectedConfig.CDRSourceConfigId };
                            return systemSelectiveAPI.load(systemSelectivePayload);
                        };
                        loadConfig(loadSystemConfigFunction);
                    });

                    UtilsService.safeApply($scope);
                    $scope.showSystemSelective = true;
                }
            };

            $scope.onPartnerConfigSelected = function (selectedConfig) {
                if (selectedConfig != undefined) {
                    $scope.showPartnerSelective = false;
                    partnerSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

                    partnerSelectiveReadyDeferred.promise.then(function () {
                        var loadPartnerConfigFunction = function () {
                            var partnerSelectivePayload = { cdrSourceConfigId: selectedConfig.CDRSourceConfigId };
                            return partnerSelectiveAPI.load(partnerSelectivePayload);
                        };
                        loadConfig(loadPartnerConfigFunction);
                    });

                    UtilsService.safeApply($scope);
                    $scope.showPartnerSelective = true;
                }
            };

            $scope.onSystemSelectiveReady = function (api) {
                systemSelectiveAPI = api;
                systemSelectiveReadyDeferred.resolve();
            };

            $scope.onPartnerSelectiveReady = function (api) {
                partnerSelectiveAPI = api;
                partnerSelectiveReadyDeferred.resolve();
            };

            $scope.start = function () {

                var inputArguments = {
                    $type: "CDRComparison.BP.Arguments.CDRComparsionProcessInput, CDRComparison.BP.Arguments",
                    SystemCDRSource: systemSelectiveAPI.getData(),
                    PartnerCDRSource: partnerSelectiveAPI.getData(),
                    SystemCDRSourceConfigId: systemConfigSelectorAPI.getSelectedIds(),
                    PartnerCDRSourceConfigId: partnerConfigSelectorAPI.getSelectedIds()
                };

                var input = {
                    InputArguments: inputArguments
                };

                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                        return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                });
            };
        }

        function load() {
            $scope.isLoading = true;

            return UtilsService.waitMultipleAsyncOperations([loadSystemConfigSelector, loadPartnerConfigSelector, loadSystemSelective, loadPartnerSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadSystemConfigSelector() {
            var systemConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            systemConfigSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: { IsPartnerCDRSource: false }
                };
                VRUIUtilsService.callDirectiveLoad(systemConfigSelectorAPI, payload, systemConfigSelectorLoadDeferred);
            });

            return systemConfigSelectorLoadDeferred.promise;
        }

        function loadPartnerConfigSelector() {
            var partnerConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            partnerConfigSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: { IsPartnerCDRSource: true }
                };
                VRUIUtilsService.callDirectiveLoad(partnerConfigSelectorAPI, payload, partnerConfigSelectorLoadDeferred);
            });

            return partnerConfigSelectorLoadDeferred.promise;
        }

        function loadSystemSelective() {
            var systemSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            systemSelectiveReadyDeferred.promise.then(function () {
                var systemSelectivePayload;
                VRUIUtilsService.callDirectiveLoad(systemSelectiveAPI, systemSelectivePayload, systemSelectiveLoadDeferred);
            });

            return systemSelectiveLoadDeferred.promise;
        }

        function loadPartnerSelective() {
            var partnerSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            partnerSelectiveReadyDeferred.promise.then(function () {
                var partnerSelectivePayload;
                VRUIUtilsService.callDirectiveLoad(partnerSelectiveAPI, partnerSelectivePayload, partnerSelectiveLoadDeferred);
            });

            return partnerSelectiveLoadDeferred.promise;
        }

        function loadConfig(loadConfigFunction) {
            $scope.isLoading = true;

            loadConfigFunction().catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('CDRComparison_CDRComparisonController', CDRComparisonController);

})(appControllers);