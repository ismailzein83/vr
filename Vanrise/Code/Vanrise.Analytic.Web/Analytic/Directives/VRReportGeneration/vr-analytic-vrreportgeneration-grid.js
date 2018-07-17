//'use strict';

//app.directive('vrAnalyticVrreportGrid', ['VR_Analytic_VRReportAPIService', 'VR_Analytic_VRReportService', 'VRNotificationService',
//    function (VR_Analytic_VRReportAPIService, VR_Analytic_VRReportService, VRNotificationService) {
//        return {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var vRReportGrid = new VRReportGrid($scope, ctrl, $attrs);
//                vRReportGrid.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: '/Client/Modules/Analytic/Directives/VRReport/Templates/VRReportGridTemplate.html'
//        };

//        function VRReportGrid($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var gridAPI;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.vRReport = [];

//                $scope.scopeModel.menuActions = function (vRReport) {
//                    var menuActions = buildCommonMenuActions();
//                    if (vRReport.menuActions != null) {
//                        for (var i = 0; i < vRReport.menuActions.length; i++)
//                            menuActions.push(vRReport.menuActions[i]);
//                    }
//                    return menuActions;
//                };

//                $scope.scopeModel.onGridReady = function (api) {
//                    gridAPI = api;
//                    defineAPI();
//                };

//                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
//                    return VR_Analytic_VRReportAPIService.GetFilteredVRReports(dataRetrievalInput).then(function (response) {
//                        if (response && response.Data) {
//                            for (var i = 0; i < response.Data.length; i++) {
//                                var vRReport = response.Data[i];
//                            }
//                        }
//                        onResponseReady(response);

//                    }).catch(function (error) {
//                        VRNotificationService.notifyExceptionWithClose(error, $scope);
//                    });
//                };                               
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (query) {
//                    return gridAPI.retrieveData(query);
//                };

//                api.onVRReportAdded = function (addedVRReport) {
                    
//                    gridAPI.itemAdded(addedVRReport);
//                };

//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }

//            function buildCommonMenuActions() {
//                return [{
//                    name: 'Edit',
//                    clicked: editVRReport,
//                    haspermission: hasEditVRReportPermission
//                }];
//            }
//            function editVRReport(vRReportItem) {
//                var onVRReportUpdated = function (updatedVRReport) {
//                    gridAPI.itemUpdated(updatedVRReport);
//                };

//                VR_Analytic_VRReportService.editVRReport(vRReportItem.Entity.VRReportId, onVRReportUpdated);
//            }
//            function hasEditVRReportPermission() {
//                return VR_Analytic_VRReportAPIService.HasEditVRReportPermission();
//            }
//        }
//    }]);
