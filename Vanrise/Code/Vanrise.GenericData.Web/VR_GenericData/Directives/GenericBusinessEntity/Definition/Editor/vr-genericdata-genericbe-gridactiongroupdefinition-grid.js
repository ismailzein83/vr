"use strict";

app.directive("vrGenericdataGenericbeGridactiongroupdefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GridActionGroupDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/GridActionGroupDefinitionGridTemplate.html"
        };

        function GridActionGroupDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each action should be unique.";

                    return null;
                };

                ctrl.addGridActionGroup = function () {
                    var onGridActionGroupAdded = function (addedItem) {
                        ctrl.datasource.push({ Entity: addedItem });
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEGridActionGroupDefinition(onGridActionGroupAdded, getContext());
                };

                ctrl.disableAddGridActionGroup = function () {
                    if (context == undefined) return true;
                    return context.getActionGroupInfos().length == 0;
                };

                ctrl.removeGridActionGroup = function (dataItem) {
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

                        context = payload.context;
                        api.clearDataSource();
                        if (payload.genericBEGridActionGroups != undefined) {
                            for (var i = 0; i < payload.genericBEGridActionGroups.length; i++) {
                                var item = payload.genericBEGridActionGroups[i];
                                ctrl.datasource.push({ Entity: item });
                            }
                        }
                    }
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
                        clicked: editGridActionGroup
                    }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editGridActionGroup(gridActionGroupObj) {
                var onGridActionGroupUpdated = function (gridActionGroup) {
                    var index = ctrl.datasource.indexOf(gridActionGroupObj);
                    ctrl.datasource[index] = { Entity: gridActionGroup };
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEGridActionGroupDefinition(onGridActionGroupUpdated, gridActionGroupObj.Entity, getContext());
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
                    for (var j = i + 1; j < ctrl.datasource.length; j++) {
                        if (ctrl.datasource[j].Entity.Title == currentItem.Title)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);