CustomerPricelistsController.$inject = ['$scope', 'UtilsService', '$q','CarrierAccountAPIService','CarrierTypeEnum','CustomerPricelistsAPIService'];

function CustomerPricelistsController($scope, UtilsService, $q, CarrierAccountAPIService, CarrierTypeEnum, CustomerPricelistsAPIService) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.data = [];
        $scope.showResult = false;
        $scope.selectedCustomer ;
        $scope.customers = [];
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CustomerPricelistsAPIService.GetCustomerPriceLists(dataRetrievalInput).then(function (response) {
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
        var query = $scope.selectedCustomer.CarrierAccountID;

        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadCustomers();
    }

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value,false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
          
        });

    }
};
appControllers.controller('BE_CustomerPricelistsController', CustomerPricelistsController);