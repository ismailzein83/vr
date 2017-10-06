(function (appControllers) {

    "use strict";

    PricingTemplateService.$inject = ['VRModalService'];

    function PricingTemplateService(VRModalService) {

        function addPricingTemplate(onPricingTemplateAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingTemplateAdded = onPricingTemplateAdded
            };
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/PricingTemplate/PricingTemplateEditor.html', null, settings);
        };
        function editPricingTemplate(pricingTemplateId, onPricingTemplateUpdated) {

            var parameters = {
                pricingTemplateId: pricingTemplateId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingTemplateUpdated = onPricingTemplateUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/PricingTemplate/PricingTemplateEditor.html', parameters, modalSettings);
        }

        function addPricingTemplateRule(context, onPricingTemplateRuleAdded) {

            var parameters = {
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingTemplateRuleAdded = onPricingTemplateRuleAdded
            };

            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/PricingTemplate/PricingTemplateRuleEditor.html', parameters, modalSettings);
        };
        function editPricingTemplateRule(pricingTemplateRule, context, onPricingTemplateRuleUpdated) {

            var parameters = {
                pricingTemplateRule: pricingTemplateRule,
                context: context
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingTemplateRuleUpdated = onPricingTemplateRuleUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/PricingTemplate/PricingTemplateRuleEditor.html', parameters, modalSettings);
        }

        return {
            addPricingTemplate: addPricingTemplate,
            editPricingTemplate: editPricingTemplate,
            addPricingTemplateRule: addPricingTemplateRule,
            editPricingTemplateRule: editPricingTemplateRule
        };
    }

    appControllers.service('WhS_Sales_PricingTemplateService', PricingTemplateService);

})(appControllers);