"use strict";

BuildRouteProcessController.$inject = ['$scope', 'UtilsService', 'WhS_Routing_RoutingDatabaseTypeEnum'];

function BuildRouteProcessController($scope, UtilsService, WhS_Routing_RoutingDatabaseTypeEnum) {

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
                    RoutingDatabaseType: $scope.selectedRoutingDatabaseType.value,
                    CodePrefixLength: $scope.codePrefixLength
                }
            };
        };
    }

    function load() {
        $scope.routingDatabaseTypes = UtilsService.getArrayEnum(WhS_Routing_RoutingDatabaseTypeEnum);
        $scope.selectedRoutingDatabaseType = UtilsService.getEnum(WhS_Routing_RoutingDatabaseTypeEnum, 'value', WhS_Routing_RoutingDatabaseTypeEnum.Current.value);
    }
}

appControllers.controller('WhS_Routing_BuildRouteProcessController', BuildRouteProcessController)



