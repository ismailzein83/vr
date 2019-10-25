(function (appControllers) {

    "use strict";
    function NodePortService() {
        function getNodePortDefinitionId() {
            return "04868FE5-9944-4E2B-B4D2-DE9C5F73E2F4";
        }

        return {
            getNodePortDefinitionId: getNodePortDefinitionId
        };

    }
    appControllers.service("Retail_NIM_NodePortService", NodePortService);
})(appControllers);