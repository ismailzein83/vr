(function (app) {

    'use strict';

    OrgChartSelectorDirective.$inject = ['VR_Sec_OrgChartAPIService', 'VR_Sec_OrgChartService', 'UtilsService', 'VRUIUtilsService'];

    function OrgChartSelectorDirective(VR_Sec_OrgChartAPIService, VR_Sec_OrgChartService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                ismultipleselection: '@',
                isdisabled: '@',
                isrequired: '@',
                hideremoveicon: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection)
                    ctrl.selectedvalues = [];

                ctrl.addOrgChart = function () {
                    var onOrgChartAdded = function (addedOrgChart) {
                        ctrl.datasource.push(addedOrgChart);

                        if ($attrs.ismultipleselection)
                            ctrl.selectedvalues.push(addedOrgChart);
                        else
                            ctrl.selectedvalues = addedOrgChart;
                    };
                    VR_Sec_OrgChartService.addOrgChart(onOrgChartAdded);
                };

                var orgChartSelector = new OrgChartSelector(ctrl, $scope, $attrs);
                orgChartSelector.initialize();
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

        function OrgChartSelector(ctrl, $scope, attrs) {
            this.initialize = initialize;

            var selectorAPI;

            function initialize() {
                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };
            }

            function getDirectiveAPI() {
                var directiveAPI = {};

                directiveAPI.load = function (payload) {
                    var selectedIds;

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds;
                    }

                    return VR_Sec_OrgChartAPIService.GetOrgChartInfo().then(function (response) {
                        selectorAPI.clearDataSource();
                        //ctrl.datasource.length = 0;

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'OrgChartId', attrs, ctrl);
                        }
                    });
                };

                directiveAPI.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('OrgChartId', attrs, ctrl);
                };

                return directiveAPI;
            }
        }

        function getDirectiveTemplate(attrs) {
            var label = attrs.label ? attrs.label : 'Org Chart';

            var ismultipleselection = '';
            if (attrs.ismultipleselection) {
                label = 'Org Charts';
                ismultipleselection = ' ismultipleselection';
            }

            var hideremoveicon = (attrs.hideremoveicon != undefined && attrs.hideremoveicon) != null ? 'hideremoveicon' : null;

            var onaddclicked = '';
            if (attrs.showaddbutton)
                onaddclicked = ' onaddclicked="ctrl.addOrgChart"';

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady"'
                    + ' label="' + label + '"'
                    + ' datasource="ctrl.datasource"'
                    + ' selectedvalues="ctrl.selectedvalues"'
                    + ' onselectionchanged="ctrl.onselectionchanged"'
                    + ' onselectitem="ctrl.onselectitem"'
                    + ' ondeselectitem="ctrl.ondeselectitem"'
                    + ' datavaluefield="OrgChartId"'
                    + ' datatextfield="Name"'
                    + onaddclicked
                    + ismultipleselection
                    + ' vr-disabled="ctrl.isdisabled"'
                    + ' isrequired="ctrl.isrequired"'
                    + hideremoveicon
                    + ' entityName="Org Chart">'
                + '</vr-select>'
            + '</div>';
        }

        return directiveDefinitionObject;
    }

    app.directive('vrSecOrgchartSelector', OrgChartSelectorDirective);

})(app);
