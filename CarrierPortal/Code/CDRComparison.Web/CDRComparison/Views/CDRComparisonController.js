(function (appControllers) {

    'use strict';

    CDRComparisonController.$inject = ['$scope', 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPInstanceService', 'WhS_BP_CreateProcessResultEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function CDRComparisonController($scope, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService, WhS_BP_CreateProcessResultEnum, UtilsService, VRUIUtilsService, VRNotificationService) {

        var systemSelectiveAPI;
        var systemSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var partnerSelectiveAPI;
        var partnerSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
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

            return UtilsService.waitMultipleAsyncOperations([loadSystemSelective, loadPartnerSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
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