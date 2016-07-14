(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope'];

    function StatusDefinitionManagementController($scope) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load({});
            };
        }

        function load() {

        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionManagementController', StatusDefinitionManagementController);

})(appControllers);