(function (appControllers) {

    'use strict';

    AccountPartDefinitionManagementController.$inject = ['$scope', 'Retail_BE_AccountPartDefinitionService', 'Retail_BE_AccountPartDefinitionAPIService', 'UtilsService'];

    function AccountPartDefinitionManagementController($scope, Retail_BE_AccountPartDefinitionService, Retail_BE_AccountPartDefinitionAPIService, UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };
            $scope.add = function () {
                var onAccountPartDefinitionAdded = function (addedAccountPartDefinition) {
                    gridAPI.onAccountPartDefinitionAdded(addedAccountPartDefinition);
                };
                Retail_BE_AccountPartDefinitionService.addAccountPartDefinition(onAccountPartDefinitionAdded);
            };

            $scope.hasAddAccountPartDefinitionPermission = function () {
                return Retail_BE_AccountPartDefinitionAPIService.HasAddAccountPartDefinitionPermission();
            };
        }
        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name
            };
        }
    }

    appControllers.controller('Retail_BE_AccountPartDefinitionManagementController', AccountPartDefinitionManagementController);

})(appControllers);