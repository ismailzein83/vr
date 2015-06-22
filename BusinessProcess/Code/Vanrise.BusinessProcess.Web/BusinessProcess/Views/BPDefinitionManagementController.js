BPDefinitionManagementController.$inject = ['$scope'];

function BPDefinitionManagementController($scope) {

  
    defineScope();
    load();

    function defineScope() {
        $scope.testModel = 'BusinessProcess_BPDefinitionManagementController';
        
    }

    function load() {
       
    }

};

appControllers.controller('BusinessProcess_BPDefinitionManagementController', BPDefinitionManagementController);