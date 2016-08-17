
(function (appControllers) {

    "use strict";

    ReprocessDefinitionManagementController.$inject = ['$scope', 'Reprocess_ReprocessDefinitionService','Reprocess_ReprocessDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ReprocessDefinitionManagementController($scope, Reprocess_ReprocessDefinitionService, Reprocess_ReprocessDefinitionAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onReprocessDefinitionAdded = function (addedReprocessDefinition) {
                    gridAPI.onReprocessDefinitionAdded(addedReprocessDefinition);
                }
                Reprocess_ReprocessDefinitionService.addReprocessDefinition(onReprocessDefinitionAdded);
            };
            $scope.hasAddReprocessDefinitionPermission = function () {
                return Reprocess_ReprocessDefinitionAPIService.HasAddReprocessDefinitionPermission();
            }
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('Reprocess_ReprocessDefinitionManagementController', ReprocessDefinitionManagementController);

})(appControllers);