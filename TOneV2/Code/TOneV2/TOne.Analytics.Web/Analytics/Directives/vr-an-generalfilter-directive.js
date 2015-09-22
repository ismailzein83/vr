﻿(function (angular, app) {

    "use strict";

    function vrDirectiveObj(analyticsService, BusinessEntityAPIService, ZonesService, CarrierAccountConnectionAPIService, CarrierTypeEnum, GenericAnalyticDimensionEnum, CurrencyAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                dimensionsvalues: "=",
                filtervalues: "=",
                selectedobject: "="
            },
            controller: function () {
                var ctrl = this;

                function onLoad() {
                    ctrl.GenericAnalyticDimensionEnum = GenericAnalyticDimensionEnum;

                    ctrl.switches = [];
                    ctrl.selectedSwitches = [];

                    ctrl.codeGroups = [];
                    ctrl.selectedCodeGroups = [];

                    ctrl.customers = [];
                    ctrl.selectedCustomers = [];

                    ctrl.suppliers = [];
                    ctrl.selectedSuppliers = [];

                    ctrl.zones = [];
                    ctrl.selectedZones = [];

                    ctrl.connections = [];
                    ctrl.selectedConnections = [];
                    ctrl.selectedConnectionIndex;

                    ctrl.currency = {
                        selectedvalues: '',
                        datasource: []
                    };

                    ctrl.filterCustomer = {
                        Dimension: GenericAnalyticDimensionEnum.Customer.value,
                        FilterValues: []
                    };
                    ctrl.filterSupplier = {
                        Dimension: GenericAnalyticDimensionEnum.Supplier.value,
                        FilterValues: []
                    };

                    ctrl.selectedobject = {
                        selecteddimensions: [],
                        selectedfilters: [],
                        fromdate: new Date(2013, 1, 1),
                        todate: new Date(),
                        currency: (ctrl.currency.selectedvalues == null || ctrl.currency.selectedvalues == "" ) ? null : ctrl.currency.selectedvalues.CurrencyID
                    };

                    loadSwitches();
                    loadCodeGroups();
                    loadCurrencies();
                    loadConnections();
                }

                function loadSwitches() {
                    return BusinessEntityAPIService.GetSwitches().then(function (response) {
                        ctrl.switches = response;
                    });
                }

                function loadCodeGroups() {
                    return BusinessEntityAPIService.GetCodeGroups().then(function (response) {
                        ctrl.codeGroups = response;
                    });
                }
                
                function loadCurrencies() {
                    return CurrencyAPIService.GetCurrencies().then(function (response) {
                        ctrl.currency.datasource = response;
                    });
                }

                function loadConnections() {
                    return CarrierAccountConnectionAPIService.GetConnectionByCarrierType(CarrierTypeEnum.Customer.value).then(function (response) {
                        ctrl.selectedConnections.length = 0;
                        ctrl.connections.length = 0;
                        ctrl.connections = response;
                    });
                }

                function searchZones(text) {
                    return ZonesService.getSalesZones(text);
                }

                function onSelectionChanged() {
                    var value;
                    switch (ctrl.selectedConnectionIndex) {
                        case 0: value = CarrierTypeEnum.Customer.value;
                            break;
                        case 1: value = CarrierTypeEnum.Supplier.value;
                            break;
                    }
                    return CarrierAccountConnectionAPIService.GetConnectionByCarrierType(value).then(function (response) {
                        ctrl.selectedConnections.length = 0;
                        ctrl.connections.length = 0;
                        ctrl.connections = response;
                    });
                }

                function onselectionvalueschanged() {
                    ctrl.selectedobject.selectedfilters = [];
                    ctrl.filterCustomer.FilterValues = [];
                    ctrl.filterSupplier.FilterValues = [];

                    ctrl.selectedCustomers.forEach(function (item) {
                        ctrl.filterCustomer.FilterValues.push(item.CarrierAccountID);
                    });
                    if (ctrl.filterCustomer.FilterValues.length > 0)
                        ctrl.selectedobject.selectedfilters.push(ctrl.filterCustomer);

                    ctrl.selectedSuppliers.forEach(function (item) {
                        ctrl.filterSupplier.FilterValues.push(item.CarrierAccountID);
                    });
                    if (ctrl.filterSupplier.FilterValues.length > 0)
                        ctrl.selectedobject.selectedfilters.push(ctrl.filterSupplier);
                    ctrl.selectedobject.currency = (ctrl.currency.selectedvalues == null || ctrl.currency.selectedvalues == "") ? null : ctrl.currency.selectedvalues.CurrencyID;
                }

                angular.extend(this, {
                    searchZones: searchZones,
                    onSelectionChanged: onSelectionChanged,
                    onselectionvalueschanged: onselectionvalueschanged
                });
               
                
                onLoad();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function () {
                return "/Client/Modules/Analytics/Directives/vr-generalfilter.html";
            }
        };

        return directiveDefinitionObject;
    }
    
    vrDirectiveObj.$inject = ['AnalyticsService', 'BusinessEntityAPIService_temp', 'ZonesService', 'CarrierAccountConnectionAPIService','CarrierTypeEnum', 'GenericAnalyticDimensionEnum','CurrencyAPIService'];
    
    app.directive('vrAnGeneralfilter', vrDirectiveObj);

})(angular, app);