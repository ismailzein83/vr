'use strict';
app.directive('vrWhsRoutingRprouteDetails', ['UtilsService', 'WhS_Routing_RPRouteAPIService',
    function (UtilsService, WhS_Routing_RPRouteAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                ctrl.policyOptions = [];

                var ctor = new rpRouteDetailsCtor(ctrl, $scope);
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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/RPRoute/Templates/RPRouteDetailTemplate.html"
        };

        function rpRouteDetailsCtor(ctrl, $scope) {
            var rpRouteDetail;

            function initializeController() {
                ctrl.rpRouteOptions = [];

                $scope.onPolicySelectItem = function (selectedItem) {
                    if (rpRouteDetail == undefined)
                        return;
                    
                    WhS_Routing_RPRouteAPIService.GetRouteOptionDetails(rpRouteDetail.RoutingDatabaseId, selectedItem.TemplateConfigID, rpRouteDetail.Entity.RoutingProductId, rpRouteDetail.Entity.SaleZoneId).then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.rpRouteOptions.push(item);
                        });
                    });
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    if (payload != undefined)
                        rpRouteDetail = payload.rpRouteDetail;

                    WhS_Routing_RPRouteAPIService.GetPoliciesOptionTemplates().then(function (response) {
                        angular.forEach(response, function (item) {
                            ctrl.policyOptions.push(item);
                        });
                    });
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);