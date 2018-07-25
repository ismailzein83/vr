'use strict';

app.directive('vrAnalyticReportgenerationGrid', ['VR_Analytic_ReportGenerationAPIService', 'VR_Analytic_ReportGenerationService', 'VRNotificationService',
    function (VR_Analytic_ReportGenerationAPIService, VR_Analytic_ReportGenerationService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var vRReportGenerationGrid = new VRReportGenerationGrid($scope, ctrl, $attrs);
                vRReportGenerationGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Analytic/Directives/VRReportGeneration/Templates/VRReportGenerationGridTemplate.html'
        };

        function VRReportGenerationGrid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridApi;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.vRReportGenerations = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }

                    function getDirectiveApi() {
                        var directiveApi = {};

                        directiveApi.load = function (query) {
                            return gridApi.retrieveData(query);
                        };
                        directiveApi.onVRReportGenerationAdded = function (vRReportGeneration) {
                            gridApi.itemAdded(vRReportGeneration);
                        };
                        return directiveApi;
                    };
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                    return VR_Analytic_ReportGenerationAPIService.GetFilteredVRReportGenerations(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                $scope.generateVRReportGeneration = function (vRReportGeneration) {
                    VR_Analytic_ReportGenerationService.generateVRReportGeneration(vRReportGeneration.ReportId);
                };

                defineMenuActions();
            };

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editVRReportGeneration
                }];
            };
            function editVRReportGeneration(vRReportGeneration) {
                var onVRReportGenerationUpdated = function (vRReportGeneration) {
                    gridApi.itemUpdated(vRReportGeneration);
                };
                VR_Analytic_ReportGenerationService.editVRReportGeneration(vRReportGeneration.ReportId, onVRReportGenerationUpdated);
            };
            



        };
        return directiveDefinitionObject;
    }]);

