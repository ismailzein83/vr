
(function (appControllers) {

    "use strict";

    ReprocessDefinitionManagementController.$inject = ['$scope', 'Reprocess_ReprocessDefinitionService','Reprocess_ReprocessDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ReprocessDefinitionManagementController($scope, Reprocess_ReprocessDefinitionService, Reprocess_ReprocessDefinitionAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;
        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onReprocessDefinitionAdded = function (addedReprocessDefinition) {
                    gridAPI.onReprocessDefinitionAdded(addedReprocessDefinition);
                };
                Reprocess_ReprocessDefinitionService.addReprocessDefinition(onReprocessDefinitionAdded);
            };
            $scope.hasAddReprocessDefinitionPermission = function () {
                return Reprocess_ReprocessDefinitionAPIService.HasAddReprocessDefinitionPermission();
            };
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
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

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
            };
        }
    }

    appControllers.controller('Reprocess_ReprocessDefinitionManagementController', ReprocessDefinitionManagementController);

})(appControllers);