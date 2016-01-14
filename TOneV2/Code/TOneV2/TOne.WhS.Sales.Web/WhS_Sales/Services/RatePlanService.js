app.service("WhS_Sales_RatePlanService", ["VRModalService", function (VRModalService) {

    return ({
        sellNewCountries: sellNewCountries,
        editSettings: editSettings,
        editPricingSettings: editPricingSettings,
        viewChanges: viewChanges
    });

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

    function viewChanges(ownerType, ownerId, onRatePlanChangesClose) {
        var parameters = {
            OwnerType: ownerType,
            OwnerId: ownerId
        };

        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Rate Plan Changes";
            modalScope.onRatePlanChangesClose = onRatePlanChangesClose;
        };

        VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanChanges.html", parameters, settings);
    }
}]);
