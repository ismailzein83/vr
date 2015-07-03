﻿BillingReportsController.$inject = ['$scope', 'ReportAPIService', 'CarrierAPIService', 'ZonesService'];

function BillingReportsController($scope, ReportAPIService, CarrierAPIService, ZonesService) {
   
    defineScope();
    load();
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
        zone:null,
        isCost: false,
        service: false,
        commission: false,
        bySupplier: false,
        margin: 10,
        isExchange: false,
        top:10
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
            paramsurl += "&isExchange=" + $scope.params.isExchange;
            paramsurl += "&margin=" + $scope.params.margin;
            paramsurl += "&top=" + $scope.params.top;
            paramsurl += "&zone=" + (($scope.params.zone == null) ? 0 : $scope.params.zone.ZoneId);
            paramsurl += "&customer=" + (($scope.params.customer == null) ? "" : $scope.params.customer.CarrierAccountID);
            paramsurl += "&supplier=" + (($scope.params.supplier == null) ? "" : $scope.params.supplier.CarrierAccountID);

            window.open("/Reports/Analytics/BillingReports.aspx?" + paramsurl, "_blank", "width=1000, height=600,scrollbars=1");
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
        loadZones();
    }

    function loadReportTypes() {
        ReportAPIService.GetAllReportDefinition().then(function (response) {
            $scope.reportsTypes = response;
        })
    }
    function loadCustomers() {
        CarrierAPIService.GetCarriers(1).then(function (response) {
            $scope.optionsCustomers = response;
        })
    }
    function loadSuppliers() {
        CarrierAPIService.GetCarriers(2).then(function (response) {
            $scope.optionsSuppliers = response;
        })
    }
    function loadZones() {
        ZonesService.getSalesZones("%").then(function (response) {
            $scope.optionsZones = response;
        })
    }

};


appControllers.controller('Analytics_BillingReportsController', BillingReportsController);