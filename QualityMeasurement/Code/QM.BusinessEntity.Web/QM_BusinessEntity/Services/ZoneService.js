(function (appControllers) {

    'use strict';

    ZoneService.$inject = ['LabelColorsEnum', 'VRCommon_CountryService'];

    function ZoneService(LabelColorsEnum, VRCommon_CountryService) {
        return {
            registerDrillDownToCountry: registerDrillDownToCountry,
            getIsOfflineColor: getIsOfflineColor
        };

        function registerDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Zones";
            drillDownDefinition.directive = "vr-qm-be-zone-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {
                countryItem.codeGroupGridAPI = directiveAPI;
                var query = {
                    CountryIds: [countryItem.Entity.CountryId],
                };
                return countryItem.codeGroupGridAPI.loadGrid(query);
            };

            VRCommon_CountryService.addDrillDownDefinition(drillDownDefinition);
        }

        function getIsOfflineColor(value) {
            switch (value) {
                case false:
                    return LabelColorsEnum.Success.color;
                case true:
                    return LabelColorsEnum.Error.color;
                default:
                    return undefined;
            }
        }
    }

    appControllers.service('Qm_BE_ZoneService', ZoneService);

})(appControllers);