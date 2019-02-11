'use strict';

app.directive('vrCommonFigureiconSelective', ['VRCommon_FigureIconAPIService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_FigureIconAPIService, UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectedvalues: '=',
            onselectionchanged: '=',
            isrequired: '=',
            hideremoveicon: '@',
            normalColNum: '@',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];
            ctrl.selectedvalues;

            var figureIconSelector = new FigureIconSelector(ctrl, $scope, $attrs);
            figureIconSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getFigureIconSelectorTemplate(attrs);
        }
    };

    function FigureIconSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {
            ctrl.datasource = [];

            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(getDirectiveAPI());
                }
            };
        }

        function getDirectiveAPI() {
            var api = {};

            api.load = function (payload) {

                var selectedIds;

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                return VRCommon_FigureIconAPIService.GetFigureIconsInfo().then(function (response) {
                    selectorAPI.clearDataSource();
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                    }
                    if (selectedIds != undefined) {
                        var selectedIcon = UtilsService.getItemByVal(ctrl.datasource, selectedIds, 'IconPath');
                        if (selectedIcon != null)
                            VRUIUtilsService.setSelectedValues(selectedIcon.Name, 'Name', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('IconPath', attrs, ctrl);
            };

            return api;
        }
    }

    function getFigureIconSelectorTemplate(attrs) {

        var multipleselection = "";
        var label = "Icon";
        return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                        + ' <div  style="display: inline-block;width:calc(100% - 35px);">'
                            + '<vr-select on-ready="ctrl.onSelectorReady"'
                                + ' datasource=" ctrl.datasource"'
                                + ' selectedvalues="ctrl.selectedvalues"'
                                + ' datavaluefield="Name"'
                                + ' datatextfield="Name"'
                                + ' isrequired="ctrl.isrequired"'
                                + ' onselectionchanged="ctrl.onselectionchanged"'
                                + ' label="' + label + '" >'
                             + '</vr-select>'
                          + '</div>'
                       + '<div ng-show="ctrl.selectedvalues" style="display: inline-block;width:30px;background-color:#0077B5;width: 30px;height: 30px;padding: 2px;position: relative;top: -12px;left: 5px;"> <img ng-src="{{ctrl.selectedvalues.IconPath}}" style="height:25px;width:25px" /> </div>'
                    + ' </vr-columns>';
    }

}]);