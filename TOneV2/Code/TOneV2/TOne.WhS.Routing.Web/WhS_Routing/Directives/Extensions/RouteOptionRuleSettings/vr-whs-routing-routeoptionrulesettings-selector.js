'use strict';

app.directive('vrWhsRoutingRouteoptionrulesettingsSelector', ['WhS_Routing_RouteOptionRuleAPIService', 'UtilsService', 'VRUIUtilsService',

    function (WhS_Routing_RouteOptionRuleAPIService, UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                selectedvalues: '=',
                onselectionchanged: '=',
                onselectitem: '=',
                ondeselectitem: '=',
                isrequired: '=',
                hideremoveicon: '@',
                normalColNum: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                var routeOptionRuleSettingsSelector = new RouteOptionRuleSettingsSelector(ctrl, $scope, $attrs);
                routeOptionRuleSettingsSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function RouteOptionRuleSettingsSelector(ctrl, $scope, attrs) {
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
                        selectedIds = payload.selectedIds;
                        filter = payload.filter;
                    }

                    return WhS_Routing_RouteOptionRuleAPIService.GetRouteOptionRuleSettingsTemplates().then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', attrs, ctrl);
                            }
                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ExtensionConfigurationId', attrs, ctrl);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Option Rule Type";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
                label = "Option Rule Types";
            }

            var hideremoveicon;
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-select datatextfield="Title" datavaluefield="ExtensionConfigurationId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" ' + multipleselection + ' on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>';
        }
    }]);