'use strict';

app.directive('npIvswitchAccountGrid', ['NP_IVSwitch_AccountAPIService', 'NP_IVSwitch_AccountService', 'VRNotificationService', 'NP_IVSwitch_AccountTypeEnum','NP_IVSwitch_StateEnum','UtilsService',
function (NP_IVSwitch_AccountAPIService, NP_IVSwitch_AccountService, VRNotificationService, NP_IVSwitch_AccountTypeEnum, NP_IVSwitch_StateEnum,UtilsService) {
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
            templateUrl: '/Client/Modules/NP_IVSwitch/Directives/Account/Templates/AccountGridTemplate.html'
        };

        function AccountGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.account = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_AccountAPIService.GetFilteredAccounts(dataRetrievalInput).then(function (response) {
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
                    return gridAPI.retrieveData(query);
                };

                api.onAccountAdded = function (addedAccount) {
                    gridAPI.itemAdded(addedAccount);
                }

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
            function editAccount(accountItem) {
                var onAccountUpdated = function (updatedAccount) {
                    gridAPI.itemUpdated(updatedAccount);
                };

                NP_IVSwitch_AccountService.editAccount(accountItem.Entity.AccountId, onAccountUpdated);
            }
            function hasEditAccountPermission() {
                return NP_IVSwitch_AccountAPIService.HasEditAccountPermission();
            }

            

        }
    }]);
