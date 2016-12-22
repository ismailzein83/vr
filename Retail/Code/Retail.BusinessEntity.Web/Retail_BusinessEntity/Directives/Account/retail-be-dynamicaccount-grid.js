'use strict';

app.directive('retailBeDynamicaccountGrid', ['VRNotificationService', 'UtilsService', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountDefinitionAPIService',
    function (VRNotificationService, UtilsService, Retail_BE_AccountAPIService, Retail_BE_AccountDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountGrid = new AccountGrid($scope, ctrl, $attrs);
                accountGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/DynamicAccountGridTemplate.html'
        };

        function AccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridColumnFieldNames = [];
            var accountViewDefinitions = [];

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.columns = [];
                $scope.scopeModel.accounts = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {

                    return Retail_BE_AccountAPIService.GetFilteredAccounts(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    var promises = [];

                    var gridQuery = query;

                    if ($scope.scopeModel.columns.length == 0) {

                        //Loading AccountGridColumns
                        var accountGridColumnsLoadPromise = getAccountGridColumnsLoadPromise();
                        promises.push(accountGridColumnsLoadPromise);

                        //Loading AccountViewDefinitions
                        var accountViewDefinitionsLoadPromise = getAccountViewDefinitionsLoadPromise();
                        promises.push(accountViewDefinitionsLoadPromise);
                    }

                    var accountGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    //Retrieving Data
                    UtilsService.waitMultiplePromises(promises).then(function () {

                        if (gridQuery == undefined)
                            gridQuery = {};
                        gridQuery.Columns = gridColumnFieldNames;

                        gridAPI.retrieveData(gridQuery).then(function () {
                            accountGridLoadPromiseDeferred.resolve();
                        }).catch(function (error) {
                            accountGridLoadPromiseDeferred.reject(error);
                        });
                    }).catch(function (error) {
                        accountGridLoadPromiseDeferred.reject(error);
                    });

                    function getAccountGridColumnsLoadPromise() {

                        var accountGridColumnAttributesLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        Retail_BE_AccountDefinitionAPIService.GetAccountGridColumnAttributes().then(function (response) {

                            var accountGridColumnAttributes = response;

                            if (accountGridColumnAttributes != undefined) {
                                for (var index = 0; index < accountGridColumnAttributes.length; index++) {
                                    var accountGridColumnAttribute = accountGridColumnAttributes[index];
                                    var column = {
                                        Field: accountGridColumnAttribute.Attribute.Field,
                                        HeaderText: accountGridColumnAttribute.Attribute.HeaderText,
                                        Type: accountGridColumnAttribute.Attribute.Type
                                    }
                                    gridColumnFieldNames.push(accountGridColumnAttribute.Name);
                                    $scope.scopeModel.columns.push(column);
                                }
                            }
                            accountGridColumnAttributesLoadPromiseDeferred.resolve();

                        }).catch(function (error) {
                            accountGridColumnAttributesLoadPromiseDeferred.reject(error);
                        });

                        return accountGridColumnAttributesLoadPromiseDeferred.promise;
                    }
                    function getAccountViewDefinitionsLoadPromise() {

                        var accountViewDefinitionsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        Retail_BE_AccountDefinitionAPIService.GetAccountViewDefinitions().then(function (response) {

                            accountViewDefinitions = response;
                            accountViewDefinitionsLoadPromiseDeferred.resolve();

                        }).catch(function (error) {
                            accountViewRuntimeEditorsLoadPromiseDeferred.reject(error);
                        });

                        return accountViewDefinitionsLoadPromiseDeferred.promise;
                    }

                    return accountGridLoadPromiseDeferred.promise;
                };

                api.onAccountAdded = function (addedAccount) {
                    gridAPI.itemAdded(addedAccount);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editAccount,
                    haspermission: hasEditAccountPermission
                });
            }
            function editAccount(account) {
                var onAccountUpdated = function (updatedAccount) {
                    //drillDownManager.setDrillDownExtensionObject(updatedAccount);
                    gridAPI.itemUpdated(updatedAccount);
                };

                Retail_BE_AccountService.editAccount(account.Entity.AccountId, account.Entity.ParentAccountId, onAccountUpdated);
            }
            function hasEditAccountPermission() {
                return Retail_BE_AccountAPIService.HasUpdateAccountPermission();
            }
        }
    }]);
