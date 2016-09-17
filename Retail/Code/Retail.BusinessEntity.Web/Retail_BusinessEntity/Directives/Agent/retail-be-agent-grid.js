'use strict';

app.directive('retailBeAgentGrid', ['Retail_BE_AgentAPIService', 'Retail_BE_AgentService', 'VRNotificationService', function (Retail_BE_AgentAPIService, Retail_BE_AgentService, VRNotificationService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var agentGrid = new AgentGrid($scope, ctrl, $attrs);
            agentGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Agent/Templates/AgentGridTemplate.html'
    };

    function AgentGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.agents = [];
            $scope.scopeModel.menuActions = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };

            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return Retail_BE_AgentAPIService.GetFilteredAgents(dataRetrievalInput).then(function (response) {
                    onResponseReady(response);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            };

            defineMenuActions();
        }
        function defineAPI() {
            var api = {};

            api.loadGrid = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onAgentAdded = function (addedAgent) {
                gridAPI.itemAdded(addedAgent);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editAgent,
                haspermission: hasEditAgentPermission
            });
        }
        function editAgent(agent) {
            var onAgentUpdated = function (updatedAgent) {
                gridAPI.itemUpdated(updatedAgent);
            };
            Retail_BE_AgentService.editAgent(agent.Entity.Id, onAgentUpdated);
        }
        function hasEditAgentPermission() {
            return Retail_BE_AgentAPIService.HasUpdateAgentPermission();
        }
    }
}]);
