(function (app) {

    'use strict';

    StrategySelectorDirective.$inject = ['StrategyAPIService', 'UtilsService', 'VRUIUtilsService'];

    function StrategySelectorDirective(StrategyAPIService, UtilsService, VRUIUtilsService) {

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
                customlabel: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var strategySelector = new StrategySelector(ctrl, $scope, $attrs);
                strategySelector.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            var multipleselection = "";

            var label = "Strategy";
            if (attrs.ismultipleselection != undefined) {
                label = "Strategies";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<div>'
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="Id" isrequired="ctrl.isrequired"'
                + ' label="' + label + '" datasource="ctrl.datasource" on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" vr-disabled="ctrl.isdisabled" onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" entityname="' + label + '"></vr-select>'
                + '</div>';
        }

        function StrategySelector(ctrl, $scope, attrs) {
            var selectorApi;

            function initializeController() {
                ctrl.onSelectorReady = function (api) {
                    selectorApi = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var filter = {};
                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return StrategyAPIService.GetStrategiesInfo(UtilsService.serializetoJson(filter)).then(function (response) {
                        selectorApi.clearDataSource();

                        if (response) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'Id', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('Id', attrs, ctrl);
                };

                if (ctrl.onReady != null && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }

    app.directive('cdranalysisFaStrategySelector', StrategySelectorDirective);

})(app);
