'use strict'

appControllers.controller('TestViewController', function ($scope, CarriersService, ZonesService) {

    this.model = 'Test View model';
    this.Input = '123';
    this.alertMsg = function () {
        alert(this.Input);
    }

    var tableColumnDefinition = [
              {
                  columnHeaderDisplayName: 'Name',
                  columnKey: 'name',
                  visible: true,
                  templateUrl: '/Client/Templates/Views/vr-gridview-v1.html',
                  enFilter: true,
                  columnSearchProperty: 'name'
              },
              {
                  columnHeaderDisplayName: 'Age',
                  columnKey: 'age',
                  visible: true,
                  enFilter: true,
                  columnSearchProperty: 'age'
              }
    ];

    this.data = [{ name: "Moroni", age: 50 },
            { name: "Tiancum", age: 43 },
            { name: "Jacob", age: 27 }];

    this.gridOptions = {
        columnDefinition: tableColumnDefinition
    };

    //DropDown
    var controller = this;
    this.customers;

    this.options = {
        selectedvalues: [],
        datasource: [],
        lastselectedvalue: ''
    };

    this.optionsZone = {
        selectedvalues: [],
        datasource: [],
        lastselectedvalue: ''
    };
    this.options.lastselectedvalue = { $type: "TOne.BusinessEntity.Entities.CarrierInfo, TOne.BusinessEntity.Entities", CarrierAccountID: "C097", Name: "TEST (test02)", $$hashKey: "object:25" }
    function loadRemoteData() {
        return CarriersService.getCustomers().then(function (customers) {
            controller.options.datasource = customers;
            //console.log(customers[0]);
        }, function (msg) {
            console.log(msg);
        });
    }
    loadRemoteData();
    this.zones;

    $scope.selectedcustomers = [];
    
    ZonesService.getSalesZones("Lebanon").then(function (items) {
        controller.optionsZone.selectedvalues = items;
    }, function (msg) {
        console.log(msg);
    });

    

    this.lstselectedcustomer = '';
    this.lstselectedroute = '';

    this.selectedvalues = function (items) {
        $scope.selectedcustomers = items;
    }

    this.routes = [{ name: "Moroni", value: 50 },
            { name: "Tiancum", value: 43 },
            { name: "Jacob", value: 27 }];

    controller.lstselectedroute = controller.routes[2];//{ name: "Jacob", value: 27 };

    this.selectedRoutes = function (items, item, data) {

        //if (items.length > 0)
        //    console.log(items[items.length - 1].name);

        //if (item.name == "Tiancum")
        //    return { name: "Jacob", value: 27 };

        //return controller.routes[1];
    }

    this.onsearch = function (text) {
        return ZonesService.getSalesZones(text);
    }



});