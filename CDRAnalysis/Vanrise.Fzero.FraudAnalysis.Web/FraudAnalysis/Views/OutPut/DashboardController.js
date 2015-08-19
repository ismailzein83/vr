DashboardController.$inject = ['$scope', 'DashboardAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DashboardController($scope, DashboardAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI_CasesSummary;
    var mainGridAPI_BTSCases;
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




        $scope.onMainGridReady_CasesSummary = function (api) {
            mainGridAPI_CasesSummary = api;
            return retrieveData_CasesSummary();
        };

        $scope.onMainGridReady_BTSCases = function (api) {
            mainGridAPI_BTSCases = api;
            return retrieveData_BTSCases();
        };

        $scope.chartSelectedMeasureReady = function (api) {
            chartSelectedMeasureAPI = api;
            getData_StrategyCases();
        };


        $scope.searchClicked = function () {
            var results = (retrieveData_CasesSummary() && getData_StrategyCases());
            return results;
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


    function getData_StrategyCases() {
        if (!chartSelectedMeasureAPI)
            return;
        $scope.strategyCases.length = 0;
        $scope.showResult = true;
        $scope.isGettingStrategyCases = true;


        return DashboardAPIService.GetStrategyCases($scope.fromDate, $scope.toDate).then(function (response) {
            console.log('Chart: response')
            console.log(response)
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
