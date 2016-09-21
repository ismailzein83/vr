SupplierSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'QM_BE_SupplierAPIService'];

function SupplierSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService, VRNotificationService , QM_BE_SupplierAPIService) {

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
                    schedulerTaskAction.$type = "QM.BusinessEntity.Business.SupplierSyncTaskActionArgument, QM.BusinessEntity.Business",
                    schedulerTaskAction.SourceSupplierReader = sourceTemplateDirectiveAPI.getData();
                    schedulerTaskAction.SourceSupplierReader.ConfigId = $scope.selectedSourceTypeTemplate.ExtensionConfigurationId;
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
        return QM_BE_SupplierAPIService.GetSupplierSourceTemplates().then(function (response) {
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceSupplierReader != undefined)
              sourceConfigId = $scope.schedulerTaskAction.data.SourceSupplierReader.ConfigId;
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");

        });
    }

    function loadSourceTemplate() {
        var loadSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
        sourceDirectiveReadyPromiseDeferred.promise.then(function () {
            var payload;
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceSupplierReader != undefined)
                payload = {
                    connectionString:$scope.schedulerTaskAction.data.SourceSupplierReader.ConnectionString
                };
            VRUIUtilsService.callDirectiveLoad(sourceTemplateDirectiveAPI, payload, loadSourceTemplatePromiseDeferred);
        });

        return loadSourceTemplatePromiseDeferred.promise;
    }
   
}
appControllers.controller('QM_BE_SupplierSynchronizeTemplateController', SupplierSynchronizeTemplateController);
