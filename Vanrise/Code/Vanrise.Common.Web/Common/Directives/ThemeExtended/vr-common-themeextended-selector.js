'use strict';

app.directive('vrCommonThemeextendedSelector', ['VRCommon_ThemeExtendedAPIService', 'UtilsService', 'VRUIUtilsService', function (VRCommon_ThemeExtendedAPIService, UtilsService, VRUIUtilsService) {

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

            var selector = new ThemeExtendedSelector(ctrl, $scope, $attrs);
            selector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getThemeExtendedSelectorTemplate(attrs);
        }
    };

    function ThemeExtendedSelector(ctrl, $scope, attrs) {

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

                return VRCommon_ThemeExtendedAPIService.GetThemesExtendedInfo().then(function (response) {
                    selectorAPI.clearDataSource();
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.datasource.push(response[i]);
                        }
                    }
                    if (selectedIds != undefined) {
                        var selectedIcon = UtilsService.getItemByVal(ctrl.datasource, selectedIds, 'ThemePath');
                        if (selectedIcon != null)
                            VRUIUtilsService.setSelectedValues(selectedIcon.Name, 'Name', attrs, ctrl);
                    }
                });
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('ThemePath', attrs, ctrl);
            };

            return api;
        }
    }

    function getThemeExtendedSelectorTemplate(attrs) {

        var multipleselection = "";
        var label = "Theme";
        return  '<vr-select on-ready="ctrl.onSelectorReady"'
                + ' datasource=" ctrl.datasource"'
                + ' selectedvalues="ctrl.selectedvalues"'
                + ' datavaluefield="Name"'
                + ' datatextfield="Name"'
                + ' isrequired="ctrl.isrequired"'
                + ' onselectionchanged="ctrl.onselectionchanged"'
                + ' label="' + label + '" >'
                + '</vr-select>';
    }

}]);