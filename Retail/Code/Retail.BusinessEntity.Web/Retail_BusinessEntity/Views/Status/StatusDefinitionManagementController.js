(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope', 'Retail_BE_StatusDefinitionService'];

    function StatusDefinitionManagementController($scope, Retail_BE_StatusDefinitionService) {

        var gridAPI;

        defineScope();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridAPI.onStatusDefinitionAdded(addedStatusDefinition);
                }
                Retail_BE_StatusDefinitionService.addStatusDefinition(onStatusDefinitionAdded);
            };
        }


        function buildGridQuery() {
            return {
                Guid: null,
                Name: $scope.scopeModel.name,
                Settings: null
            };
        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionManagementController', StatusDefinitionManagementController);

})(appControllers);