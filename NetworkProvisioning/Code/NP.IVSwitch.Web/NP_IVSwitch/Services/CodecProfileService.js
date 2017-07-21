
(function (appControllers) {

    "use strict";

    CodecProfileService.$inject = ['VRModalService','VRCommon_ObjectTrackingService', 'UtilsService'];

    function CodecProfileService(NPModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];
        function addCodecProfile(onCodecProfileAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodecProfileAdded = onCodecProfileAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/CodecProfile/CodecProfileEditor.html', null, settings);
        };

        function editCodecProfile(CodecProfileId, onCodecProfileUpdated) {
            var settings = {};

            var parameters = {
                CodecProfileId: CodecProfileId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodecProfileUpdated = onCodecProfileUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/CodecProfile/CodecProfileEditor.html', parameters, settings);
        }

        function viewHistoryCodecProfile(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/CodecProfile/CodecProfileEditor.html', modalParameters, modalSettings);
        };

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "NP_IVSwitch_CodecProfile_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryCodecProfile(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "NP_IVSwitch_CodecProfile";
        }

        function registerObjectTrackingDrillDownToCodecProfile() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, codecProfileItem) {

                codecProfileItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: codecProfileItem.Entity.CodecProfileId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return codecProfileItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }
        return {
            addCodecProfile: addCodecProfile,
            editCodecProfile: editCodecProfile,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToCodecProfile: registerObjectTrackingDrillDownToCodecProfile,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('NP_IVSwitch_CodecProfileService', CodecProfileService);

})(appControllers);