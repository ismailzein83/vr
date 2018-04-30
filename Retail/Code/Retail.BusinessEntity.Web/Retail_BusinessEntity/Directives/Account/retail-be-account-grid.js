'use strict';

app.directive('retailBeAccountGrid', ['VRNotificationService', 'UtilsService', 'Retail_BE_AccountBEService', 'Retail_BE_AccountBEAPIService', 'Retail_BE_AccountBEDefinitionAPIService', 'Retail_BE_AccountActionService','VRCommon_VRBulkActionDraftService',
function (VRNotificationService, UtilsService, Retail_BE_AccountBEService, Retail_BE_AccountBEAPIService, Retail_BE_AccountBEDefinitionAPIService, Retail_BE_AccountActionService, VRCommon_VRBulkActionDraftService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var accountGrid = new AccountGrid($scope, ctrl, $attrs);
            accountGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountGridTemplate.html'
    };

    function AccountGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var accountBEDefinitionId;
        var parentAccountId;
        var gridColumnFieldNames = [];
        var accountViewDefinitions = [];
        var accountActionDefinitions = [];

        var gridAPI;
        var gridQuery;
            
        var context;
        var bulkActionId;
        var bulkActionDraftInstance;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.columns = [];
            $scope.scopeModel.accounts = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.showMultipleSelection = false;

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_AccountBEAPIService.GetFilteredAccounts(dataRetrievalInput).then(function (response) {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            var account = response.Data[i];
                            if (bulkActionId != undefined)
                            {
                                account.isSelected = bulkActionDraftInstance.isItemSelected(account.AccountId);
                            }else
                            {
                                Retail_BE_AccountBEService.defineAccountViewTabs(accountBEDefinitionId, account, gridAPI, accountViewDefinitions);
                                Retail_BE_AccountActionService.defineAccountMenuActions(accountBEDefinitionId, account, gridAPI, accountViewDefinitions, accountActionDefinitions);
                            }
                        }
                        
                    }
                    onResponseReady(response);

                    if (bulkActionId != undefined) {
                        bulkActionDraftInstance.reEvaluateButtonsStatus();
                    }

                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            $scope.scopeModel.menuActions = function (account) {
                var menuActions = [];
                if (account.menuActions != undefined) {
                    for (var i = 0; i < account.menuActions.length; i++)
                        menuActions.push(account.menuActions[i]);
                }

                return menuActions;
            };

            $scope.scopeModel.onAccountSelect = function (account) {
                if (bulkActionDraftInstance != undefined) {
                    bulkActionDraftInstance.onSelectItem({ ItemId: account.AccountId }, account.isSelected);
                }
            };

            $scope.scopeModel.showExpandIcon = function (dataItem) {
                return (dataItem.drillDownExtensionObject != null && dataItem.drillDownExtensionObject.drillDownDirectiveTabs.length > 0);
            };
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var promises = [];

                if (payload != undefined) {
                    accountBEDefinitionId = payload.accountBEDefinitionId;
                    parentAccountId = payload.parentAccountId;
                    gridQuery = payload.query;
                    context = payload.context;
                    bulkActionId = payload.bulkActionId;
                }

                if ($scope.scopeModel.columns.length == 0) {

                    //Loading AccountGridColumns
                    var accountGridColumnsLoadPromise = getAccountGridColumnsLoadPromise();
                    promises.push(accountGridColumnsLoadPromise);

                    //Loading AccountViewDefinitions
                    var accountViewDefinitionsLoadPromise = getAccountViewDefinitionsLoadPromise();
                    promises.push(accountViewDefinitionsLoadPromise);

                    //Loading AccountViewDefinitions
                    var accountActionDefinitionsLoadPromise = getAccountActionDefinitionsLoadPromise();
                    promises.push(accountActionDefinitionsLoadPromise);
                }

                var gridLoadDeferred = UtilsService.createPromiseDeferred();

                //Retrieving Data
                UtilsService.waitMultiplePromises(promises).then(function () {

                    if (bulkActionId != undefined) {
                        $scope.scopeModel.showMultipleSelection = true;
                        bulkActionDraftInstance = VRCommon_VRBulkActionDraftService.createBulkActionDraft(getContext());
                    }

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
                                //var column = {
                                //    HeaderText: accountGridColumnAttribute.Attribute.HeaderText,
                                //    Field: accountGridColumnAttribute.Attribute.Field,
                                //    Type: accountGridColumnAttribute.Attribute.Type
                                //};
                                gridColumnFieldNames.push(accountGridColumnAttribute.Name);
                                $scope.scopeModel.columns.push(accountGridColumnAttribute.Attribute);
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
                function getAccountActionDefinitionsLoadPromise() {
                    var accountActionDefinitionsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    Retail_BE_AccountBEDefinitionAPIService.GetAccountActionDefinitions(accountBEDefinitionId).then(function (response) {
                        accountActionDefinitions = response;
                        accountActionDefinitionsLoadPromiseDeferred.resolve();
                    }).catch(function (error) {
                        accountViewRuntimeEditorsLoadPromiseDeferred.reject(error);
                    });

                    return accountActionDefinitionsLoadPromiseDeferred.promise;
                }
          

                return gridLoadDeferred.promise;
            };

            api.onAccountAdded = function (addedAccount) {
                Retail_BE_AccountBEService.defineAccountViewTabs(accountBEDefinitionId, addedAccount, gridAPI, accountViewDefinitions);
                Retail_BE_AccountActionService.defineAccountMenuActions(accountBEDefinitionId, addedAccount, gridAPI, accountViewDefinitions, accountActionDefinitions);
                gridAPI.itemAdded(addedAccount);
            };

            api.deselectAllAccounts = function () {
                bulkActionDraftInstance.deselectAllItems();
            };

            api.selectAllAccounts = function () {
                bulkActionDraftInstance.selectAllItems();
            };

            api.finalizeBulkActionDraft = function () {
                return bulkActionDraftInstance.finalizeBulkActionDraft();
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getContext() {

            var currentContext = context;
            if (currentContext == undefined)
                currentContext = {};
            currentContext.triggerRetrieveData = function () {
                gridQuery.BulkActionState = bulkActionDraftInstance.getBulkActionState();
                gridAPI.retrieveData(buildGridQuery(gridQuery));
            };

            currentContext.hasItems = function () {
                return $scope.scopeModel.accounts.length > 0;
            };
            return currentContext;
        }

        function buildGridQuery(gridQuery) {
            return {
                AccountBEDefinitionId: accountBEDefinitionId,
                ParentAccountId: parentAccountId,
                Columns: gridColumnFieldNames,
                Name: gridQuery != undefined ? gridQuery.Name : undefined,
                OnlyRootAccount: gridQuery != undefined ? gridQuery.OnlyRootAccount : undefined,
                AccountTypeIds: gridQuery != undefined ? gridQuery.AccountTypeIds : undefined,
                FilterGroup: gridQuery != undefined ? gridQuery.FilterGroup : undefined,
                StatusIds: gridQuery != undefined ? gridQuery.StatusIds : undefined,
                BulkActionState: gridQuery != undefined ? gridQuery.BulkActionState : undefined,
                AccountBulkActionId: parentAccountId == undefined?bulkActionId:undefined
            };
        }
    }
}]);
