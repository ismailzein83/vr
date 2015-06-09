BillingReportsController.$inject = ['$scope', 'ReportAPIService', 'CarriersService'];

function BillingReportsController($scope, ReportAPIService, CarriersService) {
   
    defineScope();
    load();
    $scope.reportsTypes = [];
    $scope.optionsCustomers = [];
    $scope.optionsSuppliers = [];
    $scope.reportsTypes = [];
    $scope.params= {
        fromDate: "",
        toDate: "",
        groupByCustomer: false,
        customer: null,
        supplier: null,
        isCost: false,
        service: false,
        commission: false,
        bySupplier:false
    }
    function defineScope() {
       
        $scope.openReport = function () {

            var paramsurl = "";
            paramsurl += "reportId=" + $scope.reporttype.ReportDefinitionId;
            paramsurl += "&fromDate=" + $scope.dateToString($scope.params.fromDate);
            paramsurl += "&toDate=" + $scope.dateToString($scope.params.toDate);
            paramsurl += "&groupByCustomer=" + $scope.params.groupByCustomer;
            paramsurl += "&isCost=" + $scope.params.isCost;
            paramsurl += "&service=" + $scope.params.service;
            paramsurl += "&commission=" + $scope.params.commission;
            paramsurl += "&bySupplier=" + $scope.params.bySupplier;


            paramsurl += "&customer=" + (($scope.params.customer == null) ? "" : $scope.params.customer.CarrierAccountID);
            paramsurl += "&supplier=" + (($scope.params.supplier == null) ? "" : $scope.params.supplier.CarrierAccountID);

            window.open("/Reports/Analytics/BillingReports.aspx?" + paramsurl, "_blank", "width=400, height=200,scrollbars=1");
        }
        
    }
    function load() {
        loadReportTypes();
        loadCustomers();
        loadSuppliers();
    }

    function loadReportTypes() {
        ReportAPIService.GetAllReportDefinition().then(function (response) {
            $scope.reportsTypes = response;
        })
    }
    function loadCustomers() {
        CarriersService.getCustomers().then(function (response) {
            $scope.optionsCustomers = response;
        })
    }
    function loadSuppliers() {
        CarriersService.getSuppliers().then(function (response) {
            $scope.optionsSuppliers = response;
        })
    }

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);