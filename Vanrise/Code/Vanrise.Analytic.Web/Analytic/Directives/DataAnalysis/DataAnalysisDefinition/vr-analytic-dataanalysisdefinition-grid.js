'use strict';

app.directive('vrAnalyticDataanalysisdefinitionGrid', ['VR_Analytic_DataAnalysisDefinitionAPIService', 'VR_Analytic_DataAnalysisDefinitionService','VRNotificationService',
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

                $scope.scopeModel.menuActions = function (dataAnalysisDefinition) {
                    var menuActions = buildCommonMenuActions();
                    if (dataAnalysisDefinition.menuActions != null) {
                        for (var i = 0; i < dataAnalysisDefinition.menuActions.length; i++)
                            menuActions.push(dataAnalysisDefinition.menuActions[i]);
                    }
                    return menuActions;
                };

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Analytic_DataAnalysisDefinitionAPIService.GetFilteredDataAnalysisDefinitions(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var dataAnalysisDefinition = response.Data[i];
                                VR_Analytic_DataAnalysisDefinitionService.defineDataAnalysisItemDefinitionTabsAndMenuActions(dataAnalysisDefinition, gridAPI);
                            }
                        }
                        onResponseReady(response);

                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                $scope.scopeModel.showExpandIcon = function (dataItem) {
                    return (dataItem.drillDownExtensionObject != null && dataItem.drillDownExtensionObject.drillDownDirectiveTabs.length > 0);
                };
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onDataAnalysisDefinitionAdded = function (addedDataAnalysisDefinition) {
                    VR_Analytic_DataAnalysisDefinitionService.defineDataAnalysisItemDefinitionTabsAndMenuActions(addedDataAnalysisDefinition, gridAPI);
                    gridAPI.itemAdded(addedDataAnalysisDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildCommonMenuActions() {
                return [{
                    name: 'Edit',
                    clicked: editDataAnalysisDefinition,
                    haspermission: hasEditDataAnalysisDefinitionPermission
                }];
            }
            function editDataAnalysisDefinition(dataAnalysisDefinitionItem) {
                var onDataAnalysisDefinitionUpdated = function (updatedDataAnalysisDefinition) {
                    VR_Analytic_DataAnalysisDefinitionService.defineDataAnalysisItemDefinitionTabsAndMenuActions(updatedDataAnalysisDefinition, gridAPI);
                    gridAPI.itemUpdated(updatedDataAnalysisDefinition);
                };

                VR_Analytic_DataAnalysisDefinitionService.editDataAnalysisDefinition(dataAnalysisDefinitionItem.Entity.DataAnalysisDefinitionId, onDataAnalysisDefinitionUpdated);
            }
            function hasEditDataAnalysisDefinitionPermission() {
                return VR_Analytic_DataAnalysisDefinitionAPIService.HasEditDataAnalysisDefinitionPermission();
            }
        }
    }]);
