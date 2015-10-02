'use strict';
app.directive('vrWhsBeSelectivesalezones', ['WhS_BE_SaleZoneAPIService', 'UtilsService', function (WhS_BE_SaleZoneAPIService, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            salezonepackageid: "=",
            salezonegroupsettings: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.selectedSaleZones = [];

            $scope.showSaleZonePackage = ctrl.salezonepackageid === undefined;

            var beSaleZonesCtr = new beSaleZones(ctrl, $scope.selectedSaleZones, WhS_BE_SaleZoneAPIService);
            beSaleZonesCtr.initializeController();

            $scope.onselectionvalueschanged = function () {
                ctrl.salezonegroupsettings = {
                    $type: "TOne.WhS.BusinessEntity.Entities.SelectiveSaleZonesSettings, TOne.WhS.BusinessEntity.Entities",
                    SaleZonePackageId: ctrl.salezonepackageid,
                    ZoneIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZones, "SaleZoneId")
                };
            }

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
        template: function (element, attrs) {
            return getBeSelectiveSaleZonesTemplate(attrs);
        }

    };

    function getBeSelectiveSaleZonesTemplate(attrs) {
        return '<div style="display:inline-block;width: calc(100% - 18px);">'
            + '<span ng-if="showSaleZonePackage">I am here</span>'
                   + ' <vr-select ismultipleselection isrequired label="Sale Zone" datasource="searchZones" selectedvalues="selectedSaleZones" onselectionchanged="onselectionvalueschanged" datatextfield="Name" datavaluefield="SaleZoneId"'
                   + 'entityname="Sale Zone"></vr-select></div>';
    }

    function beSaleZones(ctrl, selectedSaleZones, WhS_BE_SaleZoneAPIService) {
        
        function initializeController() {
            if (ctrl.salezonegroupsettings !== undefined && ctrl.salezonegroupsettings.ZoneIds !== undefined &&
                ctrl.salezonegroupsettings.ZoneIds !== null && ctrl.salezonegroupsettings.ZoneIds.length > 0)
            {
                var input = { PackageId: ctrl.salezonepackageid, SaleZoneIds: ctrl.salezonegroupsettings.ZoneIds };

                WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                    angular.forEach(response, function (item) {
                        selectedSaleZones.push(item);
                    });
                });
            }
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);