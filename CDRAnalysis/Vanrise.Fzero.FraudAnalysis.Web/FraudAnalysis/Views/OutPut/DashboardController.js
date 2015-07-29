DashboardController.$inject = ['$scope', 'DashboardAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DashboardController($scope, DashboardAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI_CasesSummary;
    var mainGridAPI_StrategyCases;
    var mainGridAPI_BTSCases;
    var mainGridAPI_CellCases;

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
            getData_CasesSummary();
        };

       

        $scope.onMainGridReady_StrategyCases = function (api) {
            mainGridAPI_StrategyCases = api;
            getData_StrategyCases();
        };


       

        $scope.onMainGridReady_BTSCases = function (api) {
            mainGridAPI_BTSCases = api;
            getData_BTSCases();
        };

       

        $scope.onMainGridReady_CellCases = function (api) {
            mainGridAPI_CellCases = api;
            getData_CellCases();
        };

       

        $scope.searchClicked = function () {
            mainGridAPI_CasesSummary.clearDataAndContinuePaging();
            mainGridAPI_StrategyCases.clearDataAndContinuePaging();
            mainGridAPI_BTSCases.clearDataAndContinuePaging();
            mainGridAPI_CellCases.clearDataAndContinuePaging();

            getData_CasesSummary();
            getData_StrategyCases();
            getData_BTSCases();
            getData_CellCases();

            //return true;
        };

    }

    function load() {

    }

    

    function getData_CasesSummary() {

        $scope.isGettingCasesSummary = true;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        return DashboardAPIService.GetCasesSummary(fromDate, toDate).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.casesSummary.push(itm);
            });
        }).finally(function () {
            $scope.isGettingCasesSummary = false;
        });
    }

    function getData_StrategyCases() {

        $scope.isGettingStrategyCases = true;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        return DashboardAPIService.GetStrategyCases(fromDate, toDate).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategyCases.push(itm);
            });
        }).finally(function () {
            $scope.isGettingStrategyCases = false;
        });
    }

    function getData_BTSCases() {

        $scope.isGettingBTSCases = true;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        return DashboardAPIService.GetBTSCases(fromDate, toDate).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.bTSCases.push(itm);
            });
        }).finally(function () {
            $scope.isGettingBTSCases = false;
        });
    }

    function getData_CellCases() {

        $scope.isGettingCellCases = true;

        var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        return DashboardAPIService.GetCellCases(fromDate, toDate).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.cellCases.push(itm);
            });
        }).finally(function () {
            $scope.isGettingCellCases = false;
        });
    }
}
appControllers.controller('FraudAnalysis_DashboardController', DashboardController);
