appControllers.controller('RoutingRules_RouteBlockTemplateController',
    function RouteBlockController($scope, $http,RoutingRulesTemplatesEnum) {

        $scope.subViewConnector.getActionData = function () {          
            return {
                $type: RoutingRulesTemplatesEnum.BlockTemplate.objectType
            }
        }
        
    });