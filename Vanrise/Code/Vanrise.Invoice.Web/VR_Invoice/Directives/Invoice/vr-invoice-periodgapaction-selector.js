(function (app) {

    'use strict';
    InvoicePeriodGapActionSelectorDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_Invoice_InvoicePeriodGapActionEnum'];

    function InvoicePeriodGapActionSelectorDirective(UtilsService, VRUIUtilsService, VR_Invoice_InvoicePeriodGapActionEnum) {
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

                var ctor = new InvoicePeriodGapActionSelector($scope, ctrl, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Period Gap Action";

            if (attrs.ismultipleselection != undefined) {
                label = "Period Gap Actions";
                multipleselection = "ismultipleselection";
            }
            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                            + ' <vr-select on-ready="ctrl.onSelectorReady"'
                                + ' datasource="ctrl.datasource"'
                                + ' selectedvalues="ctrl.selectedvalues"'
                                + ' onselectionchanged="ctrl.onselectionchanged"'
                                + ' datavaluefield="value"'
                                + ' datatextfield="description"'
                                + ' isrequired="ctrl.isrequired"'
                                + ' entityName="' + label + '"'
                                + ' label="' + label + '" '
                                + ' hideremoveicon>'
                            + '</vr-select>'
                    + ' </vr-columns>';
        }

        function InvoicePeriodGapActionSelector($scope, ctrl, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;
                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                }
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();

                    var promises = [];
                    ctrl.datasource = UtilsService.getArrayEnum(VR_Invoice_InvoicePeriodGapActionEnum);
                    var selectedIds;
                    var selectFirstItem;
                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                        selectFirstItem = payload.selectFirstItem;
                    }

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                    } else if (selectFirstItem)
                    {
                        var defaultValue = attrs.ismultipleselection != undefined ? [ctrl.datasource[0].value] : ctrl.datasource[0].value;
                        VRUIUtilsService.setSelectedValues(defaultValue, 'value', attrs, ctrl);
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                return api;
            }

        }

    }
app.directive('vrInvoicePeriodgapactionSelector', InvoicePeriodGapActionSelectorDirective);

})(app);