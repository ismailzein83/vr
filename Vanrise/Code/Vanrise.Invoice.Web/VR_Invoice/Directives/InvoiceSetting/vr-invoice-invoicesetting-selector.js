'use strict';
app.directive('vrInvoiceInvoicesettingSelector', ['VR_Invoice_InvoiceSettingAPIService', 'VR_Invoice_InvoiceSettingService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Invoice_InvoiceSettingAPIService, VR_Invoice_InvoiceSettingService, UtilsService, VRUIUtilsService) {

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

                var ctor = new currencyCtor(ctrl, $scope, $attrs);
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
                return getInvoiceSettingTemplate(attrs);
            }

        };


        function getInvoiceSettingTemplate(attrs) {

            var multipleselection = "";
            var label = "Invoice Setting";
            if (attrs.ismultipleselection != undefined) {
                label = "Invoice Settings";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
            {
                label = attrs.customlabel;
            }
            var addCliked = '';
            if (attrs.showaddbutton != undefined)
                addCliked = 'onaddclicked="addNewInvoiceSetting"';

            var viewCliked = "";
            if (attrs.showviewbutton != undefined)
                viewCliked = 'onviewclicked="ctrl.ViewInvoiceSetting"';

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="InvoiceSettingId" label="' + label + '" ' + addCliked + viewCliked + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="InvoiceSetting" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }

        function currencyCtor(ctrl, $scope, attrs) {
            var selectorAPI;
            var invoiceTypeId;

            var setInvoiceTypeIdPromise = UtilsService.createPromiseDeferred();
            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
                $scope.addNewInvoiceSetting = function () {
                    var onInvoiceSettingAdded = function (currencyObj) {
                        ctrl.datasource.push(currencyObj.Entity);
                        if (attrs.ismultipleselection != undefined)
                            ctrl.selectedvalues.push(currencyObj.Entity);
                        else
                            ctrl.selectedvalues = currencyObj.Entity;
                    };
                    VR_Invoice_InvoiceSettingService.addInvoiceSetting(onInvoiceSettingAdded, invoiceTypeId);
                };

                ctrl.haspermission = function () {
                    var promise = UtilsService.createPromiseDeferred();
                    setInvoiceTypeIdPromise.promise.then(function () {
                        return VR_Invoice_InvoiceSettingAPIService.HasAddInvoiceSettingPermission(invoiceTypeId).then(function(response){
                            promise.resolve(response);
                        }).catch(function (error) {
                            promise.reject(error);
                        });
                    });
                    return promise.promise;
                };

                ctrl.ViewInvoiceSetting = function (obj) {
                    VR_Invoice_InvoiceSettingService.viewInvoiceSetting(obj.InvoiceSettingId, invoiceTypeId);
                };


            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var filter;

                    selectorAPI.clearDataSource();
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        invoiceTypeId = payload.invoiceTypeId;
                        if (invoiceTypeId != undefined)
                            setInvoiceTypeIdPromise.resolve();
                        filter = payload.filter;

                    }

                    return VR_Invoice_InvoiceSettingAPIService.GetInvoiceSettingsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);
                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'InvoiceSettingId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('InvoiceSettingId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);