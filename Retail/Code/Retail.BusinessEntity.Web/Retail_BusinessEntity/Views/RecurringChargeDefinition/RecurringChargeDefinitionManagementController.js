(function (appControllers) {

    "use strict";

    RecurringChargeDefinitionManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'Retail_BE_RecurringChargeDefinitionAPIService', 'Retail_BE_RecurringChargeDefinitionService'];

    function RecurringChargeDefinitionManagementController($scope, UtilsService, VRUIUtilsService, Retail_BE_RecurringChargeDefinitionAPIService, Retail_BE_RecurringChargeDefinitionService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                return gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.add = function () {
                var onRecurringChargeDefinitionAdded = function (addedRecurringChargeDefinition) {
                    gridAPI.onRecurringChargeDefinitionAdded(addedRecurringChargeDefinition);
                };

                Retail_BE_RecurringChargeDefinitionService.addRecurringChargeDefinition(onRecurringChargeDefinitionAdded);
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
                Name: $scope.scopeModel.Name
            };
        }
    }

    appControllers.controller('Retail_BE_RecurringChargeDefinitionManagementController', RecurringChargeDefinitionManagementController);

})(appControllers);