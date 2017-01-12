"use strict";

app.directive("retailBeAccountserviceGrid", ["UtilsService", "VRNotificationService", "Retail_BE_AccountServiceAPIService", "Retail_BE_AccountServiceService", "VRUIUtilsService", "Retail_BE_ActionRuntimeService", "Retail_BE_ActionDefinitionService", "Retail_BE_EntityTypeEnum",
    function (UtilsService, VRNotificationService, Retail_BE_AccountServiceAPIService, Retail_BE_AccountServiceService, VRUIUtilsService, Retail_BE_ActionRuntimeService, Retail_BE_ActionDefinitionService, Retail_BE_EntityTypeEnum) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var accountServiceGrid = new AccountServiceGrid($scope, ctrl, $attrs);
                accountServiceGrid.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountService/Templates/AccountServiceGridTemplate.html"
        };

        function AccountServiceGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountId;

            var gridAPI;

            var drillDownManager;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.accountServices = [];

                $scope.scopeModel.getStatusColor = function (dataItem) {
                    return dataItem.Style;
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    drillDownManager = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI);

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                        ctrl.onReady(getDirectiveAPI());

                    function getDirectiveAPI() {
                        var directiveAPI = {};

                        directiveAPI.loadGrid = function (payload) {

                            if (payload != undefined) {
                                accountBEDefinitionId = payload.accountBEDefinitionId;
                                accountId = payload.AccountId
                            }

                            function buildGridQuery() {
                                return {
                                    AccountBEDefinitionId: accountBEDefinitionId,
                                    AccountId: accountId
                                };
                            }

                            return gridAPI.retrieveData(buildGridQuery());
                        };

                        directiveAPI.onAccountServiceAdded = function (accountServiceObject) {
                            drillDownManager.setDrillDownExtensionObject(accountServiceObject);
                            gridAPI.itemAdded(accountServiceObject);
                        };

                        return directiveAPI;
                    }
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_AccountServiceAPIService.GetFilteredAccountServices(dataRetrievalInput)
                        .then(function (response) {
                            if (response && response.Data) {
                                for (var i = 0; i < response.Data.length; i++) {
                                    drillDownManager.setDrillDownExtensionObject(response.Data[i]);
                                }
                            }
                            onResponseReady(response);
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                };

                defineMenuActions();
            }

            function buildDrillDownTabs() {
                var drillDownTabs = [];

                drillDownTabs.push(buildActionMonitorTab());

                function buildActionMonitorTab() {
                    var actionMonitorTab = {};

                    actionMonitorTab.title = 'Actions';
                    actionMonitorTab.directive = 'businessprocess-bp-instance-monitor-grid';

                    actionMonitorTab.loadDirective = function (actionMonitorGridAPI, accountService) {
                        accountService.actionMonitorGridAPI = actionMonitorGridAPI;
                        var actionMonitorGridPayload = {
                            EntityId: Retail_BE_ActionDefinitionService.getEntityId(Retail_BE_EntityTypeEnum.AccountService.value, accountService.Entity.AccountServiceId)
                        };
                        return accountService.actionMonitorGridAPI.loadGrid(actionMonitorGridPayload);
                    };

                    return actionMonitorTab;
                }

                return drillDownTabs;
            }

            function defineMenuActions() {

                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    var menuActions = [{
                        name: "Edit",
                        clicked: editAccountService,
                        haspermission: hasUpdateAccountServicePermission
                    }];
                    if (dataItem.ActionDefinitions != undefined) {
                        for (var i = 0; i < dataItem.ActionDefinitions.length; i++) {
                            var actionDefinition = dataItem.ActionDefinitions[i];
                            menuActions.push(defineActionDefinitionMenuAction(actionDefinition));
                        }
                    }
                    return menuActions;
                };

                function defineActionDefinitionMenuAction(actionDefinition) {
                    return {
                        name: actionDefinition.Name,
                        clicked: function (dataItem) {
                            return Retail_BE_ActionRuntimeService.openActionRuntime(dataItem.Entity.AccountServiceId, actionDefinition.ActionDefinitionId,
                                function () {
                                    $scope.scopeModel.isloadingGrid = true;
                                    return Retail_BE_AccountServiceAPIService.GetAccountServiceDetail(accountBEDefinitionId, dataItem.Entity.AccountServiceId).then(function (response) {
                                        drillDownManager.setDrillDownExtensionObject(response);
                                        gridAPI.itemUpdated(response);
                                        $scope.scopeModel.isloadingGrid = false;
                                    });
                                });
                        }
                    };
                }
            }
            function editAccountService(accountServiceObj) {
                var onAccountServiceUpdated = function (accountServiceObject) {
                    drillDownManager.setDrillDownExtensionObject(accountServiceObject);
                    gridAPI.itemUpdated(accountServiceObject);

                };
                Retail_BE_AccountServiceService.editAccountService(accountServiceObj.Entity.AccountServiceId, accountBEDefinitionId, onAccountServiceUpdated);
            }
            function hasUpdateAccountServicePermission() {
                return Retail_BE_AccountServiceAPIService.HasUpdateAccountServicePermission();
            }
        }

        return directiveDefinitionObject;
    }]);