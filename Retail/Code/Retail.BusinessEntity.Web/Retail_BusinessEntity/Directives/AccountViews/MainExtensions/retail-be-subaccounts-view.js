//(function (app) {

//    'use strict';

//    SubAccountsViewDirective.$inject = ['VRCommon_VRObjectTypePropertyDefinitionService', 'UtilsService', 'VRNotificationService'];

//    function SubAccountsViewDirective(VRCommon_VRObjectTypePropertyDefinitionService, UtilsService, VRNotificationService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new SubAccountsViewCtor($scope, ctrl);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {
//                return {
//                    pre: function ($scope, iElem, iAttrs, ctrl) {

//                    }
//                };
//            },
//            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountViews/MainExtensions/Templates/SubAccountsViewTemplate.html'
//        };

//        function SubAccountsViewCtor($scope, ctrl) {
//            this.initializeController = initializeController;

//            var gridAPI;
//            var context;

//            function initializeController() {
//                ctrl.objectTypePropertyDefinitions = [];

//                ctrl.onGridReady = function (api) {
//                    gridAPI = api;
//                    defineAPI();
//                };

//                ctrl.onAddObjectTypePropertyDefinition = function () {
//                    var onObjectTypePropertyDefinitionAdded = function (addedObjectTypePropertyDefinition) {
//                        ctrl.objectTypePropertyDefinitions.push({ Entity: addedObjectTypePropertyDefinition });
//                    };

//                    VRCommon_VRObjectTypePropertyDefinitionService.addObjectTypePropertyDefinition(getObjectTypePropertyDefinitions(), context, onObjectTypePropertyDefinitionAdded);
//                };
//                ctrl.onDeleteObjectTypePropertyDefinition = function (objectTypePropertyDefinition) {
//                    VRNotificationService.showConfirmation().then(function (confirmed) {
//                        if (confirmed) {
//                            var index = UtilsService.getItemIndexByVal(ctrl.objectTypePropertyDefinitions, objectTypePropertyDefinition.Entity.Name, 'Entity.Name');
//                            ctrl.objectTypePropertyDefinitions.splice(index, 1);
//                        }
//                    });
//                };

//                defineMenuActions();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {

//                    ctrl.objectTypePropertyDefinitions.length = 0;

//                    if (payload != undefined && payload.context != undefined)
//                        context = payload.context;

//                    if (payload != undefined && payload.properties != undefined) {
//                        for (var index in payload.properties) {
//                            if (index != "$type") {
//                                var property = payload.properties[index];
//                                ctrl.objectTypePropertyDefinitions.push({ Entity: property });
//                            }
//                        }
//                    }
//                };

//                api.getData = function () {

//                    var properties;
//                    if (ctrl.objectTypePropertyDefinitions.length > 0) {
//                        properties = {};
//                        for (var i = 0; i < ctrl.objectTypePropertyDefinitions.length; i++) {
//                            var objectTypePropertyDefinition = ctrl.objectTypePropertyDefinitions[i].Entity;
//                            properties[objectTypePropertyDefinition.Name] = objectTypePropertyDefinition;
//                        }
//                    }
//                    return properties;
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            function defineMenuActions() {
//                ctrl.menuActions = [{
//                    name: 'Edit',
//                    clicked: editObjectTypePropertyDefinition
//                }];
//            }
//            function editObjectTypePropertyDefinition(objectTypePropertyDefinition) {
//                var onObjectTypePropertyDefinitionUpdated = function (updatedObjectTypePropertyDefinition) {
//                    var index = UtilsService.getItemIndexByVal(ctrl.objectTypePropertyDefinitions, objectTypePropertyDefinition.Entity.Name, 'Entity.Name');
//                    ctrl.objectTypePropertyDefinitions[index] = { Entity: updatedObjectTypePropertyDefinition };
//                };

//                VRCommon_VRObjectTypePropertyDefinitionService.editObjectTypePropertyDefinition(objectTypePropertyDefinition.Entity.Name, getObjectTypePropertyDefinitions(), context, onObjectTypePropertyDefinitionUpdated);
//            }
//            function getObjectTypePropertyDefinitions() {

//                var fields = [];
//                for (var i = 0; i < ctrl.objectTypePropertyDefinitions.length; i++) {
//                    fields.push(ctrl.objectTypePropertyDefinitions[i].Entity);
//                }
//                return fields;
//            }
//        }
//    }

//    app.directive('retailBeSubaccountsView', SubAccountsViewDirective);

//})(app);