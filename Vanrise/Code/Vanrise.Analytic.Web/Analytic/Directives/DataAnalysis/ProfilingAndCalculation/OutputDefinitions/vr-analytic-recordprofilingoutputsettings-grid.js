
'use strict';

app.directive('vrAnalyticRecordprofilingoutputsettingsGrid', ['VR_Analytic_DataAnalysisItemDefinitionAPIService', 'VR_Analytic_DataAnalysisItemDefinitionService', 'VRNotificationService',
    function (VR_Analytic_DataAnalysisItemDefinitionAPIService, VR_Analytic_DataAnalysisItemDefinitionService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var recordProfilingOutputSettingsGrid = new RecordProfilingOutputSettingsGrid($scope, ctrl, $attrs);
                recordProfilingOutputSettingsGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/OutputDefinitions/Templates/RecordProfilingOutputSettingsGridTemplate.html'
        };

        function RecordProfilingOutputSettingsGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dataAnalysisItemDefinition = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return VR_Analytic_DataAnalysisItemDefinitionAPIService.GetFilteredDataAnalysisItemDefinitions(dataRetrievalInput).then(function (response) {
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

                api.onItemAdded = function (addedRecordProfilingOutputSettings) {
                    gridAPI.itemAdded(addedRecordProfilingOutputSettings);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editDataAnalysisItemDefinition,
                });
            }
            function editDataAnalysisItemDefinition(dataAnalysisItemDefinition) {
                var onDataAnalysisItemDefinitionUpdated = function (updatedRecordProfilingOutputSettings) {
                    gridAPI.itemUpdated(updatedRecordProfilingOutputSettings);
                };

                VR_Analytic_DataAnalysisItemDefinitionService.editDataAnalysisItemDefinition(dataAnalysisItemDefinition.Entity.DataAnalysisItemDefinitionId, dataAnalysisItemDefinition.Entity.DataAnalysisDefinitionId,
                                                                                             dataAnalysisItemDefinition.Entity.Settings.ItemDefinitionTypeId, onDataAnalysisItemDefinitionUpdated);
            }
        }
    }]);
