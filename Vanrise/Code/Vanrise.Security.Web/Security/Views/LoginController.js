LoginController.$inject = ['$scope', 'SecurityAPIService'];


function LoginController($scope, SecurityAPIService) {
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

                setCookie('TOne_LoginTokenCookie', response.Token, '', '', '');
                setCookie('TOne_LoginUserDisplayNameCookie', response.UserDisplayName, '', '', '');

                window.location.href = '/';
        }
        );       
    }

    function setCookie(name, value, expires, path, domain, secure) {
        document.cookie = name + "=" + escape(value) +
        ((expires) ? "; expires=" + expires.toGMTString() : "") +
        ("; path=/") +       //you having wrong quote here
        ((domain) ? "; domain=" + domain : "") +
        ((secure) ? "; secure" : "");
    }
}
appControllers.controller('Security_LoginController', LoginController);