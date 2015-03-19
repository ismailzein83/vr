'use strict'

appControllers.service('CarriersService', function ($http, $q,MainService) {

    return ({
        getCustomers: getCustomers
    });

    function getCustomers() {

        var getCarriersURL = MainService.getBaseURL() + "/api/BusinessEntity/GetCarriers";

        var request = $http({
            method: "get",
            url: getCarriersURL,
            params: {
                carrierType: 1
            }
        });
        return (request.then(MainService.handleSuccess, MainService.handleError));
    }

    

});

appControllers.service('ZonesService', function ($http, $q, MainService) {

    return ({
        getSalesZones: getSalesZones
    });

    function getSalesZones(filterzone) {

        var getSalesZonesURL = MainService.getBaseURL() + "/api/BusinessEntity/GetSalesZones";

        var request = $http({
            method: "get",
            url: getSalesZonesURL,
            params: {
                nameFilter: filterzone
            }
        });
        return (request.then(MainService.handleSuccess, MainService.handleError));
    }

});

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
            });
        }
        loadRemoteData();
        this.zones;
        function loadOnDemand(text) {
            //return ZonesService.getSalesZones(text).then(function (zones) {
            //    controller.zones = zones;
            //});
        }

        $scope.selectedcustomers = [];
        this.output = [];
        
        this.lstselectedcustomer = '';
        this.lstselectedroute = '';

        this.selectedvalues = function (items) {
            $scope.selectedcustomers = items;
        }
        
        this.routes = [{ name: "Moroni", value: 50 },
                { name: "Tiancum", value: 43 },
                { name: "Jacob", value: 27 }];

        this.selectedRoutes = function (items) {
            if (items.length > 0)
                console.log(items[items.length - 1].name);
        }

        this.onsearch = function (text) {
            return ZonesService.getSalesZones(text);
            //loadOnDemand(text);
        }

        
        
});