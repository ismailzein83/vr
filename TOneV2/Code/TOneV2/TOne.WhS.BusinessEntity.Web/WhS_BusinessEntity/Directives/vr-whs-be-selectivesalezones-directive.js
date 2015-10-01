'use strict';
app.directive('vrWhsBeSelectivesalezones', ['WhS_BE_SaleZoneAPIService', function (WhS_BE_SaleZoneAPIService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            salezonepackageid: "=",
            label: "@",
            selectedvalues: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.selectedSaleZones = selectedvalues;

            var beSaleZones = new beSaleZones(ctrl, WhS_BE_SaleZoneAPIService);
            beSaleZones.initializeController();

            $scope.onselectionvalueschanged = function () {
                ctrl.selectedvalues = $scope.selectedSaleZones;
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
            return getBeCarrierGroupTemplate(attrs);
        }

    };

    function getBeSelectiveSaleZonesTemplate(attrs) {

        if (attrs.salezonepackageid != undefined)
            return '<div style="display:inline-block;width: calc(100% - 18px);">'
                       + '<vr-label >' + label + '</vr-label>'
                   + ' <vr-select ismultipleselection datasource="searchZones" selectedvalues="selectedSaleZones" onselectionchanged="onselectionvalueschanged" datatextfield="Name" datavaluefield="SaleZoneId"'
                   + 'entityname="Sale Zone"></vr-select></div>';
        else
            return '<span>Nothing Selected</span>'
    }

    function beSaleZones(ctrl, WhS_BE_SaleZoneAPIService) {
        
        function initializeController() {

        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);