﻿'use strict'

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
        
        function loadRemoteData() {
            return CarriersService.getCustomers().then(function (customers) {
                controller.customers = customers;
            }, function (msg) {
                console.log(msg);
            });
        }
        loadRemoteData();
        this.zones;

        $scope.selectedcustomers = [];
        this.output = [];
        
        this.lstselectedcustomer = '';
        

        this.selectedvalues = function (items) {
            $scope.selectedcustomers = items;
        }
        
        this.routes = [{ name: "Moroni", value: 50 },
                { name: "Tiancum", value: 43 },
                { name: "Jacob", value: 27 }];

        this.lstselectedroute = { name: "Jacob", value: 27 };

        this.selectedRoutes = function (items,item) {
            //if (items.length > 0)
            //    console.log(items[items.length - 1].name);
            //console.log(item);
            //if (item.name == "Tiancum")
            //    return { name: "Jacob", value: 27 };
        }

        this.onsearch = function (text) {
            return ZonesService.getSalesZones(text);
        }

        
        
});