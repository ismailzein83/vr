(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_BPDefinitionService', 'BusinessProcess_BPDefinitionAPIService','VRNotificationService'];

    function BusinessProcess_BPDefinitionManagementController($scope, UtilsService, VRUIUtilsService, BusinessProcess_BPDefinitionService, BusinessProcess_BPDefinitionAPIService, VRNotificationService) {
        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.add = function () {
                var onBPTechnicalDefinitionAdded = function (addedBPTechnicalDefinition) {
                    gridAPI.onBPTechnicalDefinitionAdded(addedBPTechnicalDefinition);
                };

                BusinessProcess_BPDefinitionService.addBusinessProcessDefinition(onBPTechnicalDefinitionAdded);
            };

            $scope.hasAddBPDefinitionPermission = function () {
                return BusinessProcess_BPDefinitionAPIService.HasAddBPDefinitionPermission()
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
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
            filter.Title = $scope.title;
            filter.DevProjectIds = devProjectDirectiveApi.getSelectedIds();
            return filter;
        }
    }

    appControllers.controller('BusinessProcess_BP_TechnicalDefinitionManagementController', BusinessProcess_BPDefinitionManagementController);
})(appControllers);