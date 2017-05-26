'use strict';
app.directive('vrWhsBeSalepricelisttypeSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_SalePriceListTypeEnum',
    function (UtilsService, VRUIUtilsService, WhS_BE_SalePriceListTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                type: "=",
                onReady: '=',
                label: "@",
                ismultipleselection: "@",
                hideselectedvaluessection: '@',
                onselectionchanged: '=',
                isrequired: '=',
                isdisabled: "=",
                selectedvalues: "=",
                hidelabel: '@',
                normalColNum: '@',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new directiveCtor(ctrl, $scope, $attrs);
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
            if (attrs.hidelabel == undefined)
                label = 'label="PriceList Type"';
            var disabled = "";
            if (attrs.isdisabled)
                disabled = "vr-disabled='true'";

            var hideremoveicon = "";
            if (attrs.hideremoveicon)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection";

            var widthfactor = 2;
            if (attrs.widthfactor != undefined)
                widthfactor = attrs.widthfactor;


            return '<vr-columns colnum="{{ctrl.normalColNum}}" ' + disabled + '  > <vr-select ' + multipleselection + ' datasource="ctrl.datasource" isrequired="ctrl.isrequired"  ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" datatextfield="description" datavaluefield="value"'
                   + 'entityname="Pricelist Type" ' + label + ' ' + hideremoveicon + '></vr-select> </vr-columns>';

        }
        function directiveCtor(ctrl, $scope, $attrs) {
            ctrl.datasource = UtilsService.getArrayEnum(WhS_BE_SalePriceListTypeEnum);

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', $attrs, ctrl);
                };
                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }
                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', $attrs, ctrl);
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);

