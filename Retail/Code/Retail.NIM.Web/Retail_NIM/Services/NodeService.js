(function (appControllers) {

    "use strict";
    function NodeService() {
        function getNodeDefinitionId() {
            return "C20D34BE-9EDA-4DCC-8872-41A415F38468";
        }

        return {
            getNodeDefinitionId: getNodeDefinitionId
        };

    }
    appControllers.service("Retail_NIM_NodeService", NodeService);
})(appControllers);