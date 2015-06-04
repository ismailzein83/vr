BillingReportsController.$inject = ['$scope'];

function BillingReportsController($scope) {
   
    defineScope();
    load();
    function defineScope() {
       
        $scope.openReport = function () {
            window.open("/Reports/Analytics/BilllingReport.aspx?fromDate=" +$scope.dateToString( $scope.fromDate) + "&toDate=" +$scope.dateToString( $scope.toDate ), "_blank", "width=400, height=200");
        }
        
    }
    function load() {

    }

   

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);