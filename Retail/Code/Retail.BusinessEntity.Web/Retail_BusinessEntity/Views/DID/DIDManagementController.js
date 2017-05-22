(function (appControllers) {

    "use strict";

    DIDManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Retail_BE_DIDAPIService', 'Retail_BE_DIDService'];

    function DIDManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService, Retail_BE_DIDAPIService, Retail_BE_DIDService) {

        var didNumberTypeSelectorAPI;
        var didNumberTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountSelectorAPI;
        var accountSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onDIDNumberTypeSelectorReady = function (api) {
                didNumberTypeSelectorAPI = api;
                didNumberTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountSelectorReady = function (api) {
                accountSelectorAPI = api;
                accountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                return gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.add = function () {
                var onDIDAdded = function (addedDID) {
                    gridAPI.onDIDAdded(addedDID);
                };

                Retail_BE_DIDService.addDID(onDIDAdded);
            };

            $scope.scopeModel.hasAddDIDPermission = function () {
                return Retail_BE_DIDAPIService.HasAddDIDPermission();
            };
        };
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        };

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDIDNumberTypeSelector, loadAccountSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function loadDIDNumberTypeSelector() {
            var didNumberTypeSelectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            didNumberTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(didNumberTypeSelectorAPI, undefined, didNumberTypeSelectorDirectiveLoadDeferred);
            });

            return didNumberTypeSelectorDirectiveLoadDeferred.promise;
        };
        function loadAccountSelector() {
            var accountSelectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountSelectorReadyDeferred.promise.then(function () {
                Retail_BE_DIDAPIService.GetAccountDIDRelationDefinition().then(function (response) {
                    var accountDIDRelationDefinition = response;

                    var accountSelectorPayload = { AccountBEDefinitionId: accountDIDRelationDefinition.Settings.ParentBEDefinitionId };

                    if (accountDIDRelationDefinition.Settings.ParentBERuntimeSelectorFilter != undefined) {
                        accountSelectorPayload.filter = {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountConditionAccountFilter, Retail.BusinessEntity.Business",
                                AccountCondition: accountDIDRelationDefinition.Settings.ParentBERuntimeSelectorFilter.AccountCondition
                            }]
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(accountSelectorAPI, accountSelectorPayload, accountSelectorDirectiveLoadDeferred);
                });
            });

            return accountSelectorDirectiveLoadDeferred.promise;
        };

        function buildGridQuery() {
            return {
                Number: $scope.scopeModel.number,
                DIDNumberTypes: didNumberTypeSelectorAPI.getSelectedIds(),
                AccountIds: accountSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_DIDManagementController', DIDManagementController);

})(appControllers);