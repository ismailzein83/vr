'use strict';
app.directive('cpWhsCarrieraccountsSelector', ['UtilsService', 'VRUIUtilsService',"CP_WhS_CarrierAccountsAPIService",
    function (UtilsService, VRUIUtilsService, CP_WhS_CarrierAccountsAPIService) {

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
                getcustomers:'@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
                var selector = new CarrierAccountSelector(ctrl, $scope, $attrs);
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
                return getCarrierAccountSelectorTemplate(attrs);
            }

        };

        function getCarrierAccountSelectorTemplate(attrs) {

            var multipleselection = "";
            var label = "Carrier Accounts";
            if (attrs.ismultipleselection != undefined) {
                label = "Carrier Accounts";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="CarrierAccountId" isrequired="ctrl.isrequired" '
                + ' label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
                + '</vr-columns>';
        }

        function CarrierAccountSelector(ctrl, $scope, attrs) {
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
                    return VRUIUtilsService.getIdSelectedIds('CarrierAccountId', attrs, ctrl);
                };

                api.load = function (payload) {
                    var selectedIds;
                    var businessEntityDefinitionId;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                    }
                    function getFilter() {
                        var filter = {};
                        filter.BusinessEntityDefinitionId = businessEntityDefinitionId;
                        if (attrs.getsuppliers != undefined)
                            filter.GetSuppliers = true;
                        if (attrs.getcustomers != undefined)
                            filter.GetCustomers = true;
                        return filter;
                    }
                    return CP_WhS_CarrierAccountsAPIService.GetRemoteCarrierAccountsInfo(UtilsService.serializetoJson(getFilter())).then(function (response) {
                        ctrl.datasource = response;
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'CarrierAccountId', attrs, ctrl);
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
