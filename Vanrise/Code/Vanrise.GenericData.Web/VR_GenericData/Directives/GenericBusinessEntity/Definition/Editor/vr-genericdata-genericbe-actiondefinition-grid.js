"use strict";

app.directive("vrGenericdataGenericbeActiondefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ActionDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/ActionDefinitionGridTemplate.html"

        };

        function ActionDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    //if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                    //    return "You Should add at least one column.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Name in each action should be unique.";

                     return null;
                };

                ctrl.addAction= function () {
                    var onActionAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEActionDefinition(onActionAdded, getContext());
                };
                ctrl.disableAddAction = function () {
                    if (context == undefined) return true;
                    return context.getDataRecordTypeId() == undefined;
                };
                ctrl.removeAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };


                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var actions;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        actions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            actions.push({
                                GenericBEActionId: currentItem.GenericBEActionId,
                                Name: currentItem.Name,
                                Settings: currentItem.Settings
                            });
                        }
                    }
                    return actions;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.genericBEActions != undefined) {
                            for (var i = 0; i < payload.genericBEActions.length; i++) {
                                var item = payload.genericBEActions[i];
                                ctrl.datasource.push(item);
                            }
                        }
                    }
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
                    clicked: editAction
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editAction(actionObj) {
                var onActionUpdated = function (action) {
                    var index = ctrl.datasource.indexOf(actionObj);
                    ctrl.datasource[index] = action;
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEActionDefinition(onActionUpdated, actionObj, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].Name == currentItem.Name)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);