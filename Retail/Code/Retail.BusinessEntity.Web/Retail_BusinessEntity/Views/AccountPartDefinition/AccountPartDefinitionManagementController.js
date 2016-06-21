(function (appControllers) {

    'use strict';

    AccountPartDefinitionManagementController.$inject = ['$scope', 'Retail_BE_AccountPartDefinitionService', 'Retail_BE_AccountPartDefinitionAPIService', 'UtilsService'];

    function AccountPartDefinitionManagementController($scope, Retail_BE_AccountPartDefinitionService, Retail_BE_AccountPartDefinitionAPIService, UtilsService) {
        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.loadGrid(query);
            };

            $scope.scopeModel.add = function () {
                var onAccountPartDefinitionAdded = function (addedAccountPartDefinition) {
                    gridAPI.onAccountPartDefinitionAdded(addedAccountPartDefinition);
                };
                Retail_BE_AccountPartDefinitionService.addAccountPartDefinition(onAccountPartDefinitionAdded);
            };

            $scope.scopeModel.hasAddAccountPartDefinitionPermission = function () {
                return Retail_BE_AccountPartDefinitionAPIService.HasAddAccountPartDefinitionPermission();
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

    appControllers.controller('Retail_BE_AccountPartDefinitionManagementController', AccountPartDefinitionManagementController);

})(appControllers);