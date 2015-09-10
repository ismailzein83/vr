BillingReportsController.$inject = ['$scope', 'ReportAPIService', 'CarrierAccountAPIService', 'ZonesService', 'BillingStatisticsAPIService', 'MainService', 'BaseAPIService', 'UtilsService'];

function BillingReportsController($scope, ReportAPIService, CarrierAccountAPIService, ZonesService, BillingStatisticsAPIService, MainService, BaseAPIService, UtilsService) {

    defineScope();
    load();

    $scope.export = function () {

            return BillingStatisticsAPIService.ExportCarrierProfile($scope.params.fromDate, $scope.params.toDate, $scope.params.top, $scope.params.customer.CarrierAccountID).then(function (response) {
                 UtilsService.downloadFile(response.data, response.headers);
            });
        
    };

    $scope.reportsTypes = [];
    $scope.optionsCustomers = [];
    $scope.optionsSuppliers = [];
    $scope.optionsZone = [];
    $scope.reportsTypes = [];
    $scope.params = {
        fromDate: "",
        toDate: "",
        groupByCustomer: false,
        customer: null,
        supplier: null,
        zone: null,
        isCost: false,
        service: false,
        commission: false,
        bySupplier: false,
        margin: 10,
        isExchange: false,
        top: 10
    }
    function defineScope() {

        $scope.fromDate = '2013/01/01';
        $scope.toDate = '2015/01/01';

        $scope.optionsZones = function (filterText) {
            return ZonesService.getSalesZones(filterText);
        };

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
            paramsurl += "&isExchange=" + $scope.params.isExchange;
            paramsurl += "&margin=" + $scope.params.margin;
            paramsurl += "&top=" + $scope.params.top;
            paramsurl += "&zone=" + (($scope.params.zone == null) ? 0 : $scope.params.zone.ZoneId);
            paramsurl += "&customer=" + (($scope.params.customer == null) ? "" : $scope.params.customer.CarrierAccountID);
            paramsurl += "&supplier=" + (($scope.params.supplier == null) ? "" : $scope.params.supplier.CarrierAccountID);

            if (!$scope.reporttype.ParameterSettings.CustomerIdNotOptional)
                window.open("/Reports/Analytics/BillingReports.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
            else
                return $scope.export();
        }
        $scope.resetReportParams = function () {

            $scope.params = {
                fromDate: "",
                toDate: "",
                groupByCustomer: false,
                customer: null,
                supplier: null,
                zone: null,
                isCost: false,
                service: false,
                commission: false,
                bySupplier: false,
                margin: 10,
                isExchange: false,
                top: 10
            }
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
        });
    }
    function loadCustomers() {
        CarrierAccountAPIService.GetCarriers(1, true).then(function (response) {
            $scope.optionsCustomers = response;
        });
    }
    function loadSuppliers() {
        CarrierAccountAPIService.GetCarriers(2,false).then(function (response) {
            $scope.optionsSuppliers = response;
        });
    }

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);