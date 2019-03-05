(function (appControllers) {

    'use strict';

    toneModuleService.$inject = ['UISettingsService'];

    function toneModuleService(UISettingsService) {

        function isVoiceModuleEnabled() {
            return UISettingsService.getUIParameterValue("IsVoiceModuleEnabled");
        }
        function isSMSModuleEnabled() {
            return UISettingsService.getUIParameterValue("IsSMSModuleEnabled");
        }
        return ({
            isSMSModuleEnabled: isSMSModuleEnabled,
            isVoiceModuleEnabled: isVoiceModuleEnabled
        });
    }

    appControllers.service('WhS_BE_ToneModuleService', toneModuleService);

})(appControllers);
