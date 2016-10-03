'use strict';

app.directive('whsSplViewchangetypeSelector', ['WhS_SupPL_PreviewChangeTypeEnum', 'UtilsService', 'VRUIUtilsService',
    function (WhS_SupPL_PreviewChangeTypeEnum, UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            selectedvalues: '=',
            onselectionchanged: '=',
            onselectitem: '=',
            ondeselectitem: '=',
            isrequired: '=',
            hideremoveicon: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            ctrl.datasource = [];

            ctrl.selectedvalues;

            var viewChangeTypeSelector = new ViewChangeTypeSelector(ctrl, $scope, $attrs);
            viewChangeTypeSelector.initializeController();

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getUserTemplate(attrs);
        }
    };

    function ViewChangeTypeSelector(ctrl, $scope, attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {
            ctrl.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var selectedIds;

                if (payload != undefined) {
                    selectedIds = payload.selectedIds;
                }

                ctrl.datasource = UtilsService.getArrayEnum(WhS_SupPL_PreviewChangeTypeEnum);

                if (selectedIds != undefined) {
                    VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                }
            };

            api.getSelectedIds = function () {
                return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getUserTemplate(attrs) {

        
        var label = "View Change Type";

        return '<vr-columns colnum="{{ctrl.normalColNum}}"><vr-select  datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon"></vr-select></vr-columns>';
    }

}]);