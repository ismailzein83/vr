'use strict';

app.directive('retailBeAccountGrid', ['Retail_BE_AccountAPIService', 'Retail_BE_AccountService', 'Retail_BE_AccountPackageService', 'Retail_BE_AccountPackageAPIService', 'Retail_BE_AccountTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (Retail_BE_AccountAPIService, Retail_BE_AccountService, Retail_BE_AccountPackageService, Retail_BE_AccountPackageAPIService, Retail_BE_AccountTypeEnum, UtilsService, VRUIUtilsService, VRNotificationService)
{
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

    function AccountGrid($scope, ctrl, $attrs)
    {
        this.initializeController = initializeController;

        var gridAPI;
        var drillDownManager;

        function initializeController()
        {
            $scope.scopeModel = {};
            $scope.scopeModel.accounts = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api)
            {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.scopeModel.menuActions);
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady)
            {
                return Retail_BE_AccountAPIService.GetFilteredAccounts(dataRetrievalInput).then(function (response)
                {
                    if (response && response.Data) {
                        for (var i = 0; i < response.Data.length; i++) {
                            setAccountTypeDescription(response.Data[i]);
                            drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }

        function defineAPI()
        {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onAccountAdded = function (addedAccount)
            {
                setAccountTypeDescription(addedAccount);
                drillDownManager.setDrillDownExtensionObject(addedAccount);
                gridAPI.itemAdded(addedAccount);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function buildDrillDownTabs()
        {
            var drillDownTabs = [];

            drillDownTabs.push(buildSubAccountsTab());
            drillDownTabs.push(buildAssignedPackagesTab());

            function buildSubAccountsTab()
            {
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

                packagesTab.title = 'Assigned Packages';
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

            return drillDownTabs;
        }

        function defineMenuActions()
        {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editAccount,
                haspermission: hasEditAccountPermission
            });
        }

        function editAccount(account)
        {
            var onAccountUpdated = function (updatedAccount)
            {
                setAccountTypeDescription(updatedAccount);
                drillDownManager.setDrillDownExtensionObject(updatedAccount);
                gridAPI.itemUpdated(updatedAccount);
            };

            Retail_BE_AccountService.editAccount(account.Entity.AccountId, account.Entity.ParentAccountId, onAccountUpdated);
        }
        function hasEditAccountPermission() {
            return Retail_BE_AccountAPIService.HasUpdateAccountPermission();
        }

        function addSubAccount(parentAccount)
        {
            gridAPI.expandRow(parentAccount);

            var onSubAccountAdded = function (addedSubAccount)
            {
                parentAccount.subAccountGridAPI.onAccountAdded(addedSubAccount);
            };

            Retail_BE_AccountService.addAccount(parentAccount.Entity.AccountId, onSubAccountAdded);
        }
        function hasAddSubAccountPermission() {
            return Retail_BE_AccountAPIService.HasAddAccountPermission();
        }

        function assignPackage(account)
        {
            gridAPI.expandRow(account);

            var onAccountPackageAdded = function (addedAccountPackage) {
                account.accountPackageGridAPI.onAccountPackageAdded(addedAccountPackage);
            };

            Retail_BE_AccountPackageService.assignPackageToAccount(account.Entity.AccountId, onAccountPackageAdded);
        }
        function hasAssignPackagePermission() {
            return Retail_BE_AccountPackageAPIService.HasAddAccountPackagePermission();
        }

        function setAccountTypeDescription(account)
        {
            var accountType = UtilsService.getEnum(Retail_BE_AccountTypeEnum, 'value', account.Entity.Type);
            account.typeDescription = accountType.description;
        }
    }
}]);
