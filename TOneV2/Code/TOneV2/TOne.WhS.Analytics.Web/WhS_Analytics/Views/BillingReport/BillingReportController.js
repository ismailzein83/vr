"use strict";
BillingReportsController.$inject = ['$scope', 'WhS_Analytics_ReportDefinitionAPIService', 'VRNotificationService', 'UtilsService', 'SecurityService', 'VRUIUtilsService', 'PeriodEnum', 'WhS_Analytics_BillingReportAPIService', 'VRCommon_VRTempPayloadAPIService', 'InsertOperationResultEnum'];

function BillingReportsController($scope, ReportDefinitionAPIService, VRNotificationService, UtilsService, SecurityService, VRUIUtilsService, PeriodEnum, BillingReportAPIService, VRCommon_VRTempPayloadAPIService, InsertOperationResultEnum) {


    var typeSelectorAPI;
    var typeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            if (response.data.byteLength > 10000)
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
        var numberOfDays;
        if (($scope.reporttype && $scope.reporttype.ParameterSettings && $scope.reporttype.ParameterSettings.CustomerIdNotOptional == false) || $scope.toDate == null)
            return null;

        if ($scope.fromDate.getMonth() == $scope.toDate.getMonth()) {
            numberOfDays = daysInMonth($scope.fromDate.getMonth() + 1, $scope.fromDate.getFullYear());
        }
        else {
            var diffDate = new Date($scope.fromDate);
            diffDate.setMonth($scope.fromDate.getMonth() + 1);
            numberOfDays = UtilsService.diffDays($scope.fromDate, diffDate) - 1;
        }
        if ($scope.reporttype && $scope.reporttype.ParameterSettings && $scope.reporttype.ParameterSettings.CustomerIdNotOptional == true && UtilsService.diffDays($scope.fromDate, $scope.toDate) < numberOfDays) {
            return 'At least you have to choose ' + numberOfDays + ' days.';
        }
        return null;

    };

    function daysInMonth(month, year) {
        return new Date(year, month, 0).getDate();
    }

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
        service: false,
        commission: false,
        bySupplier: false,
        margin: 10,
        isExchange: false,
        top: 10,
        pageBreak: false,
        isCost: false
    };


    function defineScope() {

        $scope.today = PeriodEnum.Today;

        $scope.onTimeRangeDirectiveReady = function (api) {
            timeRangeDirectiveAPI = api;
            timeRangeReadyPromiseDeferred.resolve();
        };
        $scope.onBillingReportTypeSelectReady = function (api) {
            typeSelectorAPI = api;
            typeReadyPromiseDeferred.resolve();
        };
        $scope.onCurrencySelectReady = function (api) {
            currencySelectorAPI = api;
            currencyReadyPromiseDeferred.resolve();
        };
        $scope.onSaleZoneDirectiveReady = function (api) {
            saleZoneDirectiveAPI = api;
            saleZoneReadyPromiseDeferred.resolve();
        };

        $scope.onTypeSelectionChanged = function () {
            if (typeSelectorAPI.getSelectedIds() != undefined) {
                $scope.params.isCost = typeSelectorAPI.getSelectedIds();
                $scope.params.bySupplier = false;
            }
        };

        $scope.openReport = function () {


            if (!$scope.reporttype.ParameterSettings.CustomerIdNotOptional) {
                var obj = buildReportParameter();
                return VRCommon_VRTempPayloadAPIService.AddVRTempPayload(obj).then(function (response) {
                    if (response.Result == InsertOperationResultEnum.Succeeded.value) {
                        var tempPayloadId = response.InsertedObject;
                        var paramsurl = "";
                        paramsurl += "tempPayloadId=" + tempPayloadId;
                        paramsurl += "&reportId=" + $scope.reporttype.ReportDefinitionId;
                        var screenWidth = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
                        var left = ((screenWidth / 2) - (1000 / 2));
                        window.open("Client/Modules/WhS_Analytics/Reports/Analytics/BillingReports.aspx?" + paramsurl, "_blank", "toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=yes, copyhistory=no,width=1000, height=600,scrollbars=1 , top = 40, left = " + left + "");
                    }
                }).catch(function (error) {
                });
            }
            else
                return $scope.export();
        };


        $scope.resetReportParams = function () {
            $scope.singleCustomer = null;
            $scope.customers = [];
            $scope.suppliers = [];
            // $scope.reportRdlcType = null;
            $scope.selectedPeriod = $scope.periods[6];

            if ($scope.reporttype && $scope.reporttype.ParameterSettings && $scope.reporttype.ParameterSettings.CustomerIdNotOptional == true) {
                setTimeout(function () {
                    $scope.selectedPeriod = $scope.periods[1];
                });

            }
            else if ($scope.reporttype) {
                setTimeout(function () {
                    $scope.params.reportRdlcType = $scope.reporttype.ReportDefinitionRDLCFiles[0];
                });
            }
               

            //else {
            //    $scope.selectedPeriod = $scope.periods[6];
            //}
            $scope.params = {
                groupByCustomer: false,
                reportRdlcType: null,
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
                pageBreak: false,
                isCost: false
            };
            if (saleZoneDirectiveAPI != undefined)
                saleZoneDirectiveAPI.load();
            if (typeSelectorAPI != undefined)
                typeSelectorAPI.load();
        };
    }

    function load() {
        $scope.isLoading = true;
        loadAllControls();
    }

    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([loadReportTypes, loadBillingReportTypeSelector, loadCurrencySelector, loadCustomers, loadSingleCustomers, loadSuppliers, loadTimeRangeSelector, loadSaleZones])
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
    }

    function buildReportParameter() {
        return {
            Settings: {
                $type: "TOne.WhS.Analytics.Entities.BillingReport.ReportParameters, TOne.WhS.Analytics.Entities",
                FromTime: $scope.fromDate,
                ReportDefinitionRDLCFileId: $scope.params.reportRdlcType.ReportDefinitionRDLCFileId,
                ToTime: $scope.toDate,
                GroupByCustomer: $scope.params.groupByCustomer,
                CustomersId: customerAccountDirectiveAPI.getSelectedIds() != undefined ? customerAccountDirectiveAPI.getSelectedIds().join(",") : "",
                SuppliersId: supplierAccountDirectiveAPI.getSelectedIds() != undefined ? supplierAccountDirectiveAPI.getSelectedIds().join(",") : "",
                IsCost: typeSelectorAPI.getSelectedIds(),
                IsService: $scope.params.service,
                IsCommission: $scope.params.commission,
                GroupBySupplier: $scope.params.bySupplier,
                CurrencyId: currencySelectorAPI.getSelectedIds(),
                Margin: $scope.params.margin,
                ZonesId: saleZoneDirectiveAPI.getSelectedIds() != undefined ? saleZoneDirectiveAPI.getSelectedIds().join(",") : "",
                IsExchange: $scope.params.isExchange,
                Top: $scope.params.top,
                CurrencyDescription: $scope.selectedCurrency == null ? "United States Dollars" : $scope.selectedCurrency.Name,
                PageBreak: $scope.params.pageBreak,
                GroupByProfile: $scope.params.groupByProfile
            }
        };

    }

    function loadBillingReportTypeSelector() {
        var typeLoadPromiseDeferred = UtilsService.createPromiseDeferred();

        typeReadyPromiseDeferred.promise
            .then(function () {
                VRUIUtilsService.callDirectiveLoad(typeSelectorAPI, undefined, typeLoadPromiseDeferred);
            });
        return typeLoadPromiseDeferred.promise;
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
                period: PeriodEnum.CurrentMonth.value
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
}

appControllers.controller('WhS_Analytics_BillingReportsController', BillingReportsController);