(function (appControllers) {

    'use strict';

    ZoneService.$inject = ['VRCommon_CountryService'];

    function ZoneService(VRCommon_CountryService) {
        return {
            registerDrillDownToCountry: registerDrillDownToCountry
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
    }
    appControllers.service('Qm_BE_ZoneService', ZoneService);

})(appControllers);