'use strict';

app.directive('vrWhsRoutingRproutepolicySelector', ['WhS_Routing_RPRouteAPIService', 'UtilsService', 'VRUIUtilsService',
    function (WhS_Routing_RPRouteAPIService, UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                onselectitem: '=',
                isrequired: "@",
                selectedvalues: '=',
                hideremoveicon: "@"
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.selectedvalues;
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];

                ctrl.datasource = [];
                var ctor = new rpRoutePolicyCtor(ctrl, $scope, $attrs);
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
                return getTemplate(attrs);
            }

        };

        function getTemplate(attrs) {
            var multipleselection = "";
            var label = "Policy";
            if (attrs.ismultipleselection != undefined) {
                label = "Policies";
                multipleselection = "ismultipleselection";
            }
            var required = "";
            if (attrs.isrequired != undefined)
                required = "isrequired";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            return '<div>'
                + '<vr-select on-ready="ctrl.onSelectorReady" ' + multipleselection + '  datatextfield="Title" datavaluefield="ExtensionConfigurationId" '
            + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectitem="ctrl.onselectitem"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" ' + hideremoveicon + '></vr-select>'
               + '</div>';
        }

        function rpRoutePolicyCtor(ctrl, $scope, $attrs) {

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

                    var filter;
                    var selectDefaultPolicy;
                    var selectedIds;

                    if (payload != undefined) {
                        filter = payload.filter;
                        selectDefaultPolicy = payload.selectDefaultPolicy;
                        selectedIds = payload.selectedIds;
                    }

                    return WhS_Routing_RPRouteAPIService.GetPoliciesOptionTemplates(UtilsService.serializetoJson(filter)).then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                ctrl.datasource.push(response[i]);
                            }

                            if (selectDefaultPolicy === true) {
                                var defaultPolicy = UtilsService.getItemByVal(ctrl.datasource, true, 'IsDefault'); // The response is invalid if no default policy exists
                                VRUIUtilsService.setSelectedValues(defaultPolicy.ExtensionConfigurationId, 'ExtensionConfigurationId', $attrs, ctrl);
                            }
                            else if (selectedIds != undefined) {
                                VRUIUtilsService.setSelectedValues(selectedIds, 'ExtensionConfigurationId', $attrs, ctrl);
                            }

                        }
                    });
                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('ExtensionConfigurationId', $attrs, ctrl);
                };

                api.getDefaultPolicyId = function () {
                    return UtilsService.getItemByVal(ctrl.datasource, true, 'IsDefault').ExtensionConfigurationId;
                };

                api.getFilteredPoliciesIds = function () {
                    return UtilsService.getPropValuesFromArray(ctrl.datasource, 'ExtensionConfigurationId');
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);