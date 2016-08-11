(function (appControllers) {

    "use strict";

    VRObjectTypeDefinitionManagementController.$inject = ['$scope', 'VRCommon_VRObjectTypeDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function VRObjectTypeDefinitionManagementController($scope, VRCommon_VRObjectTypeDefinitionService, UtilsService, VRUIUtilsService) {

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
                var onVRObjectTypeDefinitionAdded = function (addedVRObjectTypeDefinition) {
                    gridAPI.onVRObjectTypeDefinitionAdded(addedVRObjectTypeDefinition);
                }
                VRCommon_VRObjectTypeDefinitionService.addVRObjectTypeDefinition(onVRObjectTypeDefinitionAdded);
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

    appControllers.controller('VRCommon_VRObjectTypeDefinitionManagementController', VRObjectTypeDefinitionManagementController);

})(appControllers);