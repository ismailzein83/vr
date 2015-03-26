'use strict'

var TestViewController = function (CarriersService, ZonesService) {

    var ctrl = this;
    load();
    loadZone();
    loadCarriers();

    ctrl.alertMsg = function () {
        alert(ctrl.Input);
    }

    ctrl.selectedRoutes = function (items, item, data) {

    }

    ctrl.onsearch = function (text) {
        return ZonesService.getSalesZones(text);
    }
    
    function load() {
        ctrl.model = 'Test View model';
        ctrl.Input = '123';
        ctrl.data = [{ name: "Moroni", age: 50 },
                    { name: "Tiancum", age: 43 },
                    { name: "Jacob", age: 27 }];

        ctrl.routes = [{ name: "Moroni", value: 50 },
            { name: "Tiancum", value: 43 },
            { name: "Jacob", value: 27 }];

        ctrl.options = {
            selectedvalues: [],
            datasource: []
        };

        ctrl.optionsZone = {
            selectedvalues: [],
            datasource: [],
            lastselectedvalue: ''
        };

        //ctrl.options.lastselectedvalue = { CarrierAccountID: "C097", Name: "TEST (test02)" };
    }

    function loadZone() {
        ZonesService.getSalesZones("Lebanon").then(function (items) {
            ctrl.optionsZone.selectedvalues = items;
        }, function (msg) {
            console.log(msg);
        });
    }

    function loadCarriers() {
        return CarriersService.getCustomers().then(function (customers) {
            ctrl.options.datasource = customers;
        }, function (msg) {
            console.log(msg);
        });
    }
    
}

TestViewController.$inject = [ 'CarriersService', 'ZonesService'];

appControllers.controller('TestViewController', TestViewController);