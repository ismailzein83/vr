(function (appControllers) {

    "use strict";

    VRNamespaceItemService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'VRCommon_VRNamespaceService','VRCommon_VRNamespaceItemAPIService'];

    function VRNamespaceItemService(VRModalService, VRCommon_ObjectTrackingService, VRCommon_VRNamespaceService, VRCommon_VRNamespaceItemAPIService) {
        var drillDownDefinitions = [];

        function addVRNamespaceItem(onVRNameSpaceItemAdded, vrNamespaceId) {
            var parameters = {
                vrNamespaceId: vrNamespaceId,
            };
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRNameSpaceItemAdded = onVRNameSpaceItemAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRDynamicCodeEditor.html', parameters, settings);
        }

        function editVRNamespaceItem(onVRNameSpaceItemUpdated, vrNameSpaceItemId) {
            var settings = {};
            var parameters = {
                vrNameSpaceItemId: vrNameSpaceItemId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRNameSpaceItemUpdated = onVRNameSpaceItemUpdated;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRDynamicCodeEditor.html', parameters, settings);
        }

        function tryCompilationResult(errorMessages, namespaceObj) {
            var modalSettings = {};
            var modalParameters = {
                errorMessages: errorMessages,
                namespaceObj: namespaceObj
            };
            modalSettings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRNamespaceItemCompilationResult.html', modalParameters, modalSettings);
        }

        function registerObjectTrackingDrillDownToVRNamespaceItem() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, vrNamespaceItem) {
                vrNamespaceItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: vrNamespaceItem.VRNamespaceItemId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return vrNamespaceItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function registerNamespaceItemDrillDownDefinitionToNamespace() {
            var drillDownDefinition = {};

            drillDownDefinition.title = "Namespace Item";
            drillDownDefinition.directive = "vr-common-namespaceitem-search";
            drillDownDefinition.haspermission = function () {
                return VRCommon_VRNamespaceItemAPIService.HasGetFilteredVRNamespaceItems();
            };
            drillDownDefinition.loadDirective = function (directiveAPI, namespace) {
                namespace.nameSpaceItemGridAPI = directiveAPI;

                var query = {
                    NameSpaceId: namespace.VRNamespaceId
                };

                return namespace.nameSpaceItemGridAPI.load(query);
            };
            VRCommon_VRNamespaceService.addDrillDownDefinition(drillDownDefinition);
        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        function getEntityUniqueName() {
            return "VR_Common_VRNamespaceItem";
        }

        return {
            addVRNamespaceItem: addVRNamespaceItem,
            editVRNamespaceItem: editVRNamespaceItem,
            tryCompilationResult: tryCompilationResult,
            registerObjectTrackingDrillDownToVRNamespaceItem: registerObjectTrackingDrillDownToVRNamespaceItem,
            getDrillDownDefinition: getDrillDownDefinition,
            registerNamespaceItemDrillDownDefinitionToNamespace: registerNamespaceItemDrillDownDefinitionToNamespace
        };
    }

    appControllers.service('VRCommon_VRNamespaceItemService', VRNamespaceItemService);

})(appControllers);