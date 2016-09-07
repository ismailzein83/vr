(function (appControllers) {

    'use strict';

    VRConcatenatedPartService.$inject = ['VRModalService'];

    function VRConcatenatedPartService(VRModalService) {
        return {
            addConcatenatedPart: addConcatenatedPart,
            editConcatenatedPart: editConcatenatedPart
        };

        function addConcatenatedPart(onConcatenatedPartAdded, context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConcatenatedPartAdded = onConcatenatedPartAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRConcatenatedPart/VRConcatenatedPartEditor.html', modalParameters, modalSettings);
        }

        function editConcatenatedPart(concatenatedPartEntity, onConcatenatedPartUpdated, context) {
            var modalParameters = {
                concatenatedPartEntity: concatenatedPartEntity,
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onConcatenatedPartUpdated = onConcatenatedPartUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/VRConcatenatedPart/VRConcatenatedPartEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_VRConcatenatedPartService', VRConcatenatedPartService);

})(appControllers);