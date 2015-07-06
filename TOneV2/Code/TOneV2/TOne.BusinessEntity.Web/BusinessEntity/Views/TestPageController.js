TestPageController.$inject = ['$scope'];

function TestPageController($scope) {
  
    defineScope();
    load();

    function defineScope() {

        $scope.testModel = 'BusinessEntity_TestPageController';
    }

    function load() {
       
    }
}

appControllers.controller('BusinessEntity_TestPageController', TestPageController);