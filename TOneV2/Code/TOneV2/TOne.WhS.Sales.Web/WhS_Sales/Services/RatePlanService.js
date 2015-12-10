app.service("WhS_Sales_RatePlanService", ["VRModalService", function (VRModalService) {

    return ({
        sellNewZones: sellNewZones,
        editSettings: editSettings,
        viewChanges: viewChanges
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

    function viewChanges(ownerType, ownerId) {
        var parameters = {
            OwnerType: ownerType,
            OwnerId: ownerId
        };

        var settings = {};
        settings.onScopeReady = function (modalScope) {
            modalScope.title = "Rate Plan Recent Changes";
        };

        VRModalService.showModal("/Client/Modules/WhS_Sales/Views/RatePlanChanges.html", parameters, settings);
    }
}]);
