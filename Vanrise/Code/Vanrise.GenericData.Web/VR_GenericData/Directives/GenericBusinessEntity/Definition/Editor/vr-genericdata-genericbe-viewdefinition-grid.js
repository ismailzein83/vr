"use strict";

app.directive("vrGenericdataGenericbeViewdefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ViewDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/ViewDefinitionGridTemplate.html"

        };

        function ViewDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one column.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Name in each should be unique.";

                     return null;
                };

                ctrl.addGridView = function () {
                    var onGridViewAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEViewDefinition(onGridViewAdded);
                };
                
                ctrl.removeView = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var views;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        views = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            views.push({
                                GenericBEViewDefinitionId: currentItem.GenericBEViewDefinitionId,
                                Name: currentItem.Name,
                                Settings: currentItem.Settings
                            });
                        }
                    }
                    return views;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        api.clearDataSource();
                        if (payload.genericBEGridViews != undefined) {
                            for (var i = 0; i < payload.genericBEGridViews.length; i++) {
                                var item = payload.genericBEGridViews[i];
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
                    clicked: editView,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editView(viewObj) {
                var onGridViewUpdated = function (view) {
                    var index = ctrl.datasource.indexOf(viewObj);
                    ctrl.datasource[index] = view;
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEViewDefinition(onGridViewUpdated, viewObj);
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