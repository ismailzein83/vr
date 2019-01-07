'use strict';

app.directive('vrWhsDealDealdefinitionSelector', ['UtilsService', '$compile', 'VRUIUtilsService', 'WhS_Deal_DealDefinitionAPIService', 'WhS_Deal_SwapDealService', 'WhS_Deal_VolumeCommitmentService', 'WhS_Deal_DealDefinitionTypeEnum',
    function (UtilsService, $compile, VRUIUtilsService, WhS_Deal_DealDefinitionAPIService, WhS_Deal_SwapDealService, WhS_Deal_VolumeCommitmentService, WhS_Deal_DealDefinitionTypeEnum) {

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
                hideremoveicon: '@',
                hasviewpremission: '='
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
                label = attrs.ismultipleselection != undefined ? 'label="Deals"' : 'label="Deal"';

            var disabled = "";
            if (attrs.isdisabled)
                disabled = "vr-disabled='true'";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection";

            var haschildcolumns = "";
            if (attrs.usefullcolumn != undefined)
                haschildcolumns = "haschildcolumns";

            var onviewclicked = "";
            if (attrs.includeviewhandler != undefined)
                onviewclicked = "onviewclicked='onViewIconClicked'";

            return '<vr-columns  colnum="{{ctrl.normalColNum}}" ' + disabled + ' ' + haschildcolumns + '  > <vr-select on-ready="scopeModel.onSelectorReady" hasviewpermission="ctrl.hasviewpremission" ' + onviewclicked + ' ' + multipleselection + ' datasource="ctrl.datasource" isrequired="ctrl.isrequired" ' + hideselectedvaluessection + ' selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" datatextfield="Name" datavaluefield="DealId"'
                + 'entityname="Deal" ' + label + ' ' + hideremoveicon + '></vr-select> </vr-columns>';

        }
        function directiveCtor(ctrl, $scope, $attrs) {
            var selectorAPI;
            $scope.onViewIconClicked = function (item) {
                if (item.ConfigId == WhS_Deal_DealDefinitionTypeEnum.SwapDeal.value)
                    WhS_Deal_SwapDealService.viewSwapDeal(item.DealId, true);
                else if (item.ConfigId == WhS_Deal_DealDefinitionTypeEnum.SwapDeal.value)
                    WhS_Deal_VolumeCommitmentService.viewVolumeCommitment(item.DealId);
            };

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};
                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('DealId', $attrs, ctrl);
                };

                api.load = function (payload) {
                    ctrl.datasource.length = 0;

                    var filter;
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    if (filter == undefined)
                        filter = {};

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);
                    return WhS_Deal_DealDefinitionAPIService.GetDealDefinitionInfo(serializedFilter).then(function (response) {
                        angular.forEach(response, function (itm) {
                            ctrl.datasource.push(itm);
                        });
                        if (selectedIds != undefined)
                            VRUIUtilsService.setSelectedValues(selectedIds, 'DealId', $attrs, ctrl);

                    });
                };

                api.clearDataSource = function () {
                    selectorAPI.clearDataSource();
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);

