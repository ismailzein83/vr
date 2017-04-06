(function (appControllers) {

    'use strict';

    SwitchConnectivityService.$inject = ['WhS_BE_SwitchConnectivityAPIService', 'VRModalService', 'VRNotificationService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function SwitchConnectivityService(WhS_BE_SwitchConnectivityAPIService, VRModalService, VRNotificationService, VRCommon_ObjectTrackingService, UtilsService) {
        var editorUrl = '/Client/Modules/WhS_BusinessEntity/Views/SwitchConnectivity/SwitchConnectivityEditor.html';
        var drillDownDefinitions = [];
        function addSwitchConnectivity(onSwitchConnectivityAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchConnectivityAdded = onSwitchConnectivityAdded;
            };

            VRModalService.showModal(editorUrl, null, settings);
        }

        function editSwitchConnectivity(switchConnectivityId, onSwitchConnectivityUpdated) {
            var parameters = {
                switchConnectivityId: switchConnectivityId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSwitchConnectivityUpdated = onSwitchConnectivityUpdated;
            };

            VRModalService.showModal(editorUrl, parameters, settings);
        }
        function getEntityUniqueName() {
            return "WhS_BusinessEntity_SwitchConnectivity";
        }

        function registerObjectTrackingDrillDownToSwitchConnectivity() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, switchConnectivityItem) {
                switchConnectivityItem.objectTrackingGridAPI = directiveAPI;
                
                var query = {
                    ObjectId: switchConnectivityItem.Entity.SwitchConnectivityId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return switchConnectivityItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "WhS_BusinessEntity_SwitchConnectivity_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistorySwitchConnectivity(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }

        function viewHistorySwitchConnectivity(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal(editorUrl, modalParameters, modalSettings);
        };
        return {
            addSwitchConnectivity: addSwitchConnectivity,
            editSwitchConnectivity: editSwitchConnectivity,
            registerObjectTrackingDrillDownToSwitchConnectivity: registerObjectTrackingDrillDownToSwitchConnectivity,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('WhS_BE_SwitchConnectivityService', SwitchConnectivityService);

})(appControllers);