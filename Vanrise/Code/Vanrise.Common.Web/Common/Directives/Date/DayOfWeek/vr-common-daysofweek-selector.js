'use strict';
app.directive('vrCommonDaysofweekSelector', ['UtilsService', 'VRUIUtilsService', 'DaysOfWeekEnum',
    function (utilsService, vruiUtilsService, daysOfWeekEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                selectedvalues: '=',
                isrequired: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                isdisabled: "=",
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues;

                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var ctor = new daysOfWeekCtor(ctrl, $scope, $attrs);
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
                return getDaysOfWeekTemplate(attrs);
            }
        };

        function getDaysOfWeekTemplate(attrs) {
            var multipleselection = "";
            var label = "Day";
            if (attrs.ismultipleselection != undefined) {
                label = "Days";
                multipleselection = "ismultipleselection";
            }

            return '<div>'
                + '<vr-select ' + multipleselection + '  on-ready="ctrl.onSelectorReady" datatextfield="description" datavaluefield="value" label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" entityName="DaysOfWeek" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" hideremoveicon="ctrl.hideremoveicon" isrequired="ctrl.isrequired"></vr-select>'
               + '</div>';
        }

        function daysOfWeekCtor(ctrl, $scope, attrs) {
            var selectorAPI;
            function initializeController() {
                getAllDaysOfWeek();
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
                        selectedIds = payload.selectedIds;
                    }

                    if (selectedIds != undefined)
                        vruiUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);

                };

                api.getSelectedIds = function () {
                    return vruiUtilsService.getIdSelectedIds('value', attrs, ctrl);
                };

                api.selectedDay = function (selectedIds) {
                    alert(selectedIds);
                    vruiUtilsService.setSelectedValues(selectedIds, 'value', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getAllDaysOfWeek() {
                for (var p in daysOfWeekEnum) {
                    ctrl.datasource.push(daysOfWeekEnum[p]);
                }

            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);