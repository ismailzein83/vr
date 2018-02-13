
app.service('VRCommon_CountryService', ['VRModalService', 'VRNotificationService', 'UtilsService','VRCommon_ObjectTrackingService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            editCountry: editCountry,
            addCountry: addCountry,
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition: getDrillDownDefinition,
            uploadCountrires: uploadCountrires,
            getEntityUniqueName: getEntityUniqueName,
            registerHistoryViewAction: registerHistoryViewAction
          

        });

        function viewHistoryCountry(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryEditor.html', modalParameters, modalSettings);
        };

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
                modalScope.title = "Upload Countries";
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/Common/Views/Country/CountryUploader.html', parameters, settings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Common_Country_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryCountry(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
           
        }

        function getEntityUniqueName() {
            return "VR_Common_Country";
        }

       
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    }]);
