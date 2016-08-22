(function (appControllers) {

    "use strict";

    StyleDefinitionManagementController.$inject = ['$scope', 'VRCommon_StyleDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function StyleDefinitionManagementController($scope, VRCommon_StyleDefinitionService, UtilsService, VRUIUtilsService) {

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
                var onStyleDefinitionAdded = function (addedStyleDefinition) {
                    gridAPI.onStyleDefinitionAdded(addedStyleDefinition);
                }
                VRCommon_StyleDefinitionService.addStyleDefinition(onStyleDefinitionAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('VRCommon_StyleDefinitionManagementController', StyleDefinitionManagementController);

})(appControllers);