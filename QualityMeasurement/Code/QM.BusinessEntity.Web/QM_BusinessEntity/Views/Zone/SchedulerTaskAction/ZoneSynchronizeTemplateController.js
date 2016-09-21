zoneSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'QM_BE_ZoneAPIService'];

function zoneSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService, QM_BE_ZoneAPIService) {

    var sourceConfigId;
    var sourceTypeDirectiveAPI;
    var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var countrySourceTypeDirectiveAPI;
    var countrySourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();   
    defineScope();
    load();

    function defineScope() {
        $scope.sourceTypeTemplates = [];

        $scope.onSourceTypeDirectiveReady = function (api) {
            sourceTypeDirectiveAPI = api;
            sourceDirectiveReadyPromiseDeferred.resolve();
        }

        $scope.onSourceCountryDirectiveReady = function (api) {
            countrySourceTypeDirectiveAPI = api;
            countrySourceDirectiveReadyPromiseDeferred.resolve();
        }

        $scope.schedulerTaskAction.getData = function () {
            var schedulerTaskAction;
            if ($scope.selectedSourceTypeTemplate != undefined) {
                if (sourceTypeDirectiveAPI != undefined) {
                    schedulerTaskAction = {};
                    schedulerTaskAction.$type = "QM.BusinessEntity.Business.ZoneSyncTaskActionArgument, QM.BusinessEntity.Business",
                    schedulerTaskAction.SourceZoneReader = sourceTypeDirectiveAPI.getData();
                    schedulerTaskAction.SourceZoneReader.ConfigId = $scope.selectedSourceTypeTemplate.ExtensionConfigurationId;
                    if(countrySourceTypeDirectiveAPI != undefined){
                        schedulerTaskAction.SourceZoneReader.CountryReader = countrySourceTypeDirectiveAPI.getData();
                    }
                }
                if (countrySourceTypeDirectiveAPI != undefined) {
                    schedulerTaskAction.SourceCountryReader = countrySourceTypeDirectiveAPI.getData();
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
        return UtilsService.waitMultipleAsyncOperations([loadSourceType, loadZoneTemplateDirective, loadCountrySourceType])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
          })
         .finally(function () {
             $scope.isLoading = false;
         });
    }
    function loadSourceType() {
        return QM_BE_ZoneAPIService.GetZoneSourceTemplates().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceZoneReader != undefined)
                sourceConfigId = $scope.schedulerTaskAction.data.SourceZoneReader.ConfigId;
            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "ExtensionConfigurationId");

        });
    }
    function loadZoneTemplateDirective() {
        var loadZoneSourceTemplatePromiseDeferred = UtilsService.createPromiseDeferred();
        sourceDirectiveReadyPromiseDeferred.promise.then(function () {
            var payload;
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceZoneReader != undefined)
                payload = {
                    connectionString: $scope.schedulerTaskAction.data.SourceZoneReader.ConnectionString
                };
            VRUIUtilsService.callDirectiveLoad(sourceTypeDirectiveAPI, payload, loadZoneSourceTemplatePromiseDeferred);
        });

        return loadZoneSourceTemplatePromiseDeferred.promise;
    }
    function loadCountrySourceType() {
        
        var loadCountrySourcePromiseDeferred = UtilsService.createPromiseDeferred();
        countrySourceDirectiveReadyPromiseDeferred.promise.then(function () {
            var payload = { };
            if ($scope.schedulerTaskAction != undefined && $scope.schedulerTaskAction.data != undefined && $scope.schedulerTaskAction.data.SourceCountryReader != undefined)
                payload = {
                    connectionString: $scope.schedulerTaskAction.data.SourceCountryReader.ConnectionString,
                    sourceConfigId: $scope.schedulerTaskAction.data.SourceCountryReader.ConfigId
                };
            VRUIUtilsService.callDirectiveLoad(countrySourceTypeDirectiveAPI, payload, loadCountrySourcePromiseDeferred);
        });

        return loadCountrySourcePromiseDeferred.promise;
    }

   
}
appControllers.controller('QM_BE_ZoneSynchronizeTemplateController', zoneSynchronizeTemplateController);
