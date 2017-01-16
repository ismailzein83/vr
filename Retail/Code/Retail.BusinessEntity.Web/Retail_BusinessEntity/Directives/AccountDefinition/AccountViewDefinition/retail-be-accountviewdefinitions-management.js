"use strict";

app.directive("retailBeAccountviewdefinitionsManagement", ["UtilsService", "VRNotificationService", "Retail_BE_AccountBEDefinitionService",
    function (UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountViewDefinition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountViewDefinition/Templates/AccountViewDefinitionsManagementTemplate.html"

        };

        function AccountViewDefinition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;

            var gridAPI;

            function initializeController() {
                ctrl.datasource = [];

                ctrl.addAccountViewDefinition = function () {
                    var onAccountViewDefinitionAdded = function (accountViewDefinition) {
                        ctrl.datasource.push({ Entity: accountViewDefinition });
                    };
                    Retail_BE_AccountBEDefinitionService.addAccountViewDefinition(accountBEDefinitionId, onAccountViewDefinitionAdded);
                };

                ctrl.removeAccountViewDefinition = function (dataItem) {
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

                        if (payload.accountViewDefinitions != undefined) {
                            for (var i = 0; i < payload.accountViewDefinitions.length; i++) {
                                var accountViewDefinition = payload.accountViewDefinitions[i];
                                ctrl.datasource.push({ Entity: accountViewDefinition });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var accountViewDefinitions;
                    if (ctrl.datasource != undefined) {
                        accountViewDefinitions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            accountViewDefinitions.push(currentItem.Entity);
                        }
                    }
                    return accountViewDefinitions;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editAccountViewDefinition,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAccountViewDefinition(accountViewDefinitionObj) {
                var onAccountViewDefinitionUpdated = function (accountViewDefinition) {
                    var index = ctrl.datasource.indexOf(accountViewDefinitionObj);
                    ctrl.datasource[index] = { Entity: accountViewDefinition };
                };
                Retail_BE_AccountBEDefinitionService.editAccountViewDefinition(accountViewDefinitionObj.Entity, accountBEDefinitionId, onAccountViewDefinitionUpdated);
            }
        }

        return directiveDefinitionObject;

    }
]);