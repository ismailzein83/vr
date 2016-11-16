(function (appControllers) {

    "use strict";

    BusinessProcess_BP_InstanceMonitorController.$inject = ['$scope', 'UtilsService','VRUIUtilsService','BusinessProcess_BPInstanceService','VRNotificationService'];

    function BusinessProcess_BP_InstanceMonitorController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceService, VRNotificationService) {
        var gridAPI;
        var filter = {};


        var bpDefinitionDirectiveApi;
        var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefinitionDirectiveApi = api;
                bpDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                gridAPI.loadGrid(filter);
            };
        }

        function getFilterObject() {
            filter = {
                DefinitionsId: bpDefinitionDirectiveApi.getSelectedIds(),
            };
        }


        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinitions])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function setTitle() {
            $scope.title = 'Business Process';
        }

        function loadBPDefinitions() {
            var loadBPDefinitionsPromiseDeferred = UtilsService.createPromiseDeferred();
            bpDefinitionReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, undefined, loadBPDefinitionsPromiseDeferred);
            });

            return loadBPDefinitionsPromiseDeferred.promise;


        }
    }

    appControllers.controller('BusinessProcess_BP_InstanceMonitorController', BusinessProcess_BP_InstanceMonitorController);
})(appControllers);