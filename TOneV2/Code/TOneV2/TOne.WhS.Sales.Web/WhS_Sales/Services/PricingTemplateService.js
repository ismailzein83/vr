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
            var settings = {};

            var parameters = {
                pricingTemplateId: pricingTemplateId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onPricingTemplateUpdated = onPricingTemplateUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/PricingTemplate/PricingTemplateEditor.html', parameters, settings);
        }

        function addPricingTemplateRule(onPricingTemplateRuleAdded) {

            var parameters = {};

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingTemplateRuleAdded = onPricingTemplateRuleAdded
            };

            VRModalService.showModal('/Client/Modules/WhS_Sales/Views/PricingTemplate/PricingTemplateRuleEditor.html', parameters, modalSettings);
        };
        function editPricingTemplateRule(pricingTemplateRule, onPricingTemplateUpdated) {

            var parameters = {
                pricingTemplateRule: pricingTemplateRule
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onColumnDefinitionUpdated = onPricingTemplateUpdated;
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