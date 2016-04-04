(function (appControllers) {

    'use strict';

    CDRComparisonController.$inject = ['$scope', 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'CDRComparison_CDRSourceConfigAPIService', 'WhS_BP_CreateProcessResultEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function CDRComparisonController($scope, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, CDRComparison_CDRSourceConfigAPIService, WhS_BP_CreateProcessResultEnum, UtilsService, VRUIUtilsService, VRNotificationService) {

        var systemSourceConfigSelectorAPI;
        var systemSourceConfigSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var systemSelectiveAPI;
        var systemSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectiveAPI;
        var partnerSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.onSystemSourceConfigSelectorReady = function (api) {
                systemSourceConfigSelectorAPI = api;
                systemSourceConfigSelectorReadyDeferred.resolve();
            };

            $scope.onSystemSelectiveReady = function (api) {
                systemSelectiveAPI = api;
                systemSelectiveReadyDeferred.resolve();
            };

            $scope.onPartnerSelectiveReady = function (api) {
                partnerSelectiveAPI = api;
                partnerSelectiveReadyDeferred.resolve();
            };

            $scope.onSystemSourceConfigSelected = function (selectedConfig) {
                $scope.isLoading = true;
                var systemSelectivePayload = {
                    cdrSourceConfigId: selectedConfig.CDRSourceConfigId
                };
                systemSelectiveAPI.load(systemSelectivePayload).finally(function () {
                    $scope.isLoading = false;
                });
            };

            $scope.saveSystemSourceConfig = function () {
                var systemSourceConfig = {
                    Name: 'System Source Config 1',
                    CDRSource: systemSelectiveAPI.getData(),
                    IsPartnerCDRSource: false
                };
                return CDRComparison_CDRSourceConfigAPIService.AddCDRSourceConfig(systemSourceConfig);
            };

            $scope.start = function () {
                var inputArguments = {
                    $type: "CDRComparison.BP.Arguments.CDRComparsionProcessInput, CDRComparison.BP.Arguments",
                    SystemCDRSource: systemSelectiveAPI.getData(),
                    PartnerCDRSource: partnerSelectiveAPI.getData()
                };

                var input = {
                    InputArguments: inputArguments
                };

                console.log(input);

                return BusinessProcess_BPInstanceAPIService.CreateNewProcess(input).then(function (response) {
                    if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                        return BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                });
            };
        }

        function load() {
            $scope.isLoading = true;

            return UtilsService.waitMultipleAsyncOperations([loadSystemSourceConfigSelector, loadSystemSelective, loadPartnerSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadSystemSourceConfigSelector() {
            var systemSourceConfigSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            systemSourceConfigSelectorReadyDeferred.promise.then(function () {
                var systemSourceConfigPayload = {
                    filter: { IsPartnerCDRSource: false }
                };
                VRUIUtilsService.callDirectiveLoad(systemSourceConfigSelectorAPI, systemSourceConfigPayload, systemSourceConfigSelectorLoadDeferred);
            });

            return systemSourceConfigSelectorLoadDeferred.promise;
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
    }

    appControllers.controller('CDRComparison_CDRComparisonController', CDRComparisonController);

})(appControllers);