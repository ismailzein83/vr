'use strict';
app.directive('vrCpCarrieraccountSelector', ['CP_SupplierPricelist_SupplierMappingAPIService', 'UtilsService', 'VRUIUtilsService',
    function (supplierMappingAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: '=',
                selectedvalues: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];


                var selector = new supplierInfoSelector(ctrl, $scope, $attrs);
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
                return getSupplierInfoTemplate(attrs);
            }

        };

        function getSupplierInfoTemplate(attrs) {

            var multipleselection = "";
            var label = "Carrier Account";
            if (attrs.ismultipleselection != undefined) {
                label = "Carrier Accounts";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="SupplierName" datavaluefield="SupplierId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady"  onselectionchanged="ctrl.onselectionchanged" entityName="'+ label +'" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</div>';
        }
        function supplierInfoSelector(ctrl, $scope, attrs) {

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
                    return VRUIUtilsService.getIdSelectedIds('SupplierId', attrs, ctrl);
                };
                api.load = function (payload) {
                    var selectedIds;
                    var filter;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }
                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);
                    return supplierMappingAPIService.GetCustomerSuppliers(serializedFilter).then(function (response) {
                        ctrl.datasource.length = 0;
                        if (attrs.ismultipleselection != undefined) {
                            ctrl.selectedvalues.length = 0;
                        }
                        angular.forEach(response, function (item) {
                            ctrl.datasource.push(item);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'SupplierId', attrs, ctrl);
                        }

                        else if (selectedIds == undefined && ctrl.datasource.length == 1) {
                            var customSelection;
                            if (attrs.ismultipleselection != undefined)
                                customSelection = ctrl.datasource[0].SupplierId;
                            else
                                customSelection = [ctrl.datasource[0].SupplierId];
                            VRUIUtilsService.setSelectedValues(customSelection, 'SupplierId', attrs, ctrl);

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