'use strict';

app.directive('retailRingoAgentrequestnumberGrid', ['Retail_Ringo_AgentNumberRequestAPIService', 'Retail_Ringo_RingoAgentNumberRequestService', 'VR_GenericData_GenericRule', 'UtilsService', 'VRNotificationService', 'Retail_Ringo_AgentNumberRequestStatusEnum', 'Retail_Ringo_AgentNumberStatusEnum',
    function (Retail_Ringo_AgentNumberRequestAPIService, Retail_Ringo_RingoAgentNumberRequestService, VR_GenericData_GenericRule, UtilsService, VRNotificationService, Retail_Ringo_AgentNumberRequestStatusEnum, Retail_Ringo_AgentNumberStatusEnum) {
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
                $scope.scopeModel.gridMenuActions = function (dataItem) {
                    var menuActions = [];
                    menuActions.push({
                        name: 'View',
                        clicked: viewNumbersRequest,
                        haspermission: hasUpdateAgentNumberRequestPermission
                    });

                    return menuActions;
                };
            }

            function viewNumbersRequest(numberRequest) {
                var onNumberRequestAdded = function (processedRequestNumber) {
                    console.log(processedRequestNumber);
                    var itemDetails = {
                        Entity: processedRequestNumber,
                        StatusDescription: processedRequestNumber.StatusDescription,
                        AgentName: numberRequest.AgentName
                    };
                    gridAPI.itemUpdated(itemDetails);
                };
                Retail_Ringo_RingoAgentNumberRequestService.viewNumbersRequest(numberRequest.Entity.Id, onNumberRequestAdded);
            }

            function hasUpdateAgentNumberRequestPermission() {
                return Retail_Ringo_AgentNumberRequestAPIService.HasUpdateAgentNumberRequestPermission();
            }
        }
    }]);
