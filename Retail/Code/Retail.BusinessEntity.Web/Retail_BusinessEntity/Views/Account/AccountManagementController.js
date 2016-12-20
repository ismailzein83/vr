(function (appControllers) {

    'use strict';

    AccountManagementController.$inject = ['$scope', 'Retail_BE_AccountService', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function AccountManagementController($scope, Retail_BE_AccountService, Retail_BE_AccountAPIService, Retail_BE_AccountTypeAPIService, UtilsService, VRUIUtilsService, VRNotificationService)
    {
        var accountFields;

        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var recordFilterDirectiveAPI;
        var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                recordFilterDirectiveAPI = api;
                recordFilterDirectiveReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([loadAccountTypeSelector, loadRecordFilterDirective]).catch(function (error) {
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
        function loadRecordFilterDirective() {
            var recordFilterDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            recordFilterDirectiveReadyDeferred.promise.then(function () {

                loadAccountFields().then(function () {

                    var recordFilterDirectivePayload = {
                        context: buildContext()
                    };
                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, recordFilterDirectiveLoadDeferred);
                });
            });

            return recordFilterDirectiveLoadDeferred.promise;
        }

        function loadAccountFields() {

            return Retail_BE_AccountTypeAPIService.GetGenericFieldDefinitionsInfo().then(function (response) {
                accountFields = response;
            });
        }
        function buildContext() {
            var context = {
                getFields: function () {
                    var fields = [];
                    if (accountFields != undefined) {
                        for (var i = 0; i < accountFields.length; i++) {
                            var accountField = accountFields[i];

                            fields.push({
                                FieldName: accountField.Name,
                                FieldTitle: accountField.Title,
                                Type: accountField.FieldType,
                            });
                        }
                    }
                    return fields;
                }
            }
            return context;
        };

        function buildGridQuery()
        {
            return {
                Name: $scope.scopeModel.name,
                AccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                FilterGroup: recordFilterDirectiveAPI.getData().filterObj,
                ParentAccountId: null
            };
        }
    }

    appControllers.controller('Retail_BE_AccountManagementController', AccountManagementController);

})(appControllers);