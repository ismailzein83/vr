appControllers.controller('RouteBlockController',
    function RouteBlockController($scope, $http) {

        $scope.subViewConnector.getActionData = function () {          
            return {
                $type: "TOne.LCR.Entities.BlockRouteActionData, TOne.LCR.Entities"
            }
        }
        
    });