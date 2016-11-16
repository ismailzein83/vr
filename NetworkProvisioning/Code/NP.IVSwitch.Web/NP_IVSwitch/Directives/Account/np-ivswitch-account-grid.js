'use strict';

app.directive('npIvswitchAccountGrid', ['NP_IVSwitch_AccountAPIService', 'NP_IVSwitch_AccountService', 'VRNotificationService', 'NP_IVSwitch_AccountTypeEnum','NP_IVSwitch_StateEnum','UtilsService','VRUIUtilsService','NP_IVSwitch_RouteService',
function (NP_IVSwitch_AccountAPIService, NP_IVSwitch_AccountService, VRNotificationService, NP_IVSwitch_AccountTypeEnum, NP_IVSwitch_StateEnum, UtilsService, VRUIUtilsService, NP_IVSwitch_RouteService) {
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
            var drillDownManager;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.account = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.scopeModel.menuActions, false);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return NP_IVSwitch_AccountAPIService.GetFilteredAccounts(dataRetrievalInput).then(function (response) {
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

            function buildDrillDownTabs() {
                var drillDownTabs = [];

                drillDownTabs.push(buildRouteTab());
           //     drillDownTabs.push(buildSubEndPointsTab());

                function buildRouteTab() {
                    var  RouteTab = {};

                    RouteTab.title = 'Route';
                    RouteTab.directive = 'np-ivswitch-route-grid';

                    RouteTab.loadDirective = function (subAccountGridAPI, parentAccount) {
                        parentAccount.subAccountGridAPI = subAccountGridAPI;
                        var subAccountGridPayload = {
                            AccountId: parentAccount.Entity.AccountId
                        };
                        return parentAccount.subAccountGridAPI.load(subAccountGridPayload);
                    };
                    RouteTab.hideDrillDownFunction = function (dataItem) {
                        return false; /////////////////////////////
                    };

                    RouteTab.parentMenuActions = [{
                        name: 'Add Route',
                        clicked: function (parentAccount) {
                            if (RouteTab.setTabSelected != undefined)
                                RouteTab.setTabSelected(parentAccount);
                            var onRouteAdded = function (addedRoute) {
                                parentAccount.subAccountGridAPI.onRouteAdded(addedRoute);
                            };
                            console.log(parentAccount.Entity.AccountId)
                            NP_IVSwitch_RouteService.addRoute(parentAccount.Entity.AccountId, onRouteAdded);
                        },
                     }];

 
                    return RouteTab;
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
