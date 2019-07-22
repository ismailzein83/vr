//"use strict";

//app.directive("vrGenericdataGenericbeBeforeinserthandlerListhandlers", ["UtilsService", "VR_GenericData_GenericBEDefinitionService", 
//    function (UtilsService, VR_GenericData_GenericBEDefinitionService) {
//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope:
//            {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new ViewDefinitionGrid($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,

//            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/OnBeforeInsertHandler/Templates/ListHandlersBeforeInsertHandlerTemplate.html"

//        };

//        function ViewDefinitionGrid($scope, ctrl, $attrs) {
//            var context;
//            this.initializeController = initializeController;
//            function initializeController() {
//                ctrl.datasource = [];
//                $scope.scopeModel = {};

//                ctrl.isValid = function () {
//                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
//                        return "You Should add at least one handler.";
//                    return null;
//                };

//                ctrl.addHandler = function () {
//                    var onGenericBEBeforeInsertHandlerAdded = function (addedItem) {
           
//                        ctrl.datasource.push({ Entity: addedItem });
//                    };

//                    VR_GenericData_GenericBEDefinitionService.addGenericBEBeforeInsertHandler(onGenericBEBeforeInsertHandlerAdded, context);
//                };

//                ctrl.disableAddHandler = function () {
//                    if (context == undefined) return true;
//                    return context.getDataRecordTypeId() == undefined;
//                };

//                ctrl.removeHandler = function (dataItem) {
//                    var index = ctrl.datasource.indexOf(dataItem);
//                    ctrl.datasource.splice(index, 1);
//                };

//                defineMenuActions();

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.getData = function () {
//                    var handlers; 
//                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
//                        handlers = [];
//                        for (var i = 0; i < ctrl.datasource.length; i++) {
//                            var currentItem = ctrl.datasource[i].Entity;
//                            handlers.push(currentItem);
//                        }
//                    }
//                    return {
//                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers.ListGenericBEOnBeforeInsertHandler, Vanrise.GenericData.MainExtensions",
//                        Handlers: handlers
//                    }
//                };

//                api.load = function (payload) {
//                    if (payload != undefined) {
//                        context = payload.context;
//                        api.clearDataSource();
//                        if (payload.settings != undefined && payload.settings.Handlers != undefined) {
//                            var handlers = payload.settings.Handlers;
//                            for (var i = 0; i < handlers.length; i++) {
//                                ctrl.datasource.push({ Entity: handlers[i] });
//                            }
//                        }
//                    }
//                };


//                api.clearDataSource = function () {
//                    ctrl.datasource.length = 0;
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function defineMenuActions() {
//                var defaultMenuActions = [
//                    {
//                        name: "Edit",
//                        clicked: editView,
//                    }];

//                $scope.gridMenuActions = function (dataItem) {
//                    return defaultMenuActions;
//                };
//            }

//            function editView(handlerObj) {
//                var onGenericBEBeforeInsertHandlerUpdated = function (handler) {
//                    var index = ctrl.datasource.indexOf(handlerObj);
//                    ctrl.datasource[index] = { Entity: handler };
//                };
//                VR_GenericData_GenericBEDefinitionService.editGenericBEBeforeInsertHandler(onGenericBEBeforeInsertHandlerUpdated, handlerObj.Entity, context);
//            }

//        }

//        return directiveDefinitionObject;

//    }
//]);