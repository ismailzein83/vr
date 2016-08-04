(function (app) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldManagementDirective.$inject = ['VR_GenericData_GenericRuleDefinitionCriteriaFieldService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldManagementDirective(VR_GenericData_GenericRuleDefinitionCriteriaFieldService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericRuleDefinitionCriteria = new GenericRuleDefinitionCriteria($scope, ctrl);
                genericRuleDefinitionCriteria.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRuleDefinition/CriteriaField/Templates/GenericRuleDefinitionCriteriaFieldManagementTemplate.html'
        };

        function GenericRuleDefinitionCriteria($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var dataRecordFieldTypeConfigs;
            var context;

            function initializeController() {
                ctrl.criteriaFields = [];
                ctrl.priorities = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.onAddCriteriaField = function () {
                    var onCriteriaFieldAdded = function (addedCriteriaField) {
                        extendCriteriaFieldObject(addedCriteriaField);
                        ctrl.criteriaFields.push(addedCriteriaField);
                        addPriorityDataItem(addedCriteriaField);
                    };
                    VR_GenericData_GenericRuleDefinitionCriteriaFieldService.addGenericRuleDefinitionCriteriaField(ctrl.criteriaFields, context, onCriteriaFieldAdded);
                };
                ctrl.onDeleteCriteriaField = function (criteriaField) { 
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaField.FieldName, 'FieldName');
                            ctrl.criteriaFields.splice(index, 1);
                            deletePriorityDataItem(criteriaField);
                        }
                    });
                }

                ctrl.onValidateCriteriaFields = function () {
                    if (ctrl.criteriaFields.length == 0) {
                        return 'No fields added';
                    }
                    return null;
                };

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {

                    if (payload != undefined && payload.Context != undefined)
                        context = payload.Context;

                    return loadDataRecordFieldTypeConfigs().then(function () {
                        if (payload != undefined && payload.GenericRuleDefinitionCriteriaFields != undefined) {
                            for (var i = 0; i < payload.GenericRuleDefinitionCriteriaFields.length; i++) {
                                var criteriaField = payload.GenericRuleDefinitionCriteriaFields[i];
                                extendCriteriaFieldObject(criteriaField);
                                ctrl.criteriaFields.push(criteriaField);
                            }
                            loadPriorityDataItemsFromPayload();
                        }
                    });

                    function loadDataRecordFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                            if (response != null) {
                                dataRecordFieldTypeConfigs = [];
                                for (var i = 0; i < response.length; i++) {
                                    dataRecordFieldTypeConfigs.push(response[i]);
                                }
                            }
                        });
                    }

                    function loadPriorityDataItemsFromPayload() {
                        var clonedCriteriaFields = UtilsService.cloneObject(ctrl.criteriaFields, true);
                        clonedCriteriaFields.sort(function (first, second) {
                            return first.Priority - second.Priority;
                        });
                        for (var j = 0; j < clonedCriteriaFields.length; j++) {
                            addPriorityDataItem(clonedCriteriaFields[j]);
                        }
                    }
                };

                api.getData = function () {
                    var data = null;
                    
                    if (ctrl.criteriaFields.length > 0) {
                        var fields = [];
                        for (var i = 0; i < ctrl.criteriaFields.length; i++) {
                            fields.push(getMappedCriteriaField(ctrl.criteriaFields[i]));
                        }
                        data = {
                            Fields: fields
                        };
                    }

                    return data;

                    function getMappedCriteriaField(dataItem) {
                        var index = UtilsService.getItemIndexByVal(ctrl.priorities, dataItem.FieldName, 'FieldName');
                        var priority = index + 1;

                        return {
                            FieldName: dataItem.FieldName,
                            Title: dataItem.Title,
                            FieldType: dataItem.FieldType,
                            RuleStructureBehaviorType: dataItem.RuleStructureBehaviorType,
                            Priority: priority,
                            ShowInBasicSearch: dataItem.ShowInBasicSearch,
                            ValueEvaluator: dataItem.ValueEvaluator
                        };
                    }
                };

                return api;
            }
            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editCriteriaField
                }];
            }

            function editCriteriaField(criteriaField) {
                var onCriteriaFieldUpdated = function (updatedCriteriaField) {
                    extendCriteriaFieldObject(updatedCriteriaField);
                    var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaField.FieldName, 'FieldName');
                    ctrl.criteriaFields[index] = updatedCriteriaField;
                };
                VR_GenericData_GenericRuleDefinitionCriteriaFieldService.editGenericRuleDefinitionCriteriaField(criteriaField.FieldName, ctrl.criteriaFields, onCriteriaFieldUpdated);
            }

            function extendCriteriaFieldObject(criteriaField) {
                var behaviorTypeObject = UtilsService.getEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum, 'value', criteriaField.RuleStructureBehaviorType);
                if (behaviorTypeObject != undefined) {
                    criteriaField.RuleStructureBehaviorTypeDescription = behaviorTypeObject.description;
                }

                var fieldTypeConfigObject = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, criteriaField.FieldType.ConfigId, 'DataRecordFieldTypeConfigId');
                if (fieldTypeConfigObject != null) {
                    criteriaField.FieldTypeDescription = fieldTypeConfigObject.Name;
                }
            }

            function addPriorityDataItem(criteriaField) {
                ctrl.priorities.push({
                    FieldName: criteriaField.FieldName
                });
            }
            function deletePriorityDataItem(criteriaField) {
                var index = UtilsService.getItemIndexByVal(ctrl.priorities, criteriaField.FieldName, 'FieldName');
                ctrl.priorities.splice(index, 1);
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitioncriteriafieldManagement', GenericRuleDefinitionCriteriaFieldManagementDirective);

})(app);