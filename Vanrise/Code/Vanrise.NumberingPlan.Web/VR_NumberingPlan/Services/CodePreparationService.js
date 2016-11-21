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

    appControllers.service("Vr_NP_CodePrepService", codePreparationService);
})(appControllers);