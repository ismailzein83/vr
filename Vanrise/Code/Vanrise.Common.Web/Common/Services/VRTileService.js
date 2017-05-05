
(function (appControllers) {

    'use strict';

    VRTileService.$inject = ['VRModalService', 'UtilsService'];

    function VRTileService(VRModalService, UtilsService) {
        return {
            addVRTile: addVRTile,
            editVRTile: editVRTile,
        };

        function addVRTile(onVRTileAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVRTileAdded = onVRTileAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/VRTile/Templates/VRTileEditor.html', modalParameters, modalSettings);
        }

        function editVRTile(vrTileEntity, onVRTileUpdated) {
            var modalParameters = {
                vrTileEntity: vrTileEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVRTileUpdated = onVRTileUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Directives/VRTile/Templates/VRTileEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_VRTileService', VRTileService);

})(appControllers);