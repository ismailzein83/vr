(function (appControllers) {

    "use strict";
    codePreparationService.$inject = ["BaseAPIService", "UtilsService", "WhS_CP_ModuleConfig", "VRModalService", "VRNotificationService"];

    function codePreparationService(BaseAPIService, UtilsService, WhS_CP_ModuleConfig, VRModalService, VRNotificationService) {

        function NotifyValidationWarning(message) {
            VRNotificationService.showWarning(message);
        }


        return ({
            NotifyValidationWarning: NotifyValidationWarning,
           
        });
    }

    appControllers.service("WhS_CodePrep_CodePrepService", codePreparationService);
})(appControllers);