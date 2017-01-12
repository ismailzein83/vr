"use strict";

app.directive("retailBeAccountactiondefinitionsManagement", ["UtilsService", "VRNotificationService", "Retail_BE_AccountBEDefinitionService",
    function (UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountActionDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/AccountActionDefinitionsManagementTemplate.html"

        };

        function AccountActionDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addAccountActionDefinition = function () {
                    var onAccountActionDefinitionAdded = function (accountActionDefinition) {
                        ctrl.datasource.push({ Entity: accountActionDefinition });
                    };
                    Retail_BE_AccountBEDefinitionService.addAccountActionDefinition(accountBEDefinitionId, onAccountActionDefinitionAdded);
                };

                ctrl.removeAccountActionDefinition = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;

                        if (payload.accountActionDefinitions != undefined) {
                            for (var i = 0; i < payload.accountActionDefinitions.length; i++) {
                                var accountActionDefinition = payload.accountActionDefinitions[i];
                                ctrl.datasource.push({ Entity: accountActionDefinition });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var accountActionDefinitions;
                    if (ctrl.datasource != undefined) {
                        accountActionDefinitions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            accountActionDefinitions.push(currentItem.Entity);
                        }
                    }
                    return accountActionDefinitions;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAccountActionDefinition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }
            function editAccountActionDefinition(accountActionDefinitionObj) {
                var onAccountActionDefinitionUpdated = function (accountActionDefinition) {
                    var index = ctrl.datasource.indexOf(accountActionDefinitionObj);
                    ctrl.datasource[index] = { Entity: accountActionDefinition };
                };

                Retail_BE_AccountBEDefinitionService.editAccountActionDefinition(accountActionDefinitionObj.Entity, accountBEDefinitionId, onAccountActionDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);