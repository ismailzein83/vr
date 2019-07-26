'use strict';

app.directive('vrGenericeditorConditionalruleFieldvalue', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new conditionalRuleFieldValueCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorConditionalRuleSetting/Templates/ConditionalRuleFieldValueSettingTemplate.html'
        };

        function conditionalRuleFieldValueCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var logicalOperatorDirectiveAPI;
            var logicalOperatorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataRecordFields = [];
                $scope.scopeModel.conditions = [];

                $scope.scopeModel.onLogicalOperatorDirectiveReady = function (api) {
                    logicalOperatorDirectiveAPI = api;
                    logicalOperatorDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.addCondition = function () {
                    prepareCondition();
                };

                $scope.scopeModel.onRemoveCondition = function (condition) {
                    var index = $scope.scopeModel.conditions.indexOf(condition);
                    if (index != -1) {
                        $scope.scopeModel.conditions.splice(index, 1);
                    }
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.conditions == undefined || $scope.scopeModel.conditions.length == 0)
                        return "You Should add at least one condition.";

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var conditions;
                    var logicalOperator;

                    if (payload != undefined) {
                        $scope.scopeModel.dataRecordFields = payload.context.getFields();
                        var genericEditorConditionalRule = payload.genericEditorConditionalRule;
                        if (genericEditorConditionalRule != undefined) {
                            conditions = genericEditorConditionalRule.Conditions;
                            logicalOperator = genericEditorConditionalRule.LogicalOperator;
                        }
                    }

                    var logicalOperatorDirectiveLoadedPromise = loadLogicalOperatorDirective();
                    promises.push(logicalOperatorDirectiveLoadedPromise);

                    if (conditions != undefined) {
                        for (var i = 0; i < conditions.length; i++) {
                            var currentCondition = conditions[i];
                            currentCondition.expanded = false;
                            currentCondition.dataRecordTypeFieldSelectionChangedLoadDeferred = UtilsService.createPromiseDeferred();
                            prepareCondition(currentCondition);

                            var dataRecordTypeFieldRuntimeEditorLoadPromise = loadDataRecordTypeFieldRuntimeEditorDirective(currentCondition);
                            promises.push(dataRecordTypeFieldRuntimeEditorLoadPromise);
                        }
                    }
                    else {
                        prepareCondition();
                    }

                    function loadDataRecordTypeFieldRuntimeEditorDirective(condition) {
                        var dataRecordTypeFieldRuntimeEditorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([condition.dataRecordTypeFieldRuntimeEditorReadyDeferred.promise, condition.dataRecordTypeFieldSelectionChangedLoadDeferred.promise]).then(function () {
                            condition.dataRecordTypeFieldSelectionChangedLoadDeferred = undefined;

                            var payload = {
                                fieldType: condition.selectedDataRecordField.Type,
                                fieldValue: condition.FieldValues != undefined ? condition.FieldValues : undefined,
                                fieldTitle: condition.selectedDataRecordField != undefined ? condition.selectedDataRecordField.FieldTitle : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(condition.dataRecordTypeFieldRuntimeEditorAPI, payload, dataRecordTypeFieldRuntimeEditorLoadDeferred);
                        });

                        return dataRecordTypeFieldRuntimeEditorLoadDeferred.promise;
                    }

                    function loadLogicalOperatorDirective() {
                        var logicalOperatorDirectiveLoadedPromiseDeferred = UtilsService.createPromiseDeferred();
                        logicalOperatorDirectiveReadyPromiseDeferred.promise.then(function () {
                            var logicalOperatorDirectivePayload;
                            if (logicalOperator != undefined) {
                                logicalOperatorDirectivePayload = { LogicalOperator: logicalOperator };
                            }
                            VRUIUtilsService.callDirectiveLoad(logicalOperatorDirectiveAPI, logicalOperatorDirectivePayload, logicalOperatorDirectiveLoadedPromiseDeferred);
                        });
                        return logicalOperatorDirectiveLoadedPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var genericEditorFieldValueCondition = [];
                    for (var i = 0; i < $scope.scopeModel.conditions.length; i++) {
                        var currentCondition = $scope.scopeModel.conditions[i];
                        genericEditorFieldValueCondition.push(buildGenericEditorFieldValueCondition(currentCondition));
                    }

                    function buildGenericEditorFieldValueCondition(condition) {
                        var fieldValues = condition.dataRecordTypeFieldRuntimeEditorAPI.getData();
                        if (!Array.isArray(fieldValues))
                            fieldValues = [fieldValues];

                        return {
                            FieldName: condition.selectedDataRecordField.FieldName,
                            FieldValues: fieldValues
                        };
                    }

                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericEditorFieldValueConditionalRule, Vanrise.GenericData.MainExtensions",
                        Conditions: genericEditorFieldValueCondition.length > 0 ? genericEditorFieldValueCondition : null,
                        LogicalOperator: logicalOperatorDirectiveAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareCondition(condition) {
                if (condition == undefined)
                    condition = {};

                if (condition.FieldName != undefined) {
                    condition.selectedDataRecordField = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, condition.FieldName, 'FieldName');
                    condition.runtimeFieldEditor = condition.selectedDataRecordField.Type.RuntimeEditor;
                }

                condition.dataRecordTypeFieldRuntimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

                condition.onDataRecordTypeFieldRuntimeEditorReady = function (api) {
                    condition.dataRecordTypeFieldRuntimeEditorAPI = api;
                    condition.dataRecordTypeFieldRuntimeEditorReadyDeferred.resolve();
                };

                condition.onDataRecordTypeFieldSelectionChanged = function (selectedDataRecordTypeField) {
                    if (selectedDataRecordTypeField == undefined)
                        return;

                    condition.title = selectedDataRecordTypeField.FieldTitle;

                    if (condition.dataRecordTypeFieldSelectionChangedLoadDeferred != undefined) {
                        condition.dataRecordTypeFieldSelectionChangedLoadDeferred.resolve();
                    } else {
                        condition.dataRecordTypeFieldRuntimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

                        condition.expanded = true;
                        condition.runtimeFieldEditor = selectedDataRecordTypeField.Type.RuntimeEditor;
                        condition.dataRecordTypeFieldRuntimeEditorReadyDeferred.promise.then(function () {

                            var payload = {
                                fieldType: selectedDataRecordTypeField.Type,
                                fieldTitle: selectedDataRecordTypeField.FieldTitle
                            };
                            var setLoader = function (value) {
                                condition.isDirectiveLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, condition.dataRecordTypeFieldRuntimeEditorAPI, payload, setLoader, undefined);
                        });
                    }
                };

                $scope.scopeModel.conditions.push(condition);
            }
        }

        return directiveDefinitionObject;
    }]);