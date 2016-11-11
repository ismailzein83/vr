'use strict';

app.directive('vrWhsBeCdpnSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_CDPNIdentificationEnum',
    function (UtilsService, VRUIUtilsService, WhS_BE_CDPNIdentificationEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                isrequired: '=',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var cdpnSelector = new CDPNSelectorCtor(ctrl, $scope, $attrs);
                cdpnSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {

                return "/Client/Modules/WhS_BusinessEntity/Directives/CDRImportSettings/Templates/CDPNSelectorTemplate.html";
            }
        };

        function CDPNSelectorCtor(ctrl, $scope, attrs) {

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

                    ctrl.datasource = UtilsService.getArrayEnum(WhS_BE_CDPNIdentificationEnum);

                    if (selectedIds != undefined) {
                        ctrl.selectedvalues = UtilsService.getEnum(WhS_BE_CDPNIdentificationEnum, 'value', selectedIds);
                    }
                };

                api.getSelectedIds = function () {

                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };


                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
    }]);