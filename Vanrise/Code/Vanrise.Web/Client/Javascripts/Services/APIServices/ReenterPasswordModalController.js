(function (appControllers) {
    'use strict';

    ReenterPasswordModalController.$inject = ['$scope', 'Sec_CookieService', 'SecurityService'];

    function ReenterPasswordModalController($scope, Sec_CookieService, SecurityService) {
        var userId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
           
        }

        function defineScope() {
            $scope.OkClicked = function () {
                SecurityService.authenticate($scope.email, $scope.password).then(function () {
                    if ($scope.onAuthenticated != undefined)
                        $scope.onAuthenticated();
                    $scope.modalContext.closeModal();
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            setTitle();
            $scope.email = Sec_CookieService.getLoggedInUserInfo().UserName;
        }

        function setTitle() {
            $scope.title = 'Session Expired. Please Enter your Password';
        }
    }

    appControllers.controller('VR_ReenterPasswordModalController', ReenterPasswordModalController);

})(appControllers);
