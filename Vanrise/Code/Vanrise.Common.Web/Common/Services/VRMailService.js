(function (appControllers) {

    'use strict';

    VRMailService.$inject = ['VRModalService'];

    function VRMailService(VRModalService) {
        return {
            sendTestEmail: sendTestEmail
        };

        function sendTestEmail(onSendTestEmail, emailSettingData) {
            var modalParameters = {
                emailSettingData: emailSettingData
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSendTestEmail = onSendTestEmail;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/TestEmail/TestEmailEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_VRMailService', VRMailService);

})(appControllers);