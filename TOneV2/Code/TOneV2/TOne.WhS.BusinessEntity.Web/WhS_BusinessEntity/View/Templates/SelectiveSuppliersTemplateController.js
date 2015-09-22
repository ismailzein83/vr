SelectiveSuppliersTemplateController.$inject = ['$scope', 'SaleZoneAPIService', 'UtilsService', 'VRNotificationService'];

function SelectiveSuppliersTemplateController($scope, SaleZoneAPIService, UtilsService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {

        $scope.searchZones = function (filter) {
            return SaleZoneAPIService.GetSalesZonesInfo($scope.saleZoneGroups.saleZonePackageId, filter);
        }

        $scope.selectedSaleZones = [];

        $scope.saleZoneGroups.getData = function () {

            return {
                $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSaleZonesSettings, TOne.WhS.BusinessEntity.Entities",
                ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
            };
        };

        $scope.dataSourceAdapter.loadTemplateData = function () {
            loadForm();
        }

        $scope.dataSourceAdapter.resetSaleZoneSelection = function () {
            $scope.selectedSaleZones = [];
        }
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.saleZoneGroups.data == undefined || isFormLoaded)
            return;

        var data = $scope.saleZoneGroups.data;
        if (data != null) {
            SaleZoneAPIService.GetSaleZonesInfoByIds($scope.saleZoneGroups.saleZonePackageId, $scope.saleZoneGroups.data.ZoneIds).then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.selectedSaleZones.push(item);
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        isFormLoaded = true;
    }

    function load() {
        loadForm();
    }
}
appControllers.controller('WhS_BE_SelectiveSuppliersTemplateController', SelectiveSuppliersTemplateController);
