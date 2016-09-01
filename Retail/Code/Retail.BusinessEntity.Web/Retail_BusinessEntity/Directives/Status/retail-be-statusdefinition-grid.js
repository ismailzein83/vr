'use strict';

app.directive('retailBeStatusdefinitionGrid', ['Retail_BE_StatusDefinitionAPIService', 'Retail_BE_StatusDefinitionService', 'VRNotificationService',
    function (Retail_BE_StatusDefinitionAPIService, Retail_BE_StatusDefinitionService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var statusDefinitionGrid = new RetailBeStatusDefinitionGrid($scope, ctrl, $attrs);
                statusDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Status/Templates/StatusDefinitionGridTemplate.html'
        };

        function RetailBeStatusDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.statusDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_StatusDefinitionAPIService.GetFilteredStatusDefinitions(dataRetrievalInput).then(function (response) {
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

                api.onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridAPI.itemAdded(addedStatusDefinition);
                }

                api.onStatusDefinitionUpdated = function (updatedStatusDefinition) {
                    gridAPI.itemUpdated(updatedStatusDefinition);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editStatusDefinition,
                    haspermission: hasEditServiceTypePermission
                });
            }

            function hasEditServiceTypePermission() {
                return Retail_BE_StatusDefinitionAPIService.HasUpdateStatusDefinitionPermission();
            }
            function editStatusDefinition(statusDefinitionItem) {
                var onStatusDefinitionUpdated = function (updatedStatusDefinition) {
                    gridAPI.itemUpdated(updatedStatusDefinition);
                };

                Retail_BE_StatusDefinitionService.editStatusDefinition(statusDefinitionItem.Entity.StatusDefinitionId, onStatusDefinitionUpdated);
            }
        }
    }]);
