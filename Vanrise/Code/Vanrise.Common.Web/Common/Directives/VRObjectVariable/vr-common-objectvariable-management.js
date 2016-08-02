(function (app) {

    'use strict';

    VRObjectVariableManagementDirective.$inject = ['VRCommon_ObjectVariableService', 'VR_GenericData_DataRecordFieldTypeConfigAPIService', 'VR_GenericData_MappingRuleStructureBehaviorTypeEnum', 'UtilsService', 'VRNotificationService'];

    function VRObjectVariableManagementDirective(VRCommon_ObjectVariableService, VR_GenericData_DataRecordFieldTypeConfigAPIService, VR_GenericData_MappingRuleStructureBehaviorTypeEnum, UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vrObjectVariable = new VRObjectVariable($scope, ctrl);
                vrObjectVariable.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: '/Client/Modules/Common/Directives/VRObjectVariable/Templates/VRObjectVariableManagementTemplate.html'
        };

        function VRObjectVariable($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var dataRecordFieldTypeConfigs;

            function initializeController() {
                ctrl.objectVariables = [];
                //ctrl.priorities = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.addObjectVariable = function () {
                    var onObjectVariableAdded = function (addedCriteriaField) {
                        //extendCriteriaFieldObject(addedCriteriaField);
                        ctrl.objectVariables.push(addedCriteriaField);
                        //addPriorityDataItem(addedCriteriaField);
                    };
                    VRCommon_ObjectVariableService.addVRObjectVariable(onObjectVariableAdded);
                };
                ctrl.validateCriteriaFields = function () {
                    if (ctrl.objectVariables.length == 0) {
                        return 'No fields added';
                    }
                    return null;
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    //return loadDataRecordFieldTypeConfigs().then(function () {

                    //api.load = function (query) {
                    //    return gridAPI.retrieveData(query);
                    //};
                        
                    if (payload != undefined) {
                        for (var i = 0; i < payload.Objects.length; i++) {
                            var objectVariable = payload.Objects[i];
                            //extendCriteriaFieldObject(criteriaField);
                            ctrl.objectVariables.push(objectVariable);
                        }
                        //loadPriorityDataItemsFromPayload();
                    }

                    //});

                    //function loadDataRecordFieldTypeConfigs() {
                    //    return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
                    //        if (response != null) {
                    //            dataRecordFieldTypeConfigs = [];
                    //            for (var i = 0; i < response.length; i++) {
                    //                dataRecordFieldTypeConfigs.push(response[i]);
                    //            }
                    //        }
                    //    });
                    //}

                    //function loadPriorityDataItemsFromPayload() {
                    //    var clonedCriteriaFields = UtilsService.cloneObject(ctrl.criteriaFields, true);
                    //    clonedCriteriaFields.sort(function (first, second) {
                    //        return first.Priority - second.Priority;
                    //    });
                    //    for (var j = 0; j < clonedCriteriaFields.length; j++) {
                    //        addPriorityDataItem(clonedCriteriaFields[j]);
                    //    }
                    //}
                };

                api.getData = function () {
                    var data;
                    var dataItem

                    if (ctrl.objectVariables.length > 0) {
                        data = {};

                        for (var i = 0; i < ctrl.objectVariables.length; i++) {
                            dataItem = ctrl.objectVariables[i];
                            data[dataItem.ObjectName] = getMappedObjectVariable(dataItem);
                        }
                    }

                    function getMappedObjectVariable(dataItem) {
                        return {
                            ObjectName: dataItem.ObjectName,
                            ObjectType: dataItem.ObjectType
                        };
                    }

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editObjectVariable
                }, {
                    name: 'Delete',
                    clicked: deleteObjectVariable
                }];
            }

            function editObjectVariable(object) {
                var onObjectVariableUpdated = function (updatedObjectVariable) {
                    //extendCriteriaFieldObject(updatedCriteriaField);
                    var index = UtilsService.getItemIndexByVal(ctrl.objectVariables, object.ObjectName, 'ObjectName');
                    ctrl.objectVariables[index] = updatedObjectVariable;
                };
                VRCommon_ObjectVariableService.editVRObjectVariable(object, onObjectVariableUpdated);
            }
            function deleteObjectVariable(criteriaField) {
                VRNotificationService.showConfirmation().then(function (confirmed) {
                    if (confirmed) {
                        var index = UtilsService.getItemIndexByVal(ctrl.criteriaFields, criteriaField.FieldName, 'FieldName');
                        ctrl.criteriaFields.splice(index, 1);
                        deletePriorityDataItem(criteriaField);
                    }
                });
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

        }
    }

    app.directive('vrCommonObjectvariableManagement', VRObjectVariableManagementDirective);

})(app);