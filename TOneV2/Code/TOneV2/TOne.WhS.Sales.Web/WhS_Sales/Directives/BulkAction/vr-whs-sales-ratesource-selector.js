'use strict';
app.directive('vrWhsSalesRatesourceSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_Sales_RateSourceEnum',
function (UtilsService, VRUIUtilsService, WhS_Sales_RateSourceEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            isdisabled: "=",
            onselectionchanged: '=',
            isrequired: "=",
            selectedvalues: '=',
            normalColNum: '@',
            onselectitem: "=",
            ondeselectitem: "="
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = UtilsService.getArrayEnum(WhS_Sales_RateSourceEnum);;
            var ctor = new selectorCtor(ctrl, $scope, $attrs);
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
        var label = "Rate Source";
        if (attrs.ismultipleselection != undefined) {
            label = "Rate Sources";
            multipleselection = "ismultipleselection";
        }
        if (attrs.customelabel != undefined)
            label = attrs.customelabel;
        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";

        return '<vr-select ' + multipleselection + '  datatextfield="description" datavaluefield="value" on-ready="ctrl.onSelectorReady"  isrequired="ctrl.isrequired" label="' + label + '" ' + hideselectedvaluessection + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"  ></vr-select>';
    }

    function selectorCtor(ctrl, $scope, $attrs) {
        var selectorApi;
        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorApi = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', $attrs, ctrl);
            };
            api.getSelectedText = function () {
                return VRUIUtilsService.getIdSelectedIds('description', $attrs, ctrl);
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