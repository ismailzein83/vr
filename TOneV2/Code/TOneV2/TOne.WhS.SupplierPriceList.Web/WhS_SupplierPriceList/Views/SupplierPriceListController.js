SupplierPriceListController.$inject = ['$scope'];

function SupplierPriceListController($scope) {
    defineScope();
    //loadParameters();
    //load();
    function loadParameters() {
    }
    function defineScope() {

        $scope.saleZonePackages = [];
        $scope.selectedSaleZonePackage;
        $scope.effectiveDate = new Date();
        $scope.zoneList;
        console.log("Test");
        $scope.upload = function () {
            return WhS_CodePrep_CodePrepAPIService.UploadSaleZonesList($scope.selectedSaleZonePackage.SaleZonePackageId, $scope.zoneList.fileId, $scope.effectiveDate).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                    return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
            });
        }
    }
    function load() {
        loadSaleZonePackages();
    }
    function loadSaleZonePackages() {
        $scope.isInitializing = true;
        return WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
            for (var i = 0; i < response.length; i++)
                $scope.saleZonePackages.push(response[i]);
        }).finally(function () {
            $scope.isInitializing = false;
        });
    }


};



appControllers.controller('WhS_SupPL_SupplierPriceListController', SupplierPriceListController);