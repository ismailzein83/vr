(function (appControllers) {

    "use strict";

    VRNamespaceItemService.$inject = ['VRModalService'];

    function VRNamespaceItemService(VRModalService) {

        function addVRNamespaceItem(onVRNameSpaceItemAdded, vrNamespaceId, isGridOpenedFromGridDrillDown) {
            var parameters = {
            vrNamespaceId: vrNamespaceId,
            isGridOpenedFromGridDrillDown: isGridOpenedFromGridDrillDown
            }
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRNameSpaceItemAdded = onVRNameSpaceItemAdded;
            };
            VRModalService.showModal('/Client/Modules/Common/Views/VRNamespace/VRDynamicCodeEditor.html', parameters, settings);
        }

        function editVRNamespaceItem(onVRNameSpaceItemUpdated, vrNameSpaceItemId, vrNamespaceId, isGridOpenedFromGridDrillDown) {
            var settings = {};
            var parameters = {
                vrNameSpaceItemId: vrNameSpaceItemId,
                vrNamespaceId:vrNamespaceId,
                isGridOpenedFromGridDrillDown: isGridOpenedFromGridDrillDown
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

        return {
            addVRNamespaceItem: addVRNamespaceItem,
            editVRNamespaceItem: editVRNamespaceItem,
            tryCompilationResult: tryCompilationResult
        };
    }

    appControllers.service('VRCommon_VRNamespaceItemService', VRNamespaceItemService);

})(appControllers);