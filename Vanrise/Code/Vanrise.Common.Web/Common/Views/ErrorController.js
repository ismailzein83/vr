ErrorController.$inject = ['$scope', 'VRNavigationService', '$location'];

function ErrorController($scope, VRNavigationService, $location) {

    var parameters = VRNavigationService.getParameters($scope);
    if (parameters != undefined) {
        $scope.error = parameters.error;
        $scope.previousUrl = decodeURIComponent(parameters.previousUrl);
    }

    $scope.goBackToPage = function () {
        $location.path($scope.previousUrl).replace();
       // VRNavigationService.goto($scope.previousUrl);
    };

}

appControllers.controller('Common_ErrorController', ErrorController);