'use strict';

app.directive('vrBebridgeBereceivedefinitionGrid', ['VR_BEBridge_BERecieveDefinitionAPIService', 'VRNotificationService', 'VR_BEBridge_BEReceiveDefinitionService',
    function (beRecieveDefinitionApiService, vrNotificationService, beReceiveDefinitionService) {

        function vrbeReceiveDefinitionGrid($scope, ctrl, $attrs) {
            var gridAPI;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.receiveDefinitionSet = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return beRecieveDefinitionApiService.GetFilteredBeReceiveDefinitions(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
                defineMenuActions();
            }

            this.initializeController = initializeController;

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onReceiveDefinitionAdded = function (addedReceiveDefinition) {
                    gridAPI.itemAdded(addedReceiveDefinition);
                };

                api.onReceiveDefinitionUpdated = function (updatedReceiveDefinition) {
                    gridAPI.itemUpdated(updatedReceiveDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function editReceiveDefinition(receiveDefinitionItem) {
                var onReceiveDefinitionUpdated = function (updatedReceiveDefinition) {
                    gridAPI.itemUpdated(updatedReceiveDefinition);
                };
                beReceiveDefinitionService.editReceiveDefinition(receiveDefinitionItem.Entity.BEReceiveDefinitionId, onReceiveDefinitionUpdated);
            }
            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editReceiveDefinition
                });
            }
        }

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var beReceiveDefinitionGrid = new vrbeReceiveDefinitionGrid($scope, ctrl, $attrs);
                beReceiveDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/BEReceiveDefinition/Templates/BEReceiveDefinitionGridTemplate.html'
        };
    }]);
