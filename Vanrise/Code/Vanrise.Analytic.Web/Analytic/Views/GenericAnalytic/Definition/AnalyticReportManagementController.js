'use strict';
AnalyticreportManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VR_Analytic_AnalyticReportService', 'VR_Analytic_AnalyticReportAPIService', 'VRModalService'];

function AnalyticreportManagementController($scope, UtilsService, VRNotificationService, VR_Analytic_AnalyticReportService, VR_Analytic_AnalyticReportAPIService, VRModalService) {
    var mainGridAPI;
    var analyticReportConfigTypes = [];
    defineScope();
    load();

    function defineScope() {

        $scope.onGridReady = function (api) {
            mainGridAPI = api;
            var filter = {};
            api.loadGrid(filter);
        };

        $scope.addAnalyticreport = function () {
            var onAnalyticreportAdded = function (analyticreportObj) {
                mainGridAPI.onAnalyticreportAdded(analyticreportObj);
            };
            VR_Analytic_AnalyticreportService.addAnalyticreport(onAnalyticreportAdded);
        };
        $scope.hasAddAnalyticReportPermission = function () {
            return VR_Analytic_AnalyticReportAPIService.HasAddAnalyticReportPermission();
        };
        $scope.searchClicked = function () {
            return mainGridAPI.loadGrid(getFilterObject());
        };

        $scope.addMenuActions = [];
    }

    function getFilterObject() {
        var query = {
            Name: $scope.analyticReportName
        };
        return query;
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadAnalyticReportTypes])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
    }

    function loadAnalyticReportTypes() {

        return VR_Analytic_AnalyticReportAPIService.GetAnalyticReportConfigTypes().then(function (response) {
            analyticReportConfigTypes = response;
            if (analyticReportConfigTypes)
            {
                for (var i = 0; i < analyticReportConfigTypes.length; i++) {
                    var analyticReportConfigType = analyticReportConfigTypes[i];
                        addMenuAction(analyticReportConfigType);
                }
                function addMenuAction(analyticReportConfigType) {
                    $scope.addMenuActions.push({
                        name: analyticReportConfigType.Title,
                        clicked: function () {
                            var onAnalyticReportAdded = function (Obj) {
                                if (mainGridAPI != undefined)
                                    mainGridAPI.onAnalyticReportAdded(Obj);
                        };
                            VR_Analytic_AnalyticReportService.addAnalyticReport(onAnalyticReportAdded, analyticReportConfigType.ExtensionConfigurationId);
                        }
                    });
                }
            }
          
        });
    }

};

appControllers.controller('VR_Analytic_AnalyticReportManagementController', AnalyticreportManagementController);