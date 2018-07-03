"use strict";

app.directive("vrGenericdataGenericbeGridactiondefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GridActionDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/GridActionDefinitionGridTemplate.html"
        };

        function GridActionDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    //if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                    //    return "You Should add at least one column.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each action should be unique.";

                     return null;
                };

                ctrl.addGridAction= function () {
                    var onGridActionAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEGridActionDefinition(onGridActionAdded, getContext());
                };
                ctrl.disableAddGridAction = function () {
                    if (context == undefined) return true;
                    return context.getActionInfos().length == 0;
                };
                ctrl.removeGridAction = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };


                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var gridActions;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        gridActions = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            gridActions.push({
                                GenericBEGridActionId: currentItem.GenericBEGridActionId,
                                GenericBEActionId: currentItem.GenericBEActionId,
                                Title: currentItem.Title,
                                ReloadGridItem:currentItem.ReloadGridItem,
                                FilterCondition: currentItem.FilterCondition
                            });
                        }
                    } 
                    return gridActions;
                };

                api.load = function (payload) {
                    if (payload != undefined) {

                        context = payload.context;
                        api.clearDataSource();
                        if (payload.genericBEGridActions != undefined) {
                            for (var i = 0; i < payload.genericBEGridActions.length; i++) {
                                var item = payload.genericBEGridActions[i];
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
                    clicked: editGridAction
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editGridAction(gridActionObj) {
                var onGridActionUpdated = function (gridAction) {
                    var index = ctrl.datasource.indexOf(gridActionObj);
                    ctrl.datasource[index] = gridAction;
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEGridActionDefinition(onGridActionUpdated, gridActionObj, getContext());
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
                        if (i != j && ctrl.datasource[j].Title == currentItem.Title)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);