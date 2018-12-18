(function (appControllers) {

    "use strict";

    TextPatternBuilderService.$inject = ['VRModalService'];

    function TextPatternBuilderService(VRModalService) {


        function openPatternHelper(onSetPattern, parts) {

            var parameter = {
                parts: parts
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSetPattern = onSetPattern;
            };
            VRModalService.showModal('/Client/Modules/Common/Directives/TextPattern/Templates/TextPatternHelperTemplate.html', parameter, modalSettings);
        }

        return {
            openPatternHelper: openPatternHelper
        };
    }

    appControllers.service('TextPatternBuilderService', TextPatternBuilderService);                                                  
})(appControllers);