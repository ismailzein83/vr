var RatePlanningController = function ($scope, $http, $location, $routeParams, notify, RoutingAPIService) {
    defineScopeObjects();
    defineScopeMethods();

    function defineScopeObjects() {
        $scope.subViewConnector = {};
    }

    function defineScopeMethods() {

        $scope.hide = function () {
            $scope.$hide();
        };
    }

}

RatePlanningController.$inject = ['$scope', '$http', '$location', '$routeParams', 'notify', 'RoutingAPIService'];
appControllers.controller('RatePlanningController', RatePlanningController)