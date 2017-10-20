'use strict';

app.directive('vrCommonTimeoffsetSelector', ['UtilsService', 'VRUIUtilsService', 'VRCommon_TimeOffsetEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_TimeOffsetEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum:'@',
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new TimeOffsetCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function TimeOffsetCtor(ctrl, $scope, attrs) {
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

                    var timeoffsets = UtilsService.getArrayEnum(VRCommon_TimeOffsetEnum);
                    if (timeoffsets != null) {
                        for (var i = 0, length = timeoffsets.length; i < length; i++) {
                            ctrl.datasource.push(timeoffsets[i]);
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {

            var multipleselection = "";
            var label = "Time Offset";

            if (attrs.ismultipleselection != undefined) {
                label = "Time Offsets";
                multipleselection = "ismultipleselection";
            }
            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            var hideremoveicon = '';
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = ' hideremoveicon ';

            return '<vr-columns colnum="{{ctrl.normalColNum}}" >' +
                '<vr-select ' + multipleselection + ' datatextfield="description" datavaluefield="value" isrequired="ctrl.isrequired" label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" '
                            + ' selectedvalues="ctrl.selectedvalues"  entityName="' + label + '" '
                            + hideremoveicon + '>' +
                        '</vr-select>'
            + '</vr-columns>';
        }
    }]);