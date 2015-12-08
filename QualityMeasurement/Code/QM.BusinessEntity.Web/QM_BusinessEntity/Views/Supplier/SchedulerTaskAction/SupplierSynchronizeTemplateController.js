SupplierSynchronizeTemplateController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'QM_BE_SupplierAPIService'];

function SupplierSynchronizeTemplateController($scope, UtilsService, VRUIUtilsService , QM_BE_SupplierAPIService) {

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
        return QM_BE_SupplierAPIService.GetSupplierSourceTemplates().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.sourceTypeTemplates.push(item);
            });

            if (sourceConfigId != undefined)
                $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");

        });
    }

   
}
appControllers.controller('QM_BE_SupplierSynchronizeTemplateController', SupplierSynchronizeTemplateController);
