
(function (appControllers) {

    "use strict";

    CodecProfileService.$inject = ['VRModalService'];

    function CodecProfileService(NPModalService) {

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


        return {
            addCodecProfile: addCodecProfile,
            editCodecProfile: editCodecProfile
        };
    }

    appControllers.service('NP_IVSwitch_CodecProfileService', CodecProfileService);

})(appControllers);