'use strict';
app.directive('vrWhsBeSalezoneSelector', ['WhS_BE_SaleZoneAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_BE_SaleZoneAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "@",
                isdisabled: "=",
                selectedvalues: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new saleZoneCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

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
                return getBeSaleZoneTemplate(attrs);
            }

        };


        function getBeSaleZoneTemplate(attrs) {

            var label = "Sale Zone";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
            {
                label = "Sale Zones";
                multipleselection = "ismultipleselection";
            }
                
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            return '<div>'
               + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SaleZoneId" '
            + required + ' label="' + label + '" datasource="ctrl.search" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '"></vr-select>'
            + '</div>'
        }

        function saleZoneCtor(ctrl, $scope, attrs) {

            var filter;
            var isDirectiveLoaded = false;

            function initializeController() {

                ctrl.search = function (nameFilter) {

                    if (filter == undefined || filter.SellingNumberPlanId == undefined)
                        return;

                    var serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_SaleZoneAPIService.GetSaleZonesInfo(nameFilter, serializedFilter);
                }
                
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    console.log(payload);

                    ctrl.selectedvalues = [];

                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds != undefined) {
                        ctrl.datasource = [];

                        var input = {
                            SellingNumberPlanId: filter.SellingNumberPlanId,
                            SaleZoneIds: selectedIds,
                            SaleZoneFilterSettings: { RoutingProductId: filter.RoutingProductId}
                        };

                        return WhS_BE_SaleZoneAPIService.GetSaleZonesInfoByIds(input).then(function (response) {
                            angular.forEach(response, function (item) {
                                ctrl.datasource.push(item);
                            });

                            VRUIUtilsService.setSelectedValues(selectedIds, 'SaleZoneId', attrs, ctrl);
                        });
                    }
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, ctrl);
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);