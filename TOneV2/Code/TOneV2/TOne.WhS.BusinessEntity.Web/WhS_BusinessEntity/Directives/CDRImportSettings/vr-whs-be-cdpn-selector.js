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
                customvalidate: '=',
                label: '@'
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
            template: function (element, attrs) {

                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "CDPN";
            var entityName = "CDPN";

            if (attrs.ismultipleselection != undefined) {
                label = "CDPNs";
                multipleselection = "ismultipleselection";
            }
            if (attrs.label != undefined)
                label = attrs.label;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">' +
                       '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label +
                           '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="' + entityName +
                           '" customvalidate="ctrl.customvalidate">' +
                       '</vr-select>' +
                   '</vr-columns>';
        }

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