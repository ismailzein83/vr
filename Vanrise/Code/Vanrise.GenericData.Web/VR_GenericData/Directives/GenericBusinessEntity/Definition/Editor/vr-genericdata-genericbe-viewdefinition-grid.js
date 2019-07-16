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
            var context;
            var gridAPI;
            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                   
                     return null;
                };

                ctrl.addGridView = function () {
                    var onGridViewAdded = function (addedItem, reopen) {
                        ctrl.datasource.push(addedItem);
                        if (reopen) {
                            VR_GenericData_GenericBEDefinitionService.addGenericBEViewDefinition(onGridViewAdded, context);
                        }
                    };
                    
                    VR_GenericData_GenericBEDefinitionService.addGenericBEViewDefinition(onGridViewAdded, context);
                };
                  ctrl.disableAddGridView = function () {
                    if (context == undefined) return true;
                    return context.getDataRecordTypeId() == undefined;
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
                                Settings: currentItem.Settings,
								Condition: currentItem.Condition,
								TextResourceKey: currentItem.TextResourceKey
                            });
                        }
                    }
                    return views;
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
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
                VR_GenericData_GenericBEDefinitionService.editGenericBEViewDefinition(onGridViewUpdated, viewObj,context);
            }

        }

        return directiveDefinitionObject;

    }
]);