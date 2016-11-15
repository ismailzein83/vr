(function (appControllers) {

    'use strict';

    CompanySettingService.$inject = ['VRModalService'];

    function CompanySettingService(VRModalService) {
        return {
            addCompanySetting: addCompanySetting,
            editCompanySetting: editCompanySetting
        };

        function addCompanySetting(onCompanySettingsAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCompanySettingsAdded = onCompanySettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanySettings/CompanySettingsEditor.html', modalParameters, modalSettings);
        }

        function editCompanySetting(companySettingEntity, onCompanySettingsUpdated) {
            var modalParameters = {
                companySettingEntity: companySettingEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCompanySettingsUpdated = onCompanySettingsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanySettings/CompanySettingsEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_CompanySettingService', CompanySettingService);

})(appControllers);