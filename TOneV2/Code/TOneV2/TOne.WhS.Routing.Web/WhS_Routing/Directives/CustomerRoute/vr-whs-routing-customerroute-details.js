'use strict';
app.directive('vrWhsRoutingCustomerrouteDetails', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new customerRouteDetailsCtor(ctrl, $scope);
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
            templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteDetailTemplate.html"
        };

        function customerRouteDetailsCtor(ctrl, $scope) {

            function initializeController() {
                $scope.routeOptionDetails = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var customerRoute;
                    if (payload != undefined)
                        customerRoute = payload.customerRoute;
                     
                    if (customerRoute != undefined && customerRoute.RouteOptionDetails != null)
                    {
                        for (var i = 0; i < customerRoute.RouteOptionDetails.length; i++) {
                            $scope.routeOptionDetails.push(customerRoute.RouteOptionDetails[i]);
                        }
                    }
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);