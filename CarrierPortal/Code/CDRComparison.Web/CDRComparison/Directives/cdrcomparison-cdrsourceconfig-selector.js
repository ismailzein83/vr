(function (app) {

    'use strict';

    CDRSourceConfigSelectorDirective.$inject = ['CDRComparison_CDRSourceConfigAPIService', 'UtilsService', 'VRUIUtilsService'];

    function CDRSourceConfigSelectorDirective(CDRComparison_CDRSourceConfigAPIService, UtilsService, VRUIUtilsService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                customlabel: "@",
                selectedvalues: '=',
                onselectionchanged: '=',
                ondeselectitem: "=",
                onselectitem: "=",
                ismultipleselection: "@",
                isrequired: "=",
                isdisabled: "=",
                onitemadded: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                //ctrl.haspermission = function () { };

                var cdrSourceConfigSelector = new CDRSourceConfigSelector(ctrl, $scope, $attrs);
                cdrSourceConfigSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getDirectiveTemplate(attrs);
            }
        };

        function CDRSourceConfigSelector(ctrl, $scope, attrs) {

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
                    var filter;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    return CDRComparison_CDRSourceConfigAPIService.GetCDRSourceConfigs(UtilsService.serializetoJson(filter)).then(function (response) {
                        ctrl.datasource.length = 0;

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }
                        }

                        if (selectedIds) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'CDRSourceConfigId', attrs, ctrl);
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('CDRSourceConfigId', attrs, ctrl);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);
            }

        }

        function getDirectiveTemplate(attrs) {

            var multipleselection = "";

            var label = "Source Configuration";
            if (attrs.ismultipleselection != undefined) {
                label = "Source Configurations";
                multipleselection = "ismultipleselection";
            }

            if (attrs.customlabel != undefined)
                label = attrs.customlabel;

            return '<div><vr-select on-ready="ctrl.onSelectorReady" label="' + label + '" entityName="Source Configuration" datasource="ctrl.datasource" datavaluefield="CDRSourceConfigId" datatextfield="Name" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + multipleselection + ' isrequired="ctrl.isrequired" vr-disabled="ctrl.isdisabled" haspermission="ctrl.haspermission"></vr-select></div>';
        }
    }

    app.directive('cdrcomparisonCdrsourceconfigSelector', CDRSourceConfigSelectorDirective);

})(app);