"use strict";

BuildRoutesByCodePrefixController.$inject = ['$scope'];

function BuildRoutesByCodePrefixController($scope) {
    
    defineScope();
    load();

    function defineScope() {
        $scope.createProcessInput.getData = function () {
            return {
                InputArguments: {
                    $type: "TOne.WhS.Routing.BP.Arguments.BuildRoutesByCodePrefixInput, TOne.WhS.Routing.BP.Arguments",
                    CodePrefix: $scope.codePrefix,
                    EffectiveOn: $scope.effectiveOn,
                    IsFuture: $scope.isFuture
                }
            };
        };
    }

    function load() {
        
    }
}

appControllers.controller('WhS_Routing_BuildRoutesByCodePrefixController', BuildRoutesByCodePrefixController)



