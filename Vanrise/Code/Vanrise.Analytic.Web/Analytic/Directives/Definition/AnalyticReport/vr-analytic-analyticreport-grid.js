"use strict";

app.directive("vrAnalyticAnalyticreportGrid", ['VRNotificationService', 'VRModalService', 'VR_Analytic_AnalyticReportService', 'UtilsService', 'VR_Analytic_AnalyticReportAPIService', 'VRUIUtilsService', function (VRNotificationService, VRModalService, VR_Analytic_AnalyticReportService, UtilsService, VR_Analytic_AnalyticReportAPIService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var analyticReportGrid = new AnalyticReportGrid($scope, ctrl, $attrs);
            analyticReportGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Analytic/Directives/Definition/AnalyticReport/Templates/AnalyticReportGridTemplate.html"

    };

    function AnalyticReportGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.analyticReports = [];
            $scope.gridMenuActions = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};

                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onAnalyticReportAdded = function (tableObj) {
                        gridAPI.itemAdded(tableObj);
                    };
                    return directiveAPI;
                }
            };

            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VR_Analytic_AnalyticReportAPIService.GetFilteredAnalyticReports(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editAnalyticReport,
                haspermission:hasEditAnalyticReportPermission
            }];
        }
        function hasEditAnalyticReportPermission() {
            return VR_Analytic_AnalyticReportAPIService.HasEditAnalyticReportPermission();

        }
        function editAnalyticReport(dataItem) {
            var onEditAnalyticReport = function (analyticReportObj) {
                gridAPI.itemUpdated(analyticReportObj);
            };
            VR_Analytic_AnalyticReportService.editAnalyticReport(dataItem.Entity.AnalyticReportId, onEditAnalyticReport, dataItem.Entity.Settings.ConfigId);
        }
    }


    return directiveDefinitionObject;

}]);