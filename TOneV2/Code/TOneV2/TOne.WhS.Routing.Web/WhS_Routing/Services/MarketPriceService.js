
(function (appControllers) {

    "use strict";

    MarketPriceService.$inject = ['VRModalService'];

    function MarketPriceService(VRModalService) {

        function addMarketPrice(marketPrices, onMarketPriceAdded) {
            var settings = {};

            var parameters = {
                marketPrices: marketPrices
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onMarketPriceAdded = onMarketPriceAdded
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleSettings/MarketPriceEditor.html', parameters, settings);
        };

        function editMarketPrice(marketPriceObj, marketPrices, onMarketPriceUpdated) {
            var settings = {};

            var parameters = {
                marketPrice: marketPriceObj,
                marketPrices: marketPrices
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onMarketPriceUpdated = onMarketPriceUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Views/RouteOptionRule/RouteOptionRuleSettings/MarketPriceEditor.html', parameters, settings);
        }


        return {
            addMarketPrice: addMarketPrice,
            editMarketPrice: editMarketPrice
        };
    }

    appControllers.service('WhS_Routing_MarketPriceService', MarketPriceService);

})(appControllers);