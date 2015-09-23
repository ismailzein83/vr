SelectiveSuppliersTemplateController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'UtilsService', 'VRNotificationService'];

function SelectiveSuppliersTemplateController($scope, WhS_BE_SaleZoneAPIService, UtilsService, VRNotificationService) {

    defineScope();
    load();

    function defineScope() {

        $scope.searchZones = function (filter) {
            return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo($scope.saleZoneGroups.saleZonePackageId, filter);
        }

        $scope.selectedSaleZones = [];

        $scope.saleZoneGroups.getData = function () {

            return {
                $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSaleZonesSettings, TOne.WhS.BusinessEntity.Entities",
                ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
            };
        };

        $scope.saleZoneGroups.loadTemplateData = function () {
            loadForm();
        }

        $scope.saleZoneGroups.resetSaleZoneSelection = function () {
            $scope.selectedSaleZones = [];
        }
    }

    var isFormLoaded;
    function loadForm() {

        if ($scope.saleZoneGroups.data == undefined || isFormLoaded)
            return;

        var data = $scope.saleZoneGroups.data;
        if (data != null) {

            if ($scope.saleZoneGroups.data.ZoneIds != undefined)
            {
                var input = { PackageId: $scope.saleZoneGroups.saleZonePackageId, SaleZoneIds: $scope.saleZoneGroups.data.ZoneIds };
                WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                    angular.forEach(response, function (item) {
                        $scope.selectedSaleZones.push(item);
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }

        isFormLoaded = true;
    }

    function load() {
        loadForm();
    }
}
appControllers.controller('WhS_BE_SelectiveSuppliersTemplateController', SelectiveSuppliersTemplateController);
