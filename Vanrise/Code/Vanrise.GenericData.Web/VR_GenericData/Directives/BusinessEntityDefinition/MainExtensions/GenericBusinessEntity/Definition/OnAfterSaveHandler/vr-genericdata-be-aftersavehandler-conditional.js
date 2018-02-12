"use strict";

app.directive("vrGenericdataBeAftersavehandlerConditional", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ConditionalHandlersGrid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericBusinessEntity/Definition/OnAfterSaveHandler/Templates/ConditionalHandlersGridTemplate.html"
        };

        function ConditionalHandlersGrid($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one handler.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each handler should be unique.";

                    return null;
                };

                ctrl.addConditionalHandler = function () {
                    var onConditionalHandlerAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBEConditionalHandler(onConditionalHandlerAdded, getContext());
                };
              
                ctrl.removeConditionalHandler = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };


                defineMenuActions();

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var handlers;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        handlers = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            handlers.push({
                                ConditionalAfterSaveHandlerItemId: currentItem.ConditionalAfterSaveHandlerItemId,
                                Name: currentItem.Name,
                                Handler: currentItem.Handler,
                                Condition: currentItem.Condition
                            });
                        }
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers.ConditionalAfterSaveHandler, Vanrise.GenericData.MainExtensions",
                        Handlers: handlers
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.settings != undefined && payload.settings.Handlers != undefined) {
                            var data = payload.settings.Handlers;
                            for (var i = 0; i < data.length; i++) {
                                ctrl.datasource.push(data[i]);
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
                    clicked: editConditionalHandler
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editConditionalHandler(conditionalHandlerObj) {
                var onConditionalHandlerUpdated = function (conditionalHandler) {
                    var index = ctrl.datasource.indexOf(conditionalHandlerObj);
                    ctrl.datasource[index] = conditionalHandler;
                };
                VR_GenericData_GenericBEDefinitionService.editGenericBEConditionalHandler(onConditionalHandlerUpdated, conditionalHandlerObj, getContext());
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