'use strict';

app.directive('cpRingoAgentnumbersGrid', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'CP_Ringo_AgentNumbersAPIService',
    function (UtilsService, VRUIUtilsService, VRNotificationService, CP_Ringo_AgentNumbersAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var switchGrid = new RetailBeSwitchGrid($scope, ctrl, $attrs);
                switchGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/CP_Ringo/Directives/AgentNumbers/Templates/RingoAgentNumbersGridTemplate.html'
        };

        function RetailBeSwitchGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.agentNumbers = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    console.log(dataRetrievalInput);
                    return CP_Ringo_AgentNumbersAPIService.GetFilteredAgentNumbers(dataRetrievalInput).then(function (response) {
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

                api.onAgentNumbersAdded = function (addedAgentNumbers) {
                    console.log(addedAgentNumbers);
                    for (var i = 0; i < addedAgentNumbers.length; i++) {
                        gridAPI.itemAdded(addedAgentNumbers[i]);
                    }

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {

            }
        }
    }]);
