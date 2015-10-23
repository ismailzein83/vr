(function (appControllers) {

    "use strict";

    selectiveSaleZonesTemplateController.$inject = ['$scope', 'WhS_BE_SaleZoneAPIService', 'UtilsService', 'VRNotificationService'];

    function selectiveSaleZonesTemplateController($scope, WhS_BE_SaleZoneAPIService, UtilsService, VRNotificationService) {
        defineScope();
        load();

        function defineScope() {

            $scope.searchZones = function (filter) {
                return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo($scope.saleZoneGroups.sellingNumberPlanId, filter);
            }

            $scope.selectedSaleZones = [];

            $scope.saleZoneGroups.getData = function () {

                return {
                    $type: "TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups.SelectiveSaleZoneGroup, TOne.WhS.BusinessEntity.MainExtensions",
                    SellingNumberPlanId: $scope.saleZoneGroups.sellingNumberPlanId,
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

                if ($scope.saleZoneGroups.data.ZoneIds != undefined) {
                    var input = { SellingNumberPlanId: $scope.saleZoneGroups.sellingNumberPlanId, SaleZoneIds: $scope.saleZoneGroups.data.ZoneIds };
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

    appControllers.controller('WhS_BE_SelectiveSaleZonesTemplateController', selectiveSaleZonesTemplateController);
})(appControllers);
