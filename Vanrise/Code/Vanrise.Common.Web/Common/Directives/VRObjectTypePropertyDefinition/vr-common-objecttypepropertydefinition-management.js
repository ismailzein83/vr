(function (app) {

    'use strict';

    ObjectTypePropertyDefinitionManagementDirective.$inject = ['VRCommon_VRObjectTypePropertyDefinitionService', 'UtilsService', 'VRNotificationService'];

    function ObjectTypePropertyDefinitionManagementDirective(VRCommon_VRObjectTypePropertyDefinitionService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var objectTypePropertyDefinition = new ObjectTypePropertyDefinition($scope, ctrl);
                objectTypePropertyDefinition.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/Common/Directives/VRObjectTypePropertyDefinition/Templates/VRObjectTypePropertyDefinitionManagementTemplate.html'
        };

        function ObjectTypePropertyDefinition($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var context;

            function initializeController() {
                ctrl.objectTypePropertyDefinitions = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddObjectTypePropertyDefinition = function () {
                    var onObjectTypePropertyDefinitionAdded = function (addedObjectTypePropertyDefinition) {
                        ctrl.objectTypePropertyDefinitions.push(addedObjectTypePropertyDefinition);
                    };
                    VRCommon_VRObjectTypePropertyDefinitionService.addObjectTypePropertyDefinition(ctrl.objectTypePropertyDefinitions, context, onObjectTypePropertyDefinitionAdded);
                };
                ctrl.onDeleteObjectTypePropertyDefinition = function (objectTypePropertyDefinition) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.objectTypePropertyDefinitions, objectTypePropertyDefinition.Name, 'Name');
                            ctrl.objectTypePropertyDefinitions.splice(index, 1);
                        }
                    });
                }

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.objectTypePropertyDefinitions = [];

                    if (payload != undefined && payload.context != undefined)
                        context = payload.context;

                    if (payload != undefined && payload.properties != undefined) {
                        for (var index in payload.properties) 
                            if (index != "$type") {
                                var property = payload.properties[index];
                                ctrl.objectTypePropertyDefinitions.push(property);
                            }          
                    }
                };

                api.getData = function () {

                    if (ctrl.objectTypePropertyDefinitions.length > 0) {
                        var properties = {};
                        for (var i = 0; i < ctrl.objectTypePropertyDefinitions.length; i++) {
                            var objectTypePropertyDefinition = ctrl.objectTypePropertyDefinitions[i];
                            properties[objectTypePropertyDefinition.Name] = objectTypePropertyDefinition;
                        }
                    }

                    return properties;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editObjectTypePropertyDefinition
                }];
            }

            function editObjectTypePropertyDefinition(objectTypePropertyDefinition) {
                var onObjectTypePropertyDefinitionUpdated = function (updatedObjectTypePropertyDefinition) {
                    var index = UtilsService.getItemIndexByVal(ctrl.objectTypePropertyDefinitions, objectTypePropertyDefinition.Name, 'Name');
                    ctrl.objectTypePropertyDefinitions[index] = updatedObjectTypePropertyDefinition;
                };
                VRCommon_VRObjectTypePropertyDefinitionService.editObjectTypePropertyDefinition(objectTypePropertyDefinition.Name, ctrl.objectTypePropertyDefinitions, context, onObjectTypePropertyDefinitionUpdated);
            }
        }
    }

    app.directive('vrCommonObjecttypepropertydefinitionManagement', ObjectTypePropertyDefinitionManagementDirective);

})(app);