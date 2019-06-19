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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/ConditionalRuleContainerRuntimeSettingTemplate.html"
        };

        function GenericEditorConditionalRuleRuntimeSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var allFieldValuesByName;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            var parentFieldValues;
            var genericContext;
            var genericEditorConditionalRule;

            var conditionalRuleContainerServiceState;
            var conditionState;

            var editorRuntimeAPI;
            var editorRuntimeDirectiveReadyPromiseDeferred;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    editorRuntimeAPI = api;

                    var directivePayload = {
                        selectedValues: selectedValues,
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
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, editorRuntimeAPI, directivePayload, setLoader, undefined).then(function () {
                        if (editorRuntimeDirectiveReadyPromiseDeferred == undefined)
                            editorRuntimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

                        editorRuntimeDirectiveReadyPromiseDeferred.resolve();
                    });
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    console.log(payload);

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

                            conditionalRuleContainerServiceState = new VR_GenericData_GenericEditorConditionalRuleContainerService.initializeConditionalRuleEvaluation(getInitializeConditionalRuleEvaluationPayload());
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    if (editorRuntimeAPI != undefined && $scope.scopeModel.showDirective) {
                        editorRuntimeAPI.setData(dicData);
                    }
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    if (conditionalRuleContainerServiceState.onFieldValueChanged != undefined) {
                        conditionalRuleContainerServiceState.onFieldValueChanged(allFieldValuesByFieldNames);
                    }

                    var onFieldValueChangedDeferred = UtilsService.createPromiseDeferred();

                    if (editorRuntimeDirectiveReadyPromiseDeferred != undefined) {
                        editorRuntimeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var _promises = [];
                            if (editorRuntimeAPI != undefined) {
                                if (editorRuntimeAPI.onFieldValueChanged != undefined && typeof (editorRuntimeAPI.onFieldValueChanged) == "function") {
                                    var onFieldValueChangedPromise = editorRuntimeAPI.onFieldValueChanged(allFieldValuesByFieldNames);
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
                    if (editorRuntimeAPI == undefined)
                        return null;

                    var _promises = [];

                    if (editorRuntimeAPI.setFieldValues != undefined && typeof (editorRuntimeAPI.setFieldValues) == "function") {
                        var onFieldValueSettedPromise = editorRuntimeAPI.setFieldValues(fieldValuesByNames);
                        if (onFieldValueSettedPromise != undefined)
                            _promises.push(onFieldValueSettedPromise);
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function getInitializeConditionalRuleEvaluationPayload() {
                return {
                    genericEditorConditionalRule: genericEditorConditionalRule,
                    fieldValuesByFieldName: allFieldValuesByName,
                    postConditionalRuleEvaluationAction: function (conditionResult) {
                        if (conditionResult == conditionState)
                            return;

                        $scope.scopeModel.showDirective = conditionResult;

                        conditionState = conditionResult;
                        if (!conditionResult) {
                            resetEditorRuntimeDirective();
                        }

                        function resetEditorRuntimeDirective() {
                            var dicData = {};
                            if (editorRuntimeAPI != undefined) {
                                editorRuntimeAPI.setData(dicData);
                            }

                            for (var key in dicData) {
                                dicData[key] = undefined;
                            }

                            if (genericContext != undefined && genericContext.notifyFieldValuesChanged != undefined && typeof (genericContext.notifyFieldValuesChanged) == "function")
                                genericContext.notifyFieldValuesChanged(dicData);

                            editorRuntimeAPI = undefined;
                            editorRuntimeDirectiveReadyPromiseDeferred = undefined;
                        }
                    }
                };
            }
        }

        return directiveDefinitionObject;
    }
]);