
app.service('VRCommon_OverriddenConfigService', ['VRModalService', 'VRNotificationService', 'UtilsService', 'VRCommon_OverriddenConfigGroupService', 'VRCommon_OverriddenConfigAPIService', 'VRCommon_ObjectTrackingService',
    function (VRModalService, VRNotificationService, UtilsService, VRCommon_OverriddenConfigGroupService, VRCommon_OverriddenConfigAPIService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            editOverriddenConfig: editOverriddenConfig,
            addOverriddenConfig: addOverriddenConfig,
            registerObjectTrackingDrillDownToOverreddinConfig: registerObjectTrackingDrillDownToOverreddinConfig,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction
        });

        function viewHistoryOverreddinConfig(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Common/Views/OverriddenConfig/OverriddenConfigEditor.html', modalParameters, modalSettings);
        };
        function editOverriddenConfig(overriddenConfigId, onOverriddenConfigUpdated) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onOverriddenConfigUpdated = onOverriddenConfigUpdated;
            };
            var parameters = {
                OverriddenConfigId: overriddenConfigId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/OverriddenConfig/OverriddenConfigEditor.html', parameters, settings);
        }
        function addOverriddenConfig(onOverriddenConfigAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onOverriddenConfigAdded = onOverriddenConfigAdded;
            };
            var parameters = {};
           
            VRModalService.showModal('/Client/Modules/Common/Views/OverriddenConfig/OverriddenConfigEditor.html', parameters, settings);
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Common_OverriddenConfig_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryOverreddinConfig(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "VR_Common_OverriddenConfig";
        }

        function registerObjectTrackingDrillDownToOverreddinConfig() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, overreddinConfigItem) {

                overreddinConfigItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: overreddinConfigItem.OverriddenConfigurationId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return overreddinConfigItem.objectTrackingGridAPI.load(query);
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
