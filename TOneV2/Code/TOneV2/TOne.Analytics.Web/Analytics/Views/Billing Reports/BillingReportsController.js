BillingReportsController.$inject = ['$scope', 'ReportAPIService', 'CarrierAccountAPIService', 'ZonesService', 'BillingStatisticsAPIService', 'MainService', 'BaseAPIService', 'UtilsService', 'AnalyticsService', 'CurrencyAPIService', 'SecurityService'];

function BillingReportsController($scope, ReportAPIService, CarrierAccountAPIService, ZonesService, BillingStatisticsAPIService, MainService, BaseAPIService, UtilsService, analyticsService, currencyAPIService, SecurityService) {

    defineScope();
    load();

    $scope.export = function () {
        return BillingStatisticsAPIService.ExportCarrierProfile($scope.fromDate, $scope.toDate, $scope.params.top, $scope.params.customer.CarrierAccountID, $scope.params.selectedCurrency.CurrencyID, $scope.params.selectedCurrency.Name).then(function (response) {
                 UtilsService.downloadFile(response.data, response.headers);
            });
        
    };

    $scope.reportsTypes = [];
    $scope.optionsCustomers = [];
    $scope.optionsSuppliers = [];
    $scope.reportsTypes = [];
    $scope.optionsCurrencies = []; 
    $scope.periods = analyticsService.getPeriods();
    $scope.selectedPeriod = $scope.periods[1];
    var selectedPeriod;
    $scope.params = {
        groupByCustomer: false,
        selectedCustomers: [],
        customer: null,
        selectedSuppliers: [],
        selectedCurrency : null ,
        zones: [],
        isCost: false,
        service: false,
        commission: false,
        bySupplier: false,
        margin: 10,
        isExchange: false,
        top: 10
    }
    $scope.periodSelectionChanged = function () {
        if ($scope.selectedPeriod != undefined && $scope.selectedPeriod.value != -1) {
            var date = $scope.selectedPeriod.getInterval();
            $scope.fromDate = date.from;
            $scope.toDate = date.to;
        }

    }
    $scope.onValueChanged = function () {
        if ($scope.selectedPeriod != selectedPeriod) {
            var customize = {
                value: -1,
                description: "Customize"
            }
            selectedPeriod = $scope.selectedPeriod;
            $scope.selectedPeriod = customize;
        }
    }

    $scope.customvalidateTestFrom = function (fromDate) {
        return UtilsService.validateDates(fromDate, $scope.toDate);
    };
    $scope.customvalidateTestTo = function (toDate) {
        return UtilsService.validateDates($scope.fromDate, toDate);
    };
    function defineScope() {

        $scope.fromDate = '01/01/2013';
        $scope.toDate =  new Date();
        $scope.optionsZones = function (filterText) {
            return ZonesService.getSalesZones(filterText);
        };

        $scope.openReport = function () {
            var paramsurl = "";
            paramsurl += "reportId=" + $scope.reporttype.ReportDefinitionId;
            paramsurl += "&fromDate=" + $scope.dateToString($scope.fromDate);
            paramsurl += "&toDate=" + $scope.dateToString($scope.toDate);
            paramsurl += "&groupByCustomer=" + $scope.params.groupByCustomer;
            paramsurl += "&isCost=" + $scope.params.isCost;
            paramsurl += "&service=" + $scope.params.service;
            paramsurl += "&commission=" + $scope.params.commission;
            paramsurl += "&bySupplier=" + $scope.params.bySupplier;
            paramsurl += "&isExchange=" + $scope.params.isExchange;
            paramsurl += "&margin=" + $scope.params.margin;
            paramsurl += "&top=" + $scope.params.top;
            paramsurl += "&zone=" + (($scope.params.zones.length == 0) ? "" : getIdsList($scope.params.zones, 'ZoneId'));
            paramsurl += "&customer=" + (($scope.params.selectedCustomers.length == 0) ? "" : getIdsList($scope.params.selectedCustomers, 'CarrierAccountID'));
            paramsurl += "&supplier=" + (($scope.params.selectedSuppliers.length == 0) ? "" : getIdsList($scope.params.selectedSuppliers, 'CarrierAccountID'));
            paramsurl += "&currency=" + (($scope.params.selectedCurrency == null) ? "USD" : $scope.params.selectedCurrency.CurrencyID);
            paramsurl += "&currencyDesc=" + (($scope.params.selectedCurrency == null) ? "United States Dollars" :encodeURIComponent( $scope.params.selectedCurrency.Name));

            paramsurl += "&Auth-Token="  +encodeURIComponent( SecurityService.getUserToken() ) ;


            if (!$scope.reporttype.ParameterSettings.CustomerIdNotOptional)
                window.open("/Reports/Analytics/BillingReports.aspx?" + paramsurl , "_blank", "width=1000, height=600,scrollbars=1");
            else
                return $scope.export();
        }
        $scope.resetReportParams = function () {

            $scope.params = {              
                groupByCustomer: false,
                selectedCustomers: [],
                selectedSuppliers: [],
                customer: null,
                selectedCurrency: $scope.optionsCurrencies[getMainCurrencyIndex($scope.optionsCurrencies)],
                // isCost: false,
                zones: [],
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
        loadCurrencies();

        loadCustomers();
        //loadSuppliers();
    }

    function getIdsList(tab, attname) {
        var list = [];
        for (var i = 0; i < tab.length ; i++) 
            list[list.length] = tab[i][attname];        
        return list.join(",");        
        
    }
    function loadCurrencies() {
        currencyAPIService.GetVisibleCurrencies().then(function (response) {
            $scope.optionsCurrencies = response;
            $scope.params.selectedCurrency = $scope.optionsCurrencies[getMainCurrencyIndex($scope.optionsCurrencies)];

        });
    }
    function loadReportTypes() {
        ReportAPIService.GetAllReportDefinition().then(function (response) {
            $scope.reportsTypes = response;
        });
    }
    function getMainCurrencyIndex(Currencies) {
        var index = -1;
        for (var i = 0; i < Currencies.length ; i++)
            if (Currencies[i].IsMainCurrency == "Y") {
                index = i;
                return index;
            }
        return index;
    }

    function loadCustomers() {
        CarrierAccountAPIService.GetCarriers(1, true).then(function (response) {
            $scope.optionsCustomers = response;
        });
    }
    //function loadSuppliers() {
    //    CarrierAccountAPIService.GetCarriers(2,false).then(function (response) {
    //        $scope.optionsSuppliers = response;
    //    });
    //}

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);