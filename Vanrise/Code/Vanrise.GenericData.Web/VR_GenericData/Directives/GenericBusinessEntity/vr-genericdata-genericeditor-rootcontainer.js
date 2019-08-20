"use strict";

app.directive("vrGenericdataGenericeditorRootcontainer", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBusinessEntityAPIService", "VR_GenericData_GenericBusinessEntityService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntityRootEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityRootContainerEditor.html"
        };

        function GenericBusinessEntityRootEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var dataRecordTypeId;
            var allFieldValuesByFieldNames = {};
            var globalLoaderRegisteredDirectives = [];

            var runtimeEditorAPI;
            var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var allRootContainerChildDirectivesLoadDeferred = UtilsService.createPromiseDeferred();
            var isEditMode;
            var context;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    runtimeEditorAPI = api;
                    runtimeEditorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var promises = [];

                    var definitionSettings;
                    var historyId;
                    var parentFieldValues;

                    if (payload != undefined) {
                        isEditMode = payload.isEditMode;
                        selectedValues = payload.selectedValues;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        definitionSettings = payload.definitionSettings;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        $scope.scopeModel.runtimeEditor = payload.runtimeEditor;
                        context = payload.context;
                    }

                    if (selectedValues != undefined) {
                        for (var key in selectedValues) {
                            var value = selectedValues[key];
                            if (Array.isArray(value))
                                allFieldValuesByFieldNames[key] = value;
                            else
                                allFieldValuesByFieldNames[key] = [value];
                        }
                    }

                    var innerPromises = [];

                    if (parentFieldValues != undefined) {
                        var getDependentFieldValuesPromise = getDependentFieldValues();
                        innerPromises.push(getDependentFieldValuesPromise);
                    }

                    var loadEditorRuntimeDirectivePromiseDefered = UtilsService.createPromiseDeferred();
                    promises.push(loadEditorRuntimeDirectivePromiseDefered.promise);

                    UtilsService.waitMultiplePromises(innerPromises).then(function () {
                        getLoadEditorRuntimeDirective().then(function () {
                            loadEditorRuntimeDirectivePromiseDefered.resolve();
                        });
                    });

                    function getDependentFieldValues() {
                        var getDependentFieldValuesPromiseDeferred = UtilsService.createPromiseDeferred();

                        var fieldValues = {};
                        for (var prop in parentFieldValues) {
                            if (parentFieldValues[prop] != undefined)
                                fieldValues[prop] = parentFieldValues[prop].value;
                        }

                        var input = {
                            DataRecordTypeId: dataRecordTypeId,
                            FieldValues: fieldValues
                        };

                        VR_GenericData_GenericBusinessEntityAPIService.GetDependentFieldValues(input).then(function (response) {
                            if (response) {
                                for (var prop in response) {
                                    var dependentFieldValue = response[prop];
                                    if (prop != "$type") {
                                        allFieldValuesByFieldNames[prop] = !Array.isArray(dependentFieldValue) ? [dependentFieldValue] : dependentFieldValue;
                                        if (prop in parentFieldValues) {
                                            parentFieldValues[prop].value = dependentFieldValue;
                                            parentFieldValues[prop].isHidden = false;
                                        }
                                        else {
                                            parentFieldValues[prop] = {
                                                value: dependentFieldValue,
                                                isHidden: false,
                                                isDisabled: true
                                            };
                                        }
                                    }
                                }
                            }

                            getDependentFieldValuesPromiseDeferred.resolve();
                        });

                        return getDependentFieldValuesPromiseDeferred.promise;
                    }

                    function getLoadEditorRuntimeDirective() {
                        var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();

                        runtimeEditorReadyDeferred.promise.then(function () {

                            var runtimeEditorPayload = {
                                selectedValues: selectedValues,
                                allFieldValuesByName: allFieldValuesByFieldNames,
                                dataRecordTypeId: dataRecordTypeId,
                                definitionSettings: definitionSettings,
                                historyId: historyId,
                                parentFieldValues: parentFieldValues,
                                genericContext: buildGenericContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
                        });

                        return runtimeEditorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        allRootContainerChildDirectivesLoadDeferred.resolve();
                    });
                };

                api.getData = function () {
                    return runtimeEditorAPI.getData();
                };

                api.setData = function (dicData) {
                    runtimeEditorAPI.setData(dicData);
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(api);
                }
            }

            function buildGenericContext() {

                var genericContext = {
                    notifyFieldValueChanged: function (changedField) { //changedField = {fieldName: 'name', fieldValues: ['value1', 'value2', ...] }
                        var fieldName = changedField.fieldName;
                        var fieldValues = changedField.fieldValues;
                        var _promises = [];

                        if (!VR_GenericData_GenericBusinessEntityService.tryUpdateAllFieldValuesByFieldName(fieldName, fieldValues, allFieldValuesByFieldNames)) {
                            return UtilsService.waitMultiplePromises(_promises);
                        }

                        if (runtimeEditorAPI.onFieldValueChanged != undefined && typeof (runtimeEditorAPI.onFieldValueChanged) == "function") {

                            var promise = runtimeEditorAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                            if (promise != undefined) {
                                _promises.push(promise);
                            }
                        }

                        return UtilsService.waitMultiplePromises(_promises);
                    },
                    notifyFieldValuesChanged: function (changedFields) { //changedFields = {'fieldName1': ['value1', 'value2', ...], 'fieldName2': ['value1', 'value2', ...], ... }
                        if (VR_GenericData_GenericBusinessEntityService.tryUpdateAllFieldValuesByFieldNames(changedFields, allFieldValuesByFieldNames) && runtimeEditorAPI.onFieldValueChanged != undefined && typeof (runtimeEditorAPI.onFieldValueChanged) == "function") {
                            var promise = runtimeEditorAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                            if (promise != undefined) {
                                return promise;
                            }
                        }

                        return UtilsService.waitMultiplePromises([]);
                    },
                    getFieldValues: function () {
                        var dicFieldValues = {};
                        runtimeEditorAPI.setData(dicFieldValues);
                        return dicFieldValues;
                    },
                    getFieldValuesPromise: function () {
                        var getFieldValuesPromiseDeferred = UtilsService.createPromiseDeferred();
                        allRootContainerChildDirectivesLoadDeferred.promise.then(function () {
                            var dicFieldValues = {};
                            runtimeEditorAPI.setData(dicFieldValues);
                            getFieldValuesPromiseDeferred.resolve(dicFieldValues);
                        });

                        return getFieldValuesPromiseDeferred.promise;
                    },
                    setLoader: function (id, value) {
                        if (value) {
                            $scope.scopeModel.isRootContainerLoading = true;
                            if (globalLoaderRegisteredDirectives.indexOf(id) == -1)
                                globalLoaderRegisteredDirectives.push(id);
                        }
                        else {
                            var index = globalLoaderRegisteredDirectives.indexOf(id);
                            globalLoaderRegisteredDirectives.splice(index, 1);
                        }

                        $scope.scopeModel.isRootContainerLoading = (globalLoaderRegisteredDirectives.length != 0);
                    },
                    setFieldValues: function (fieldValuesByNameDict) { //fieldValuesByNamesDict = {'fieldName' :'value1', .... }
                        if (fieldValuesByNameDict == undefined)
                            return UtilsService.waitMultiplePromises([]);

                        $scope.scopeModel.isRootContainerLoading = true;

                        var rootPromiseNode = {
                            promises: [],
                            getChildNode: function () {
                                var getDependentFieldValuesPromiseDeferred = UtilsService.createPromiseDeferred();
                                var valuesToSet = {};

                                var input = {
                                    DataRecordTypeId: dataRecordTypeId,
                                    FieldValues: fieldValuesByNameDict
                                };

                                VR_GenericData_GenericBusinessEntityAPIService.GetDependentFieldValues(input).then(function (response) {
                                    for (var prop in response) {
                                        if (prop == "$type")
                                            continue;

                                        var dependentFieldValue = response[prop];
                                        var convertedFieldValue = !Array.isArray(dependentFieldValue) ? [dependentFieldValue] : dependentFieldValue;
                                        VR_GenericData_GenericBusinessEntityService.tryUpdateAllFieldValuesByFieldName(prop, convertedFieldValue, allFieldValuesByFieldNames);

                                        valuesToSet[prop] = dependentFieldValue;
                                    }

                                    getDependentFieldValuesPromiseDeferred.resolve();
                                });

                                return {
                                    promises: [allRootContainerChildDirectivesLoadDeferred.promise, getDependentFieldValuesPromiseDeferred.promise],
                                    getChildNode: function () {
                                        var _promises = [];
                                        var onFieldValueSettedPromise = runtimeEditorAPI.setFieldValues(valuesToSet);
                                        if (onFieldValueSettedPromise != undefined)
                                            _promises.push(onFieldValueSettedPromise);
                                        return {
                                            promises: _promises
                                        };
                                    }
                                };
                            }
                        };

                        return UtilsService.waitPromiseNode(rootPromiseNode).then(function () {
                            $scope.scopeModel.isRootContainerLoading = (globalLoaderRegisteredDirectives.length != 0);
                        });
                    },
                    isAddMode: function () {
                        return isEditMode == false;
                    },
                    getRecordFields: function () {
                        if (context == undefined || context.getFields == undefined || typeof (context.getFields) != "function")
                            return undefined;

                        var contextFields = context.getFields();
                        return contextFields != undefined && contextFields.length > 0 ? contextFields : undefined;
                    }
                };

                return genericContext;
            }
        }

        return directiveDefinitionObject;
    }
]);