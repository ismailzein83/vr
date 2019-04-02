"use strict";

app.directive("retailBeAccountactiondefinitionsActiongroupGrid", ["UtilsService", "VRNotificationService", "Retail_BE_AccountBEDefinitionService",
    function (UtilsService, VRNotificationService, Retail_BE_AccountBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ActionGroupDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountActionDefinition/Templates/ActionGroupDefinitionGridTemplate.html'
        };

        function ActionGroupDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var accountBEDefinitionId;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each action should be unique.";

                    return null;
                };

                ctrl.addActionGroup = function () {
                    var onActionGroupAdded = function (addedItem) {
                        ctrl.datasource.push({ Entity: addedItem });
                    };

                    Retail_BE_AccountBEDefinitionService.addAccountActionGroupDefinition(accountBEDefinitionId , onActionGroupAdded);
                };

                ctrl.removeActionGroup = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;

                        api.clearDataSource();
                        if (payload.accountActionGroupDefinitions != undefined) {
                            for (var i = 0; i < payload.accountActionGroupDefinitions.length; i++) {
                                var item = payload.accountActionGroupDefinitions[i];
                                ctrl.datasource.push({ Entity: item });
                            }
                        }
                    }

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];
                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var gridActionGroups;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        gridActionGroups = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            gridActionGroups.push(currentItem.Entity);
                        }
                    }
                    return gridActionGroups;
                };

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                    {
                        name: "Edit",
                        clicked: editActionGroup
                    }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editActionGroup(actionGroupObj) {
                var onActionGroupUpdated = function (actionGroup) {
                    var index = ctrl.datasource.indexOf(actionGroupObj);
                    ctrl.datasource[index] = { Entity: actionGroup };
                };
                Retail_BE_AccountBEDefinitionService.editAccountActionGroupDefinition(accountBEDefinitionId, actionGroupObj.Entity, onActionGroupUpdated);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i].Entity;
                    for (var j = i+1; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].Entity.Title == currentItem.Title)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);