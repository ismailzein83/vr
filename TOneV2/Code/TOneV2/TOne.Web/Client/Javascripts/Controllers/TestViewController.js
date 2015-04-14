'use strict'

var TestViewController = function (CarriersService, ZonesService) {

    var ctrl = this;
    load();
    //loadZone();
    //loadCarriers();

    ctrl.selectedRoutes = function (items, item, data) {

    }

    ctrl.onsearch = function (text) {
        return ZonesService.getSalesZones(text);
    }

    ctrl.onready = function (api) {
        //ZonesService.getSalesZones("Lebanon").then(function (items) {
        //    api.setSelectedValues(items);
        //}, function (msg) {
        //    console.log(msg);
        //});

        
    }
    

    function load() {
        ctrl.model = 'Test View model';
        ctrl.Input1 = '';
        ctrl.Input2 = '';
        ctrl.data = [{ name: "Moroni", age: 50 },
                    { name: "Tiancum", age: 43 },
                    { name: "Jacob", age: 27 }];

        ctrl.routes = [{ name: "Moroni", value: 50 },
            { name: "Tiancum", value: 43 },
            { name: "Jacob", value: 27 }];

        ctrl.routesOptions = {
            lastselectedvalue: {},
            datasource: [{ name: "Moroni", value: 50 },
            { name: "Tiancum", value: 43 },
            { name: "Jacob", value: 27 }]
        };

        ctrl.options = {
            selectedvalues: [],
            lastselectedvalue: {},
            datasource: []
        };

        ctrl.customvalidate1 = function (api) {
            
            var isvalid = false;
            if (api == undefined || api.selectedvalues == undefined) isvalid = false;
            else if (api.selectedvalues.length == 2) isvalid = true;

            if (isvalid) return '';
            return 'Maximum length : 2';
        }

        ctrl.customvalidate2 = function (api) {

            var isvalid = false;
            if (api == undefined || api.lastselectedvalue == undefined) isvalid = false;
            else if (api.lastselectedvalue.Name == 'TEST') isvalid = true;

            if (isvalid) return '';
            return 'Name : TEST';
        }

        ctrl.validationGroup = [];

        ctrl.optionsFilter = {
            selectedvalues: [],
            datasource: ['5', '10', '15'],
            onselectionchanged: function (selectedvalues, datasource) {
                console.log(selectedvalues);
                console.log(datasource);
            }
        };

        ctrl.optionsZone = {
            selectedvalues: [],
            datasource: function (text) {
                console.log(text);
                return ZonesService.getSalesZones(text);
            },
            onselectionchanged: function (selectedvalues, datasource) {
                console.log(selectedvalues);
                console.log(datasource);
            }
        };

        ctrl.alertMsg = function () {

        };
        
        ctrl.validateForm = function () {
            if (ctrl.api == undefined || ctrl.api.isvalid == undefined) return true;
            return ! ctrl.api.isvalid();
        };
        
    }
    
    function loadZone() {
        ZonesService.getSalesZones("L").then(function (items) {
            ctrl.optionsZone.datasource = items;
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