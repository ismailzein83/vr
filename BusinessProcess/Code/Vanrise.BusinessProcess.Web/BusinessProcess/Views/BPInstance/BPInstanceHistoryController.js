(function (appControllers) {

    "use strict";

    BusinessProcess_BP_InstanceHistoryController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPInstanceService', 'VRValidationService','VRNotificationService'];

    function BusinessProcess_BP_InstanceHistoryController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPInstanceService, VRValidationService, VRNotificationService) {
        var gridAPI;
        var filter = {};


        var bpDefinitionDirectiveApi;
        var bpDefinitionReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var bpInstanceStatusDirectiveApi;
        var bpInstanceStatusReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        loadAllControls();

        function defineScope() {
            $scope.onBPDefinitionDirectiveReady = function (api) {
                bpDefinitionDirectiveApi = api;
                bpDefinitionReadyPromiseDeferred.resolve();
            };

            $scope.onBPInstanceStatusDirectiveReady = function (api) {
                bpInstanceStatusDirectiveApi = api;
                bpInstanceStatusReadyPromiseDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.fromDate = new Date();

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };
        }

        function getFilterObject() {
            filter = {
                DefinitionsId: bpDefinitionDirectiveApi.getSelectedIds(),
                InstanceStatus: bpInstanceStatusDirectiveApi.getSelectedIds(),
                DateFrom: $scope.fromDate,
                DateTo: $scope.toDate
            };
        }

        function loadAllControls() {
            $scope.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinitions, loadBPInstanceStatus])
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

        function loadBPInstanceStatus() {
            var loadBPInstanceStatusPromiseDeferred = UtilsService.createPromiseDeferred();
            bpInstanceStatusReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(bpInstanceStatusDirectiveApi, undefined, loadBPInstanceStatusPromiseDeferred);
            });

            return loadBPInstanceStatusPromiseDeferred.promise;
        }
    }

    appControllers.controller('BusinessProcess_BP_InstanceHistoryController', BusinessProcess_BP_InstanceHistoryController);
})(appControllers);