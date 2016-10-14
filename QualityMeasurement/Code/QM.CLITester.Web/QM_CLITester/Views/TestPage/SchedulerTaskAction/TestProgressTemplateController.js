TestProgressTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Qm_CliTester_TestCallAPIService'];

function TestProgressTemplateController($scope, UtilsService, VRUIUtilsService, Qm_CliTester_TestCallAPIService) {

    defineScope();
    load();
    var sourceConfigId;
    var sourceTypeDirectiveAPI;
    var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    function defineScope() {
        $scope.sourceTypeTemplates = [];

        $scope.onSourceTypeDirectiveReady = function (api) {
            sourceTypeDirectiveAPI = api;
            var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
        }
        $scope.schedulerTaskAction.getData = function () {
            var CLITestConnectorObj = sourceTypeDirectiveAPI.getData();
            CLITestConnectorObj.ConfigId = $scope.selectedSourceTypeTemplate.ExtensionConfigurationId;

            return {
                $type: "QM.CLITester.Business.TestProgressTaskActionArgument, QM.CLITester.Business",
                CLITestConnector: CLITestConnectorObj,
                MaximumRetryCount: $scope.maximumRetryCount
            };
        };
    }

    function load() {
        $scope.isLoading = true;
        if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined)
            $scope.maximumRetryCount = $scope.schedulerTaskAction.data.MaximumRetryCount;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadSourceType])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
          })
         .finally(function () {
             $scope.isLoading = false;
         });
    }
    function loadSourceType() {
        return Qm_CliTester_TestCallAPIService.GetTestTemplates().then(function (response) {
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.CLITestConnector != undefined)
                sourceConfigId = $scope.schedulerTaskAction.data.CLITestConnector.ConfigId;
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");
        });
    }
}
appControllers.controller('QM_CliTester_TestProgressTemplateController', TestProgressTemplateController);
