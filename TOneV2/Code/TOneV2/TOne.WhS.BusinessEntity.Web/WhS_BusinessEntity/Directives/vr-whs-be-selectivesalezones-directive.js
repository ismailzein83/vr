'use strict';
app.directive('vrWhsBeSelectivesalezones', ['WhS_BE_SaleZoneAPIService', 'WhS_BE_SaleZonePackageAPIService', 'UtilsService',
    function (WhS_BE_SaleZoneAPIService, WhS_BE_SaleZonePackageAPIService, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onloaded: '=',
            salezonepackageid: "="
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            $scope.selectedSaleZones = [];

            $scope.saleZonePackages = [];
            $scope.selectedSaleZonePackage = undefined;

            $scope.showSaleZonePackage = ctrl.salezonepackageid == undefined;

            var beSaleZonesCtor = new beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService);
            beSaleZonesCtor.initializeController();

            $scope.searchZones = function (filter) {
                var packageId;
                if (ctrl.salezonepackageid == undefined)
                    packageId = $scope.selectedSaleZonePackage.SaleZonePackageId;
                else
                    packageId = ctrl.salezonepackageid;

                return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(packageId, filter);
            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                   
                }
            }
        },
        templateUrl: function (element, attrs) {
            return getBeSelectiveSaleZonesTemplate(attrs);
        }

    };

    function getBeSelectiveSaleZonesTemplate(attrs) {
        return '/Client/Modules/WhS_BusinessEntity/Directives/Templates/SelectiveSaleZonesDirectiveTemplate.html';
    }

    function beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
        
        function initializeController() {
            
            loadSaleZonePackages();
        }

        function defineAPI()
        {
            var api = {};

            api.getData = function()
            {
                return {
                    $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSaleZonesSettings, TOne.WhS.BusinessEntity.Entities",
                    SaleZonePackageId: ctrl.salezonepackageid == undefined ? $scope.selectedSaleZonePackage.SaleZonePackageId : ctrl.salezonepackageid,
                    ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
                };
            }

            api.setData = function (saleZoneGroupSettings)
            {
                $scope.isLoadingDirective = true;

                var packageId;

                if (ctrl.salezonepackageid == undefined)
                {
                    $scope.selectedSaleZonePackage = UtilsService.getItemByVal($scope.saleZonePackages, saleZoneGroupSettings.SaleZonePackageId, "SaleZonePackageId");
                    packageId = saleZoneGroupSettings.SaleZonePackageId;
                }
                else
                {
                    packageId = ctrl.salezonepackageid;
                }

                if (saleZoneGroupSettings.ZoneIds.length > 0) {
                    var input = { PackageId: packageId, SaleZoneIds: saleZoneGroupSettings.ZoneIds };

                    WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.selectedSaleZones.push(item);
                        });
                    }).catch(function (error) {
                        //TODO handle the case of exceptions
                        
                    }).finally(function () {
                        $scope.isLoadingDirective = false;
                    });
                }
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }

        function loadSaleZonePackages() {
            $scope.isLoadingDirective = true;
            return WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.saleZonePackages.push(item);
                });
            }).catch(function (error) {
                //TODO handle the case of exceptions

            }).finally(function () {
                $scope.isLoadingDirective = false;
                //Prepare and initiate the API stating the directive as ready to be used
                defineAPI();
            });
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);