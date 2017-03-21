"use strict";

app.directive("vrAccountbalanceAccounttypeGridcolumns", ["UtilsService", "VRNotificationService", "VR_AccountBalance_AccountTypeService",
    function (UtilsService, VRNotificationService, VR_AccountBalance_AccountTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new GridColumns($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeGridColumns.html"

        };

        function GridColumns($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];

                ctrl.addGridColumn = function () {
                    var onGridColumnAdded = function (source) {
                        ctrl.datasource.push({ Entity: source });
                    };
                    VR_AccountBalance_AccountTypeService.addGridColumn(onGridColumnAdded, getContext());
                };

                ctrl.removeGridColumn = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var gridColumns;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        gridColumns = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            gridColumns.push(currentItem.Entity);
                        }
                    }
                    return gridColumns;
                };

                api.load = function (payload) {

                    if (payload != undefined) {
                        context = payload.context;

                        if (payload.gridColumns != undefined) {
                            for (var i = 0; i < payload.gridColumns.length; i++) {
                                var gridColumn = payload.gridColumns[i];
                                ctrl.datasource.push({ Entity: gridColumn });
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
                    clicked: editGridColumn,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editGridColumn(gridColumnObj) {
                var onGridColumnUpdated = function (gridColumn) {
                    var index = ctrl.datasource.indexOf(gridColumnObj);
                    ctrl.datasource[index] = { Entity: gridColumn };
                };
                VR_AccountBalance_AccountTypeService.editGridColumn(gridColumnObj.Entity, onGridColumnUpdated, getContext());
            }
            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);