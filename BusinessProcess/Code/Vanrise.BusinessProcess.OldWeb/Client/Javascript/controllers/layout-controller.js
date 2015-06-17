function LayoutCtrl($scope, $location, $rootScope) {
    $scope.backHome = function () {
        $location.path("/ListTracking");
    };
    $rootScope.filter = {};
}  