'use strict';

app.directive('vrWhsBeCarrieraccountSelector', ['WhS_BE_CarrierAccountAPIService', 'UtilsService', 'VRUIUtilsService', '$compile', 'WhS_BE_CarrierAccountService',
    function (WhS_BE_CarrierAccountAPIService, UtilsService, VRUIUtilsService, $compile, WhS_BE_CarrierAccountService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                ondeselectallitems: '=',
                onblurdropdown: '=',
                selectedvalues: "=",
                getcustomers: "@",
                getsuppliers: "@",
                getexchangecarriers: '@',
                isrequired: '=',
                ismultipleselection: "@",
                hideselectedvaluessection: '@',
                hideremoveicon: "@",
                hidelabel: '@',
                normalColNum: '@',
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new carriersCtor(ctrl, $scope, WhS_BE_CarrierAccountAPIService, $attrs);
                ctor.initializeController();
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
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var label;

            if (attrs.ismultipleselection != undefined) {
                if ((attrs.getcustomers != undefined && attrs.getsuppliers != undefined) || attrs.getexchangecarriers != undefined) {
                    label = 'Carriers';
                }
                else if (attrs.getcustomers != undefined) {
                    label = 'Customers';
                }
                else {
                    label = 'Suppliers';
                }
            }
            else {
                if ((attrs.getcustomers != undefined && attrs.getsuppliers != undefined) || attrs.getexchangecarriers != undefined) {
                    label = 'Carrier';
                }
                else if (attrs.getcustomers != undefined) {
                    label = 'Customer';
                }
                else {
                    label = 'Supplier';
                }
            }

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            var customlabel = '';
            if (attrs.customlabel != undefined)
                customlabel = 'customlabel="{{ctrl.customlabel}}"';

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var haschildcolumns = "";
            if (attrs.usefullcolumn != undefined)
                haschildcolumns = "haschildcolumns";

            var viewCliked = '';
            if (attrs.showviewbutton != undefined)
                viewCliked = 'onviewclicked="ctrl.ViewCarrierAccount"';

            var ismultipleselection = "";
            if (attrs.ismultipleselection != undefined)
                ismultipleselection = "ismultipleselection";

            //To be added on multiple selection to add grouping functionality, the style section is to be added to the outer div
            //var groupStyle = 'style="display:inline-block;width: calc(100% - 18px);"';
            //var groupHtml = ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';

            return '<vr-columns colnum="{{ctrl.normalColNum}}" ' + haschildcolumns + '> ' +
                        '<vr-select hasviewpermission="ctrl.hasviewpermission"  isrequired="ctrl.isrequired" on-ready="ctrl.onSelectorReady" datasource="ctrl.datasource" ' +
                            ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"  ondeselectallitems="ctrl.ondeselectallitems" onblurdropdown="ctrl.onblurdropdown" ' +
                            ' onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" datalockfield="Locked" datatextfield="Name" datavaluefield="CarrierAccountId" ' +
                            ' datadisabledfield="IsDisabled" ' + ' label="' + label + '" ' + customlabel + ' ' + hidelabel + ' ' + hideselectedvaluessection + ' ' + hideremoveicon +
                            ' ' + ismultipleselection + ' ' + viewCliked + '>' +
                        '</vr-select>' +
                   '</vr-columns>';
        }

        function carriersCtor(ctrl, $scope, WhS_BE_CarrierAccountAPIService, attrs) {
            this.initializeController = initializeController;

            var selectorApi;

            function initializeController() {
                ctrl.ViewCarrierAccount = function (obj) {
                    WhS_BE_CarrierAccountService.viewCarrierAccount(obj.CarrierAccountId);
                };

                ctrl.hasviewpermission = function () {
                    return WhS_BE_CarrierAccountAPIService.HasViewCarrierAccountPermission();
                };

                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorApi.clearDataSource();

                    var filter;
                    var selectedIds;
                    var lockedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                        lockedIds = payload.lockedIds;
                    }

                    if (filter == undefined)
                        filter = {};
                    filter.GetCustomers = attrs.getcustomers != undefined;
                    filter.GetSuppliers = attrs.getsuppliers != undefined;
                    filter.GetExchangeCarriers = attrs.getexchangecarriers != undefined;

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);

                    return WhS_BE_CarrierAccountAPIService.GetCarrierAccountInfo(serializedFilter).then(function (response) {
                        ctrl.datasource.length = 0;
                        angular.forEach(response, function (itm) {
                            if (lockedIds != undefined && lockedIds.indexOf(itm.CarrierAccountId) > -1) {
                                itm.Locked = true;
                            }
                            ctrl.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'CarrierAccountId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CarrierAccountId', attrs, ctrl);
                };

                api.getSelectedValues = function () {
                    return ctrl.selectedvalues;
                };

                api.hasSingleItem = function () {
                    return ctrl.datasource.length == 1;
                };

                api.selectItem = function (carrierAccountId) {
                    var carrierAccoutnItem = UtilsService.getItemByVal(ctrl.datasource, carrierAccountId, "CarrierAccountId");
                    ctrl.selectedvalues.push(carrierAccoutnItem);
                };

                api.excludeItem = function (carrierAccountId) {
                    var carrierAccoutnItem = UtilsService.getItemByVal(ctrl.datasource, carrierAccountId, "CarrierAccountId");
                    carrierAccoutnItem.IsDisabled = true;
                };

                api.cancelExcludeItem = function (carrierAccountId) {
                    var carrierAccoutnItem = UtilsService.getItemByVal(ctrl.datasource, carrierAccountId, "CarrierAccountId");
                    carrierAccoutnItem.IsDisabled = false;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);