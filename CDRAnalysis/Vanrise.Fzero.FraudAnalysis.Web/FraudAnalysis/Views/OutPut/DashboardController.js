DashboardController.$inject = ['$scope', 'DashboardAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];

function DashboardController($scope, DashboardAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var mainGridAPI_CasesSummary;

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

        $scope.onMainGridReady_CasesSummary = function (api) {
            mainGridAPI_CasesSummary = api;
            getData_CasesSummary();
        };

        $scope.loadMoreData_CasesSummary = function () {
            return getData_CasesSummary();
        }

        $scope.searchClicked = function () {
            mainGridAPI_CasesSummary.clearDataAndContinuePaging();
            return getData_CasesSummary();
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
}
appControllers.controller('FraudAnalysis_DashboardController', DashboardController);
