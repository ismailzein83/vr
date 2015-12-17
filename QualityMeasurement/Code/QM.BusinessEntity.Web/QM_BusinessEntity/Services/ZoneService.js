
app.service('Qm_BE_ZoneService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_CountryService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_CountryService) {
        return ({
            registerDrillDownToCountry: registerDrillDownToCountry
        });

        function registerDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Zones";
            drillDownDefinition.directive = "vr-qm-be-zone-grid";

            drillDownDefinition.loadDirective = function (directiveAPI, countryItem) {
                countryItem.codeGroupGridAPI = directiveAPI;
                var query = {
                    CountriesIds: [countryItem.Entity.CountryId],
                };
                return countryItem.codeGroupGridAPI.loadGrid(query);
            };

            VRCommon_CountryService.addDrillDownDefinition(drillDownDefinition);
        }

    }]);
