'use strict';
app.directive('vrCpPricelistgroupedstatusSelector', ['CP_SupplierPricelist_PriceListGroupedStatusEnum', 'UtilsService', 'VRUIUtilsService',
    function (CP_SupplierPricelist_PriceListGroupedStatusEnum, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
               
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var selector = new pricelistGroupedStatusSelector(ctrl, $scope, $attrs);
                selector.initializeController();


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
                return getPriceListGroupedStatusTemplate(attrs);
            }

        };

        function getPriceListGroupedStatusTemplate(attrs) {

            var multipleselection = "";
            var label = "Pricelist Status";
            if (attrs.ismultipleselection != undefined) {
                label = "Pricelist Statuses ";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return   '<vr-select ' + multipleselection + '  datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" '
                   + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
        }
        

        function pricelistGroupedStatusSelector(ctrl, $scope, attrs) {

            var selectorAPI;
            function initializeController() {
                $scope.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                }
            }
                

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    var enumvalues;
                    if (ctrl.selectedvalues && ctrl.selectedvalues.length > 0) {
                        enumvalues = [];
                        for (var i = 0; i < ctrl.selectedvalues.length; i++) {
                            enumvalues = enumvalues.concat(ctrl.selectedvalues[i].valuesEnum);
                        }
                    }
                    return  enumvalues;              

                }

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    ctrl.datasource = UtilsService.getArrayEnum(CP_SupplierPricelist_PriceListGroupedStatusEnum);
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    }


                }
                
             
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }


        return directiveDefinitionObject;
    }]);