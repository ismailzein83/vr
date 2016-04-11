"use strict";

app.directive("vrAnalyticGridfilter", ['UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'VRValidationService', 'VR_Analytic_AnalyticConfigurationAPIService',
function (UtilsService, VRNotificationService, VRUIUtilsService, VRValidationService, VR_Analytic_AnalyticConfigurationAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var genericFilter = new GenericFilter($scope, ctrl, $attrs);
            genericFilter.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: function () {
            return "/Client/Modules/Analytic/Directives/Grid/Templates/AnalyticReportGridFilter.html";
        }
    };
    function GenericFilter($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var customerAccountDirectiveAPI;
        var customerAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var supplierAccountDirectiveAPI;
        var supplierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var sellingNumberPlanDirectiveAPI;
        var sellingNumberPlanReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var countryDirectiveApi;
        var countryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var currencyDirectiveAPI;
        var currencyReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var saleZoneDirectiveAPI;
        var saleZoneReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            ctrl.dimensions = [];
            ctrl.measures = [];

            ctrl.periods = [];
            ctrl.selectedCountries = [];
            ctrl.selectedCustomers = [];
            ctrl.selectedSuppliers = [];
            ctrl.selecteddimensions = [];
            ctrl.selectedMeasures = [];
            ctrl.measureThresholds = [];

            ctrl.fromdate = new Date();

            ctrl.validateDateTime = function () {
                return VRValidationService.validateTimeRange(ctrl.fromdate, ctrl.todate);
            }

            ctrl.onCustomerAccountDirectiveReady = function (api) {
                customerAccountDirectiveAPI = api;
                customerAccountReadyPromiseDeferred.resolve();
            }

            ctrl.onSupplierAccountDirectiveReady = function (api) {
                supplierAccountDirectiveAPI = api;
                supplierAccountReadyPromiseDeferred.resolve();
            }

            ctrl.onCurrencyDirectiveReady = function (api) {
                currencyDirectiveAPI = api;
                currencyReadyPromiseDeferred.resolve();
            }

            ctrl.onCountryDirectiveReady = function (api) {
                countryDirectiveApi = api;
                countryReadyPromiseDeferred.resolve();
            }

            ctrl.onSellingNumberPlanDirectiveReady = function (api) {
                sellingNumberPlanDirectiveAPI = api;
                sellingNumberPlanReadyPromiseDeferred.resolve();
            }

            ctrl.onSaleZoneDirectiveReady = function (api) {
                saleZoneDirectiveAPI = api;
                saleZoneReadyPromiseDeferred.resolve();
            }

            ctrl.onSelectSellingNumberPlan = function (selectedItem) {
                ctrl.showSaleZoneSelector = true;

                var payload = {
                    sellingNumberPlanId: selectedItem.SellingNumberPlanId,
                }

                var setLoader = function (value) { ctrl.isLoadingSaleZonesSection = value };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, saleZoneDirectiveAPI, payload, setLoader);
            }

            ctrl.isrequired = function () {
                return ctrl.periods.length == 0;
            }

            defineAPI();

        }

        function defineAPI() {
            var api = {};

            api.getData = function () {

                var selectedobject = {
                    selecteddimensions: UtilsService.getPropValuesFromArray(ctrl.selecteddimensions, "value"),
                    //selectedfilters: selectedfilters,
                    selectedperiod: ctrl.selectedperiod != undefined ? ctrl.selectedperiod.value : undefined,
                    fromdate: ctrl.fromdate,
                    todate: ctrl.todate,
                    currency: currencyDirectiveAPI != undefined ? currencyDirectiveAPI.getSelectedIds() : undefined
                };
                return selectedobject;
            }

            api.load = function (payload) {
                ctrl.isLoadCustomers;
                ctrl.isLoadSuppliers;
                ctrl.isLoadSellingNumberPlan;
                ctrl.isLoadCountries;
                ctrl.isLoadCurrencies;

                var promises = [];

                function loadSellingNumberPlanSection() {
                    var loadSellingNumberPlanPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadSellingNumberPlanPromiseDeferred.promise);
                    sellingNumberPlanReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(sellingNumberPlanDirectiveAPI, undefined, loadSellingNumberPlanPromiseDeferred);
                    });
                }

                function loadCustomers() {
                    var loadCustomerAccountPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCustomerAccountPromiseDeferred.promise);
                    customerAccountReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(customerAccountDirectiveAPI, undefined, loadCustomerAccountPromiseDeferred);
                    });
                }

                function loadSuppliers() {
                    var loadSupplierAccountPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadSupplierAccountPromiseDeferred.promise);
                    supplierAccountReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(supplierAccountDirectiveAPI, undefined, loadSupplierAccountPromiseDeferred);
                    });
                }

                function loadCountries() {
                    var loadCountryPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCountryPromiseDeferred.promise);
                    countryReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(countryDirectiveApi, undefined, loadCountryPromiseDeferred);
                    });
                }

                function loadCurrencies() {
                    var loadCurrencyPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(loadCurrencyPromiseDeferred.promise);
                    currencyReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(currencyDirectiveAPI, undefined, loadCurrencyPromiseDeferred);
                    });
                }

                function loadDimensions() {

                    return VR_Analytic_AnalyticConfigurationAPIService.GetDimensionsInfo().then(function (response) {
                        ctrl.dimensions.length = 0;
                        angular.forEach(response, function (itm) {
                            ctrl.dimensions.push(itm);
                        });
                    });
                }

                function loadMeasures() {
                    return VR_Analytic_AnalyticConfigurationAPIService.GetMeasuresInfo().then(function (response) {
                        ctrl.measures.length = 0;
                        angular.forEach(response, function (itm) {
                            ctrl.measures.push(itm);
                        });
                    });
                }

                loadDimensions();
                loadMeasures();
                loadCustomers();
                loadSuppliers();
                loadSellingNumberPlanSection();
                loadCountries();
                loadCurrencies();
                return UtilsService.waitMultiplePromises(promises);
            }

            if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                ctrl.onReady(api);
        }
    }
    return directiveDefinitionObject;

}]);