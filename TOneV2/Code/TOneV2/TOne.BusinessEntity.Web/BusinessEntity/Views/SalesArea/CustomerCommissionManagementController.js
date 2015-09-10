﻿CustomerCommissionManagementController.$inject = ['$scope', 'UtilsService', '$q', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'CustomerCommissionAPIService'];

function CustomerCommissionManagementController($scope, UtilsService, $q, CarrierAccountAPIService, CarrierTypeEnum, CustomerCommissionAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.data = [];
        $scope.showResult = false;
        $scope.selectedCustomer;
        $scope.selectedZones=[];
        $scope.customers = [];
        $scope.effectiveFrom=new Date();
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CustomerCommissionAPIService.GetCustomerCommissions(dataRetrievalInput).then(function (response) {
                onResponseReady(response);
                $scope.showResult = true;
            })
        };
        $scope.getData = function () {
            return retrieveData();
        };
    }

    function retrieveData() {
        if (mainGridAPI == undefined)
            return;
        var query ={
            
            CustomerId: $scope.selectedCustomer != undefined ? $scope.selectedCustomer.CarrierAccountID : null,
            ZoneIds: $scope.selectedZones.length > 0 ? UtilsService.getPropValuesFromArray($scope.selectedZones, 'ZoneId') : undefined,
            EffectiveFrom: $scope.effectiveFrom
            }

        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadCustomers();
    }

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        });

    }
};
appControllers.controller('BE_CustomerCommissionManagementController', CustomerCommissionManagementController);