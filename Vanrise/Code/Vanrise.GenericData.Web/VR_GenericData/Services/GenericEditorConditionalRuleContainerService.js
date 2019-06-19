(function (appControllers) {

    'use strict';

    GenericEditorConditionalRuleContainerService.$inject = ['UtilsService'];

    function GenericEditorConditionalRuleContainerService(UtilsService) {

        var conditionalRuleActions = {};

        function registerFieldValueConditionalRuleAction() {
            conditionalRuleActions["FieldValueConditionalRuleAction"] = function (context) {

                context.subscribeToFieldValueChangedFunction = function (subscribeToFieldValueChangedContext) { //Not Supported for multiple selection values - Supported for multiple condition values
                    var genericEditorConditionalRule = subscribeToFieldValueChangedContext.genericEditorConditionalRule;
                    var fieldValuesByFieldName = subscribeToFieldValueChangedContext.fieldValuesByFieldName;

                    var conditionalRuleEvaluationResult = evaluateConditions(genericEditorConditionalRule.Conditions, fieldValuesByFieldName);

                    function evaluateConditions(conditions, fieldValuesByFieldName) {
                        if (fieldValuesByFieldName == undefined)
                            return false;

                        for (var i = 0; i < conditions.length; i++) {
                            var fieldName = conditions[i].FieldName;
                            var fieldValues = conditions[i].FieldValues;

                            if (fieldValuesByFieldName[fieldName] == undefined)
                                return false;

                            var currentValue = fieldValuesByFieldName[fieldName];
                            if (currentValue != undefined && Array.isArray(currentValue))
                                currentValue = currentValue[0];

                            var containValue = false;
                            for (var j = 0; j < fieldValues.length; j++) {
                                if (currentValue == fieldValues[j]) {
                                    containValue = true;
                                    break;
                                }
                            }

                            if (!containValue)
                                return false;
                        }

                        return true;
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