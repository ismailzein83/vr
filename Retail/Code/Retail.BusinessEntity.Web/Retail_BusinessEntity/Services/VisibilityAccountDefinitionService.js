﻿(function (appControllers) {

    'use stict';

    VisibilityAccountDefinition.$inject = ['VRModalService'];

    function VisibilityAccountDefinition(VRModalService) {

        function addVisibilityAccountDefinition(onVisibilityAccountDefinitionAdded) {

            var parameters = {
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountDefinitionAdded = onVisibilityAccountDefinitionAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionEditor.html', parameters, modalSettings);
        };
        function editVisibilityAccountDefinition(visibilityAccountDefinition, retailBEVisibilityEditorRuntime, onVisibilityAccountDefinitionUpdated) {

            var parameters = {
                visibilityAccountDefinition: visibilityAccountDefinition,
                retailBEVisibilityEditorRuntime: retailBEVisibilityEditorRuntime
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountDefinitionUpdated = onVisibilityAccountDefinitionUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/Templates/VisibilityAccountDefinitionEditor.html', parameters, modalSettings);
        }

        function addVisibilityAccountType(accountBEDefinitionId, onVisibilityAccountTypeAdded) {

            var parameters = {
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountTypeAdded = onVisibilityAccountTypeAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionAccountTypes/Templates/VisibilityAccountTypeEditor.html', parameters, modalSettings);
        };
        function editVisibilityAccountType(visibilityAccountType, accountBEDefinitionId, onVisibilityAccountTypeUpdated) {

            var parameters = {
                visibilityAccountType: visibilityAccountType,
                accountBEDefinitionId: accountBEDefinitionId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onVisibilityAccountTypeUpdated = onVisibilityAccountTypeUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionAccountTypes/Templates/VisibilityAccountTypeEditor.html', parameters, modalSettings);
        }


        return {
            addVisibilityAccountDefinition: addVisibilityAccountDefinition,
            editVisibilityAccountDefinition: editVisibilityAccountDefinition,
            addVisibilityAccountType: addVisibilityAccountType,
            editVisibilityAccountType: editVisibilityAccountType
        };
    }

    appControllers.service('Retail_BE_VisibilityAccountDefinitionService', VisibilityAccountDefinition);

})(appControllers);