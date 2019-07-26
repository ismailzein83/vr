(function (appControllers) {

    'use strict';

    GenericEditorConditionalRuleContainerService.$inject = ['UtilsService', 'VR_GenericData_RecordQueryLogicalOperatorEnum'];

    function GenericEditorConditionalRuleContainerService(UtilsService, VR_GenericData_RecordQueryLogicalOperatorEnum) {

        var conditionalRuleActions = {};

        function registerFieldValueConditionalRuleAction() {
            conditionalRuleActions["FieldValueConditionalRuleAction"] = function (context) {

                context.subscribeToFieldValueChangedFunction = function (subscribeToFieldValueChangedContext) { //Not Supported for multiple selection values - Supported for multiple condition values
                    var genericEditorConditionalRule = subscribeToFieldValueChangedContext.genericEditorConditionalRule;
                    var fieldValuesByFieldName = subscribeToFieldValueChangedContext.fieldValuesByFieldName;

                    var conditionalRuleEvaluationResult = evaluateConditions(genericEditorConditionalRule, fieldValuesByFieldName);

                    function evaluateConditions(genericEditorConditionalRule, fieldValuesByFieldName) {
                        var conditions = genericEditorConditionalRule.Conditions;
                        var logicalOperator = genericEditorConditionalRule.LogicalOperator;

                        if (fieldValuesByFieldName == undefined)
                            return false;

                        for (var i = 0; i < conditions.length; i++) {
                            var fieldName = conditions[i].FieldName;
                            var fieldValues = conditions[i].FieldValues;


                            var containValue = false;
                            var currentValue = fieldValuesByFieldName[fieldName];

                            if (currentValue != undefined) {
                                if (Array.isArray(currentValue))
                                    currentValue = currentValue[0];

                                for (var j = 0; j < fieldValues.length; j++) {
                                    if (currentValue == fieldValues[j]) {
                                        containValue = true;
                                        break;
                                    }
                                }
                            }

                            switch (logicalOperator) {
                                case VR_GenericData_RecordQueryLogicalOperatorEnum.And.value: if (!containValue) { return false; } break;
                                case VR_GenericData_RecordQueryLogicalOperatorEnum.Or.value: if (containValue) { return true; } break;
                            }

                        }

                        switch (logicalOperator) {
                            case VR_GenericData_RecordQueryLogicalOperatorEnum.And.value: return true;
                            case VR_GenericData_RecordQueryLogicalOperatorEnum.Or.value: return false;
                        }
                    }

                    return conditionalRuleEvaluationResult;
                };
            };
        }

        function initializeConditionalRuleEvaluation(context) {
            var actionName = context.genericEditorConditionalRule.ActionName;
            var postConditionalRuleEvaluationAction = context.postConditionalRuleEvaluationAction;

            var conditionalRuleActionContext = {};
            var conditionalRuleAction = conditionalRuleActions[actionName];
            conditionalRuleAction(conditionalRuleActionContext);

            function getFieldValueChangedFunctionContext(context, allFieldValuesByFieldNames) {
                return {
                    fieldValuesByFieldName: allFieldValuesByFieldNames,
                    genericEditorConditionalRule: context.genericEditorConditionalRule,
                    postConditionalRuleEvaluationAction: context.postConditionalRuleEvaluationAction
                };
            }

            return {
                onFieldValueChanged: function (allFieldValuesByFieldNames) {
                    var onFieldValueChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                    if (conditionalRuleActionContext.subscribeToFieldValueChangedFunction != undefined) {
                        var conditionalRuleEvaluationResult = conditionalRuleActionContext.subscribeToFieldValueChangedFunction(getFieldValueChangedFunctionContext(context, allFieldValuesByFieldNames));
                        postConditionalRuleEvaluationAction(conditionalRuleEvaluationResult);
                        onFieldValueChangedPromiseDeferred.resolve();
                    }
                    else {
                        onFieldValueChangedPromiseDeferred.resolve();
                    }

                    return onFieldValueChangedPromiseDeferred.promise;
                }
            };
        }

        return {
            registerFieldValueConditionalRuleAction: registerFieldValueConditionalRuleAction,
            initializeConditionalRuleEvaluation: initializeConditionalRuleEvaluation
        };
    }

    appControllers.service('VR_GenericData_GenericEditorConditionalRuleContainerService', GenericEditorConditionalRuleContainerService);

})(appControllers);