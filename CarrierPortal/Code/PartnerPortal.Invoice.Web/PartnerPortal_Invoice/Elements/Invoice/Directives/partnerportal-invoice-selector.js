'use strict';
app.directive('partnerportalInvoiceSelector', [ 'PartnerPortal_Invoice_InvoiceAPIService', 'UtilsService', 'VRUIUtilsService', '$filter',
    function (PartnerPortal_Invoice_InvoiceAPIService, UtilsService, VRUIUtilsService, $filter) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                hideremoveicon: '@',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new getInvoiceAccount(ctrl, $scope, $attrs);
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
                return getInvoiceAccountTemplate(attrs);
            }

        };


        function getInvoiceAccountTemplate(attrs) {

            var multipleselection = "";
            var label = "Account";
            if (attrs.ismultipleselection != undefined) {
                label = "Accounts";
                multipleselection = "ismultipleselection";
            }

            return  '<vr-columns colnum="{{ctrl.normalColNum}}" ng-show="ctrl.datasource.length > 1"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="PortalInvoiceAccountId" '+
                    'label="' + label + '"  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Account" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired"></vr-select></vr-columns>';
        }

        function getInvoiceAccount(ctrl, $scope, attrs) {
            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };


            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedIds;
                    var invoiceViewerTypeId;


                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        invoiceViewerTypeId = payload.invoiceViewerTypeId;
                    }

                    var getInvoiceAccountsPromise = getInvoiceAccounts();
                    promises.push(getInvoiceAccountsPromise);
                  
                 

                    function getInvoiceAccounts() {
                        return PartnerPortal_Invoice_InvoiceAPIService.GetInvoiceAccounts(invoiceViewerTypeId).then(function (response) {
                            selectorAPI.clearDataSource();

                            if (response != undefined) {
                                for (var i = 0; i < response.length; i++)
                                    ctrl.datasource.push(response[i]);

                                if (selectedIds != undefined)
                                    VRUIUtilsService.setSelectedValues(selectedIds, 'PortalInvoiceAccountId', attrs, ctrl);
                                else if (ctrl.datasource.length == 1) {
                                    var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].PortalInvoiceAccountId] : ctrl.datasource[0].PortalInvoiceAccountId;
                                    VRUIUtilsService.setSelectedValues(defaultValue, 'PortalInvoiceAccountId', attrs, ctrl);
                                }
                            }
                        });
                    }
                 
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('PortalInvoiceAccountId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);