(function (appControllers) {

    'use strict';

    CompanyDefinitionService.$inject = ['VRModalService', 'UtilsService'];

    function CompanyDefinitionService(VRModalService, UtilsService) {
        return {
            addCompanyDefinition: addCompanyDefinition,
            editCompanyDefinition: editCompanyDefinition,
        };

        function addCompanyDefinition(onCompanyDefinitionAdded) {
            var modalParameters = {
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCompanyDefinitionAdded = onCompanyDefinitionAdded;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanyDefinition/CompanyDefinitionEditor.html', modalParameters, modalSettings);
        }

        function editCompanyDefinition(companyDefinitionEntity, onCompanyDefinitionUpdated) {
            var modalParameters = {
                companyDefinitionEntity: companyDefinitionEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onCompanyDefinitionUpdated = onCompanyDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Common/Views/CompanyDefinition/CompanyDefinitionEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VRCommon_CompanyDefinitionService', CompanyDefinitionService);

})(appControllers);