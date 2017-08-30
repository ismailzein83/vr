'use strict';
app.directive('whsBeFinancialaccountdefinitionSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_FinancialAccountDefinitionAPIService',
function (UtilsService, VRUIUtilsService, WhS_BE_FinancialAccountDefinitionAPIService) {

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

            ctrl.datasource = [];
            var ctor = new financialAccountDefinitionSelectorCtor(ctrl, $scope, $attrs);
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

        var multipleselection = "";
        var label = "Type";
        if (attrs.ismultipleselection != undefined) {
            label = "Types";
            multipleselection = "ismultipleselection";
        }

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";

        return '<vr-columns colnum="{{ctrl.normalColNum}}" >'
           + '<span  vr-disabled="ctrl.isdisabled">'
            + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="FinancialAccountDefinitionId" on-ready="ctrl.onSelectorReady"  isrequired="ctrl.isrequired" label="' + label + '" ' + hideselectedvaluessection + '  datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues"   onselectionchanged="ctrl.onselectionchanged"  onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ></vr-select>'
           + '</span></vr-columns>';
    }

    function financialAccountDefinitionSelectorCtor(ctrl, $scope, $attrs) {
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
                return VRUIUtilsService.getIdSelectedIds('FinancialAccountDefinitionId', $attrs, ctrl);
            };
            api.getSelectedValue = function () {
                return ctrl.selectedvalues;
            };
            api.load = function (payload) {
                selectorApi.clearDataSource();
                ctrl.datasource.length = 0;
                var selectedIds;
                var filter;
                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                    filter = payload.filter;
                }

                var serializedFilter = {};
                if (filter != undefined)
                    serializedFilter = UtilsService.serializetoJson(filter);

                return WhS_BE_FinancialAccountDefinitionAPIService.GetFinancialAccountDefinitionInfo(serializedFilter).then(function (response) {
                    angular.forEach(response, function (itm) {
                        ctrl.datasource.push(itm);
                    });

                    if (selectedIds != undefined) {
                        VRUIUtilsService.setSelectedValues(selectedIds, 'FinancialAccountDefinitionId', $attrs, ctrl);
                    }
                });
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);