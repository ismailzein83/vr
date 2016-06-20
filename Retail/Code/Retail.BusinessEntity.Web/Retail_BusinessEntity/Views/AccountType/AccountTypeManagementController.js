(function (appControllers) {

    'use strict';

    AccountTypeManagementController.$inject = ['$scope', 'Retail_BE_AccountTypeService', 'Retail_BE_AccountTypeAPIService', 'UtilsService'];

    function AccountTypeManagementController($scope, Retail_BE_AccountTypeService, Retail_BE_AccountTypeAPIService, UtilsService) {
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
                var onAccountTypeAdded = function (addedAccountType) {
                    gridAPI.onAccountTypeAdded(addedAccountType);
                };
                Retail_BE_AccountTypeService.addAccountType(onAccountTypeAdded);
            };

            $scope.scopeModel.hasAddAccountTypePermission = function () {
                return Retail_BE_AccountTypeAPIService.HasAddAccountTypePermission();
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

    appControllers.controller('Retail_BE_AccountTypeManagementController', AccountTypeManagementController);

})(appControllers);