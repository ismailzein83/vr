zoneSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'QM_BE_ZoneAPIService'];

function zoneSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService, QM_BE_ZoneAPIService) {

    var sourceConfigId;
    var sourceTypeDirectiveAPI;
    var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    var countrySourceTypeDirectiveAPI;
    var countrySourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    defineScope();
    load();
  

    console.log(sourceDirectiveReadyPromiseDeferred)

    function defineScope() {
        $scope.sourceTypeTemplates = [];

        $scope.onSourceTypeDirectiveReady = function (api) {
            sourceTypeDirectiveAPI = api;
            var setLoader = function (value) { $scope.isLoadingSourceTypeDirective = value };
            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceTypeDirectiveAPI, undefined, setLoader, sourceDirectiveReadyPromiseDeferred);
        }

        $scope.onSourceCountryDirectiveReady = function (api) {
            countrySourceTypeDirectiveAPI = api;
            countrySourceDirectiveReadyPromiseDeferred.resolve();
        }

        $scope.schedulerTaskAction.getData = function () {
            return {
                $type: "QM.BusinessEntity.Business.ZoneSyncTaskActionArgument, QM.BusinessEntity.Business",
                SourceZoneReader: sourceTypeDirectiveAPI.getData(),
                SourceCountryReader: null
            };
        };
            
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }
    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadSourceType, loadCountrySourceType])
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
          })
         .finally(function () {
             $scope.isLoading = false;
         });
    }
    function loadSourceType() {
        return QM_BE_ZoneAPIService.GetZoneSourceTemplates().then(function (response) {
            console.log(response)
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

        });
    }

    function loadCountrySourceType() {
        
        var loadCountrySourcePromiseDeferred = UtilsService.createPromiseDeferred();
        countrySourceDirectiveReadyPromiseDeferred.promise.then(function () {
            var payload = {
            };

            VRUIUtilsService.callDirectiveLoad(countrySourceTypeDirectiveAPI, payload, loadCountrySourcePromiseDeferred);
        });

        return loadCountrySourcePromiseDeferred.promise;
    }

   
}
appControllers.controller('QM_BE_ZoneSynchronizeTemplateController', zoneSynchronizeTemplateController);
