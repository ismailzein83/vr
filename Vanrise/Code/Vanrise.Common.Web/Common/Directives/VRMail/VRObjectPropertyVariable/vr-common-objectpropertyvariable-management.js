(function (app) {

    'use strict';

    ObjectPropertyVariableManagementDirective.$inject = ['VRCommon_ObjectPropertyVariableService', 'UtilsService', 'VRNotificationService'];

    function ObjectPropertyVariableManagementDirective(VRCommon_ObjectPropertyVariableService, UtilsService, VRNotificationService) {
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
            templateUrl: '/Client/Modules/Common/Directives/VRMail/VRObjectPropertyVariable/Templates/VRObjectPropertyVariableManagementTemplate.html'
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
                        //extendObjectPropertyVariableObject(addedObjectPropertyVariable);
                        ctrl.objectPropertyVariables.push(addedObjectPropertyVariable);

                    };
                    VRCommon_ObjectPropertyVariableService.addObjectPropertyVariable(context, onObjectPropertyVariableAdded);
                };
                ctrl.onDeleteObjectPropertyVariable = function (objectPropertyVariable) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.objectPropertyVariables, objectPropertyVariable.FieldName, 'FieldName');
                            ctrl.objectPropertyVariables.splice(index, 1);
                            deletePriorityDataItem(objectPropertyVariable);
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

                    if (payload != undefined && payload.objectPropertyVariables != undefined) {
                        for (var i = 0; i < payload.objectPropertyVariables.length; i++) {
                            var objectPropertyVariable = payload.objectPropertyVariables[i];
                            //extendObjectPropertyVariableObject(objectPropertyVariable);
                            ctrl.objectPropertyVariables.push(objectPropertyVariable);
                        }
                    }              
                };

                api.getData = function () {
                    var data = null;

                    if (ctrl.objectPropertyVariables.length > 0) {
                        var fields = [];
                        for (var i = 0; i < ctrl.objectPropertyVariables.length; i++) {
                            fields.push(getMappedObjectPropertyVariable(ctrl.objectPropertyVariables[i]));
                        }
                        data = {
                            Fields: fields
                        };
                    }

                    return data;

                    function getMappedObjectPropertyVariable(dataItem) {
                        var index = UtilsService.getItemIndexByVal(ctrl.priorities, dataItem.FieldName, 'FieldName');
                        var priority = index + 1;

                        return {
                            FieldName: dataItem.FieldName,
                            Title: dataItem.Title,
                            FieldType: dataItem.FieldType,
                            RuleStructureBehaviorType: dataItem.RuleStructureBehaviorType,
                            Priority: priority,
                            ShowInBasicSearch: dataItem.ShowInBasicSearch,
                            ValueObjectName: dataItem.ValueObjectName,
                            ValueEvaluator: dataItem.ValueEvaluator
                        };
                    }
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
                    extendObjectPropertyVariableObject(updatedObjectPropertyVariable);
                    var index = UtilsService.getItemIndexByVal(ctrl.objectPropertyVariables, objectPropertyVariable.FieldName, 'FieldName');
                    ctrl.objectPropertyVariables[index] = updatedObjectPropertyVariable;
                };
                VR_GenericData_GenericRuleDefinitionObjectPropertyVariableService.editObjectPropertyVariable(objectPropertyVariable.FieldName, ctrl.objectPropertyVariables, context, onObjectPropertyVariableUpdated);
            }

            function extendObjectPropertyVariableObject(objectPropertyVariable) {
                var behaviorTypeObject = UtilsService.getEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum, 'value', objectPropertyVariable.RuleStructureBehaviorType);
                if (behaviorTypeObject != undefined) {
                    objectPropertyVariable.RuleStructureBehaviorTypeDescription = behaviorTypeObject.description;
                }

                var fieldTypeConfigObject = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, objectPropertyVariable.FieldType.ConfigId, 'DataRecordFieldTypeConfigId');
                if (fieldTypeConfigObject != null) {
                    objectPropertyVariable.FieldTypeDescription = fieldTypeConfigObject.Name;
                }
            }
        }
    }

    app.directive('vrCommonObjectpropertyvariableManagement', ObjectPropertyVariableManagementDirective);

})(app);