'use strict';

app.directive('vrAnalyticDataanalysisdefinitionGrid', ['VR_Analytic_DataAnalysisDefinitionAPIService', 'VR_Analytic_DataAnalysisDefinitionService', 'VRNotificationService',
    function (VR_Analytic_DataAnalysisDefinitionAPIService, VR_Analytic_DataAnalysisDefinitionService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataAnalysisDefinitionGrid = new DataAnalysisDefinitionGrid($scope, ctrl, $attrs);
                dataAnalysisDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/DataAnalysisDefinition/Templates/DataAnalysisDefinitionGridTemplate.html'
        };

        function DataAnalysisDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataAnalysisDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Analytic_DataAnalysisDefinitionAPIService.GetFilteredDataAnalysisDefinitions(dataRetrievalInput).then(function (response) {
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

                api.onDataAnalysisDefinitionAdded = function (addedDataAnalysisDefinition) {
                    gridAPI.itemAdded(addedDataAnalysisDefinition);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editDataAnalysisDefinition,
                });
            }

            function editDataAnalysisDefinition(dataAnalysisDefinitionItem) {
                var onDataAnalysisDefinitionUpdated = function (updatedDataAnalysisDefinition) {
                    gridAPI.itemUpdated(updatedDataAnalysisDefinition);
                };

                VR_Analytic_DataAnalysisDefinitionService.editDataAnalysisDefinition(dataAnalysisDefinitionItem.Entity.DataAnalysisDefinitionId, onDataAnalysisDefinitionUpdated);
            }
        }
    }]);
