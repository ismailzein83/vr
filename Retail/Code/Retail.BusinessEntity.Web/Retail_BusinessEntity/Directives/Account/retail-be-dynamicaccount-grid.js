'use strict';

app.directive('retailBeDynamicaccountGrid', ['VRNotificationService', 'UtilsService', 'Retail_BE_AccountAPIService', 'Retail_BE_AccountBEDefinitionAPIService', 'Retail_BE_AccountService',
    function (VRNotificationService, UtilsService, Retail_BE_AccountAPIService, Retail_BE_AccountBEDefinitionAPIService, Retail_BE_AccountService) {
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

            var accountBEDefinitionId;
            var parentAccountId;
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

                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var account = response.Data[i];
                                Retail_BE_AccountService.defineAccountViewTabsAndMenuActions(accountBEDefinitionId, account, accountViewDefinitions, gridAPI);
                            }
                        }
                        onResponseReady(response);

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var gridQuery;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        parentAccountId = payload.parentAccountId;
                        gridQuery = payload.query;
                    }

                    if ($scope.scopeModel.columns.length == 0) {

                        //Loading AccountGridColumns
                        var accountGridColumnsLoadPromise = getAccountGridColumnsLoadPromise();
                        promises.push(accountGridColumnsLoadPromise);

                        //Loading AccountViewDefinitions
                        var accountViewDefinitionsLoadPromise = getAccountViewDefinitionsLoadPromise();
                        promises.push(accountViewDefinitionsLoadPromise);
                    }

                    var gridLoadDeferred = UtilsService.createPromiseDeferred();

                    //Retrieving Data
                    UtilsService.waitMultiplePromises(promises).then(function () {

                        gridQuery = buildGridQuery(gridQuery);
                        gridAPI.retrieveData(gridQuery).then(function () {
                            gridLoadDeferred.resolve();
                        }).catch(function (error) {
                            gridLoadDeferred.reject(error);
                        });

                    }).catch(function (error) {
                        gridLoadDeferred.reject(error);
                    });

                    function getAccountGridColumnsLoadPromise() {

                        var accountGridColumnAttributesLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        Retail_BE_AccountBEDefinitionAPIService.GetAccountGridColumnAttributes(accountBEDefinitionId, parentAccountId).then(function (response) {

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

                        Retail_BE_AccountBEDefinitionAPIService.GetAccountViewDefinitions(accountBEDefinitionId).then(function (response) {

                            accountViewDefinitions = response;
                            accountViewDefinitionsLoadPromiseDeferred.resolve();

                        }).catch(function (error) {
                            accountViewRuntimeEditorsLoadPromiseDeferred.reject(error);
                        });

                        return accountViewDefinitionsLoadPromiseDeferred.promise;
                    }
                    function buildGridQuery(gridQuery) {
                        if (gridQuery == undefined)
                            gridQuery = {};

                        gridQuery.AccountBEDefinitionId = accountBEDefinitionId;
                        gridQuery.ParentAccountId = parentAccountId;
                        gridQuery.Columns = gridColumnFieldNames;
                        return gridQuery;
                    }

                    return gridLoadDeferred.promise;
                };

                api.onAccountAdded = function (addedAccount) {
                    Retail_BE_AccountService.defineAccountViewTabsAndMenuActions(addedAccount, accountViewDefinitions, gridAPI);
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
                }, {
                    name: 'Open Account 360 Degree',
                    clicked: openAccount360DegreeEditor,
                });
            }
            function editAccount(account) {
                var onAccountUpdated = function (updatedAccount) {
                    Retail_BE_AccountService.defineAccountViewTabsAndMenuActions(updatedAccount, accountViewDefinitions, gridAPI);
                    gridAPI.itemUpdated(updatedAccount);
                };

                Retail_BE_AccountService.editAccount(account.Entity.AccountId, account.Entity.ParentAccountId, onAccountUpdated);
            }
            function openAccount360DegreeEditor(account) {
                Retail_BE_AccountService.openAccount360DegreeEditor(account.Entity.AccountId);
            }
            function hasEditAccountPermission() {
                return Retail_BE_AccountAPIService.HasUpdateAccountPermission();
            }
        }
    }]);
