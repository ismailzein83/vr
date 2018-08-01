'use strict';

app.directive('vrAnalyticReportgenerationGrid', ['VR_Analytic_ReportGenerationAPIService', 'VR_Analytic_ReportGenerationService', 'VRNotificationService', 'VRCommon_ObjectTrackingService', 'VRUIUtilsService', 'VR_Analytic_AccessLevel',
function (VR_Analytic_ReportGenerationAPIService, VR_Analytic_ReportGenerationService, VRNotificationService, VRCommon_ObjectTrackingService, VRUIUtilsService, VR_Analytic_AccessLevel) {
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
            var gridDrillDownTabsObj;
            var DrillDownDefinitionsArray = [];
            this.initializeController = initializeController;
            var gridApi;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.vRReportGenerations = [];
                $scope.scopeModel.onGridReady = function (api) {
                    gridApi = api;
                    var drillDownDefinitions = VR_Analytic_ReportGenerationService.getDrillDownDefinition();
                    registerObjectTrackingDrillDownToReportGeneration();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(DrillDownDefinitionsArray, gridApi, $scope.scopeModel.gridMenuActions);

                    function registerObjectTrackingDrillDownToReportGeneration() {
                        var drillDownDefinition = {};

                        drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                        drillDownDefinition.directive = "vr-common-objecttracking-grid";


                        drillDownDefinition.loadDirective = function (directiveAPI, reportItem) {
                            reportItem.objectTrackingGridAPI = directiveAPI;
                            var query = {
                                ObjectId: reportItem.ReportId,
                                EntityUniqueName: VR_Analytic_ReportGenerationService.getEntityUniqueName(),
                            };
                            return reportItem.objectTrackingGridAPI.load(query);
                        };

                        for (var i = 0; i < drillDownDefinitions.length; i++) {
                            DrillDownDefinitionsArray.push(drillDownDefinitions[i]);
                        }
                        DrillDownDefinitionsArray.push(drillDownDefinition);
                    }

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveApi());
                    }
                    function getDirectiveApi() {
                        var directiveApi = {};

                        directiveApi.load = function (query) {
                            return gridApi.retrieveData(query);
                        };
                        directiveApi.onVRReportGenerationAdded = function (vRReportGeneration) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(vRReportGeneration);
                            gridApi.itemAdded(vRReportGeneration);
                        };
                        return directiveApi;
                    };
                };
                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                    return VR_Analytic_ReportGenerationAPIService.GetFilteredVRReportGenerations(dataRetrievalInput)
                    .then(function (response) {
                        if (response.Data != undefined) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
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

                $scope.scopeModel.gridMenuActions = function (dataItem) {

                    var menuActions = [];
                    if (dataItem.DoesUserHaveManageAccess || dataItem.AccessLevel == VR_Analytic_AccessLevel.Private.description) {
                        var menuAction1 = {
                            name: "Edit",
                            clicked: editVRReportGeneration
                        };
                        menuActions.push(menuAction1);
                    }
                    return menuActions;
                };
            }
            function editVRReportGeneration(vRReportGeneration) {
                var onVRReportGenerationUpdated = function (vRReportGeneration) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(vRReportGeneration);
                    gridApi.itemUpdated(vRReportGeneration);
                };
                VR_Analytic_ReportGenerationService.editVRReportGeneration(vRReportGeneration.ReportId, onVRReportGenerationUpdated);
            };            



        };
        return directiveDefinitionObject;
    }]);

