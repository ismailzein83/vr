(function (appControllers) {

    "use strict";

    BusinessProcess_VRWorkflowManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_VRWorkflowAPIService','VRNotificationService'];

    function BusinessProcess_VRWorkflowManagementController($scope, UtilsService, VRUIUtilsService, BusinessProcess_VRWorkflowService, BusinessProcess_VRWorkflowAPIService, VRNotificationService) {


        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addClicked = function () {
                var onVRWorkflowAdded = function (addedVRWorkflow) {
                    gridAPI.onVRWorkflowAdded(addedVRWorkflow);
                };

                BusinessProcess_VRWorkflowService.addVRWorkflow(onVRWorkflowAdded);
            };

            $scope.hasAddVRWorkflowPermission = function () {
                return BusinessProcess_VRWorkflowAPIService.HasAddVRWorkflowPermission();
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDevProjectSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
        function getFilterObject() {
            var filter = {};
            filter.Name = $scope.name;
            filter.Title = $scope.title;
            filter.DevProjectIds = devProjectDirectiveApi.getSelectedIds();

            return filter;
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowManagementController', BusinessProcess_VRWorkflowManagementController);
})(appControllers);