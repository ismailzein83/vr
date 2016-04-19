(function (appControllers) {

    'use strict';

    PriceListTemplateService.$inject = ['VRModalService'];

    function PriceListTemplateService(VRModalService) {

        function editOutputPriceListTemplate(priceListTemplateId, onPriceListTemplateUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onPriceListTemplateUpdated = onPriceListTemplateUpdated;
            };
            var parameters = {
                PriceListTemplateId: priceListTemplateId
            };

            VRModalService.showModal("/Client/Modules/XBooster_PriceListConversion/Views/PriceListTemplateEditor.html", parameters, settings);
        }

        function addOutputPriceListTemplate(onPriceListTemplateAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onPriceListTemplateAdded = onPriceListTemplateAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/XBooster_PriceListConversion/Views/PriceListTemplateEditor.html", parameters, settings);
        }

        function openOutputPriceListTemplates(onOutputPriceListTemplateChoosen) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onOutputPriceListTemplateChoosen = onOutputPriceListTemplateChoosen;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/XBooster_PriceListConversion/Views/OutputPriceListTemplateEditor.html", parameters, settings);
        }

        function saveInputPriceListTemplate(onPriceListTemplatSaved, configDetails, priceListTemplateId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onPriceListTemplatSaved = onPriceListTemplatSaved;
            };
            var parameters = {
                ConfigDetails:configDetails,
                PriceListTemplateId: priceListTemplateId
            };

            VRModalService.showModal("/Client/Modules/XBooster_PriceListConversion/Views/InputPriceListTemplateEditor.html", parameters, settings);
        }


        return ({
            editOutputPriceListTemplate: editOutputPriceListTemplate,
            addOutputPriceListTemplate: addOutputPriceListTemplate,
            openOutputPriceListTemplates: openOutputPriceListTemplates,
            saveInputPriceListTemplate: saveInputPriceListTemplate
        })
    }

    appControllers.service('XBooster_PriceListConversion_PriceListTemplateService', PriceListTemplateService);

})(appControllers);
