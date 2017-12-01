'use strict';
app.directive('vrInvoiceInvoicesettingpartsdefinitionSelector', ['VR_Invoice_InvoiceTypeAPIService', 'UtilsService', 'VRUIUtilsService',
    function (VR_Invoice_InvoiceTypeAPIService, UtilsService, VRUIUtilsService) {

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

                var ctor = new partConfigCtor(ctrl, $scope, $attrs);
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
                return getConfigTemplate(attrs);
            }

        };


        function getConfigTemplate(attrs) {

            var multipleselection = "";
            var label = "Part";
            if (attrs.ismultipleselection != undefined) {
                label = "Parts";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
            {
                label = attrs.customlabel;
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="Name" datavaluefield="PartConfigId" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="Part" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }

        function partConfigCtor(ctrl, $scope, attrs) {
            var selectorAPI;
            var invoiceTypeId;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
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
                        filter = payload.filter;

                    }
                    return VR_Invoice_InvoiceTypeAPIService.GetInvoiceSettingPartsInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++)
                                ctrl.datasource.push(response[i]);
                            if (selectedIds != undefined)
                                VRUIUtilsService.setSelectedValues(selectedIds, 'PartConfigId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('PartConfigId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);