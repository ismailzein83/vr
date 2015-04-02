'use strict'

var TestViewController = function (CarriersService, ZonesService) {

    var ctrl = this;
    load();
    loadZone();
    loadCarriers();

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

        ctrl.validationoptions1 = {
        };

        ctrl.customvalidate = function (api) {
            if (api == undefined || api.lastselectedvalue == undefined) return false;
                
            if (api.lastselectedvalue.Name == 'TEST') return true;
            return false;
        }

        ctrl.validationGroup = [];
        ctrl.validationGroup.push(ctrl.validationoptions1);
        ctrl.validationGroup.push(ctrl.validationoptions2);

        ctrl.optionsZone = {
            selectedvalues: [],
            datasource: [],
            lastselectedvalue: ''
        };

        ctrl.validationOptions = {
            value: ctrl.options.selectedvalues
        };

        ctrl.alertMsg = function () {
            //ctrl.validationOptions.value = ctrl.options.selectedvalues;
            //console.log(ctrl.validationOptions.value);
            //console.log(ctrl.options.selectedvalues);
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