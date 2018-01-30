"use strict";

app.directive("vrGenericdataGenericBeColumndefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ColumnDefinitionGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/Editor/Templates/ColumnDefinitionGridTemplate.html"

        };

        function ColumnDefinitionGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0)
                        return null;
                    return "You Should add at least one column.";
                };

                ctrl.addGridColumn = function () {
                    var onGridColumnAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEColumnDefinition(onGridColumnAdded, getContext());
                };
                ctrl.disableAddGridColumn = function () {
                    if (context == undefined) return true;
                    return context.getDataRecordTypeId() == undefined;
                };
                ctrl.removeColumn = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var columns;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        columns = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            columns.push({
                                FieldName: currentItem.FieldName,
                                FieldTitle: currentItem.FieldTitle,
                                GridColumnSettings: currentItem.GridColumnSettings
                            });
                        }
                    }
                    return columns;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.columnDefinitions != undefined) {
                            for (var i = 0; i < payload.columnDefinitions.length; i++) {
                                var item = payload.columnDefinitions[i];
                                ctrl.datasource.push(item);
                            }
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editColumn,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editColumn(columnObj) {
                var onGridColumnUpdated = function (column) {
                    var index = ctrl.datasource.indexOf(columnObj);
                    ctrl.datasource[index] = column;
                };

                VR_GenericData_GenericBEDefinitionService.editGenericBEColumnDefinition(columnObj, onGridColumnUpdated, getContext());
            }
            function getContext() {
                return context;
            }
        }

        return directiveDefinitionObject;

    }
]);