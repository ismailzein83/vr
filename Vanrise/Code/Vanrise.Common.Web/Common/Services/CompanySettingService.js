(function (appControllers) {

    'use strict';

    CompanySettingService.$inject = ['VRModalService', 'UtilsService'];

    function CompanySettingService(VRModalService, UtilsService) {
        return {
            addCompanySetting: addCompanySetting,
            editCompanySetting: editCompanySetting,
            viewCompanySetting: viewCompanySetting
        };

        function addCompanySetting(onCompanySettingsAdded, setDefault, context, isSingleInsert) {
            var modalParameters = {
                setDefault: setDefault,
                context: context,
                isSingleInsert: isSingleInsert
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCompanySettingsAdded = onCompanySettingsAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanySettings/CompanySettingsEditor.html', modalParameters, modalSettings);
        }

        function editCompanySetting(companySettingEntity, onCompanySettingsUpdated, context) {
            var modalParameters = {
                companySettingEntity: companySettingEntity,
                context: context
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCompanySettingsUpdated = onCompanySettingsUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanySettings/CompanySettingsEditor.html', modalParameters, modalSettings);
        }

        function viewCompanySetting(companySettingEntity, context) {
            var modalParameters = {
                companySettingEntity: companySettingEntity,
                context: context
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