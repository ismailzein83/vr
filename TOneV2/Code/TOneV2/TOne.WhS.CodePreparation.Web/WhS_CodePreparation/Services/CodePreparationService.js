(function (appControllers) {

    "use strict";
    codePreparationService.$inject = ["VRNotificationService"];

    function codePreparationService(VRNotificationService) {

        function NotifyValidationWarning(message) {
            VRNotificationService.showWarning(message);
        }


        return ({
            NotifyValidationWarning: NotifyValidationWarning,
           
        });
    }

    appControllers.service("WhS_CP_CodePrepService", codePreparationService);
})(appControllers);