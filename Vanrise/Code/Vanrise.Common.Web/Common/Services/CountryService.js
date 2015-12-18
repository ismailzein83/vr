
app.service('VRCommon_CountryService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {
        var drillDownDefinitions = [];
        return ({
            editCountry: editCountry,
            addCountry: addCountry,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            uploadCountrires: uploadCountrires

        });
        function editCountry(countryId, onCountryUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCountryUpdated = onCountryUpdated;
            };
            var parameters = {
                CountryId: countryId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryEditor.html', parameters, settings);
        }
        function addCountry(onCountryAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCountryAdded = onCountryAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryEditor.html', parameters, settings);
        }

        function uploadCountrires() {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.title = "Upload Countrires";
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryUploader.html', parameters, settings);
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
