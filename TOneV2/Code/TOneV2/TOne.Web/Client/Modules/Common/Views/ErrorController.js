ErrorController.$inject = ['$scope', 'VRNavigationService'];

function ErrorController($scope, VRNavigationService) {

    var parameters = VRNavigationService.getParameters($scope);
    if (parameters != undefined) {
        $scope.error = parameters.error;
        $scope.previousUrl = decodeURIComponent(parameters.previousUrl);
    }

    $scope.goBackToPage = function () {
        VRNavigationService.goto($scope.previousUrl);
    };

}

appControllers.controller('Common_ErrorController', ErrorController);