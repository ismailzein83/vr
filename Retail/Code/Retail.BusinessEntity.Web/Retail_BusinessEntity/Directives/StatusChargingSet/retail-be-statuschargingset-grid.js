'use strict';

app.directive('retailBeStatusdefinitionGrid', ['Retail_BE_StatusChargingSetAPIService', 'Retail_BE_StatusChargingSetService ', 'VRNotificationService',
    function (retailBeStatusChargingSetApiService, retailBeStatusChargingSetService, vrNotificationService) {

        function retailBeStatusChargingSetGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridAPI.itemAdded(addedStatusDefinition);
                }

                api.onStatusDefinitionUpdated = function (updatedStatusDefinition) {
                    gridAPI.itemUpdated(updatedStatusDefinition);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.statusDefinition = [];
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

                // defineMenuActions();
            } //function defineMenuActions() {
            //    $scope.scopeModel.menuActions.push({
            //        name: 'Edit',
            //        clicked: editStatusDefinition,
            //    });
            //}

            //function editStatusDefinition(statusDefinitionItem) {
            //    var onStatusDefinitionUpdated = function (updatedStatusDefinition) {
            //        gridAPI.itemUpdated(updatedStatusDefinition);
            //    };

            //    retailBeStatusChargingService.editStatusDefinition(statusDefinitionItem.Entity.StatusDefinitionId, onStatusDefinitionUpdated);
            //}
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
