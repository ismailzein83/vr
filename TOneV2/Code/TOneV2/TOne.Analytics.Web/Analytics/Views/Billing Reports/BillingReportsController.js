BillingReportsController.$inject = ['$scope', 'ReportAPIService'];

function BillingReportsController($scope, ReportAPIService) {
   
    defineScope();
    load();
    $scope.reportsTypes = [];
        $scope.params = {
            fromDate: "",
            toDate:""
        }
    function defineScope() {
       
        $scope.openReport = function () {
            alert($scope.params.fromDate)
            window.open("/Reports/Analytics/BillingReports.aspx?fromDate=" + $scope.dateToString($scope.params.fromDate) + "&toDate=" + $scope.dateToString($scope.params.toDate), "_blank", "width=400, height=200");
        }
        
    }
    function load() {
        loadReportTypes();
    }

    function loadReportTypes() {
        ReportAPIService.GetAllReportDefinition().then(function (response) {
            $scope.reportsTypes = response;
        })
    }

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);