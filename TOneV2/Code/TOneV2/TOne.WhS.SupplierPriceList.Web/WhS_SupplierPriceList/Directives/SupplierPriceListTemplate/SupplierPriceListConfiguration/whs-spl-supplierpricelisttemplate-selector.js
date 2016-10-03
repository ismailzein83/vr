(function (app) {

    'use strict';

    SupplierpricelisttemplateSelector.$inject = ['WhS_SupPL_SupplierPriceListTemplateAPIService', 'UtilsService', 'VRUIUtilsService'];

    function SupplierpricelisttemplateSelector(WhS_SupPL_SupplierPriceListTemplateAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                customlabel: "@",
                selectedvalues: '=',
                onselectionchanged: '=',
                ondeselectitem: "=",
                onselectitem: "=",
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                onitemadded: "=",
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                //ctrl.haspermission = function () { };

                var inputpricelistTemplateSelector = new InputpricelistTemplateSelector(ctrl, $scope, $attrs);
                inputpricelistTemplateSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function InputpricelistTemplateSelector(ctrl, $scope, attrs) {

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

                    var selectedIds;
                    var filter;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return WhS_SupPL_SupplierPriceListTemplateAPIService.GetSupplierPriceListTemplatesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'PriceListTemplateId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('PriceListTemplateId', attrs, ctrl);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = "";

            var label = "PriceList Template";
            if (attrs.ismultipleselection != undefined) {
                label = "PriceList Templates";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select on-ready="ctrl.onSelectorReady" label="' + label + '" entityName="PriceList Template" datasource="ctrl.datasource" datavaluefield="PriceListTemplateId" datatextfield="Name" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + multipleselection + ' isrequired="ctrl.isrequired" vr-disabled="ctrl.isdisabled" haspermission="ctrl.haspermission"></vr-select></vr-columns>';
        }
    }

    app.directive('whsSplSupplierpricelisttemplateSelector', SupplierpricelisttemplateSelector);

})(app);