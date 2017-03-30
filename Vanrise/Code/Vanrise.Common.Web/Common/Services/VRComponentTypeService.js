
(function (appControllers) {

    "use strict";

    VRComponentTypeService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function VRComponentTypeService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        function addVRComponentType(extensionConfigId, onVRComponentTypeAdded) {
            var settings = {};
            var parameters = {
                extensionConfigId: extensionConfigId,
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onVRComponentTypeAdded = onVRComponentTypeAdded
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRComponentType/VRComponentTypeEditor.html', parameters, settings);
        };

        function getEntityUniqueName(vrComponentTypeConfigId) {
            return "VR_Common_ComponentType_" + vrComponentTypeConfigId;
        }

        function registerObjectTrackingDrillDownToVRComponentType() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrComponentTypeItem) {
                vrComponentTypeItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrComponentTypeItem.Entity.VRComponentTypeId,
                    EntityUniqueName: getEntityUniqueName(vrComponentTypeItem.Entity.Settings.VRComponentTypeConfigId),

                };

                return vrComponentTypeItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function editVRComponentType(extensionConfigId, vrComponentTypeId, onVRComponentTypeUpdated) {
            var settings = {};

            var parameters = {
                vrComponentTypeId: vrComponentTypeId,
                extensionConfigId: extensionConfigId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRComponentTypeUpdated = onVRComponentTypeUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRComponentType/VRComponentTypeEditor.html', parameters, settings);
        }


        return {
            addVRComponentType: addVRComponentType,
            editVRComponentType: editVRComponentType,
            registerObjectTrackingDrillDownToVRComponentType: registerObjectTrackingDrillDownToVRComponentType,
            getDrillDownDefinition: getDrillDownDefinition
        };
    }

    appControllers.service('VRCommon_VRComponentTypeService', VRComponentTypeService);

})(appControllers);