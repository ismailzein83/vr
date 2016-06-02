'use strict';

app.directive('retailBeAccountGrid', ['Retail_BE_AccountAPIService', 'Retail_BE_AccountService', 'Retail_BE_AccountTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', function (Retail_BE_AccountAPIService, Retail_BE_AccountService, Retail_BE_AccountTypeEnum, UtilsService, VRUIUtilsService, VRNotificationService)
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

            $scope.scopeModel.onGridReady = function (api)
            {
                gridAPI = api;
                drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.menuActions);
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
            return [{
                title: 'Sub Accounts',
                directive: 'retail-be-account-grid',
                loadDirective: function (subAccountGridAPI, parentAccount)
                {
                    parentAccount.subAccountGridAPI = subAccountGridAPI;

                    var subAccountGridPayload = {
                        ParentAccountId: parentAccount.Entity.AccountId
                    };

                    return parentAccount.subAccountGridAPI.load(subAccountGridPayload);
                }
            }];
        }

        function defineMenuActions()
        {
            $scope.scopeModel.menuActions = [{
                name: 'Edit',
                clicked: editAccount,
                //haspermission: hasEditAccountPermission
            }, {
                name: 'Add Sub Account',
                clicked: addSubAccount,
                //haspermission: hasAddSubAccountPermission
            }, {
                name: 'Assign Package',
                clicked: assignPackage,
                //haspermission: hasAddSubAccountPermission
            }];
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
            return WhS_BE_RoutingProductAPIService.HasUpdateRoutingProductPermission();
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
            return WhS_Routing_RouteRuleAPIService.HasAddRulePermission();
        }

        function assignPackage(account)
        {
            console.log('Not implemented');
        }
        function hasAssignPackagePermission() { }

        function setAccountTypeDescription(account)
        {
            var accountType = UtilsService.getEnum(Retail_BE_AccountTypeEnum, 'value', account.Entity.Type);
            account.typeDescription = accountType.description;
        }
    }
}]);
