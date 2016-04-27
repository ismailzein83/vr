SwitchMigrateTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'WhS_BE_SwitchAPIService'];

function SwitchMigrateTemplateController($scope, UtilsService, VRUIUtilsService, VRNotificationService, WhS_BE_SwitchAPIService) {

    var sourceTemplateDirectiveAPI;
    var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    defineScope();
    load();
    var sourceConfigId;
    
    function defineScope() {
        $scope.sourceTypeTemplates = [];

        $scope.onSourceTypeDirectiveReady = function (api) {
            sourceTemplateDirectiveAPI = api;
            sourceDirectiveReadyPromiseDeferred.resolve();
        }
        $scope.schedulerTaskAction.getData = function () {

            var schedulerTaskAction;
            if ($scope.selectedSourceTypeTemplate != undefined) {
                if (sourceTemplateDirectiveAPI != undefined) {
                    schedulerTaskAction = {};
                    schedulerTaskAction.$type = "TOne.WhS.DBSync.Business.SwitchSyncTaskActionArgument, TOne.WhS.DBSync.Business",
                    schedulerTaskAction.SourceSwitchReader = sourceTemplateDirectiveAPI.getData();
                    schedulerTaskAction.SourceSwitchReader.ConfigId = $scope.selectedSourceTypeTemplate.TemplateConfigID;
                }
            }
            return schedulerTaskAction;

        };
        
            
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadSourceType, loadSourceTemplate])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
          })
         .finally(function () {
             $scope.isLoading = false;
         });
    }
    function loadSourceType() {
        return WhS_BE_SwitchAPIService.GetSwitchSourceTemplates().then(function (response) {
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceSwitchReader != undefined)
                sourceConfigId = $scope.schedulerTaskAction.data.SourceSwitchReader.ConfigId;
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

        });
    }

    function loadSourceTemplate() {
        var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
        sourceDirectiveReadyPromiseDeferred.promise.then(function () {
            var payload;
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceSwitchReader != undefined)
                payload = {
                    connectionString: $scope.schedulerTaskAction.data.SourceSwitchReader.ConnectionString
                };
            VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, payload, loadSourceTemplatePromiseDeferred);
        });

        return loadSourceTemplatePromiseDeferred.promise;
    }
   
}
appControllers.controller('QM_BE_SwitchMigrateTemplateController', SwitchMigrateTemplateController);
