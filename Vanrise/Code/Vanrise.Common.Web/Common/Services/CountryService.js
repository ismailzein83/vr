﻿
app.service('VRCommon_CountryService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {

    return ({   
        editCountry: editCountry,
        addCountry: addCountry
       
    });
    function editCountry(obj, onCountryUpdated) {
        var settings = {
            useModalTemplate: true

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Country");
            modalScope.onCountryUpdated = onCountryUpdated;
        };
        var parameters = {
            CountryId: obj.CountryId
        };

        VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryEditor.html', parameters, settings);
    }
    function addCountry(onCountryAdded) {
        var settings = {
            useModalTemplate: true

        };

        settings.onScopeReady = function (modalScope) {
            modalScope.title = UtilsService.buildTitleForAddEditor("Country");
            modalScope.onCountryAdded = onCountryAdded;
        };
        var parameters = {};

        VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryEditor.html', parameters, settings);
    }
}]);
