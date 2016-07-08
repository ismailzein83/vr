(function (appControllers) {

    'use strict';

    PriceListTemplateService.$inject = ['VRModalService'];

    function PriceListTemplateService(VRModalService) {

        function saveInputPriceListTemplate(onPriceListTemplatSaved, configDetails, priceListTemplateId) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onPriceListTemplatSaved = onPriceListTemplatSaved;
            };
            var parameters = {
                ConfigDetails: configDetails,
                PriceListTemplateId: priceListTemplateId
            };

            VRModalService.showModal("/Client/Modules/XBooster_PriceListConversion/Views/InputPriceListTemplateEditor.html", parameters, settings);
        }


        return ({
            saveInputPriceListTemplate: saveInputPriceListTemplate
        })
    }

    appControllers.service('WhS_SupPL_PriceListTemplateService', PriceListTemplateService);

})(appControllers);
