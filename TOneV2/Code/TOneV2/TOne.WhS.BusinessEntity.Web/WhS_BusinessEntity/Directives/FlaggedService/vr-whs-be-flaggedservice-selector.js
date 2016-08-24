'use strict';
app.directive('vrWhsBeFlaggedserviceSelector', ['UtilsService', 'VRUIUtilsService',
function (UtilsService, VRUIUtilsService) {

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
            ondeselectitem: "=",
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            ctrl.datasource = [];
            var ctor = new dealCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getTemplate(attrs);
        }

    };


    function getTemplate(attrs) {

        var multipleselection = "";
        var label = "Flagged Service";
        if (attrs.ismultipleselection != undefined) {
            label = "Flagged Services";
            multipleselection = "ismultipleselection";
        }

        return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="FlaggedServiceId" on-ready="ctrl.onSelectorReady"  isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" vr-disabled="ctrl.isdisabled"></vr-select>'
           + '</vr-columns>';
    }

    function dealCtor(ctrl, $scope, $attrs) {
        var selectorApi;
        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorApi = api;
                defineAPI();
            }
        }

        function defineAPI() {
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('FlaggedServiceId', $attrs, ctrl);
            }
            api.load = function (payload) {
                selectorApi.clearDataSource();
                ctrl.datasource.length = 0;
                var selectedIds;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                ctrl.datasource.push({
                    FlaggedServiceId: 0,
                    Name: "Wholesale"
                }, {
                    FlaggedServiceId: 1,
                    Name: "Retail"
                }, {
                    FlaggedServiceId: 2,
                    Name: "Premium"
                }, {
                    FlaggedServiceId: 3,
                    Name: "CLI"
                });
                if (selectedIds != undefined)
                    VRUIUtilsService.setSelectedValues(selectedIds, 'FlaggedServiceId', $attrs, ctrl);
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);