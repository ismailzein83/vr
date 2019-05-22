"use strict";

app.directive("vrGenericdataGenericeditorRootcontainer", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericBusinessEntityAPIService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

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
            var allFieldValuesByFieldNames = {};
            //var settedFieldValues;

            var runtimeEditorAPI;
            var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();


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

                    var businessEntityDefinitionId;
                    var dataRecordTypeId;
                    var definitionSettings;
                    var historyId;
                    var parentFieldValues;

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        selectedValues = payload.selectedValues;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        definitionSettings = payload.definitionSettings;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        $scope.scopeModel.runtimeEditor = payload.runtimeEditor;
                    }

                    if (selectedValues != undefined) {
                        for (var key in selectedValues) {
                            var value = selectedValues[key];
                            if (typeof (value) == "object")
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
                            fieldValues[prop] = parentFieldValues[prop].value;
                        }

                        var input = {
                            BusinessEntityDefinitionId: businessEntityDefinitionId,
                            FieldValues: fieldValues
                        };

                        VR_GenericData_GenericBusinessEntityAPIService.GetDependentFieldValues(input).then(function (response) {
                            if (response) {
                                for (var prop in response) {
                                    var dependentFieldValue = response[prop];
                                    if (prop != "$type") {
                                        allFieldValuesByFieldNames[prop] = typeof (dependentFieldValue) == "object" ? dependentFieldValue : [dependentFieldValue];

                                        parentFieldValues[prop] = {
                                            value: dependentFieldValue,
                                            isHidden: false,
                                            isDisabled: true
                                        };

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

                    return UtilsService.waitMultiplePromises(promises);
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
                var context = {
                    notifyFieldValueChanged: function (changedField) {  //changedField = {fieldName : 'name', fieldValues : ['value1', 'value2'...] }
                        var fieldName = changedField.fieldName;
                        var fieldValues = changedField.fieldValues;

                        var _promises = [];

                        if (!tryUpdateAllFieldValuesByFieldNames(fieldName, fieldValues)) {
                            return UtilsService.waitMultiplePromises(_promises);
                        }

                        if (runtimeEditorAPI.onFieldValueChanged != undefined && typeof (runtimeEditorAPI.onFieldValueChanged) == "function") {

                            var promise = runtimeEditorAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                            if (promise != undefined) {
                                _promises.push(promise);
                            }
                        }

                        function tryUpdateAllFieldValuesByFieldNames(fieldName, fieldValues) {
                            var oldValues = allFieldValuesByFieldNames[fieldName];
                            if (oldValues == undefined && fieldValues == undefined)
                                return false;

                            if (!(fieldName in allFieldValuesByFieldNames) || fieldValues == undefined || oldValues == undefined || oldValues.length != fieldValues.length) {
                                allFieldValuesByFieldNames[fieldName] = fieldValues;
                                return true;
                            }

                            for (var i = 0; i < fieldValues.length; i++) {
                                var currentValue = fieldValues[i];
                                if (oldValues.indexOf(currentValue) == -1) {
                                    allFieldValuesByFieldNames[fieldName] = fieldValues;
                                    return true;
                                }
                            }

                            return false;
                        }

                        return UtilsService.waitMultiplePromises(_promises);
                    },
                    //setFieldValues: function (fieldValuesByNameDict) { //fieldValuesByNamesDict = {'name' :'value1', .... }
                    //    var _promises = [];

                    //    //var valuesToSet = {};
                    //    //if (settedFieldValues == undefined) {
                    //    //    settedFieldValues = fieldValuesByNameDict;
                    //    //    valuesToSet = fieldValuesByNameDict;
                    //    //}
                    //    //else {
                    //    //    for (var key in fieldValuesByNameDict) {
                    //    //        if (settedFieldValues[key] == undefined) {
                    //    //            settedFieldValues[key] = fieldValuesByNameDict[key];
                    //    //            valuesToSet[key] = fieldValuesByNameDict[key];
                    //    //        }
                    //    //    }
                    //    //}

                    //    if (Object.keys(fieldValuesByNameDict).length > 0) {

                    //        var onFieldValueSettedPromise = runtimeEditorAPI.setFieldValues(fieldValuesByNameDict);
                    //        if (onFieldValueSettedPromise != undefined)
                    //            _promises.push(onFieldValueSettedPromise);
                    //    }

                    //    return UtilsService.waitMultiplePromises(_promises);
                    //}
                };

                return context;
            }
        }

        return directiveDefinitionObject;
    }
]);