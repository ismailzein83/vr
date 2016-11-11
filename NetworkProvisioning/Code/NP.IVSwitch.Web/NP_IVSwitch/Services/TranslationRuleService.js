
(function (appControllers) {

    "use strict";

    TranslationRuleService.$inject = ['VRModalService'];

    function TranslationRuleService(NPModalService) {

        function addTranslationRule(onTranslationRuleAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onTranslationRuleAdded = onTranslationRuleAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/TranslationRule/TranslationRuleEditor.html', null, settings);
        };

        function editTranslationRule(TranslationRuleId, onTranslationRuleUpdated) {
            var settings = {};

            var parameters = {
                TranslationRuleId: TranslationRuleId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onTranslationRuleUpdated = onTranslationRuleUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/TranslationRule/TranslationRuleEditor.html', parameters, settings);
        }

        return {
            addTranslationRule: addTranslationRule,
            editTranslationRule: editTranslationRule
        };
    }

    appControllers.service('NP_IVSwitch_TranslationRuleService', TranslationRuleService);

})(appControllers);