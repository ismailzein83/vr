InitiateTestTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Qm_CliTester_TestCallAPIService'];

function InitiateTestTemplateController($scope, UtilsService, VRUIUtilsService, Qm_CliTester_TestCallAPIService) {

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
            return {
                $type: "QM.CLITester.Business.InitiateTestTaskActionArgument, QM.CLITester.Business",
                CLITestConnector: sourceTypeDirectiveAPI.getData()
            };
        };

    }

    function load() {
        $scope.isLoading = true;
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
        return Qm_CliTester_TestCallAPIService.GetTestCallSourceTemplates().then(function (response) {
            console.log(response);
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

        });
    }


}
appControllers.controller('QM_CliTester_InitiateTestTemplateController', InitiateTestTemplateController);
