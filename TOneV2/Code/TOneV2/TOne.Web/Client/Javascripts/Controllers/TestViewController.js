'use strict'

appControllers.service('MainService', function () {
    
    return ({
        getBaseURL: getBaseURL
    });

    function getBaseURL(){
        var pathArray = location.href.split('/');
        return pathArray[0] + '//' + pathArray[2];
    }

});

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
        return (request.then(handleSuccess, handleError));
    }

    function handleError(response) {
        if (!angular.isObject(response.data) || !response.data.message) {
            return ($q.reject("An unknown error occurred."));
        }
        return ($q.reject(response.data.message));
    }

    function handleSuccess(response) {
        return (response.data);
    }

});

appControllers.controller('TestViewController', function (CarriersService) {

        this.model = 'Test View model';
        this.Input = '123';
        this.alertMsg = function () {
            alert(this.Input);
        };

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
        this.selectedvalues;
        function loadRemoteData() {
            return CarriersService.getCustomers().then(function (customers) {
                controller.customers = customers;
            });
        }
        loadRemoteData();
        
});