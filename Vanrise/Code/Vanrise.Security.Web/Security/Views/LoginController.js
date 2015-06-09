LoginController.$inject = ['$scope', 'SecurityAPIService'];


function LoginController($scope, SecurityAPIService) {
    var arrMenuAction = [];

    defineScope();
    load();

    function defineScope() {
        $scope.Login = login;
    }

    function load() {
    }

    function login()
    {

    }
}
appControllers.controller('Security_LoginController', LoginController);