'use strict';

app.directive('vrAnalyticGridsubtablepositionSelector', ['VR_Analytic_GridSubTablePositionEnum', 'UtilsService', 'VRUIUtilsService',
       function (VR_Analytic_GridSubTablePositionEnum, UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            normalColNum: '@',
            isrequired: '=',
            onselectionchanged: '=',
            selectedvalues: '=',
            isrequired: "=",
            normalColNum: '='
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;
            if ($attrs.ismultipleselection != undefined)
                ctrl.selectedvalues = [];

            var positionSelector = new PositionSelector(ctrl, $scope, $attrs);
            positionSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getUserTemplate(attrs);
        }
    };

    function PositionSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        var directiveAPI;
        var directiveReadyDeferred;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
            ctrl.datasource.datasource = [];
            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload = {
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var positionValue;
                var promises = [];
                ctrl.datasource = UtilsService.getArrayEnum(VR_Analytic_GridSubTablePositionEnum);

                if (payload != undefined) {
                    positionValue = payload.positionValue;

                    if (positionValue != undefined) {
                        VRUIUtilsService.setSelectedValues(positionValue, 'value', attrs, ctrl);
                    }
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getUserTemplate(attrs) {
        var label = 'Position Value';

        var hidelabel = "";
        if (attrs.hidelabel != undefined)
            hidelabel = "hidelabel";

        var haschildcolumns = "";
        if (attrs.usefullcolumn != undefined)
            haschildcolumns = "haschildcolumns";

        var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

        var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

        return '<vr-columns colnum="{{ctrl.normalColNum}}"  ' + haschildcolumns + '>'
            + ' <vr-select on-ready="scopeModel.onSelectorReady"'
            + ' datasource="ctrl.datasource"'
            + ' selectedvalues="ctrl.selectedvalues"'
            + ' datavaluefield="value"'
            + ' datatextfield="description"'
            + ' ' + hideselectedvaluessection
            + ' ' + hideremoveicon
            + ' ' + hidelabel
            + ' isrequired="ctrl.isrequired!=undefined ? ctrl.isrequired : true"'
            + ' onselectionchanged="ctrl.onselectionchanged"'
            + ' label="' + label + '"'
            + ' >'
            + '</vr-select>'
            + ' </vr-columns>';
    }

}]);