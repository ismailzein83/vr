SupplierPriceListController.$inject = ['$scope', 'WhS_SupPL_SupplierPriceListAPIService', 'WhS_BP_CreateProcessResultEnum', 'BusinessProcessService'];

function SupplierPriceListController($scope, WhS_SupPL_SupplierPriceListAPIService, WhS_BP_CreateProcessResultEnum, BusinessProcessService) {
    defineScope();
    loadParameters();
    load();
    function loadParameters() {
    }
    function defineScope() {
        $scope.selectedSupplier;
        $scope.effectiveDate = new Date();
        $scope.zoneList;
        $scope.upload = function () {
            return WhS_SupPL_SupplierPriceListAPIService.UploadSupplierPriceList($scope.selectedSupplier.CarrierAccountId, $scope.zoneList.fileId, $scope.effectiveDate == undefined ? {} : $scope.effectiveDate).then(function (response) {
                if (response.Result == WhS_BP_CreateProcessResultEnum.Succeeded.value)
                    return BusinessProcessService.openProcessTracking(response.ProcessInstanceId);
            });
        }
        $scope.onselectionchanged = function () {
        }
    }
    function load() {
     
    }
};
appControllers.controller('WhS_SupPL_SupplierPriceListController', SupplierPriceListController);