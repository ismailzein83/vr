(function (appControllers) {

    'use strict';

    GenericEditorConditionalRuleContainerService.$inject = ['UtilsService'];

    function GenericEditorConditionalRuleContainerService(UtilsService) {

        var conditionalRuleActions = [];

        function registerConditionalRuleAction(conditionalRuleAction) {
            conditionalRuleActions.push(conditionalRuleAction);
        }

        function registerFieldValueConditionalRuleAction() {
            var fieldValueConditionalRuleAction = {
                ActionTypeName: "FieldValueConditionalRuleAction",
                ExecuteAction: function (payload) {
                    if (payload == undefined || payload.genericEditorConditionalRule == undefined)
                        return;

                    var genericEditorConditionalRule = payload.genericEditorConditionalRule;

                    
                }
            };

            registerConditionalRuleAction(fieldValueConditionalRuleAction);
        }

        return {
            registerConditionalRuleAction: registerConditionalRuleAction,
            registerFieldValueConditionalRuleAction: registerFieldValueConditionalRuleAction
        };
    }

    appControllers.service('VR_GenericData_GenericEditorConditionalRuleContainerService', GenericEditorConditionalRuleContainerService);

})(appControllers);

