app.service("WhS_Sales_MainService", ["VRModalService", function (VRModalService) {

    return ({
        sellNewZones: sellNewZones,
        editSettings: editSettings
    });

    function sellNewZones(customerId, onCustomerZonesSold) {
        var parameters = {
            CustomerId: customerId
        };

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.onCustomerZonesSold = onCustomerZonesSold;
        };

        VRModalService.showModal("/Client/Modules/WhS_Sales/Views/SellNewZones.html", parameters, settings);
    }

    function editSettings(settings, onSettingsUpdate) {
        var parameters = {
            settings: settings
        };

        var modalSettings = {};

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.onSettingsUpdate = onSettingsUpdate;
        };

        VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanSettings.html", parameters, modalSettings);
    }
}]);
