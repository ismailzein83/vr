(function (app) {

    'use strict';

    GenericRuleDefinitionCriteriaFieldManagementDirective.$inject = ['VR_GenericData_GenericRuleDefinitionCriteriaFieldService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRNotificationService'];

    function GenericRuleDefinitionCriteriaFieldManagementDirective(VR_GenericData_GenericRuleDefinitionCriteriaFieldService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRNotificationService) {
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
                };
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
                ctrl.criteriaGroups = [];
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
                        addGroupCriteriaField(addedCriteriaField);
                    };
                    VR_GenericData_GenericRuleDefinitionCriteriaFieldService.addGenericRuleDefinitionCriteriaField(ctrl.criteriaFields, context, onCriteriaFieldAdded);
                };
                ctrl.onDeleteCriteriaField = function (criteriaField) {
                    VRNotificationService.showConfirmation().then(function (confirmed) {
                        if (confirmed) {
                            var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaField.FieldName, 'FieldName');
                            ctrl.criteriaFields.splice(index, 1);
                            deletePriorityDataItem(criteriaField);
                            deleteGroupCriteriaField(criteriaField);
                        }
                    });
                };

                ctrl.onValidateCriteriaFields = function () {
                    if (ctrl.criteriaFields.length == 0) {
                        return 'No fields added';
                    }
                    return null;
                };

                ctrl.addCriteriaGroup = function () {
                    var group = getCriteriaGroup();
                    ctrl.criteriaGroups.push(group);
                };

                ctrl.onDeleteCriteriaGroup = function (criteriaGroup) {
                    var index = UtilsService.getItemIndexByVal(ctrl.criteriaGroups, criteriaGroup.id, 'id');
                    ctrl.criteriaGroups.splice(index, 1);
                };

                ctrl.validateCriteriaGroupFields = function () {
                    if (ctrl.criteriaGroups.length > 0) {
                        var fieldNames = [];
                        for (var i = 0; i < ctrl.criteriaGroups.length; i++) {
                            var criteriaGroup = ctrl.criteriaGroups[i];
                            if (criteriaGroup.selectedFields != undefined && criteriaGroup.selectedFields.length > 0) {
                                for (var k = 0; k < criteriaGroup.selectedFields.length; k++) {
                                    fieldNames.push(criteriaGroup.selectedFields[k].value);
                                }
                            }
                        }
                        while (fieldNames.length > 0) {
                            var nameToValidate = fieldNames[0];
                            fieldNames.splice(0, 1);
                            if (!validateName(nameToValidate, fieldNames)) {
                                return 'Two or more groups have the same field.';
                            }
                        }
                        return null;
                    }
                    function validateName(name, array) {
                        for (var j = 0; j < array.length; j++) {
                            if (array[j] == name)
                                return false;
                        }
                        return true;
                    }
                    return null;
                };
                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.context != undefined)
                        context = payload.context;
                    return loadDataRecordFieldTypeConfigs().then(function () {
                        if (payload != undefined && payload.GenericRuleDefinitionCriteriaFields != undefined) {
                            for (var i = 0; i < payload.GenericRuleDefinitionCriteriaFields.length; i++) {
                                var criteriaField = payload.GenericRuleDefinitionCriteriaFields[i];
                                extendCriteriaFieldObject(criteriaField);
                                ctrl.criteriaFields.push(criteriaField);
                            }
                            loadPriorityDataItemsFromPayload();
                            if (payload.GenericRuleDefinitionCriteriaGroups != undefined && payload.GenericRuleDefinitionCriteriaGroups.length > 0) {
                                loadCriteriaGroupItemsFromPayload();
                            }
                        }
                    });

                    function loadDataRecordFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
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

                    function loadCriteriaGroupItemsFromPayload() {
                        for (var i = 0; i < payload.GenericRuleDefinitionCriteriaGroups.length; i++) {
                            var criteriaGroup = payload.GenericRuleDefinitionCriteriaGroups[i];
                            ctrl.criteriaGroups.push(getCriteriaGroup(criteriaGroup));
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
                            $type: "Vanrise.GenericData.Entities.GenericRuleDefinitionCriteria, Vanrise.GenericData.Entities",
                            Fields: fields,
                            Groups: getCriteriaGroups()
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
                            IgnoreCase: dataItem.IgnoreCase,
                            ValueObjectName: dataItem.ValueObjectName,
                            ValuePropertyName: dataItem.ValuePropertyName,
                            ValueEvaluator: dataItem.ValueEvaluator
                        };
                    }

                    function getCriteriaGroups() {
                        var groups = [];
                        if (ctrl.criteriaGroups.length > 0) {
                            for (var i = 0; i < ctrl.criteriaGroups.length; i++) {
                                var group = ctrl.criteriaGroups[i];
                                groups.push({
                                    GroupId: group.id,
                                    GroupTitle: group.groupTitle,
                                    Fields: getCriteriaGroupFields(group.selectedFields)
                                });
                            }
                        }
                        return groups;
                    }

                    function getCriteriaGroupFields(selectedGroupFields) {
                        var groupFields = [];
                        if (selectedGroupFields != undefined && selectedGroupFields.length > 0) {
                            for (var i = 0; i < selectedGroupFields.length; i++) {
                                groupFields.push({
                                    FieldName: selectedGroupFields[i].value
                                });
                            }
                        }
                        return groupFields;
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
                    updatePriorityDataItem(updatedCriteriaField);
                    updateGroupCriteriaField(updatedCriteriaField);
                };
                VR_GenericData_GenericRuleDefinitionCriteriaFieldService.editGenericRuleDefinitionCriteriaField(criteriaField.FieldName, ctrl.criteriaFields, context, onCriteriaFieldUpdated);
            }

            function extendCriteriaFieldObject(criteriaField) {
                var behaviorTypeObject = UtilsService.getEnum(VR_GenericData_MappingRuleStructureBehaviorTypeEnum, 'value', criteriaField.RuleStructureBehaviorType);
                if (behaviorTypeObject != undefined) {
                    criteriaField.RuleStructureBehaviorTypeDescription = behaviorTypeObject.description;
                }

                var fieldTypeConfigObject = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, criteriaField.FieldType.ConfigId, 'ExtensionConfigurationId');
                if (fieldTypeConfigObject != null) {
                    criteriaField.FieldTypeDescription = fieldTypeConfigObject.Name;
                }
            }

            function addPriorityDataItem(criteriaField) {
                ctrl.priorities.push({
                    FieldName: criteriaField.FieldName
                });
            }

            function addGroupCriteriaField(criteriaField) {
                if (ctrl.criteriaGroups != undefined && ctrl.criteriaGroups.length > 0) {
                    for (var i = 0; i < ctrl.criteriaGroups.length; i++) {
                        var criteriaGroup = ctrl.criteriaGroups[i];
                        criteriaGroup.fields.push({
                            description: criteriaField.Title,
                            value: criteriaField.FieldName});
                    }
                }
            }

            function deletePriorityDataItem(criteriaField) {
                var index = UtilsService.getItemIndexByVal(ctrl.priorities, criteriaField.FieldName, 'FieldName');
                ctrl.priorities.splice(index, 1);
            }
            function deleteGroupCriteriaField(criteriaField) {
                if (ctrl.criteriaGroups != undefined && ctrl.criteriaGroups.length > 0) {
                    for (var i = 0; i < ctrl.criteriaGroups.length; i++) {
                        var criteriaGroup = ctrl.criteriaGroups[i];
                        if (criteriaGroup.fields != undefined) {
                            var fieldIndex = UtilsService.getItemIndexByVal(criteriaGroup.fields, criteriaField.FieldName, 'value');
                            if (fieldIndex > -1)
                                criteriaGroup.fields.splice(fieldIndex, 1);
                        }
                        if (criteriaGroup.selectedFields != undefined) {
                            var selectedFieldIndex = UtilsService.getItemIndexByVal(criteriaGroup.selectedFields, criteriaField.FieldName, 'value');
                            if (selectedFieldIndex > -1)
                                criteriaGroup.selectedFields.splice(selectedFieldIndex, 1);
                        }
                    }
                }
            }
            function updatePriorityDataItem(criteriaField) {
                for (var i = 0; i < ctrl.priorities.length; i++) {
                    var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, ctrl.priorities[i].FieldName, 'FieldName');
                    if (index < 0) {
                        ctrl.priorities[i].FieldName = criteriaField.FieldName;
                    }
                }
            }

            function updateGroupCriteriaField(criteriaField) {
                if (ctrl.criteriaGroups != undefined && ctrl.criteriaGroups.length > 0) {
                    for (var i = 0; i < ctrl.criteriaGroups.length; i++) {
                        var criteriaGroup = ctrl.criteriaGroups[i];
                        if (criteriaGroup.fields != undefined && criteriaGroup.fields.length > 0) {
                            for (var k = 0; k < criteriaGroup.fields.length; k++) {
                                var valueFieldIndex = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaGroup.fields[k].value, 'FieldName');
                                var descriptionFieldIndex = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaGroup.fields[k].description, 'Title');

                                if (valueFieldIndex < 0 || descriptionFieldIndex<0) {
                                    ctrl.criteriaGroups[i].fields[k].description = criteriaField.Title;
                                    ctrl.criteriaGroups[i].fields[k].value = criteriaField.FieldName;
                                }
                            }
                            for (var l = 0; l < criteriaGroup.selectedFields.length; l++) {
                                var valueSelectedFieldIndex = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaGroup.selectedFields[l].value, 'FieldName');
                                var descriptionSelectedFieldIndex = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaGroup.selectedFields[l].description, 'Title');

                                if (valueSelectedFieldIndex < 0 || descriptionSelectedFieldIndex<0) {
                                    ctrl.criteriaGroups[i].selectedFields[l].description = criteriaField.Title;
                                    ctrl.criteriaGroups[i].selectedFields[l].value = criteriaField.FieldName;
                                }
                            }
                        }
                    }
                }
            }
            function getCriteriaGroup(criteriaGroupEntity) {
                var group = {};

                if (criteriaGroupEntity == undefined) {
                    group.id = UtilsService.guid();
                }
                else {
                    group.id = criteriaGroupEntity.GroupId;
                    group.groupTitle = criteriaGroupEntity.GroupTitle;
                }

            
                group.onFieldSelectorReadyDeferred = UtilsService.createPromiseDeferred();

                group.fields = [];
                group.selectedFields = [];

                group.onFieldSelectorReady = function (api) {
                    group.fieldSelectorAPI = api;
                    group.onFieldSelectorReadyDeferred.resolve();
                };

                group.onFieldSelectorReadyDeferred.promise.then(function () {
                    if (ctrl.criteriaFields.length > 0) {
                        for (var i = 0; i < ctrl.criteriaFields.length; i++) {
                            var criteriaField = ctrl.criteriaFields[i];
                            group.fields.push({
                                description: criteriaField.Title,
                                value: criteriaField.FieldName
                            });
                        }
                        if (criteriaGroupEntity != undefined && criteriaGroupEntity.Fields != undefined && criteriaGroupEntity.Fields.length > 0) {
                            for (var j = 0; j < criteriaGroupEntity.Fields.length; j++) {
                                var selectedField = UtilsService.getItemByVal(group.fields, criteriaGroupEntity.Fields[j].FieldName, 'value');
                                if (selectedField != null)
                                    group.selectedFields.push(selectedField);
                            }
                        }
                    }
                });
                return group;
            }
        }
    }

    app.directive('vrGenericdataGenericruledefinitioncriteriafieldManagement', GenericRuleDefinitionCriteriaFieldManagementDirective);

})(app);