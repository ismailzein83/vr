(function (appControllers) {

    'use strict';

    AccountManagementController.$inject = ['$scope', 'Retail_BE_AccountService', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountTypeEnum', 'UtilsService'];

    function AccountManagementController($scope, Retail_BE_AccountService, Retail_BE_AccountAPIService, Retail_BE_AccountTypeEnum, UtilsService)
    {
        var gridAPI;

        defineScope();
        load();

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.accountTypes = UtilsService.getArrayEnum(Retail_BE_AccountTypeEnum);
            $scope.scopeModel.selectedAccountTypes = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function ()
            {
                var onAccountAdded = function (addedAccount) {
                    gridAPI.onAccountAdded(addedAccount);
                };

                Retail_BE_AccountService.addAccount(undefined, onAccountAdded);
            };

            $scope.scopeModel.hasAddAccountPermission = function () {
                return Retail_BE_AccountAPIService.HasAddAccountPermission();
            };
        }

        function load() {

        }

        function buildGridQuery()
        {
            return {
                Name: $scope.scopeModel.name,
                AccountTypes: UtilsService.getPropValuesFromArray($scope.scopeModel.selectedAccountTypes, 'value'),
                ParentAccountId: null
            };
        }
    }

    appControllers.controller('Retail_BE_AccountManagementController', AccountManagementController);

})(appControllers);