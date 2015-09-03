SupplierTODManagementController.$inject = ['$scope', 'CarrierAccountAPIService', 'ZonesService', 'VRModalService', 'CarrierTypeEnum', 'SupplierTODAPIService'];
function SupplierTODManagementController($scope, CarrierAccountAPIService, ZonesService, VRModalService, CarrierTypeEnum, SupplierTODAPIService) {
    var gridApi;
   
    function load() {
        loadSuppliers();

    }
    function defineScope() {
        $scope.suppliers = [];
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
            return SupplierTODAPIService.GetFilteredSupplierTOD(dataRetrievalInput)
            .then(function (response) {
                onResponseReady(response);
            });
        };
    }

    function retrieveData() {
        var query = {
           };
        if ($scope.selectedSupplier != undefined && $scope.selectedSupplier != null)
            query.SupplierId = $scope.selectedSupplier.CarrierAccountID;

        else
            query.SupplierId = null;
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

  

    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
        });
    }
    defineScope();
    load();
}

appControllers.controller('Supplier_SupplierTODManagementController', SupplierTODManagementController);