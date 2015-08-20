LoginController.$inject = ['$scope', 'SecurityAPIService', 'SecurityService'];


function LoginController($scope, SecurityAPIService, SecurityService) {
    defineScope();
    load();

    function defineScope() {
        
        $scope.Login = login;
    }

    function load() {
    }

    function login()
    {
        var credentialsObject = {
            Email: $scope.email,
            Password: $scope.password
        };

        return SecurityAPIService.Authenticate(credentialsObject)
            .then(function (response) {
                var userInfo = JSON.stringify(response);
                console.log(userInfo);
                SecurityService.createAccessCookie(userInfo);
                window.location.href = '/';
        }
        );       
    }

   
}
appControllers.controller('Security_LoginController', LoginController);