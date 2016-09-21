ProfileSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'QM_CLITester_ProfileAPIService', 'VRNotificationService'];

function ProfileSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService, QM_CLITester_ProfileAPIService, VRNotificationService) {

    defineScope();
    load();
    var sourceConfigId;
    var sourceTypeDirectiveAPI;

    var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    function defineScope() {

        $scope.sourceTypeTemplates = [];

        $scope.onSourceTypeDirectiveReady = function (api) {
            sourceTypeDirectiveAPI = api;
            loadForm();
        }

        $scope.schedulerTaskAction.getData = function () {
            var sourceProfileReader;
            sourceProfileReader = sourceTypeDirectiveAPI.getData();
            sourceProfileReader.ConfigID = $scope.selectedSourceTypeTemplate.ExtensionConfigurationId;

            return {
                $type: "QM.CLITester.Business.ProfileSyncTaskActionArgument, QM.CLITester.Business",
                SourceProfileReader: sourceProfileReader
            };
        };
    }


    var isFormLoaded;
    function loadForm() {
        if ($scope.schedulerTaskAction.data == undefined || isFormLoaded)
            return;

        var data = $scope.schedulerTaskAction.data;
        if (data != null &&  data.SourceProfileReader !=undefined) {
            $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, data.SourceProfileReader.ConfigId, "ExtensionConfigurationId");
        }

        if (data.SourceProfileReader && sourceTypeDirectiveAPI != undefined) {
            sourceTypeDirectiveAPI.load(data.SourceProfileReader)
            isFormLoaded = true;
        }
        else {
            isFormLoaded = false;
        }
    }



    function load() {
        $scope.isLoading = true;
        loadAllControls().then(function () {
            loadForm()
        }).catch(function () {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isLoading = false;
        });
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
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");

        });
    }


}
appControllers.controller('QM_CLITester_ProfileSynchronizeTemplateController', ProfileSynchronizeTemplateController);
