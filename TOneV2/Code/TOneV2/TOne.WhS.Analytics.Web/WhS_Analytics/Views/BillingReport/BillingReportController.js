"use strict";
BillingReportsController.$inject = ['$scope', 'WhS_Analytics_ReportDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'SecurityService', 'VRUIUtilsService', 'PeriodEnum', 'WhS_Analytics_BillingReportAPIService'];

function BillingReportsController($scope, ReportDefinitionAPIService, VRNotificationService, UtilsService,  SecurityService, VRUIUtilsService, PeriodEnum, BillingReportAPIService) {

    var currencySelectorAPI;
    var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var customerAccountDirectiveAPI;
    var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var singleCustomerAccountDirectiveAPI;
    var singleCustomerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var supplierAccountDirectiveAPI;
    var supplierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    var timeRangeDirectiveAPI;
    var timeRangeReadyPromiseDeferred = UtilsService.createPromiseDeferred();


    var saleZoneDirectiveAPI;
    var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    load();

    $scope.export = function () {
        var customers = (singleCustomerAccountDirectiveAPI != undefined && singleCustomerAccountDirectiveAPI.getSelectedIds() != undefined) ? singleCustomerAccountDirectiveAPI.getSelectedIds() : "";
        return BillingReportAPIService.ExportCarrierProfile($scope.fromDate, $scope.toDate, $scope.params.top, customers, currencySelectorAPI.getSelectedIds(), $scope.selectedCurrency.Symbol, $scope.selectedCurrency.Name).then(function (response) {
            if (response.data.byteLength > 6000)
                UtilsService.downloadFile(response.data, response.headers);
            else
                VRNotificationService.showWarning("No data to display");
        });
    };

    $scope.reportsTypes = [];

    $scope.optionsCustomers = [];
    $scope.optionsSuppliers = [];
    $scope.optionsCurrencies = [];

    $scope.periods = UtilsService.getArrayEnum(PeriodEnum);
    $scope.selectedPeriod = $scope.periods[6];

    $scope.onCustomerAccountDirectiveReady = function (api) {
        customerAccountDirectiveAPI = api;
        customerAccountReadyPromiseDeferred.resolve();
    };
    $scope.onSingleCustomerAccountDirectiveReady = function (api) {
        singleCustomerAccountDirectiveAPI = api;
        singleCustomerAccountReadyPromiseDeferred.resolve();
    };
    $scope.validateBusinessCaseStatus = function () {
        if ($scope.reporttype && $scope.reporttype.ParameterSettings && $scope.reporttype.ParameterSettings.CustomerIdNotOptional == true && UtilsService.diffDays($scope.fromDate, $scope.toDate) < 28) {
            return 'At least you have to choose 28 days.';
        }
        return null;

    };
    $scope.onSupplierAccountDirectiveReady = function (api) {
        supplierAccountDirectiveAPI = api;
        supplierAccountReadyPromiseDeferred.resolve();
    };

    $scope.params = {
        groupByCustomer: false,
        groupByProfile: false,
        selectedCustomers: [],
        customer: null,
        selectedSuppliers: [],
        selectedCurrency: null,
        zones: [],
        isCost: false,
        service: false,
        commission: false,
        bySupplier: false,
        margin: 10,
        isExchange: false,
        top: 10,
        pageBreak: false
    };


    function defineScope() {

        $scope.today = PeriodEnum.Today;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };

        $scope.onCurrencySelectReady = function (api) {
            currencySelectorAPI = api;
            currencyReadyPromiseDeferred.resolve();
        };

        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        };

        $scope.openReport = function () {
            var customers = (customerAccountDirectiveAPI != undefined && customerAccountDirectiveAPI.getSelectedIds() != undefined) ? customerAccountDirectiveAPI.getSelectedIds() : "";

            var suppliers = (supplierAccountDirectiveAPI != undefined && supplierAccountDirectiveAPI.getSelectedIds() != undefined) ? supplierAccountDirectiveAPI.getSelectedIds() : "";

            var zones = (saleZoneDirectiveAPI != undefined && saleZoneDirectiveAPI.getSelectedIds() != undefined) ? saleZoneDirectiveAPI.getSelectedIds() : "";

            var paramsurl = "";
            paramsurl += "reportId=" + $scope.reporttype.ReportDefinitionId;
            paramsurl += "&fromDate=" + UtilsService.dateToServerFormat($scope.fromDate);
            paramsurl += "&toDate=" + UtilsService.dateToServerFormat($scope.toDate);
            paramsurl += "&groupByCustomer=" + $scope.params.groupByCustomer;
            paramsurl += "&groupByProfile=" + $scope.params.groupByProfile;
            paramsurl += "&isCost=" + $scope.params.isCost;
            paramsurl += "&service=" + $scope.params.service;
            paramsurl += "&commission=" + $scope.params.commission;
            paramsurl += "&bySupplier=" + $scope.params.bySupplier;
            paramsurl += "&isExchange=" + $scope.params.isExchange;
            paramsurl += "&margin=" + $scope.params.margin;
            paramsurl += "&top=" + $scope.params.top;
            paramsurl += "&zone=" + zones;
            paramsurl += "&customer=" + customers;
            paramsurl += "&supplier=" + suppliers;
            paramsurl += "&currency=" + currencySelectorAPI.getSelectedIds();
            paramsurl += "&currencyDesc=" + (($scope.params.selectedCurrency == null) ? "United States Dollars" : encodeURIComponent($scope.params.selectedCurrency.Name));
            paramsurl += "&pageBreak=" + $scope.params.pageBreak;
            paramsurl += "&Auth-Token=" + encodeURIComponent(SecurityService.getUserToken());
            if (!$scope.reporttype.ParameterSettings.CustomerIdNotOptional)
                window.open("Client/Modules/WhS_Analytics/Reports/Analytics/BillingReports.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
            else
                return $scope.export();
        };

        $scope.resetGroupBySupplier = function () {
            $scope.params.bySupplier = false;
        };
        $scope.resetReportParams = function () {
            $scope.singleCustomer = null;
            $scope.customers = [];
            $scope.suppliers = [];
            $scope.selectedPeriod = $scope.periods[6];

            if ($scope.reporttype && $scope.reporttype.ParameterSettings && $scope.reporttype.ParameterSettings.CustomerIdNotOptional == true) {
                setTimeout(function() {
                    $scope.selectedPeriod = $scope.periods[1];
                });

            }
            else {
                $scope.selectedPeriod = $scope.periods[1];
            }
            $scope.params = {
                groupByCustomer: false,
                groupByProfile: false,
                selectedCustomers: [],
                selectedSuppliers: [],
                customer: null,
                selectedCurrency: "",
                zones: [],
                service: false,
                commission: false,
                bySupplier: false,
                margin: 10,
                isExchange: false,
                top: 10,
                pageBreak: false
            };
            if (saleZoneDirectiveAPI != undefined)
                saleZoneDirectiveAPI.load();
        };
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadReportTypes, loadCurrencySelector, loadCustomers, loadSingleCustomers, loadSuppliers, loadTimeRangeSelector, loadSaleZones])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function loadCurrencySelector() {
        var currencyLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        currencyReadyPromiseDeferred.promise
            .then(function () {
                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, { selectSystemCurrency: true }, currencyLoadPromiseDeferred);
            });
        return currencyLoadPromiseDeferred.promise;
    }

    function loadReportTypes() {
        ReportDefinitionAPIService.GetAllReportDefinition().then(function (response) {
            $scope.reportsTypes = response;
        });
    }

    function loadCustomers() {
        var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        customerAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, undefined, loadCustomerAccountPromiseDeferred);
        });

        return loadCustomerAccountPromiseDeferred.promise;
    }
    function loadSingleCustomers() {
        var loadSingleCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        singleCustomerAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(singleCustomerAccountDirectiveAPI, undefined, loadSingleCustomerAccountPromiseDeferred);
        });

        return loadSingleCustomerAccountPromiseDeferred.promise;
    }

    function loadSuppliers() {
        var loadSupplierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
        supplierAccountReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(supplierAccountDirectiveAPI, undefined, loadSupplierAccountPromiseDeferred);
        });

        return loadSupplierAccountPromiseDeferred.promise;
    }

    function loadTimeRangeSelector() {
        var timeRangeLoadPromiseDeferred = UtilsService.createPromiseDeferred();
        timeRangeReadyPromiseDeferred.promise.then(function () {
            var payload = {
                period: PeriodEnum.LastMonth.value
            };
            VRUIUtilsService.callDirectiveLoad(timeRangeDirectiveAPI, payload, timeRangeLoadPromiseDeferred);
        });
        return timeRangeLoadPromiseDeferred.promise;
    }

    function loadSaleZones() {
        var loadSaleZonesPromiseDeferred = UtilsService.createPromiseDeferred();
        saleZoneReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(saleZoneDirectiveAPI, undefined, loadSaleZonesPromiseDeferred);
        });

        return loadSaleZonesPromiseDeferred.promise;
    }
};

appControllers.controller('WhS_Analytics_BillingReportsController', BillingReportsController);