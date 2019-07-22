"use strict";

app.directive("vrGenericdataConditionalrulecontainereditorsettingRuntime", ["UtilsService", "VRUIUtilsService", "VR_GenericData_GenericEditorConditionalRuleContainerService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericEditorConditionalRuleContainerService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorConditionalRuleRuntimeSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/ConditionalRuleContainer/Templates/ConditionalRuleContainerRuntimeSettingTemplate.html"
        };

        function GenericEditorConditionalRuleRuntimeSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var isFirstLoad = true;

            var selectedValues;
            var allFieldValuesByName;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            var parentFieldValues;
            var genericContext;
            var genericEditorConditionalRule;

            var initializeConditionalRuleEvaluationState;
            var currentConditionalRuleEvaluationResult;

            var editorRuntimeDirectiveAPI;
            var editorRuntimeDirectiveReadyPromiseDeferred;  //Defined and Undefined in postConditionalRuleEvaluationAction

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    editorRuntimeDirectiveAPI = api;
                    editorRuntimeDirectiveReadyPromiseDeferred.resolve();

                    if (!isFirstLoad) {
                        var directivePayload = {
                            allFieldValuesByName: allFieldValuesByName,
                            definitionSettings: definitionSettings.EditorDefinitionSetting,
                            dataRecordTypeId: dataRecordTypeId,
                            historyId: historyId,
                            parentFieldValues: parentFieldValues,
                            genericContext: genericContext
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isDirectiveLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, editorRuntimeDirectiveAPI, directivePayload, setLoader, undefined);
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        genericContext = payload.genericContext;

                        if (definitionSettings != undefined) {
                            genericEditorConditionalRule = definitionSettings.GenericEditorConditionalRule;
                            if (definitionSettings.EditorDefinitionSetting != undefined) {
                                $scope.scopeModel.runtimeEditor = definitionSettings.EditorDefinitionSetting.RuntimeEditor;
                            }

                            initializeConditionalRuleEvaluationState = new VR_GenericData_GenericEditorConditionalRuleContainerService.initializeConditionalRuleEvaluation(getInitializeConditionalRuleEvaluationContext());
                        }

                        if (initializeConditionalRuleEvaluationState != undefined && initializeConditionalRuleEvaluationState.onFieldValueChanged != undefined) {

                            var loadRuntimeEditorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                            promises.push(loadRuntimeEditorDirectivePromiseDeferred.promise);

                            initializeConditionalRuleEvaluationState.onFieldValueChanged(allFieldValuesByName).then(function () {
                                if ($scope.scopeModel.showDirective) {
                                    loadRuntimeEditorDirective().then(function () {
                                        loadRuntimeEditorDirectivePromiseDeferred.resolve();
                                    });
                                }
                                else {
                                    loadRuntimeEditorDirectivePromiseDeferred.resolve();
                                }
                            });
                        }
                    }

                    function loadRuntimeEditorDirective() {
                        var loadRuntimeEditorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        editorRuntimeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var directivePayload = {
                                selectedValues: selectedValues,
                                allFieldValuesByName: allFieldValuesByName,
                                definitionSettings: definitionSettings.EditorDefinitionSetting,
                                dataRecordTypeId: dataRecordTypeId,
                                historyId: historyId,
                                parentFieldValues: parentFieldValues,
                                genericContext: genericContext
                            };
                            VRUIUtilsService.callDirectiveLoad(editorRuntimeDirectiveAPI, directivePayload, loadRuntimeEditorDirectivePromiseDeferred);
                        });

                        return loadRuntimeEditorDirectivePromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        isFirstLoad = false;
                    });
                };

                api.setData = function (dicData) {
                    if (editorRuntimeDirectiveAPI != undefined && $scope.scopeModel.showDirective) {
                        editorRuntimeDirectiveAPI.setData(dicData);
                    }
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    if (initializeConditionalRuleEvaluationState.onFieldValueChanged != undefined) {
                        initializeConditionalRuleEvaluationState.onFieldValueChanged(allFieldValuesByFieldNames);
                    }

                    var onFieldValueChangedDeferred = UtilsService.createPromiseDeferred();

                    if ($scope.scopeModel.showDirective) {
                        editorRuntimeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var _promises = [];
                            if (editorRuntimeDirectiveAPI != undefined) {
                                if (editorRuntimeDirectiveAPI.onFieldValueChanged != undefined && typeof (editorRuntimeDirectiveAPI.onFieldValueChanged) == "function") {
                                    var onFieldValueChangedPromise = editorRuntimeDirectiveAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                                    if (onFieldValueChangedPromise != undefined)
                                        _promises.push(onFieldValueChangedPromise);
                                }
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                onFieldValueChangedDeferred.resolve();
                            });
                        });
                    }
                    else {
                        onFieldValueChangedDeferred.resolve();
                    }

                    return onFieldValueChangedDeferred.promise;
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    if (editorRuntimeDirectiveAPI == undefined)
                        return null;

                    var _promises = [];

                    if (editorRuntimeDirectiveAPI.setFieldValues != undefined && typeof (editorRuntimeDirectiveAPI.setFieldValues) == "function") {
                        var onFieldValueSettedPromise = editorRuntimeDirectiveAPI.setFieldValues(fieldValuesByNames);
                        if (onFieldValueSettedPromise != undefined)
                            _promises.push(onFieldValueSettedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function getInitializeConditionalRuleEvaluationContext() {

                var postConditionalRuleEvaluationAction = function (conditionalRuleEvaluationResult) {
                    if (conditionalRuleEvaluationResult == currentConditionalRuleEvaluationResult)
                        return;

                    if (conditionalRuleEvaluationResult) {
                        editorRuntimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    } else {
                        resetEditorRuntimeDirective();
                    }

                    $scope.scopeModel.showDirective = conditionalRuleEvaluationResult;
                    currentConditionalRuleEvaluationResult = conditionalRuleEvaluationResult;

                    function resetEditorRuntimeDirective() {
                        var dicData = {};
                        if (editorRuntimeDirectiveAPI != undefined) {
                            editorRuntimeDirectiveAPI.setData(dicData);
                        }

                        for (var key in dicData) {
                            dicData[key] = undefined;
                        }

                        if (genericContext != undefined && genericContext.notifyFieldValuesChanged != undefined && typeof (genericContext.notifyFieldValuesChanged) == "function")
                            genericContext.notifyFieldValuesChanged(dicData);

                        editorRuntimeDirectiveAPI = undefined;
                        editorRuntimeDirectiveReadyPromiseDeferred = undefined;
                    }
                };

                return {
                    genericEditorConditionalRule: genericEditorConditionalRule,
                    fieldValuesByFieldName: allFieldValuesByName,
                    postConditionalRuleEvaluationAction: postConditionalRuleEvaluationAction
                };
            }
        }

        return directiveDefinitionObject;
    }
]);