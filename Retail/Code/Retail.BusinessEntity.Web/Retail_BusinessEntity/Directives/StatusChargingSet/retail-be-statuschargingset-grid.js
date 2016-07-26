'use strict';

app.directive('retailBeStatuschargingsetGrid', ['Retail_BE_StatusChargingSetAPIService', 'VRNotificationService',
    function (retailBeStatusChargingSetApiService, vrNotificationService) {

        function retailBeStatusChargingSetGrid($scope, ctrl, $attrs) {
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.statusChargingSet = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return retailBeStatusChargingSetApiService.GetFilteredStatusChargingSet(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };
            }

            this.initializeController = initializeController;

            var gridAPI;

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onStatusChargingSetAdded = function (addedStatusChargingSet) {
                    gridAPI.itemAdded(addedStatusChargingSet);
                }

                api.onStatusChargingSetUpdated = function (updatedStatusChargingSet) {
                    gridAPI.itemUpdated(updatedStatusChargingSet);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var statusDefinitionGrid = new retailBeStatusChargingSetGrid($scope, ctrl, $attrs);
                statusDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/StatusChargingSet/Templates/StatusChargingSetGridTemplate.html'
        };
    }]);
