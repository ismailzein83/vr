//(function (appControllers) {

//    "use strict";

//    VRReportManagementController.$inject = ['$scope', 'VR_Analytic_VRReportService', 'VR_Analytic_VRReportAPIService', 'UtilsService', 'VRUIUtilsService'];

//    function VRReportManagementController($scope, VR_Analytic_VRReportService, VR_Analytic_VRReportAPIService, UtilsService, VRUIUtilsService) {

//        var gridAPI;

//        defineScope();
//        load();


//        function defineScope() {
//            $scope.scopeModel = {};

//            $scope.scopeModel.search = function () {
//                return gridAPI.load(buildGridQuery());
//            };
//            $scope.scopeModel.add = function () {
//                var onVRReportAdded = function (addedonVRReport) {
//                    gridAPI.onVRReportAddedAdded(addedonVRReport);
//                };

//                VR_Analytic_VRReportService.addDataAnalysisDefinition(onVRReportAdded);
//            };

//            $scope.scopeModel.onGridReady = function (api) {
//                gridAPI = api;
//                gridAPI.load({});
//            };
//        }
//        function load() {

//        }

//        function buildGridQuery() {
//            return {
//                Name: $scope.scopeModel.name,
//            };
//        }
//    }

//    appControllers.controller('VR_Analytic_VRReportManagementController', VRReportManagementController);

//})(appControllers);