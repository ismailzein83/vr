
'use strict';

app.directive('vrAnalyticDataanalysisitemdefinitionGrid', ['VR_Analytic_DataAnalysisItemDefinitionAPIService', 'VR_Analytic_DataAnalysisItemDefinitionService', 'VRNotificationService', 'VRUIUtilsService',
    function (VR_Analytic_DataAnalysisItemDefinitionAPIService, VR_Analytic_DataAnalysisItemDefinitionService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var dataAnalysisItemDefinitionGrid = new DataAnalysisItemDefinitionGrid($scope, ctrl, $attrs);
                dataAnalysisItemDefinitionGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/DataAnalysisItemDefinition/Templates/DataAnalysisItemDefinitionGridTemplate.html'
        };

        function DataAnalysisItemDefinitionGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;
            var gridDrillDownTabsObj;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataAnalysisItemDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = VR_Analytic_DataAnalysisItemDefinitionService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetFilteredDataAnalysisItemDefinitions(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
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

                api.onItemAdded = function (addedDataAnalysisItemDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedDataAnalysisItemDefinition);
                    gridAPI.itemAdded(addedDataAnalysisItemDefinition);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editDataAnalysisItemDefinition
                });
            }
            function editDataAnalysisItemDefinition(dataAnalysisItemDefinition) {
                var onDataAnalysisItemDefinitionUpdated = function (updatedDataAnalysisItemDefinition) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedDataAnalysisItemDefinition);
                    gridAPI.itemUpdated(updatedDataAnalysisItemDefinition);
                };

                VR_Analytic_DataAnalysisItemDefinitionService.editDataAnalysisItemDefinition(dataAnalysisItemDefinition.Entity.DataAnalysisItemDefinitionId, dataAnalysisItemDefinition.Entity.DataAnalysisDefinitionId,
                                                                                             dataAnalysisItemDefinition.Entity.Settings.ItemDefinitionTypeId, onDataAnalysisItemDefinitionUpdated);
            }
        }
    }]);
