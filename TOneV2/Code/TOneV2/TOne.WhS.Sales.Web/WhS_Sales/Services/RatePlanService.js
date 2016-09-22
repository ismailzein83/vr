(function (appControllers) {

    'use strict';

    RatePlanService.$inject = ['VRModalService'];

    function RatePlanService(VRModalService) {
        return {
            sellNewCountries: sellNewCountries,
            editSettings: editSettings,
            editPricingSettings: editPricingSettings,
            viewFutureRate: viewFutureRate,
            viewInvalidRates: viewInvalidRates
        };

        function sellNewCountries(customerId, onCountriesSold) {
            var parameters = {
                CustomerId: customerId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCountriesSold = onCountriesSold;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/SellNewCountries.html", parameters, settings);
        }

        function editSettings(settings, onSettingsUpdated) {
            var parameters = {
                settings: settings
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSettingsUpdated = onSettingsUpdated;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanSettings.html", parameters, modalSettings);
        }

        function editPricingSettings(ratePlanSettings, pricingSettings, onPricingSettingsUpdated) {
            var parameters = {
                ratePlanSettings: ratePlanSettings,
                pricingSettings: pricingSettings
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onPricingSettingsUpdated = onPricingSettingsUpdated;
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanPricingSettings.html", parameters, modalSettings);
        }

        function viewFutureRate(zoneName, futureRate)
        {
            var parameters = {
                zoneName: zoneName,
                futureRate: futureRate
            };

            var settings = {};

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/FutureRate.html", parameters, settings);
        }

        function viewInvalidRates(calculatedRates, onSaved)
        {
            var parameters = {
                calculatedRates: calculatedRates
            };

            var settings = {
                onScopeReady: function (modalScope) {
                    modalScope.onSaved = onSaved;
                }
            };

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/InvalidRate.html", parameters, settings);
        }
    }

    appControllers.service('WhS_Sales_RatePlanService', RatePlanService);

})(appControllers);
