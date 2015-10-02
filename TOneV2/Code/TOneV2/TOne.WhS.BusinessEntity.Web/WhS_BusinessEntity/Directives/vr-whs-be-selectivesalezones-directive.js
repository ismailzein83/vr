'use strict';
app.directive('vrWhsBeSelectivesalezones', ['WhS_BE_SaleZoneAPIService', 'UtilsService', function (WhS_BE_SaleZoneAPIService, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onloaded: '=',
            salezonepackageid: "="
        },
        controller: function ($scope, $element, $attrs) {
            $scope.isGettingData = false;

            var ctrl = this;
            $scope.selectedSaleZones = [];

            $scope.showSaleZonePackage = ctrl.salezonepackageid === undefined;

            var beSaleZonesCtr = new beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService);
            beSaleZonesCtr.initializeController();

            $scope.searchZones = function (filter) {
                return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(ctrl.salezonepackageid, filter);
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
        return '/Client/Modules/WhS_BusinessEntity/Directives/SelectiveSaleZoneDirectiveTemplate.html';
    }

    function beSaleZones(ctrl, $scope, WhS_BE_SaleZoneAPIService) {
        
        function initializeController() {
            //Load default data if any

            //Prepare and initiate the API stating the directive as ready to be used
            defineAPI();
        }

        function defineAPI()
        {
            var api = {};

            api.getData = function()
            {
                return {
                    $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSaleZonesSettings, TOne.WhS.BusinessEntity.Entities",
                    SaleZonePackageId: ctrl.salezonepackageid,
                    ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
                };
            }

            api.setData = function (salezonegroupsettings)
            {
                $scope.isGettingData = true;
                if (salezonegroupsettings !== undefined && salezonegroupsettings.ZoneIds !== undefined &&
                    salezonegroupsettings.ZoneIds !== null && salezonegroupsettings.ZoneIds.length > 0) {
                    var input = { PackageId: ctrl.salezonepackageid, SaleZoneIds: salezonegroupsettings.ZoneIds };

                    WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                        angular.forEach(response, function (item) {
                            $scope.selectedSaleZones.push(item);
                        });
                    }).catch(function (error) {
                        //TODO handle the case of exceptions
                        
                    }).finally(function () {
                        $scope.isGettingData = false;
                    });
                }
            }

            if (ctrl.onloaded != null)
                ctrl.onloaded(api);
        }

        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);