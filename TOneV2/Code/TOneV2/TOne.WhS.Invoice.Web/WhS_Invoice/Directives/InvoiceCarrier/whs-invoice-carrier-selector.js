'use strict';
app.directive('whsInvoiceCarrierSelector', ['WhS_Invoice_InvoiceAPIService', 'UtilsService', 'VRUIUtilsService','$filter',
    function (WhS_Invoice_InvoiceAPIService, UtilsService, VRUIUtilsService, $filter) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                getcustomers: "@",
                getsuppliers: "@",
                ismultipleselection: "@",
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new carriersCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'carrierCtrl',
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

                label = (attrs.getcustomers != undefined) ? "Customers" : "Suppliers";
                label = (attrs.getcustomers != undefined && attrs.getsuppliers != undefined) ? "Carriers" : label;
            }
            else {
                label = (attrs.getcustomers != undefined) ? "Customer" : "Supplier";
                label = (attrs.getcustomers != undefined && attrs.getsuppliers != undefined) ? "Carrier" : label;
            }

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            //To be added on multiple selection to add grouping functionality, the style section is to be added to the outer div
            //var groupStyle = 'style="display:inline-block;width: calc(100% - 18px);"';
            //var groupHtml = ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';

            var ismultipleselection = "";
            if (attrs.ismultipleselection != undefined)
                ismultipleselection = "ismultipleselection";

            return '<vr-columns colnum="{{carrierCtrl.normalColNum}}"> <vr-select  datasource="carrierCtrl.carrierTypes" selectedvalues="carrierCtrl.selectedCarrierTypes" onselectionchanged="carrierCtrl.carrierTypeSelectionChanged"   datatextfield="description" datavaluefield="value" label="Invoice Type" ' +ismultipleselection + ' '
              + hideremoveicon + ' isrequired="carrierCtrl.isrequired"></vr-select></vr-columns> '
               + ' <vr-columns vr-loader="carrierCtrl.isloadingCarriers" colnum="{{carrierCtrl.normalColNum}}"> <vr-select isrequired="carrierCtrl.isrequired" on-ready="carrierCtrl.onSelectorReady" datasource="carrierCtrl.datasource" selectedvalues="carrierCtrl.selectedvalues" onselectionchanged="carrierCtrl.onselectionchanged"  datatextfield="Name" datavaluefield="InvoiceCarrierId" label="'
                + label + '" ' + hideselectedvaluessection + '  ' + hideremoveicon + ' ' + ismultipleselection + '></vr-select></vr-columns>';
        }

        function carriersCtor(ctrl, $scope, attrs) {

            var selectorApi;
            var context;
            function initializeController() {

                ctrl.selectedvalues = [];
                ctrl.datasource = [];
                ctrl.selectedCarrierTypes = [];
                ctrl.carrierTypes = [{
                    value: 0,
                    description: "Profile"
                }, {
                    value: 1,
                    description: "Account"
                }];
              
                ctrl.carrierTypeSelectionChanged = function () {
                    if (selectorApi != undefined) {
                        selectorApi.clearDataSource();
                        ctrl.datasource.length = 0;

                    }
                    if (context != undefined)
                    {
                        context.reloadPregeneratorActions();
                    }
                    loadCarrierAccounts(attrs);
                };

                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        context = payload.context;
                    }
                };
                api.getData = function () {
                    var partnerPrefix;
                    if (ctrl.selectedCarrierTypes != undefined) {
                        if (ctrl.selectedCarrierTypes.length == 1)
                            partnerPrefix = ctrl.selectedCarrierTypes[0].description;
                        else
                            partnerPrefix = ctrl.selectedCarrierTypes.description;

                    }
                    return {
                        partnerPrefix: partnerPrefix,
                        selectedIds: VRUIUtilsService.getIdSelectedIds('InvoiceCarrierId', attrs, ctrl)
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadCarrierAccounts(attrs) {
                ctrl.isloadingCarriers = true;
                var filter = {};
                if (ctrl.selectedCarrierTypes != undefined) {
                    filter.CarrierTypes = [];
                    if (ctrl.selectedCarrierTypes.length > 0) {
                        for (var i = 0; i < ctrl.selectedCarrierTypes.length; i++) {
                            var selectedCarrierType = ctrl.selectedCarrierTypes[i];
                            filter.CarrierTypes.push(selectedCarrierType.value)
                        }
                    } else {
                        filter.CarrierTypes.push(ctrl.selectedCarrierTypes.value)
                    }
                };

                filter.GetCustomers = attrs.getcustomers != undefined;
                filter.GetSuppliers = attrs.getsuppliers != undefined;
                var serializedFilter = {};
                if (filter != undefined) {
                    serializedFilter = UtilsService.serializetoJson(filter);
                };
                return WhS_Invoice_InvoiceAPIService.GetInvoiceCarriers(serializedFilter).then(function (response) {
                    var data = $filter('orderBy')(response, 'Name');
                    angular.forEach(data, function (itm) {

                        ctrl.datasource.push(itm);
                    });
                    ctrl.isloadingCarriers = false;
                });
            };
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);