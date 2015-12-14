ProfileSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'QM_CLITester_ProfileAPIService'];

function ProfileSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService, QM_CLITester_ProfileAPIService) {

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
            console.log('ProfileSynchronizeTemplateController.$scope.schedulerTaskAction.getData')


            var data = 
             {
                $type: "QM.CLITester.Business.ProfileSyncTaskActionArgument, QM.CLITester.Business",
                SourceProfileReader: sourceTypeDirectiveAPI.getData()
             };

            console.log(data)

            return data;
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
        return QM_CLITester_ProfileAPIService.GetProfileSourceTemplates().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

        });
    }


}
appControllers.controller('QM_CLITester_ProfileSynchronizeTemplateController', ProfileSynchronizeTemplateController);
