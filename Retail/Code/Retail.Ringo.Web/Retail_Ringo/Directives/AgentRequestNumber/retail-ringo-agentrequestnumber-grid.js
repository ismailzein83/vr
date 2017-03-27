'use strict';

app.directive('retailRingoAgentrequestnumberGrid', ['Retail_Ringo_AgentNumberRequestAPIService', 'VRNotificationService',
    function (Retail_Ringo_AgentNumberRequestAPIService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var grid = new RingoAgentRequestNumberGrid($scope, ctrl, $attrs);
                grid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Ringo/Directives/AgentRequestNumber/Templates/AgentRequestNumberGridTemplate.html'
        };

        function RingoAgentRequestNumberGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.agentNumberRequests = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_Ringo_AgentNumberRequestAPIService.GetFilteredAgentNumberRequests(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Add Rule',
                    clicked: addRule
                });

            }
            function addRule(numberRequest) {
                var onNumberRequestAdded = function (numberRequest) {
                    gridAPI.itemAdded(numberRequest);
                };

            }
            function rejectRequest(numberRequest) {

            }
            function hasAddRulePermission() {
                return true;
            }
        }
    }]);
