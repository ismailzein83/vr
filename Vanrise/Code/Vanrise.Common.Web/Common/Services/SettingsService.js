
app.service('VRCommon_SettingsService', ['VRModalService', 'UtilsService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, UtilsService,VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];

        function editSettings(settingsId, onSettingsUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSettingsUpdated = onSettingsUpdated;
            };
            var parameters = {
                settingsId: settingsId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Settings/SettingsEditor.html', parameters, settings);
        }
        function viewSettings(settingsId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            var parameters = {
                settingsId: settingsId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Settings/SettingsEditor.html', parameters, settings);
        }


        function viewHistorySettings(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Common/Views/Settings/SettingsEditor.html', modalParameters, modalSettings);
        };
        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Common_Setting_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistorySettings(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }
        function getEntityUniqueName() {
            return "VR_Common_Setting";
        }
        function registerObjectTrackingDrillDownToSetting() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, settingsItem) {

                settingsItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: settingsItem.Entity.SettingId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return settingsItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
        return ({
            editSettings: editSettings,
            viewSettings: viewSettings,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToSetting: registerObjectTrackingDrillDownToSetting,
            registerHistoryViewAction: registerHistoryViewAction
        });
    }]);
