(function (app) {

    'use strict';

    ZoneSelectorDirective.$inject = ['Retail_BE_ZoneAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ZoneSelectorDirective(Retail_BE_ZoneAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectitem: "=",
                ondeselectitem: "=",
                onselectionchanged: '=',
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                customlabel: "@",
                normalColNum: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

                var zoneSelector = new ZoneSelector(ctrl, $scope, $attrs);
                zoneSelector.initializeController();
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
                return getDirectiveTemplate(attrs);
            }
        };

        function ZoneSelector(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var selectorAPI;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter;
                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }
                    if (filter != undefined) {
                        return Retail_BE_ZoneAPIService.GetZonesByCountryId(UtilsService.serializetoJson(filter)).then(function (response) {
                            selectorAPI.clearDataSource();
                            ctrl.datasource.length = 0;
                            if (response) {
                                for (var i = 0; i < response.length; i++) {
                                    ctrl.datasource.push(response[i]);
                                }
                            }
                            if (selectedIds) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ZoneId', attrs, ctrl);
                            }
                        });
                    }

                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ZoneId', attrs, ctrl);
                };

                return api;
            }
        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = '';

            var label = 'zone';
            if (attrs.ismultipleselection != undefined) {
                label = 'Zones';
                multipleselection = 'ismultipleselection';
            }

            if (attrs.customlabel != undefined) {
                label = attrs.customlabel;
            }

            var hideselectedvaluessection = (attrs.hideselectedvaluessection != undefined) ? 'hideselectedvaluessection' : null;

            var hideremoveicon = (attrs.hideremoveicon != undefined) ? 'hideremoveicon' : null;

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="AnalyticReportId"'
                    + ' datatextfield="Name"'
                    + ' ' + multipleselection
                    + ' ' + hideselectedvaluessection
                    + ' isrequired="ctrl.isrequired"'
                    + ' ' + hideremoveicon
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' label="' + label + '"'
                    + ' entityName="' + label + '"'
                + '</vr-select>'
            + '</vr-columns>';
        }
    }

    app.directive('retailBeZoneSelector', ZoneSelectorDirective);

})(app);
