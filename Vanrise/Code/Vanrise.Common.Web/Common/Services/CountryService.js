
app.service('VRCommon_CountryService', ['VRModalService', 'VRNotificationService', 'UtilsService','VRCommon_ObjectTrackingService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            editCountry: editCountry,
            addCountry: addCountry,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            uploadCountrires: uploadCountrires,
            registerObjectTrackingDrillDownToCountry: registerObjectTrackingDrillDownToCountry

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

        function getEntityUniqueName() {
            return "VR_Common_Country";
        }

        function registerObjectTrackingDrillDownToCountry() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI,countryItem) {
               
                countryItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: countryItem.Entity.CountryId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return countryItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
