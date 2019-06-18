//"use strict";

//app.directive("vrGenericdataGenericbeCustomactiondefinitionGrid", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
//    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

//        var directiveDefinitionObject = {

//            restrict: "E",
//            scope:
//            {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;

//                var ctor = new CustomActionDefinitionGrid($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Editor/Templates/CustomActionDefinitionGridTemplate.html"

//        };

//        function CustomActionDefinitionGrid($scope, ctrl, $attrs) {

//            var gridAPI;
//            var context;

//            this.initializeController = initializeController;
//            function initializeController() {
//                ctrl.datasource = [];
//                ctrl.isValid = function () {
                    
//                    if (ctrl.datasource.length > 0 && checkDuplicateName())
//                        return "Name in each custom action should be unique.";

//                     return null;
//                };

//                ctrl.addCustomAction= function () {
//                    var onCustomActionAdded = function (addedItem) {
//                        ctrl.datasource.push(addedItem);
//                    };

//                    VR_GenericData_GenericBEDefinitionService.addGenericBECustomActionDefinition(onCustomActionAdded, getContext());
//                };
//                ctrl.disableAddCustomAction = function () {
//                    if (context == undefined) return true;
//                    return context.getDataRecordTypeId() == undefined;
//                };
//                ctrl.removeCustomAction = function (dataItem) {
//                    var index = ctrl.datasource.indexOf(dataItem);
//                    ctrl.datasource.splice(index, 1);
//                };


//                defineMenuActions();

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.getData = function () {
//                    var customActions;
//                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
//                        customActions = [];
//                        for (var i = 0; i < ctrl.datasource.length; i++) {
//                            var currentItem = ctrl.datasource[i]; 
//                            customActions.push({
//                                GenericBECustomActionId: currentItem.GenericBECustomActionId,
//                                Title: currentItem.Title,
//                                ButtonType: currentItem.ButtonType,
//								Settings: currentItem.Settings
//                            });
//                        }
//                    }
//                    return customActions;
//                };

//                api.load = function (payload) {
//                    if (payload != undefined) {
//                        context = payload.context;
//                        api.clearDataSource();
//                        if (payload.genericBECustomActions != undefined) {
//                            for (var i = 0; i < payload.genericBECustomActions.length; i++) {
//                                var item = payload.genericBECustomActions[i];
//                                ctrl.datasource.push(item);
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
//                {
//                    name: "Edit",
//                        clicked: editCustomAction
//                }];

//                $scope.gridMenuActions = function (dataItem) {
//                    return defaultMenuActions;
//                };
//            }

//			function editCustomAction(customActionObj) {
//                var onCustomActionUpdated = function (customAction) {
//                    var index = ctrl.datasource.indexOf(customActionObj);
//                    ctrl.datasource[index] = customAction;
//                };
//                VR_GenericData_GenericBEDefinitionService.editGenericBECustomActionDefinition(onCustomActionUpdated, customActionObj, getContext());
//            }
//            function getContext() {
//                var currentContext = context;
//                if (currentContext == undefined)
//                    currentContext = {};
//                return currentContext;
//            }

//            function checkDuplicateName() {
//                for (var i = 0; i < ctrl.datasource.length; i++) {
//                    var currentItem = ctrl.datasource[i];
//                    for (var j = 0; j < ctrl.datasource.length; j++) {
//                        if (i != j && ctrl.datasource[j].Title == currentItem.Title)
//                            return true;
//                    }
//                }
//                return false;
//            }
//        }

//        return directiveDefinitionObject;

//    }
//]);