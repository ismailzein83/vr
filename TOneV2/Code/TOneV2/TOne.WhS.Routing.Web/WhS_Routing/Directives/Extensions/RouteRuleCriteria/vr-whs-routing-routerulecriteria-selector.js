
'use strict';

app.directive('vrWhsRoutingRouterulecriteriaSelector', ['WhS_Routing_RouteRuleAPIService', 'UtilsService', 'VRUIUtilsService',

    function (WhS_Routing_RouteRuleAPIService, UtilsService, VRUIUtilsService) {
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

                var routeRuleSettingsSelector = new RouteRuleSettingsSelector(ctrl, $scope, $attrs);
                routeRuleSettingsSelector.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function RouteRuleSettingsSelector(ctrl, $scope, attrs) {
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
                    var defaultRuleCriteria;

                    return WhS_Routing_RouteRuleAPIService.GetRouteRuleCriteriaTemplates().then(function (response) {
                        selectorAPI.clearDataSource();

                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                var currentRuleCriteria = response[i];
                                if (currentRuleCriteria.IsDefault)
                                    defaultRuleCriteria = currentRuleCriteria.ExtensionConfigurationId;
                                ctrl.datasource.push(currentRuleCriteria);
                            }

                            if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', attrs, ctrl);
                            }
                            else if (defaultRuleCriteria != undefined) {
                                if (Object.prototype.toString.call(ctrl.selectedvalues) === '[object Array]') {
                                    var tempDefaultRuleCriteria = [];
                                    tempDefaultRuleCriteria.push(defaultRuleCriteria);
                                    VRUIUtilsService.setSelectedValues(tempDefaultRuleCriteria, 'ExtensionConfigurationId', attrs, ctrl);
                                }
                                else {
                                    VRUIUtilsService.setSelectedValues(defaultRuleCriteria, 'ExtensionConfigurationId', attrs, ctrl);
                                }
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
            var label = "Rule Criteria";

            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
                label = "Rule Criteria";
            }

            var hideremoveicon;
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<vr-select datatextfield="Title" datavaluefield="ExtensionConfigurationId" isrequired="ctrl.isrequired" label="' + label +
                       '" datasource="ctrl.datasource" ' + multipleselection + '  on-ready="ctrl.onSelectorReady" selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"' +
                       '" onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ' + hideremoveicon + ' customvalidate="ctrl.customvalidate">' +
                   '</vr-select>';
        }
    }]);