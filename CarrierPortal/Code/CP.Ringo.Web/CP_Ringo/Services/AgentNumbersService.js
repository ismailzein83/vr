(function (appControllers) {

    'use stict';

    AgentNumbersService.$inject = ['VRModalService'];

    function AgentNumbersService(VRModalService) {

        function addAgentNumbers(onAgentNumbersAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onAgentNumbersAdded = onAgentNumbersAdded
            };

            VRModalService.showModal('/Client/Modules/CP_Ringo/Views/AgentNumbers/AgentNumbersEditor.html', null, settings);
        };

        return {
            addAgentNumbers: addAgentNumbers,
        };
    }

    appControllers.service('CP_Ringo_AgentNumbersService', AgentNumbersService);

})(appControllers);