RoleManagementController.$inject = ['$scope', 'RoleAPIService'];


function RoleManagementController($scope, RoleAPIService) {

    defineScope();
    load();

    function defineScope() {
        $scope.testModel = "Security_RoleManagementController";
        $scope.roles = [];
    }

    function load() {
        getData();
    }

    function getData()
    {
        RoleAPIService.GetRoles()
        .then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.roles.push(itm);
            });
        });
    }
  
}
appControllers.controller('Security_RoleManagementController', RoleManagementController);