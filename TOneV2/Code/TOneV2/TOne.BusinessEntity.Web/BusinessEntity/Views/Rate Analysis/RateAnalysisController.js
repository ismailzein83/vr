RateAnalysisController.$inject = ['$scope', 'UtilsService', '$q', 'CarrierAccountAPIService', 'CarrierTypeEnum', 'RateAnalysisAPIService','ChangeEnum'];

function RateAnalysisController($scope, UtilsService, $q, CarrierAccountAPIService, CarrierTypeEnum, RateAnalysisAPIService, ChangeEnum) {
    var mainGridAPI;
    defineScope();
    load();
    function defineScope() {
        $scope.data = [];
        $scope.showResult = false;
        $scope.selectedSupplier;
        $scope.selectedZone;
        $scope.selectedCustomer;
        $scope.suppliers = [];
        $scope.customers = [];
        $scope.effectiveDate;
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        }
        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RateAnalysisAPIService.GetRateAnalysis(dataRetrievalInput).then(function (response) {
                console.log(response);
                onResponseReady(response);
                $scope.showResult = true;
            })
        };
        $scope.getData = function () {
            return retrieveData();
        };
        $scope.test = "tet";
        $scope.getChangeIcon = function (dataItem) {
            switch (dataItem.Change) {
                case ChangeEnum.Increase.value: return ChangeEnum.Increase.icon;
                case ChangeEnum.Decrease.value: return ChangeEnum.Decrease.icon;
                case ChangeEnum.New.value: return ChangeEnum.New.icon;
            }
        }
    }

    function retrieveData() {
        if (mainGridAPI == undefined)
            return;
        console.log($scope.selectedZone);
        var query = {
            ZoneId: $scope.selectedZone.ZoneId,
            EffectedDate:$scope.effectiveDate,
            CustomerId: $scope.selectedCustomer != undefined ? $scope.selectedCustomer.CarrierAccountID : null,
            SupplierId: $scope.selectedSupplier!=undefined?$scope.selectedSupplier.CarrierAccountID:null
        }

        return mainGridAPI.retrieveData(query);
    }
    function load() {
        loadSuppliers();
        loadCustomers();
    }

    function loadSuppliers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Supplier.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.suppliers.push(itm);
            });
          //  $scope.selectedSupplier = $scope.suppliers[0];
        });

    }
    function loadCustomers() {
        return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.Customer.value, false).then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.customers.push(itm);
            });
         //   $scope.selectedSupplier = $scope.suppliers[0];
        });

    }
};
appControllers.controller('BE_RateAnalysisController', RateAnalysisController);