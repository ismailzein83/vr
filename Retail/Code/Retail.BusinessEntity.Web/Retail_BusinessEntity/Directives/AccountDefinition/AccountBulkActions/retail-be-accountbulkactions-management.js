"use strict";

app.directive("retailBeAccountbulkactionsManagement", ["UtilsService", "VRNotificationService", "Retail_BE_AccountBEDefinitionService",
    function (UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBulkActions($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountBulkActions/Templates/AccountBulkActionsManagementTemplate.html"

        };

        function AccountBulkActions($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var accountBEDefinitionId;
            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addAccountBulkAction = function () {
                    var onAccountBulkActionAdded = function (accountBulkAction) {
                        ctrl.datasource.push({ Entity: accountBulkAction });
                    };
                    Retail_BE_AccountBEDefinitionService.addAccountBulkAction(accountBEDefinitionId, onAccountBulkActionAdded);
                };

                ctrl.removeAccountBulkAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        if (payload.accountBulkActions != undefined) {
                            for (var i = 0; i < payload.accountBulkActions.length; i++) {
                                var accountBulkAction = payload.accountBulkActions[i];
                                ctrl.datasource.push({ Entity: accountBulkAction });
                            }
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    var accountBulkActions;
                    if (ctrl.datasource != undefined) {
                        accountBulkActions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            accountBulkActions.push(currentItem.Entity);
                        }
                    }
                    return accountBulkActions;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAccountBulkAction,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editAccountBulkAction(accountBulkActionObj) {
                var onAccountBulkActionUpdated = function (accountBulkAction) {
                    var index = ctrl.datasource.indexOf(accountBulkActionObj);
                    ctrl.datasource[index] = { Entity: accountBulkAction };
                };

                Retail_BE_AccountBEDefinitionService.editAccountBulkAction(accountBulkActionObj.Entity, accountBEDefinitionId, onAccountBulkActionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);