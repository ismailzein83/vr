"use strict";

BuildRouteProcessController.$inject = ['$scope'];

function BuildRouteProcessController($scope) {

    defineScope();
    load();

    function defineScope() {
        $scope.createProcessInput.getData = function () {
            return {
                InputArguments: {
                    $type: "TOne.WhS.Routing.BP.Arguments.RoutingProcessInput, TOne.WhS.Routing.BP.Arguments",
                    EffectiveTime: $scope.effectiveOn,
                    RoutingDatabaseType: $scope.isFuture
                }
            };
        };
    }

    function load() {

    }
}

appControllers.controller('WhS_Routing_BuildRouteProcessController', BuildRouteProcessController)



