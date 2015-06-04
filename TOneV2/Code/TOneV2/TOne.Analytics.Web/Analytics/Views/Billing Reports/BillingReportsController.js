BillingReportsController.$inject = ['$scope', 'ReportAPIService'];

function BillingReportsController($scope, ReportAPIService) {
   
    defineScope();
    load();
    $scope.reportsTypes = [];
    function defineScope() {
       
        $scope.openReport = function () {
            window.open("/Reports/Analytics/BillingReports.aspx?fromDate=" +$scope.dateToString( $scope.fromDate) + "&toDate=" +$scope.dateToString( $scope.toDate ), "_blank", "width=400, height=200");
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