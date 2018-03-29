(function (appControllers) {
    "use strict";
    EnterprisesDIDsViewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'Retail_Teles_EnterpriseAPIService','VRUIUtilsService'];
    function EnterprisesDIDsViewController($scope, UtilsService, VRNotificationService, Retail_Teles_EnterpriseAPIService, VRUIUtilsService) {

        var gridAPI;
        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {

            $scope.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
            $scope.hasSaveAccountEnterprisesPermission = function () {
                return Retail_Teles_EnterpriseAPIService.HasSaveAccountEnterprisesPermission();
            };
            $scope.save = function () {
                return Retail_Teles_EnterpriseAPIService.SaveAccountEnterprisesDIDs().then(function () {
                    VRNotificationService.showSuccess("Saved Successfully.");
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
        }
        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadAccountSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function loadAccountSelector() {
            var accountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            accountSelectorReadyDeferred.promise.then(function () {
                var accountSelectorPayload = {
                    AccountBEDefinitionId: "9a427357-cf55-4f33-99f7-745206dee7cd"
                };
                
                VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, accountSelectorPayload, accountSelectorLoadDeferred);
            });
            return accountSelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                DIDNumber: $scope.didNumber,
                AccountIds: accountSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_Teles_EnterprisesDIDsViewController', EnterprisesDIDsViewController);

})(appControllers);