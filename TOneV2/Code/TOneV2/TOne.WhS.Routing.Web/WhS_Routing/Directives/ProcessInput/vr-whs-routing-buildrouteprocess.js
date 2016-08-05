"use strict";

app.directive("vrWhsRoutingBuildrouteprocess", ['UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'WhS_Routing_CodePrefixOptions',
    function (UtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, WhS_Routing_CodePrefixOptions) {
    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl:"/Client/Modules/WhS_Routing/Directives/ProcessInput/Templates/BuildRouteProcessTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;



        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            
            $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
            $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
            $scope.codePrefixOptions = WhS_Routing_CodePrefixOptions;
            $scope.selectedCodePrefixOption = WhS_Routing_CodePrefixOptions[2];

            $scope.onRoutingProductDatabaseTypeSelectionChanged = function () {
                $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
            }

            /* directive API definition */

            var api = {};
            api.getData = function () {
                return {
                    InputArguments: {
                        $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                        EffectiveTime: !$scope.isFuture ? $scope.effectiveOn : null,
                        IsFuture: $scope.isFuture,
                        RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                        RoutingProcessType: WhS_Routing_RoutingProcessTypeEnum.CustomerRoute.value,
                        CodePrefixLength: $scope.selectedCodePrefixOption
                    }
                };
            };

            api.load = function (payload) {
                if (!$scope.isFuture)
                    $scope.effectiveOn = new Date();
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
