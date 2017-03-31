(function (appControllers) {

    'use strict';

    RingoAgentNumberRequestService.$inject = ['Retail_Ringo_AgentNumberRequestAPIService', 'VRModalService', 'VRNotificationService'];

    function RingoAgentNumberRequestService(Retail_Ringo_AgentNumberRequestAPIService, VRModalService, VRNotificationService) {
        return {
            rejectAgentNumberRequest: rejectAgentNumberRequest,
            viewNumbersRequest: viewNumbersRequest
        };

        function viewNumbersRequest(agentNumberRequestId, onAgentNumberRequestProcessed)
        {
            var modalParameters = {
                agentNumberRequestId: agentNumberRequestId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAgentNumberRequestProcessed = onAgentNumberRequestProcessed;
            };

            VRModalService.showModal('/Client/Modules/Retail_Ringo/Views/RingoAgentRequestNumber/RingoAgentRequestNumbeEditor.html', modalParameters, modalSettings);

        }

        // scope is the grid's $scope
        function rejectAgentNumberRequest(scope, object, onAgentNumberRequestRejected) {
            VRNotificationService.showConfirmation().then(function (confirmed) {
                if (confirmed) {
                    Retail_Ringo_AgentNumberRequestAPIService.UpdateAgentNumberRequest(object).then(function (response) {
                        if (response) {
                            var deleted = VRNotificationService.notifyOnItemDeleted('Numbers Request', response);

                            if (deleted && onAgentNumberRequestRejected && typeof onAgentNumberRequestRejected == 'function') {
                                onAgentNumberRequestRejected();
                            }
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, scope);
                    });
                }
            });
        }
    }

    appControllers.service('Retail_Ringo_RingoAgentNumberRequestService', RingoAgentNumberRequestService);

})(appControllers);
