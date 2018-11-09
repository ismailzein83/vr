'use strict';
app.directive('cpWhsCarrieraccountsSelector', ['UtilsService', 'VRUIUtilsService', "CP_WhS_CarrierAccountsAPIService","CP_WhS_CarrierAccountActivationStatusEnum",
    function (UtilsService, VRUIUtilsService, CP_WhS_CarrierAccountsAPIService, CP_WhS_CarrierAccountActivationStatusEnum) {
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
                ctrl.label = "Carrier Account";
                if ($attrs.ismultipleselection != undefined) {
                    ctrl.selectedvalues = [];
                    ctrl.label = "Carrier Accounts";
                }
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
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
            return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                + '<vr-select ' + multipleselection+' datatextfield="Name" datavaluefield="AccountId" isrequired="ctrl.isrequired" '
                + ' label="{{ctrl.label}}"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" on-ready="onSelectorReady" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityname="Account" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
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
                    return VRUIUtilsService.getIdSelectedIds('AccountId', attrs, ctrl);
                };

                api.load = function (payload) {
                   
                    if (attrs.customlabel != undefined)
                        ctrl.label = attrs.customlabel;
                    if (payload.fieldTitle != undefined) {
                        ctrl.label = payload.fieldTitle;
                        if (attrs.ismultipleselection != undefined) 
                            ctrl.label += "s";
                    }
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
                        ctrl.datasource.length = 0;

                        angular.forEach(response, function (itm) {
                            if (itm.ActivationStatus != CP_WhS_CarrierAccountActivationStatusEnum.Inactive.value) {
                                ctrl.datasource.push(itm);
                            }
                        });
                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'AccountId', attrs, ctrl);
                        }
                    });
                };

                api.selectIfSingleItem = function () {
                    selectorAPI.selectIfSingleItem();
                };

                api.isSingleItem = function () {
                    if (ctrl.datasource.length == 1)
                        return true;
                    return false;
                };

                api.getSingleItem = function () {
                    return ctrl.datasource[0];
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);
