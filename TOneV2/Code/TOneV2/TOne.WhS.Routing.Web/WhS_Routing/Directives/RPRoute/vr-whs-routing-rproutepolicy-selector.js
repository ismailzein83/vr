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
                }
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
                + '<vr-select ' + multipleselection + '  datatextfield="Name" datavaluefield="TemplateConfigID" '
            + required + ' label="' + label + '" datasource="ctrl.datasource" selectedvalues="ctrl.selectedvalues" onselectitem="ctrl.onselectitem"  onselectionchanged="ctrl.onselectionchanged" entityName="' + label + '" ' + hideremoveicon + '></vr-select>'
               + '</div>'
        }

        function rpRoutePolicyCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                defineAPI();
            }
            var policies;
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.datasource.length = 0;
                    var filteredIds;
                    var selectedId;

                    if (policies != undefined) {
                        fillPolicies(payload);
                    }
                    else {
                        policies = [];
                        return WhS_Routing_RPRouteAPIService.GetPoliciesOptionTemplates().then(function (response) {
                            angular.forEach(response, function (itm) {
                                policies.push(itm);
                            });
                            fillPolicies(payload);
                        });
                    }
                }

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('TemplateConfigID', $attrs, ctrl);
                }

                api.getFilteredPolicies = function () {
                    if (policies != undefined)
                        return policies;
                    else
                        return ctrl.datasource;

                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function fillPolicies(payload) {
                if (payload != undefined) {
                    angular.forEach(payload.filteredIds, function (filteredId) {
                        var policy = UtilsService.getItemByVal(policies, filteredId, 'TemplateConfigID');
                        if (policy != null)
                            ctrl.datasource.push(policy);
                    });
                    if (payload.selectedId != undefined)
                        VRUIUtilsService.setSelectedValues(payload.selectedId, 'TemplateConfigID', $attrs, ctrl);
                }
                else {
                    angular.forEach(ctrl.policies, function (itm) {
                        ctrl.datasource.push(itm);
                    });
                }
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);