(function (app) {

    'use strict';

    GenericRuleDefinitionCriteriaDirective.$inject = ['VR_GenericData_GenericRuleDefinitionCriteriaFieldService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaDirective(VR_GenericData_GenericRuleDefinitionCriteriaFieldService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRNotificationService) {

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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRuleDefinition/Criteria/Templates/GenericRuleDefinitionCriteriaTemplate.html'
        };

        function GenericRuleDefinitionCriteria($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var dataRecordFieldTypeConfigs;

            function initializeController() {
                ctrl.criteriaFields = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
                ctrl.addCriteriaField = function () {
                    var onGenericRuleDefinitionCriteriaFieldAdded = function (addedCriteriaField) {
                        extendCriteriaFieldObject(addedCriteriaField);
                        ctrl.criteriaFields.push(addedCriteriaField);
                    };
                    VR_GenericData_GenericRuleDefinitionCriteriaFieldService.addGenericRuleDefinitionCriteriaField(ctrl.criteriaFields, onGenericRuleDefinitionCriteriaFieldAdded);
                };
                ctrl.validateCriteriaFields = function () {
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
                    return loadDataRecordFieldTypeConfigs().then(function () {
                        if (payload != undefined) {
                            for (var i = 0; i < payload.GenericRuleDefinitionCriteriaFields.length; i++) {
                                extendCriteriaFieldObject(payload.GenericRuleDefinitionCriteriaFields[i]);
                                ctrl.criteriaFields.push(payload.GenericRuleDefinitionCriteriaFields[i]);
                            }
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
                };

                api.getData = function () {
                    var data = null;
                    
                    if (ctrl.criteriaFields.length > 0) {
                        var fields = [];
                        for (var i = 0; i < ctrl.criteriaFields.length; i++) {
                            ctrl.criteriaFields[i].Priority = i + 1;
                            fields.push(ctrl.criteriaFields[i]);
                        }
                        data = {
                            Fields: fields
                        };
                    }

                    return data;
                };

                return api;
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editCriteriaField
                }, {
                    name: 'Delete',
                    clicked: deleteCriteriaField
                }];
            }

            function editCriteriaField(criteriaField) {
                var onGenericRuleDefinitionCriteriaFieldUpdated = function (updatedCriteriaField) {
                    extendCriteriaFieldObject(updatedCriteriaField);
                    var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaField.FieldName, 'FieldName');
                    ctrl.criteriaFields[index] = updatedCriteriaField;
                };
                VR_GenericData_GenericRuleDefinitionCriteriaFieldService.editGenericRuleDefinitionCriteriaField(criteriaField.FieldName, ctrl.criteriaFields, onGenericRuleDefinitionCriteriaFieldUpdated);
            }

            function extendCriteriaFieldObject(criteriaField) {
                var behaviorTypeObject = UtilsService.getEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum, 'value', criteriaField.RuleStructureBehaviorType);
                if (behaviorTypeObject != undefined) {
                    criteriaField.RuleStructureBehaviorTypeDescription = behaviorTypeObject.description;
                }
                var fieldTypeConfigObject = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, criteriaField.FieldType.ConfigId, 'DataRecordFieldTypeConfigId');
                if (fieldTypeConfigObject != undefined) {
                    criteriaField.FieldTypeDescription = fieldTypeConfigObject.Name;
                }
            }

            function deleteCriteriaField(criteriaField) {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaField.FieldName, 'FieldName');
                        if (index != undefined && index >= 0) {
                            ctrl.criteriaFields.splice(index, 1);
                            VRNotificationService.showSuccess('Criteria field deleted');
                        }
                        else {
                            VRNotificationService.showError('Criteria field was not deleted');
                        }
                    }
                });
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitioncriteria', GenericRuleDefinitionCriteriaDirective);

})(app);