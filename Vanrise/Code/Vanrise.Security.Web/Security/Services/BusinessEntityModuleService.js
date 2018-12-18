(function (appControllers) {

    'use strict';

    BusinessEntityModuleService.$inject = ['VR_Sec_ViewAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function BusinessEntityModuleService(VR_Sec_ViewAPIService, UtilsService, VRModalService, VRNotificationService) {
        return ({
            addBusinessEntityModule: addBusinessEntityModule,
            updateBusinessEntityModule: updateBusinessEntityModule,
        });

        function addBusinessEntityModule(onBusinessEntityModuleAdded, parentId) {
            var modalSettings = {
            };
            var parameters = {
                parentId: parentId
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityModuleAdded = onBusinessEntityModuleAdded;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/BusinessEntity/BusinessEntityModuleEditor.html', parameters, modalSettings);
        }

        function updateBusinessEntityModule(businessEntityModuleId, onBusinessEntityModuleUpdated) {
            var modalParameters = {
                businessEntityModuleId: businessEntityModuleId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onBusinessEntityModuleUpdated = onBusinessEntityModuleUpdated;
            };

            VRModalService.showModal('/Client/Modules/Security/Views/BusinessEntity/BusinessEntityModuleEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('VR_Sec_BusinessEntityModuleService', BusinessEntityModuleService);

})(appControllers);
