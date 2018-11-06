'use strict';
app.directive('cpWhsSupplierzonesSelector', ['UtilsService', 'VRUIUtilsService', "CP_WhS_SupplierZonesAPIService",
    function (UtilsService, VRUIUtilsService, CP_WhS_SupplierZonesAPIService) {

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
                getsuppliers: '@',
                getcustomers: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var selector = new SupplierZonesSelector(ctrl, $scope, $attrs);
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
                return getSupplierZonesSelectorTemplate(attrs);
            }

        };

        function getSupplierZonesSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "Supplier Zone";
            if (attrs.ismultipleselection != undefined) {
                label = "Supplier Zones";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="SupplierZoneId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</vr-columns>';
        }

        function SupplierZonesSelector(ctrl, $scope, attrs) {
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
                    return VRUIUtilsService.getIdSelectedIds('SupplierZoneId', attrs, ctrl);
                };

                api.load = function (payload) {
                    var selectedIds;
                    var businessEntityDefinitionId;
                    var supplierId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        supplierId = payload.supplierId;
                    }
                    function getFilter() {
                        return {
                            BusinessEntityDefinitionId: businessEntityDefinitionId,
                            SupplierId: supplierId
                        };
                    }
                    return CP_WhS_SupplierZonesAPIService.GetRemoteSupplierZonesInfo(UtilsService.serializetoJson(getFilter())).then(function (response) {
                        ctrl.datasource = response;
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierZoneId', attrs, ctrl);
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
