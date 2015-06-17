TestViewController.$inject = ['$scope'];

function TestViewController($scope) {
   
    $scope.testModel = 'TestViewController';
}
appControllers.controller('BusinessProcess_TestViewController', TestViewController);