LoginController.$inject = ['$scope', 'SecurityAPIService', 'SecurityService', 'VRNotificationService'];


function LoginController($scope, SecurityAPIService, SecurityService, VRNotificationService) {
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
                if (VRNotificationService.notifyOnUserAuthenticated(response))
                {
                    var userInfo = JSON.stringify(response.AuthenticationObject);
                    SecurityService.createAccessCookie(userInfo);
                    window.location.href = '/';
                }                
        }
        );       
    }

   
}
appControllers.controller('Security_LoginController', LoginController);