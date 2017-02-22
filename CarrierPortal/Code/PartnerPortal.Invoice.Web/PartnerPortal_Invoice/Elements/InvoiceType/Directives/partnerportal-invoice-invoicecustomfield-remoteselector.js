'use strict';

app.directive('partnerportalInvoiceInvoicecustomfieldRemoteselector', ['UtilsService', 'VRUIUtilsService', 'PartnerPortal_Invoice_InvoiceTypeAPIService',
    function (UtilsService, VRUIUtilsService, PartnerPortal_Invoice_InvoiceTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new InvoiceCustomFieldRemoteSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function InvoiceCustomFieldRemoteSelectorCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

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
                    selectorAPI.clearDataSource();

                    var selectedIds;
                    var filter;
                    var connectionId;
                    var invoiceTypeId;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                        connectionId = payload.connectionId;
                        invoiceTypeId = payload.invoiceTypeId;
                    }
                    return PartnerPortal_Invoice_InvoiceTypeAPIService.GetRemoteInvoiceTypeCustomFieldsInfo(connectionId, invoiceTypeId).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                var item = response[i];
                                ctrl.datasource.push({ FieldName: item, FieldDescription: item });
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'FieldName', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('FieldName', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Invoice Custom Field";

            if (attrs.ismultipleselection != undefined) {
                label = "Invoice Custom Fields";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                        '<vr-select ' + multipleselection + ' datatextfield="FieldDescription" datavaluefield="FieldName" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" '
                            + ' hideremoveicon="ctrl.hideremoveicon" customvalidate="ctrl.customvalidate">' +
                        '</vr-select>' +
                    '</vr-columns>';
        }
    }]);