(function (appControllers) {

    'use stict';

    ActionRuntimeService.$inject = ['VRModalService'];

    function ActionRuntimeService(VRModalService) {

        function openActionRuntime(accountId,actionDefinitionId) {
            var settings = {};
            settings.onScopeReady = function (modalScope) {
            };
            var parameters = {
                accountId: accountId,
                actionDefinitionId: actionDefinitionId
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Action/Runtime/ActionRuntimeEditor.html', parameters, settings);
        };

        return {
            openActionRuntime: openActionRuntime,
        };
    }

    appControllers.service('Retail_BE_ActionRuntimeService', ActionRuntimeService);

})(appControllers);