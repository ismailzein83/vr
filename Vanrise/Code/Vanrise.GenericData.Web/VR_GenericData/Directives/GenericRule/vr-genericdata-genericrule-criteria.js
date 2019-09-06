(function (app) {

    'use strict';

    GenericRuleCriteriaDirective.$inject = ['VR_GenericData_DataRecordFieldAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericUIService'];

    function GenericRuleCriteriaDirective(VR_GenericData_DataRecordFieldAPIService, UtilsService, VRUIUtilsService, VR_GenericData_GenericUIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericRuleCriteria($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericRule/Templates/GenericRuleCriteriaTemplate.html'
        };

        function GenericRuleCriteria($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var genericUIObj;
            var criteriaFields = [];
            var criteriaDefinitionGroups;
            var criteriaAccessibility;
            var criteriaPredefinedData;
            var criteriaFieldsValues;
            var dataRecordFieldTypeConfigs;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.criteriaObjects = [];
                $scope.scopeModel.hiddenCriteriaFields = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    criteriaFieldsValues = undefined;
                    if (payload == undefined || payload.criteriaDefinitionFields == undefined) {
                        return;
                    }

                    var criteriaDefinitionFields = payload.criteriaDefinitionFields;
                    genericUIObj = VR_GenericData_GenericUIService.createGenericUIObj(criteriaDefinitionFields);
                    criteriaFieldsValues = payload.criteriaFieldsValues;
                    criteriaAccessibility = payload.criteriaAccessibility;
                    criteriaPredefinedData = payload.criteriaPredefinedData;
                    criteriaDefinitionGroups = payload.criteriaDefinitionGroups;

                    var fieldsValues;
                    if (criteriaFieldsValues != undefined)
                        fieldsValues = UtilsService.cloneObject(criteriaFieldsValues, false);

                    var initialPromises = [];

                    var getDataRecordFieldTypeConfigsPromise = getDataRecordFieldTypeConfigs();
                    initialPromises.push(getDataRecordFieldTypeConfigsPromise);

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var promises = [];
                            if (criteriaDefinitionFields != undefined && criteriaDefinitionFields.length > 0) {
                                buildCriteriaFieldsArray();

                                var loadCriteriaObjectsPromise = loadCriteriaObjects();
                                promises.push(loadCriteriaObjectsPromise);
                            }

                            return {
                                promises: promises
                            };
                        }
                    };

                    function getDataRecordFieldTypeConfigs() {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                            dataRecordFieldTypeConfigs = response;
                        });
                    }

                    function buildCriteriaFieldsArray() {

                        for (var i = 0; i < criteriaDefinitionFields.length; i++) {
                            var field = criteriaDefinitionFields[i];

                            var dataFieldTypeConfig = UtilsService.getItemByVal(dataRecordFieldTypeConfigs, field.FieldType.ConfigId, 'ExtensionConfigurationId');
                            field.genericUIContext = genericUIObj.getFieldContext(field);

                            extendCritireaRuntimeFieldEditor(field, dataFieldTypeConfig);
                            criteriaFields.push(field);
                        }

                        function extendCritireaRuntimeFieldEditor(field, dataFieldTypeConfig) {
                            field.runtimeEditor = {
                                directive: dataFieldTypeConfig.RuntimeEditor,
                                directiveAPI: undefined,
                                isFieldDirectiveLoading: false,
                                onFieldSelectionChangedPromiseDeferred: undefined
                            };

                            field.runtimeEditor.onDirectiveReady = function (api) {
                                field.runtimeEditor.directiveAPI = api;
                                field.runtimeEditor.onReadyPromiseDeferred.resolve();
                            };
                        }
                    }

                    function loadCriteriaObjects() {
                        var _loadPromises = [];

                        for (var k = 0; k < criteriaFields.length; k++) {
                            var criteriaField = criteriaFields[k];
                            var isAccessible = true;

                            if (criteriaAccessibility != undefined) {
                                var accessibleField = criteriaAccessibility[criteriaField.FieldName];
                                if (accessibleField != undefined && accessibleField.notAccessible) {
                                    $scope.scopeModel.hiddenCriteriaFields.push(criteriaField);

                                    criteriaField.runtimeEditor.onReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    var loadCritireaFieldDirectivePromise = loadCritireaFieldDirective(criteriaField);
                                    _loadPromises.push(loadCritireaFieldDirectivePromise);
                                    continue;
                                }
                            }

                            if (tryExtendFieldToGroup(criteriaField, _loadPromises))
                                continue;

                            criteriaField.runtimeEditor.onReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                            var loadCritireaFieldDirectivePromise = loadCritireaFieldDirective(criteriaField);
                            _loadPromises.push(loadCritireaFieldDirectivePromise);

                            $scope.scopeModel.criteriaObjects.push({
                                field: criteriaField,
                                isGroup: false
                            });
                        }

                        function tryExtendFieldToGroup(criteriaField, loadPromises) {
                            if (criteriaDefinitionGroups == undefined || criteriaDefinitionGroups.length == 0) {
                                return false;
                            }

                            for (var j = 0; j < criteriaDefinitionGroups.length; j++) {
                                var criteriaDefinitionGroup = criteriaDefinitionGroups[j];

                                var criteriaDefField = UtilsService.getItemByVal(criteriaDefinitionGroup.Fields, criteriaField.FieldName, 'FieldName');
                                if (criteriaDefField == undefined)
                                    continue;

                                var selectedFieldValue = criteriaFieldsValues != undefined ? criteriaFieldsValues[criteriaField.FieldName] : undefined;
                                if (selectedFieldValue != undefined) {
                                    criteriaField.runtimeEditor.onFieldSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();
                                    criteriaField.runtimeEditor.onReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                                    var loadCritireaFieldDirectivePromise = loadCritireaFieldDirective(criteriaField);
                                    loadPromises.push(loadCritireaFieldDirectivePromise);
                                }

                                var criteriaObject = UtilsService.getItemByVal($scope.scopeModel.criteriaObjects, criteriaDefinitionGroup.GroupId, 'groupId');
                                if (criteriaObject != undefined) {
                                    if (selectedFieldValue != undefined) {
                                        criteriaObject.groupObj.selectedField = criteriaField;
                                    }
                                    criteriaObject.groupObj.fields.push(criteriaField);
                                }
                                else {
                                    $scope.scopeModel.criteriaObjects.push({
                                        groupObj: buildGroupObject(criteriaDefinitionGroup, criteriaField, selectedFieldValue != undefined),
                                        groupId: criteriaDefinitionGroup.GroupId,
                                        isGroup: true
                                    });
                                }

                                return true;
                            }

                            return false;
                        }

                        function buildGroupObject(criteriaDefinitionGroup, criteriaField, isSelectedField) {

                            var onGroupCriteriaFieldSelectionChanged = function (selectedCriteriaField) {
                                if (selectedCriteriaField == undefined || selectedCriteriaField.runtimeEditor == undefined)
                                    return;

                                if (selectedCriteriaField.runtimeEditor.onFieldSelectionChangedPromiseDeferred != undefined) {
                                    selectedCriteriaField.runtimeEditor.onFieldSelectionChangedPromiseDeferred.resolve();
                                }
                                else {
                                    selectedCriteriaField.runtimeEditor.onReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                                    selectedCriteriaField.isFieldDirectiveLoading = true;
                                    loadCritireaFieldDirective(selectedCriteriaField).then(function () {
                                        genericUIObj.resendCritireaFieldValues(selectedCriteriaField);
                                        selectedCriteriaField.isFieldDirectiveLoading = false;
                                    });
                                }
                            };

                            var onBeforeGroupCriteriaFieldSelectionChanged = function () {
                                var criteriaGroup = UtilsService.getItemByVal($scope.scopeModel.criteriaObjects, criteriaDefinitionGroup.GroupId, 'groupId');
                                var selectedField = criteriaGroup.groupObj.selectedField;
                                if (selectedField == undefined) {
                                    return;
                                }

                                if (selectedField.runtimeEditor.onFieldSelectionChangedPromiseDeferred != undefined) {
                                    return;
                                }

                                if (selectedField.genericUIContext != undefined && selectedField.genericUIContext.notifyValueChanged != undefined) {
                                    selectedField.genericUIContext.notifyValueChanged(undefined);
                                }
                            };

                            return {
                                groupTitle: criteriaDefinitionGroup.GroupTitle,
                                fields: [criteriaField],
                                selectedField: isSelectedField ? criteriaField : undefined,
                                onGroupCriteriaFieldSelectionChanged: onGroupCriteriaFieldSelectionChanged,
                                onBeforeGroupCriteriaFieldSelectionChanged: onBeforeGroupCriteriaFieldSelectionChanged
                            };
                        }

                        function loadCritireaFieldDirective(field) {
                            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

                            var promises = [field.runtimeEditor.onReadyPromiseDeferred.promise];
                            if (field.runtimeEditor.onFieldSelectionChangedPromiseDeferred != undefined) {
                                promises.push(field.runtimeEditor.onFieldSelectionChangedPromiseDeferred.promise);
                            }

                            UtilsService.waitMultiplePromises(promises).then(function () {
                                field.runtimeEditor.onFieldSelectionChangedPromiseDeferred = undefined;

                                var payload = {
                                    fieldTitle: field.Title,
                                    fieldType: field.FieldType,
                                    fieldValue: (fieldsValues != undefined) ? fieldsValues[field.FieldName] : (criteriaPredefinedData != undefined ? criteriaPredefinedData[field.FieldName] : undefined),
                                    genericUIContext: field.genericUIContext
                                };
                                VRUIUtilsService.callDirectiveLoad(field.runtimeEditor.directiveAPI, payload, loadPromiseDeferred);
                            });

                            return loadPromiseDeferred.promise;
                        }

                        return UtilsService.waitMultiplePromises(_loadPromises).then(function () {
                            fieldsValues = undefined;
                        });
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                        genericUIObj.loadingFinish();
                    });
                };

                api.getData = function () {
                    var genericRuleCriteria = { FieldsValues: {} };

                    if ($scope.scopeModel.criteriaObjects.length > 0) {

                        for (var i = 0; i < $scope.scopeModel.criteriaObjects.length; i++) {
                            var criteriaObject = $scope.scopeModel.criteriaObjects[i];
                            var fieldValue;
                            if (criteriaObject.isGroup) {
                                if (criteriaObject.groupObj.selectedField == undefined) {
                                    continue;
                                }

                                fieldValue = criteriaObject.groupObj.selectedField.runtimeEditor.directiveAPI.getData();
                                if (fieldValue != undefined) {
                                    genericRuleCriteria.FieldsValues[criteriaObject.groupObj.selectedField.FieldName] = fieldValue;
                                }
                            }
                            else {
                                fieldValue = criteriaObject.field.runtimeEditor.directiveAPI.getData();
                                if (fieldValue != undefined) {
                                    genericRuleCriteria.FieldsValues[criteriaObject.field.FieldName] = fieldValue;
                                }
                            }
                        }
                    }

                    if ($scope.scopeModel.hiddenCriteriaFields.length > 0) {
                        for (var j = 0; j < $scope.scopeModel.hiddenCriteriaFields.length; j++) {
                            var hiddenCriteriaField = $scope.scopeModel.hiddenCriteriaFields[j];
                            var fieldValue = hiddenCriteriaField.runtimeEditor.directiveAPI.getData();
                            if (fieldValue != undefined) {
                                genericRuleCriteria.FieldsValues[hiddenCriteriaField.FieldName] = fieldValue;
                            }
                        }
                    }

                    return Object.keys(genericRuleCriteria.FieldsValues).length > 0 ? genericRuleCriteria : undefined;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
    app.directive('vrGenericdataGenericruleCriteria', GenericRuleCriteriaDirective);
})(app);