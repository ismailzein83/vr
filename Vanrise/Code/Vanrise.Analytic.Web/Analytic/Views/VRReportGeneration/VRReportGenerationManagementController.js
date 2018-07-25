(function (appControllers) {

    "use strict";

    VRReportGenerationManagementController.$inject = ['$scope', 'VR_Analytic_ReportGenerationService', 'VR_Analytic_ReportGenerationAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRReportGenerationManagementController($scope, VR_Analytic_ReportGenerationService ,VR_Analytic_ReportGenerationAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                return gridAPI.load(buildGridQuery());
            };
            $scope.scopeModel.add = function () {
                var onVRReportGenerationAdded = function (addedonVRReportGeneration) {
                    gridAPI.onVRReportGenerationAdded(addedonVRReportGeneration);
                };

                VR_Analytic_ReportGenerationService.addVRReportGeneration(onVRReportGenerationAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }
        function load() {
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('VR_Analytic_VRReportGenerationManagementController', VRReportGenerationManagementController);

})(appControllers);