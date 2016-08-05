"use strict";

BuildRouteProcessController.$inject = ['$scope', 'UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum', 'WhS_Routing_RoutingProcessTypeEnum', 'WhS_Routing_CodePrefixOptions'];

function BuildRouteProcessController($scope, UtilsService, WhS_Routing_RoutingDatabaseTypeEnum, WhS_Routing_RoutingProcessTypeEnum, WhS_Routing_CodePrefixOptions) {

    defineScope();
    load();

    function defineScope() {
        $scope.onRoutingProductDatabaseTypeSelectionChanged = function () {
            $scope.isFuture = $scope.selectedRoutingDatabaseType == WhS_Routing_RoutingDatabaseTypeEnum.Future;
        }

        $scope.createProcessInput.getData = function () {
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
    }

    function load() {
        $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
        $scope.codePrefixOptions = WhS_Routing_CodePrefixOptions;
        $scope.selectedCodePrefixOption = WhS_Routing_CodePrefixOptions[2];

        if (!$scope.isFuture)
            $scope.effectiveOn = new Date();
    }
}

appControllers.controller('WhS_Routing_BuildRouteProcessController', BuildRouteProcessController)



