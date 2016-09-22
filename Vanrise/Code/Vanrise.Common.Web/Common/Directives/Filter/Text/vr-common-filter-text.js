'use strict';

app.directive('vrCommonFilterText', ['UtilsService', 'VRUIUtilsService', function (VRCommon_TextFilterTypeEnum, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            label: '@',
            isrequired: '=',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            
            ctrl.labelText = ($attrs.label != undefined) ? ctrl.label : 'Text';

            var filterText = new FilterText(ctrl, $scope, $attrs);
            filterText.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Common/Directives/Filter/Text/Templates/TextFilterTemplate.html'
    };

    function FilterText(ctrl, $scope, attrs) {
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
                var selectedIds;

                if (payload != undefined) {
                    ctrl.text = payload.text;
                    selectedIds = payload.textFilterTypeValue;
                }

                var selectorPayload = { selectedIds: selectedIds }
                return selectorAPI.load(selectorPayload);
            };

            api.getData = function () {
                var data;
                var selectedId = selectorAPI.getSelectedIds();
                if (selectedId != undefined && ctrl.text != undefined && ctrl.text != '') {
                    data = {
                        Text: ctrl.text,
                        TextFilterType: selectorAPI.getSelectedIds()
                    };
                }
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);