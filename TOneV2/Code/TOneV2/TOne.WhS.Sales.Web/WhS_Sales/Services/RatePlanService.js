(function (appControllers) {

    'use strict';

    RatePlanService.$inject = ['VRModalService'];

    function RatePlanService(VRModalService) {
        return {
            sellNewCountries: sellNewCountries,
            editSettings: editSettings,
            editPricingSettings: editPricingSettings,
            viewFutureRate: viewFutureRate,
            viewInvalidRates: viewInvalidRates,
            viewZoneInfo: viewZoneInfo
        };

        function sellNewCountries(customerId, countryChanges, saleAreaSettings, ratePlanSettings, onCountryChangesUpdated) {

        	var parameters = {
        		customerId: customerId,
        		countryChanges: countryChanges,
        		saleAreaSettings: saleAreaSettings,
        		ratePlanSettings: ratePlanSettings
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
            	modalScope.onCountryChangesUpdated = onCountryChangesUpdated;
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

        function viewZoneInfo(ownerType, ownerId, zoneId, zoneName, zoneBED, zoneEED, currencyId) {
            var parameters = {
                ownerType: ownerType,
                ownerId: ownerId,
                zoneId: zoneId,
                zoneName: zoneName,
                zoneBED: zoneBED,
                zoneEED: zoneEED,
                currencyId: currencyId
            };

            var settings;

            VRModalService.showModal("/Client/Modules/WhS_Sales/Views/ZoneInfo.html", parameters, settings);
        }
    }

    appControllers.service('WhS_Sales_RatePlanService', RatePlanService);

})(appControllers);