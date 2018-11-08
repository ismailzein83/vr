'use strict';
app.directive('cpWhsSalezonesSelector', ['UtilsService', 'VRUIUtilsService', "CP_WhS_SaleZonesAPIService",
    function (UtilsService, VRUIUtilsService, CP_WhS_SaleZonesAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var selector = new SaleZonesSelector(ctrl, $scope, $attrs);
                selector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getSaleZonesSelectorTemplate(attrs);
            }

        };

        function getSaleZonesSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "Sale Zone";
            if (attrs.ismultipleselection != undefined) {
                label = "Sale Zones";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SaleZoneId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</vr-columns>';
        }

        function SaleZonesSelector(ctrl, $scope, attrs) {
            var selectorAPI;

            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {

                var api = {};

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('SaleZoneId', attrs, ctrl);
                };

                api.load = function (payload) {
                    var selectedIds;
                    var businessEntityDefinitionId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    }
                    function getFilter() {
                        return {
                            BusinessEntityDefinitionId: businessEntityDefinitionId
                        };
                    }
                    CP_WhS_SaleZonesAPIService.GetRemoteSaleZonesInfo(UtilsService.serializetoJson(getFilter())).then(function (response) {
                        ctrl.datasource = response;

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SaleZoneId', attrs, ctrl);
                        }
                    });
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);
