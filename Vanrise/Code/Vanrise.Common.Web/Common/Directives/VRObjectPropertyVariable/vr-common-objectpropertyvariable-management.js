(function (app) {

    'use strict';

    ObjectPropertyVariableManagementDirective.$inject = ['VRCommon_VRObjectPropertyVariableService', 'UtilsService', 'VRNotificationService'];

    function ObjectPropertyVariableManagementDirective(VRCommon_VRObjectPropertyVariableService, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var objectPropertyVariable = new ObjectPropertyVariable($scope, ctrl);
                objectPropertyVariable.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/Common/Directives/VRObjectPropertyVariable/Templates/VRObjectPropertyVariableManagementTemplate.html'
        };

        function ObjectPropertyVariable($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var dataRecordFieldTypeConfigs;
            var context;

            function initializeController() {
                ctrl.objectPropertyVariables = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.onAddObjectPropertyVariable = function () {
                    var onObjectPropertyVariableAdded = function (addedObjectPropertyVariable) {
                        ctrl.objectPropertyVariables.push(addedObjectPropertyVariable);
                    };
                    VRCommon_VRObjectPropertyVariableService.addObjectPropertyVariable(ctrl.objectPropertyVariables, context, onObjectPropertyVariableAdded);
                };
                ctrl.onDeleteObjectPropertyVariable = function (objectPropertyVariable) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.objectPropertyVariables, objectPropertyVariable.VariableName, 'VariableName');
                            ctrl.objectPropertyVariables.splice(index, 1);
                        }
                    });
                }

                ctrl.onValidateObjectPropertyVariables = function () {
                    if (ctrl.objectPropertyVariables.length == 0) {
                        return 'No fields added';
                    }
                    return null;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.context != undefined)
                        context = payload.context;

                    if (payload != undefined && payload.variables != undefined) {
                        for (var i = 0; i < payload.variables.length; i++) {
                            var variable = payload.variables[i];
                            ctrl.objectPropertyVariables.push(variable);
                        }
                    }              
                };

                api.getData = function () {

                    if (ctrl.objectPropertyVariables.length > 0) {
                        var variables = [];
                        for (var i = 0; i < ctrl.objectPropertyVariables.length; i++) 
                            variables.push(ctrl.objectPropertyVariables[i]);
                    }

                    return variables;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editObjectPropertyVariable
                }];
            }

            function editObjectPropertyVariable(objectPropertyVariable) {
                var onObjectPropertyVariableUpdated = function (updatedObjectPropertyVariable) {
                    var index = UtilsService.getItemIndexByVal(ctrl.objectPropertyVariables, objectPropertyVariable.VariableName, 'VariableName');
                    ctrl.objectPropertyVariables[index] = updatedObjectPropertyVariable;
                };
                VRCommon_VRObjectPropertyVariableService.editObjectPropertyVariable(objectPropertyVariable.VariableName, ctrl.objectPropertyVariables, context, onObjectPropertyVariableUpdated);
            }
        }
    }

    app.directive('vrCommonObjectpropertyvariableManagement', ObjectPropertyVariableManagementDirective);

})(app);