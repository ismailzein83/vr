"use strict";

DashboardController.$inject = ['$scope', 'UtilsService', 'DashboardAPIService'];

function DashboardController($scope,UtilsService, DashboardAPIService) {

    var mainGridAPI_CasesSummary;
    var mainGridAPI_BTSCases;
    var mainGridAPI_BTSHighValueCases;
    var mainGridAPI_DailyVolumeLooses;


    var chartSelectedMeasureAPI;

    defineScope();
    load();

    function defineScope() {

        var Now = new Date();

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;

        $scope.casesSummary = [];
        $scope.strategyCases = [];
        $scope.bTSCases = [];
        $scope.bTSHighValueCases = [];
        $scope.dailyVolumeLooses = [];

        $scope.onMainGridReady_CasesSummary = function (api) {
            mainGridAPI_CasesSummary = api;
            return retrieveData_CasesSummary();
        };

        $scope.onMainGridReady_BTSCases = function (api) {
            mainGridAPI_BTSCases = api;
            return retrieveData_BTSCases();
        };

        $scope.onMainGridReady_BTSHighValueCases =function(api)
        {
            mainGridAPI_BTSHighValueCases = api;
            return retrieveData_BTSHighValueCases();
        }

        $scope.onMainGridReady_DailyVolumeLooses = function (api) {
            mainGridAPI_DailyVolumeLooses = api;
            return retrieveData_DailyVolumeLooses();
        };

        $scope.chartSelectedMeasureReady = function (api) {
            chartSelectedMeasureAPI = api;
            getData_StrategyCases();
        };

        $scope.searchClicked = function () {
            retrieveData_CasesSummary();
            getData_StrategyCases()
            retrieveData_BTSCases()
            retrieveData_DailyVolumeLooses()
            retrieveData_BTSHighValueCases()
        };

        $scope.dataRetrievalFunction_CaseSummary = function (dataRetrievalInput, onResponseReady) {
            return DashboardAPIService.GetCasesSummary(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

        $scope.dataRetrievalFunction_BTSCases = function (dataRetrievalInput, onResponseReady) {
            return DashboardAPIService.GetBTSCases(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }

        $scope.dataRetrievalFunction_BTSHighValueCases = function (dataRetrievalInput, onResponseReady) {
            return DashboardAPIService.GetTop10BTSHighValue(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            })
        }

        $scope.dataRetrievalFunction_DailyVolumeLooses = function (dataRetrievalInput, onResponseReady) {
            return DashboardAPIService.GetDailyVolumeLooses(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }
    }

    function load() {

    }

    function BuildSearchQuery()
    {
        var query = {
            FromDate: $scope.fromDate,
            ToDate: $scope.toDate
        };

        return query;
    }

    function retrieveData_CasesSummary() {
        return mainGridAPI_CasesSummary.retrieveData(BuildSearchQuery());
    }

    function retrieveData_BTSCases() {
        return mainGridAPI_BTSCases.retrieveData(BuildSearchQuery());
    }

    function retrieveData_BTSHighValueCases() {
        return mainGridAPI_BTSHighValueCases.retrieveData(BuildSearchQuery());
    }

    function retrieveData_DailyVolumeLooses() {
        return mainGridAPI_DailyVolumeLooses.retrieveData(BuildSearchQuery());
    }

    function getMappedMappingResults() {

        $scope.selectedMappingResults = UtilsService.getArrayEnum(Integration_MappingResultEnum);

        var mappedMappingResults = [];

        for (var i = 0; i < $scope.selectedMappingResults.length; i++) {
            mappedMappingResults.push($scope.selectedMappingResults[i].value);
        }

        return mappedMappingResults;
    }

    function getData_StrategyCases() {
        if (!chartSelectedMeasureAPI)
            return;
        $scope.strategyCases.length = 0;
        $scope.showResult = true;
        $scope.isGettingStrategyCases = true;


        return DashboardAPIService.GetStrategyCases($scope.fromDate, $scope.toDate).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategyCases.push(itm);
            });

            var chartDefinition = {
                type: "pie",
                title: "Strategy Cases",
                yAxisTitle: "StrategyName"
            };

            var seriesDefinitions = [{
                title: "Fraud Cases",
                titlePath: "StrategyName",
                valuePath: "CountCases"
            }];
            chartSelectedMeasureAPI.renderSingleDimensionChart($scope.strategyCases, chartDefinition, seriesDefinitions);

        }).finally(function () {
            $scope.isGettingStrategyCases = false;
        });
    }

}
appControllers.controller('FraudAnalysis_DashboardController', DashboardController);
