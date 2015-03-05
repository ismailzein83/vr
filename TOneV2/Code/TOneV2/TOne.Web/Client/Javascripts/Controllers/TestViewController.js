appControllers.controller('TestViewController',
    function DefaultController($scope) {
        $scope.model = 'Test View model';
        $scope.Input = '123';
        $scope.alertMsg = function () {
            alert($scope.Input);
        };
    });