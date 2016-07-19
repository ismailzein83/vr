﻿(function (appControllers) {

    'use stict';

    ActionRuntimeService.$inject = ['VRModalService'];

    function ActionRuntimeService(VRModalService) {

        function openActionRuntime(entityId, actionDefinitionId, reloadMethod) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
            };
            var parameters = {
                entityId: entityId,
                actionDefinitionId: actionDefinitionId,
                reloadMethod: reloadMethod
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Runtime/ActionRuntimeEditor.html', parameters, settings);
        };

        return {
            openActionRuntime: openActionRuntime,
        };
    }

    appControllers.service('Retail_BE_ActionRuntimeService', ActionRuntimeService);

})(appControllers);