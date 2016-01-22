'use strict';
app.directive('vrFzeroFraudanalysisFixedprefixesSelector', ['Fzero_FraudAnalysis_DefineFixedPrefixesAPIService', 'UtilsService', '$compile', 'VRUIUtilsService', 'Fzero_FraudAnalysis_MainService',
function (Fzero_FraudAnalysis_DefineFixedPrefixesAPIService, UtilsService, $compile, VRUIUtilsService, Fzero_FraudAnalysis_MainService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            ismultipleselection: "@",
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "@",
            onselectitem: "=",
            ondeselectitem: "=",
            isdisabled: "=",
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            $scope.addNewPrefix = function () {
                var onPrefixAdded = function (prefixObj) {
                    ctrl.datasource.push(prefixObj.Entity);
                    if ($attrs.ismultipleselection != undefined)
                        ctrl.selectedvalues.push(prefixObj.Entity);
                    else
                        ctrl.selectedvalues = prefixObj.Entity;
                };
                Fzero_FraudAnalysis_MainService.addNewFixedPrefix(onPrefixAdded);
            }




            var ctor = new prefixCtor(ctrl, $scope, $attrs);
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
        var label = "Prefix";
        if (attrs.ismultipleselection != undefined) {
            label = "Fixed Prefixes";
            multipleselection = "ismultipleselection";
        }

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewPrefix"';

        return '<div>'
            + '<vr-select ' + multipleselection + '  datatextfield="Prefix" datavaluefield="Prefix" '
        + required + ' label="' + label + '" ' + addCliked + ' datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" entityName="Prefix" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem"></vr-select>'
           + '</div>'


    }

    function prefixCtor(ctrl, $scope, $attrs) {
        var baseApi;
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            ctrl.onBaseSelectorReady = function (api) {
                baseApi = api;
            }
            var api = {};
            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('Prefix', $attrs, ctrl);
            }

            api.load = function (payload) {

                var filter;
                var selectedIds;
                var selectAll;
                if (payload != undefined) {
                    filter = payload.filter;
                    selectedIds = payload.selectedIds;
                    selectAll = payload.selectAll;
                }
                var serializedFilter = {};
                ctrl.filter = undefined
                if (filter != undefined) {
                    ctrl.filter = filter;
                    serializedFilter = UtilsService.serializetoJson(filter);
                }

                return getPrefixesInfo($attrs, ctrl, selectedIds, serializedFilter, baseApi, selectAll);

            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }

    function getPrefixesInfo(attrs, ctrl, selectedIds, serializedFilter, baseApi, selectAll) {

        var allIds = [];

        if (baseApi != undefined)
            baseApi.clearDataSource();
        return Fzero_FraudAnalysis_DefineFixedPrefixesAPIService.GetPrefixesInfo(serializedFilter).then(function (response) {
            ctrl.datasource.length = 0;
            angular.forEach(response, function (itm) {
                ctrl.datasource.push(itm);
                allIds.push(itm.Prefix);
            });

            if (selectedIds != undefined) {
                VRUIUtilsService.setSelectedValues(selectedIds, 'Prefix', attrs, ctrl);
            }
            else if (selectAll != undefined && selectAll) {
                VRUIUtilsService.setSelectedValues(allIds, 'Prefix', attrs, ctrl);
            }

        });
    }

    return directiveDefinitionObject;
}]);