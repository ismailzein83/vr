CustomerPricelistsController.$inject = ['$scope', 'RawCDRLogAPIService', 'UtilsService', '$q', 'BusinessEntityAPIService_temp', 'RawCDRLogMeasureEnum', 'VRModalService', 'VRNotificationService','CarrierAccountAPIService','CarrierTypeEnum','CustomerPricelistsAPIService'];

function CustomerPricelistsController($scope, RawCDRLogAPIService, UtilsService, $q, BusinessEntityAPIService, RawCDRLogMeasureEnum, VRModalService, VRNotificationService, CarrierAccountAPIService, CarrierTypeEnum, CustomerPricelistsAPIService) {
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
            $scope.selectedCustomer = $scope.customers[0];
        });

    }
};
appControllers.controller('Analytics_CustomerPricelistsController', CustomerPricelistsController);