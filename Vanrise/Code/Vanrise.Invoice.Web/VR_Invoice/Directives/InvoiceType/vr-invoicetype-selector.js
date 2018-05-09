(function (app) {

    'use strict';

    InvoicetypeSelector.$inject = ['VR_Invoice_InvoiceTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function InvoicetypeSelector(VR_Invoice_InvoiceTypeAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                customlabel: "@",
                normalColNum: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var invoicetype = new Invoicetype(ctrl, $scope, $attrs);
                invoicetype.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function Invoicetype(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return VR_Invoice_InvoiceTypeAPIService.GetInvoiceTypesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorAPI.clearDataSource();
                        ctrl.datasource.length = 0;
                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }
                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'InvoiceTypeId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('InvoiceTypeId', attrs, ctrl);
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'Invoice Type';
            if (attrs.ismultipleselection != undefined) {
                label = 'Invoice Types';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : '';

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : '';

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<span vr-disabled="ctrl.isdisabled">'
                    + '<vr-select on-ready="ctrl.onSelectorReady"'
                        + ' datasource="ctrl.datasource"'
                        + ' selectedvalues="ctrl.selectedvalues"'
                        + ' onselectionchanged="ctrl.onselectionchanged"'
                        + ' onselectitem="ctrl.onselectitem"'
                        + ' ondeselectitem="ctrl.ondeselectitem"'
                        + ' datavaluefield="InvoiceTypeId"'
                        + ' datatextfield="Name"'
                        + ' ' + multipleselection
                        + ' ' + hideselectedvaluessection
                        + ' isrequired="ctrl.isrequired"'
                        + ' ' + hideremoveicon
                        + ' label="' + label + '"'
                        + ' entityName="' + label + '" '
                        + hidelabel 
                    + ' </vr-select>'
                + '</span>'
            + '</vr-columns>';
        }
    }

    app.directive('vrInvoicetypeSelector', InvoicetypeSelector);

})(app);
