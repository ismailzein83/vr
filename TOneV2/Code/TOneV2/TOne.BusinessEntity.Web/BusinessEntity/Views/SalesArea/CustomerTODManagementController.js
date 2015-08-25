CustomerTODManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'ZonesService', 'VRModalService', 'CarrierTypeEnum', 'TODAPIService'];
function CustomerTODManagementController($scope, CarrierAccountAPIService, ZonesService, VRModalService, CarrierTypeEnum, TODAPIService) {
    var gridApi;
   
    function load() {
        $scope.isLoading = true;
        loadCustomers();

    }
    function defineScope() {
        $scope.customers = [];
        $scope.searchZones = [];
        $scope.selectedZones = [];
        $scope.datasource = [];
        $scope.effectiveOn = new Date();
        defineMenuActions();
        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };
        $scope.searchZones = function (text) {
            return ZonesService.getSalesZones(text);
        }
        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return TODAPIService.GetFilteredCustomerTOD(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        };
    }

    function retrieveData() {
        var query = {
           };
        if ($scope.selectedCustomer != undefined && $scope.selectedCustomer != null)
            query.CustomerId = $scope.selectedCustomer.CarrierAccountID;

        else
            query.CustomerId = null;
        if ($scope.selectedZones.length > 0) {
            query.ZoneIds = [];
            angular.forEach($scope.selectedZones, function (z) {
                query.ZoneIds.push(z.ZoneId);
            });

        }
        else
            query.ZoneIds = [];
        query.EffectiveOn = $scope.effectiveOn;
        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit"//,
            //clicked: editCarrierAccount
        }];
    }

  

    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value ,false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
        }).finally(function () {
            $scope.isLoading = false;

        });
    }
    defineScope();
    load();
}

appControllers.controller('Customer_CustomerTODManagementController', CustomerTODManagementController);