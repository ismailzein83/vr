
(function (appControllers) {

    "use strict";

    CodecDefService.$inject = ['VRModalService'];

    function CodecDefService(NPModalService) {

        function addCodecDef(onCodecDefAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onCodecDefAdded = onCodecDefAdded
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/CodecDef/CodecDefEditor.html', null, settings);
        };

        return {
            addCodecDef: addCodecDef,
         };
    }

    appControllers.service('NP_IVSwitch_CodecDefService', CodecDefService);

})(appControllers);