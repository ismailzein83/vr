'use strict';

app.directive('retailBeAccountGrid', ['Retail_BE_AccountAPIService', 'Retail_BE_AccountService', 'Retail_BE_AccountPackageService', 'Retail_BE_AccountPackageAPIService', 'Retail_BE_AccountIdentificationService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Retail_BE_AccountServiceAPIService', 'Retail_BE_AccountServiceService', 'Retail_BE_ActionRuntimeService', 'Retail_BE_EntityTypeEnum', 'Retail_BE_ActionDefinitionService', 'VR_AccountBalance_BillingTransactionService',
    function (Retail_BE_AccountAPIService, Retail_BE_AccountService, Retail_BE_AccountPackageService, Retail_BE_AccountPackageAPIService, Retail_BE_AccountIdentificationService, UtilsService, VRUIUtilsService, VRNotificationService, Retail_BE_AccountServiceAPIService, Retail_BE_AccountServiceService, Retail_BE_ActionRuntimeService, Retail_BE_EntityTypeEnum, Retail_BE_ActionDefinitionService, VR_AccountBalance_BillingTransactionService) {
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Account/Templates/AccountGridTemplate.html'
        };

        function AccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var drillDownManager;
            var gridQery;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.accounts = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.scopeModel.menuActions, true);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_AccountAPIService.GetFilteredAccounts(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                $scope.scopeModel.getStatusColor = function (dataItem) {
                    return dataItem.Style;
                };

                $scope.scopeModel.getMenuActions = function (dataItem) {
                    var menuActions = [];
                    if (dataItem.drillDownExtensionObject.menuActions != undefined) {
                        for (var i = 0; i < dataItem.drillDownExtensionObject.menuActions.length; i++) {
                            var menuAction = dataItem.drillDownExtensionObject.menuActions[i];
                            menuActions.push(menuAction);
                        }
                    }
                    if (dataItem.ActionDefinitions != undefined) {
                        for (var i = 0; i < dataItem.ActionDefinitions.length; i++) {
                            var actionDefinition = dataItem.ActionDefinitions[i];
                            menuActions.push(defineActionDefinitionMenuAction(actionDefinition));
                        }
                    }
                    return menuActions;
                };

                defineMenuActions();

                function defineActionDefinitionMenuAction(actionDefinition) {
                    return {
                        name: actionDefinition.Name,
                        clicked: function (dataItem) {
                            return Retail_BE_ActionRuntimeService.openActionRuntime(dataItem.Entity.AccountId, actionDefinition.ActionDefinitionId,
                                function () {
                                    $scope.scopeModel.isloadingGrid = true;
                                    return Retail_BE_AccountAPIService.GetAccountDetail(dataItem.Entity.AccountId).then(function (response) {
                                        drillDownManager.setDrillDownExtensionObject(response);
                                        gridAPI.itemUpdated(response);
                                        $scope.scopeModel.isloadingGrid = false;
                                    });
                                });
                        }
                    };
                }
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    gridQery = query;
                    return gridAPI.retrieveData(query);
                };

                api.onAccountAdded = function (addedAccount) {
                    drillDownManager.setDrillDownExtensionObject(addedAccount);
                    gridAPI.itemAdded(addedAccount);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildDrillDownTabs() {
                var drillDownTabs = [];

                drillDownTabs.push(buildSubAccountsTab());
                drillDownTabs.push(buildAssignedPackagesTab());
                drillDownTabs.push(buildAssignedServicesTab());
                drillDownTabs.push(buildIdentificationRulesTab());
                drillDownTabs.push(buildActionMonitorTab());
                // drillDownTabs.push(buildBalanceTab());
                drillDownTabs.push(buildBillingTransactionTab());

                function buildSubAccountsTab() {
                    var subAccountsTab = {};

                    subAccountsTab.title = 'Sub Accounts';
                    subAccountsTab.directive = 'retail-be-account-grid';

                    subAccountsTab.loadDirective = function (subAccountGridAPI, parentAccount) {
                        parentAccount.subAccountGridAPI = subAccountGridAPI;
                        var subAccountGridPayload = {
                            ParentAccountId: parentAccount.Entity.AccountId
                        };
                        return parentAccount.subAccountGridAPI.load(subAccountGridPayload);
                    };
                    subAccountsTab.hideDrillDownFunction = function (dataItem) {
                        return !dataItem.CanAddSubAccounts;
                    };

                    subAccountsTab.parentMenuActions = [{
                        name: 'Add Sub Account',
                        clicked: function (parentAccount) {
                            if (subAccountsTab.setTabSelected != undefined)
                                subAccountsTab.setTabSelected(parentAccount);
                            var onSubAccountAdded = function (addedSubAccount) {
                                parentAccount.subAccountGridAPI.onAccountAdded(addedSubAccount);
                            };
                            Retail_BE_AccountService.addAccount(parentAccount.Entity.AccountId, onSubAccountAdded);
                        },
                        haspermission: hasAddSubAccountPermission
                    }];

                    subAccountsTab.haspermission = function () {
                        return Retail_BE_AccountAPIService.HasViewAccountsPermission();
                    };

                    return subAccountsTab;
                }
                function buildAssignedPackagesTab() {
                    var packagesTab = {};

                    packagesTab.title = 'Packages';
                    packagesTab.directive = 'retail-be-accountpackage-grid';

                    packagesTab.loadDirective = function (accountPackageGridAPI, account) {
                        account.accountPackageGridAPI = accountPackageGridAPI;
                        var accountPackageGridPayload = {
                            AssignedToAccountId: account.Entity.AccountId
                        };
                        return account.accountPackageGridAPI.load(accountPackageGridPayload);
                    };

                    packagesTab.parentMenuActions = [{
                        name: 'Assign Package',
                        clicked: function (account) {
                            if (packagesTab.setTabSelected != undefined)
                                packagesTab.setTabSelected(account);
                            var onAccountPackageAdded = function (addedAccountPackage) {
                                account.accountPackageGridAPI.onAccountPackageAdded(addedAccountPackage);
                            };
                            Retail_BE_AccountPackageService.assignPackageToAccount(account.Entity.AccountId, onAccountPackageAdded);
                        },
                        haspermission: hasAssignPackagePermission
                    }];

                    packagesTab.haspermission = function () {
                        return Retail_BE_AccountPackageAPIService.HasViewAccountPackagesPermission();
                    };

                    return packagesTab;
                }
                function buildIdentificationRulesTab() {
                    var identificationRuleTab = {};

                    identificationRuleTab.title = 'Identification Rules';
                    identificationRuleTab.directive = 'retail-be-accountidentification-grid';

                    identificationRuleTab.loadDirective = function (accountIdentificationRulesGridAPI, account) {
                        account.accountIdentificationRulesGridAPI = accountIdentificationRulesGridAPI;
                        var accountIdentificationRulesGridAPIGridPayload = {
                            AccountId: account.Entity.AccountId
                        };
                        return account.accountIdentificationRulesGridAPI.load(accountIdentificationRulesGridAPIGridPayload);
                    };

                    identificationRuleTab.parentMenuActions = [{
                        name: 'Assign Identification Rule',
                        clicked: function (account) {
                            if (identificationRuleTab.setTabSelected != undefined)
                                identificationRuleTab.setTabSelected(account);

                            var onAccountIdentificationRuleAdded = function (addedIdentificationRule) {
                                account.accountIdentificationRulesGridAPI.onAccountIdentificationRuleAdded(addedIdentificationRule);
                            };
                            Retail_BE_AccountIdentificationService.assignIdentificationRuleToAccount(account.Entity.AccountId, onAccountIdentificationRuleAdded);
                        }
                    }];

                    return identificationRuleTab;
                }

                function buildAssignedServicesTab() {
                    var servicesTab = {};

                    servicesTab.title = 'Services';
                    servicesTab.directive = 'retail-be-accountservice-grid';

                    servicesTab.loadDirective = function (accountServiceGridAPI, account) {
                        account.accountServiceGridAPI = accountServiceGridAPI;
                        var accountServiceGridPayload = {
                            AccountId: account.Entity.AccountId
                        };
                        return account.accountServiceGridAPI.loadGrid(accountServiceGridPayload);
                    };

                    servicesTab.parentMenuActions = [{
                        name: 'Assign Service',
                        clicked: function (account) {
                            if (servicesTab.setTabSelected != undefined)
                                servicesTab.setTabSelected(account);
                            assignService(account);
                        },
                        haspermission: hasAssignServicePermission
                    }];
                    return servicesTab;
                }

                function buildActionMonitorTab() {
                    var actionMonitorTab = {};

                    actionMonitorTab.title = 'Actions';
                    actionMonitorTab.directive = 'businessprocess-bp-instance-monitor-grid';

                    actionMonitorTab.loadDirective = function (actionMonitorGridAPI, account) {
                        account.actionMonitorGridAPI = actionMonitorGridAPI;
                        var actionMonitorGridPayload = {
                            EntityId: Retail_BE_ActionDefinitionService.getEntityId(Retail_BE_EntityTypeEnum.Account.value, account.Entity.AccountId)
                        };
                        return account.actionMonitorGridAPI.loadGrid(actionMonitorGridPayload);
                    };

                    return actionMonitorTab;
                }

                function buildBalanceTab() {
                    var balanceTab = {};

                    balanceTab.title = 'Balance';
                    balanceTab.directive = 'vr-livebalance-currentaccountbalance';

                    balanceTab.loadDirective = function (balanceAPI, account) {
                        account.balanceAPI = balanceAPI;
                        var balancePayload = {
                            accountId: [account.Entity.AccountId],
                            accountTypeId: "20b0c83e-6f53-49c7-b52f-828a19e6dc2a"
                        };
                        return account.balanceAPI.load(balancePayload);
                    };
                    return balanceTab;
                }

                function buildBillingTransactionTab() {
                    var billingTransactionTab = {};

                    billingTransactionTab.title = 'Financial Transactions';
                    billingTransactionTab.directive = 'vr-accountbalance-billingtransaction-search';

                    billingTransactionTab.loadDirective = function (billingTransactionGridAPI, account) {
                        account.billingTransactioGridAPI = billingTransactionGridAPI;
                        var billingTransactionGridPayload = {
                            AccountsIds: [account.Entity.AccountId],
                            AccountTypeId: "20b0c83e-6f53-49c7-b52f-828a19e6dc2a"
                        };
                        return account.billingTransactioGridAPI.loadDirective(billingTransactionGridPayload);
                    };
                    return billingTransactionTab;
                }

                return drillDownTabs;
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
                    drillDownManager.setDrillDownExtensionObject(updatedAccount);
                    gridAPI.itemUpdated(updatedAccount);
                };

                Retail_BE_AccountService.editAccount(account.Entity.AccountId, account.Entity.ParentAccountId, onAccountUpdated);
            }
            function hasEditAccountPermission() {
                return Retail_BE_AccountAPIService.HasUpdateAccountPermission();
            }

            function addSubAccount(parentAccount) {
                gridAPI.expandRow(parentAccount);

                var onSubAccountAdded = function (addedSubAccount) {
                    parentAccount.subAccountGridAPI.onAccountAdded(addedSubAccount);
                };

                Retail_BE_AccountService.addAccount(parentAccount.Entity.AccountId, onSubAccountAdded);
            }
            function hasAddSubAccountPermission() {
                return Retail_BE_AccountAPIService.HasAddAccountPermission();
            }

            function assignPackage(account) {
                gridAPI.expandRow(account);

                var onAccountPackageAdded = function (addedAccountPackage) {
                    account.accountPackageGridAPI.onAccountPackageAdded(addedAccountPackage);
                };

                Retail_BE_AccountPackageService.assignPackageToAccount(account.Entity.AccountId, onAccountPackageAdded);
            }
            function hasAssignPackagePermission() {
                return Retail_BE_AccountPackageAPIService.HasAddAccountPackagePermission();
            }

            function assignService(account) {

                var onAccountServiceAdded = function (addedAccountService) {
                    account.accountServiceGridAPI.onAccountServiceAdded(addedAccountService);
                };

                Retail_BE_AccountServiceService.addAccountService(onAccountServiceAdded, account.Entity.AccountId);
            }
            function hasAssignServicePermission() {
                return Retail_BE_AccountServiceAPIService.HasAddAccountServicePermission();
            }
        }
    }]);
