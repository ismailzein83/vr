(function (appControllers) {

    'use strict';

    CompanySettingService.$inject = ['VRModalService','UtilsService'];

    function CompanySettingService(VRModalService, UtilsService) {
        return {
            addCompanySetting: addCompanySetting,
            editCompanySetting: editCompanySetting,
            viewCompanySetting: viewCompanySetting
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

        function viewCompanySetting(companySettingEntity) {
            var modalParameters = {
                companySettingEntity: companySettingEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanySettings/CompanySettingsEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_CompanySettingService', CompanySettingService);

})(appControllers);