DashboardController.$inject = ['$scope', 'DashboardAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DashboardController($scope, DashboardAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI_CasesSummary;
    var mainGridAPI_BTSCases;
    var mainGridAPI_CellCases;
    var chartSelectedMeasureAPI;

    defineScope();
    load();




    function defineScope() {


        var Now = new Date();
        Now.setDate(Now.getDate() + 1);

        var Yesterday = new Date();
        Yesterday.setDate(Yesterday.getDate() - 1);

        $scope.fromDate = Yesterday;
        $scope.toDate = Now;


        $scope.customvalidateFrom = function (fromDate) {
            return validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTo = function (toDate) {
            return validateDates($scope.fromDate, toDate);
        };
        function validateDates(fromDate, toDate) {
            if (fromDate == undefined || toDate == undefined)
                return null;
            var from = new Date(fromDate);
            var to = new Date(toDate);
            if (from.getTime() > to.getTime())
                return "Start should be before end";
            else
                return null;
        }






        $scope.casesSummary = [];
        $scope.strategyCases = [];
        $scope.bTSCases = [];
        $scope.cellCases = [];




        $scope.onMainGridReady_CasesSummary = function (api) {
            mainGridAPI_CasesSummary = api;
            return retrieveData_CasesSummary();
        };

        $scope.onMainGridReady_BTSCases = function (api) {
            mainGridAPI_BTSCases = api;
            return retrieveData_getData_BTSCases();
        };



        $scope.onMainGridReady_CellCases = function (api) {
            mainGridAPI_CellCases = api;
            return retrieveData_CellCases();
        };

        $scope.chartSelectedMeasureReady = function (api) {
            chartSelectedMeasureAPI = api;
            getData_StrategyCases();
        };


        $scope.searchClicked = function () {
            var results = (retrieveData_CasesSummary() && getData_StrategyCases() && retrieveData_CellCases());
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


        $scope.dataRetrievalFunction_CellCases = function (dataRetrievalInput, onResponseReady) {
            return DashboardAPIService.GetCellCases(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        }



    }

    function load() {

    }


    function BuildSearchQuery()
    {
        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        var query = {
            FromDate: fromDate,
            ToDate: toDate
        };

        return query;
    }

    function retrieveData_CasesSummary() {
        return mainGridAPI_CasesSummary.retrieveData(BuildSearchQuery());
    }


    function retrieveData_BTSCases() {
        return mainGridAPI_BTSCases.retrieveData(BuildSearchQuery());
    }

    function retrieveData_CellCases() {
        return mainGridAPI_CellCases.retrieveData(BuildSearchQuery());
    }




    function getData_StrategyCases() {
        if (!chartSelectedMeasureAPI)
            return;
        $scope.strategyCases.length = 0;
        $scope.showResult = true;
        $scope.isGettingStrategyCases = true;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        return DashboardAPIService.GetStrategyCases(fromDate, toDate).then(function (response) {
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
