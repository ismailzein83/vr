(function (appControllers) {

    'use strict';

    PriceListTemplateService.$inject = ['VRModalService', 'VRNotificationService'];

    function PriceListTemplateService(VRModalService, VRNotificationService) {
        function editPriceListTemplate(priceListTemplateId, onPriceListTemplateUpdated) {
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

        function addPriceListTemplate(onPriceListTemplateAdded) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onPriceListTemplateAdded = onPriceListTemplateAdded;
            };
            var parameters = {};

            VRModalService.showModal("/Client/Modules/XBooster_PriceListConversion/Views/PriceListTemplateEditor.html", parameters, settings);
        }

        return ({
            editPriceListTemplate: editPriceListTemplate,
            addPriceListTemplate: addPriceListTemplate
        })
    }

    appControllers.service('XBooster_PriceListConversion_PriceListTemplateService', PriceListTemplateService);

})(appControllers);
