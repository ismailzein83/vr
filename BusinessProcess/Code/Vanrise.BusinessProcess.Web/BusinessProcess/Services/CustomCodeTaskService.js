(function (appControllers) {

    "use strict";
    CustomCodeTaskService.$inject = ['VRModalService'];
    function CustomCodeTaskService(VRModalService) {
       

        function tryCompilationResult(errorMessages) {
            var modalSettings = {};

            var modalParameters = {
                errorMessages: errorMessages
            };

            modalSettings.onScopeReady = function (modalScope) {
            };

            VRModalService.showModal('/Client/Modules/BusinessProcess/Directives/CustomCode/Templates/CustomCodeTaskCompilationResult.html', modalParameters, modalSettings);
        };

        return ({
            tryCompilationResult:tryCompilationResult
        });
    }
    appControllers.service('BusinessProcess_CustomCodeTaskService', CustomCodeTaskService);

})(appControllers);