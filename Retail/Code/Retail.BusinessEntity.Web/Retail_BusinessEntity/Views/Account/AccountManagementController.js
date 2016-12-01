(function (appControllers) {

    'use strict';

    AccountManagementController.$inject = ['$scope', 'Retail_BE_AccountService', 'Retail_BE_AccountAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function AccountManagementController($scope, Retail_BE_AccountService, Retail_BE_AccountAPIService, UtilsService, VRUIUtilsService, VRNotificationService)
    {
        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;

        defineScope();
        load();

        function defineScope()
        {
            $scope.scopeModel = {};

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

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
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadAccountTypeSelector() {
            var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountTypeSelectorReadyDeferred.promise.then(function () {
                var payload = {
                    filter: { RootAccountTypeOnly: true }
                };
                VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, payload, accountTypeSelectorLoadDeferred);
            });

            return accountTypeSelectorLoadDeferred.promise;
        }

        function buildGridQuery()
        {
            return {
                Name: $scope.scopeModel.name,
                AccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                ParentAccountId: null
            };
        }
    }

    appControllers.controller('Retail_BE_AccountManagementController', AccountManagementController);

})(appControllers);