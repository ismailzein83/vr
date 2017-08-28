
(function (appControllers) {

    'use stict';

    GenericLKUPService.$inject = ['VRModalService',  'UtilsService', 'VRCommon_ObjectTrackingService'];

    function GenericLKUPService(VRModalService,  UtilsService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addGenericLKUP(onGenericLKUPAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericLKUPAdded = onGenericLKUPAdded
            };

            VRModalService.showModal('/Client/Modules/Common/Views/GenericLKUP/GenericLKUPEditor.html', null, settings);
        }

        function editGenericLKUP(genericLKUPItemId, onGenericLKUPUpdated) {
            var settings = {};

            var parameters = {
                genericLKUPItemId: genericLKUPItemId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onGenericLKUPUpdated = onGenericLKUPUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/GenericLKUP/GenericLKUPEditor.html', parameters, settings);
        }
       
        function viewHistoryGenericLKUP(context) {
            var parameters = {
                context: context
            };
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            VRModalService.showModal('/Client/Modules/Common/Views/GenericLKUP/GenericLKUPEditor.html', parameters, settings);
        };

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "VR_Common_GenericLKUPItem_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryGenericLKUP(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "VR_Common_GenericLKUPItem";
        }
        function registerObjectTrackingDrillDownToGenericLKUP() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, genericLKUPItem) {

                genericLKUPItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: genericLKUPItem.Entity.GenericLKUPItemId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return genericLKUPItem.objectTrackingGridAPI.load(query);
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
            addGenericLKUP: addGenericLKUP,
            editGenericLKUP: editGenericLKUP,
            registerObjectTrackingDrillDownToGenericLKUP: registerObjectTrackingDrillDownToGenericLKUP,
            getDrillDownDefinition: getDrillDownDefinition,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('VR_Common_GenericLKUPService', GenericLKUPService);

})(appControllers);