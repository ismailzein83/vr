ResetPasswordController.$inject = ['$scope', 'SecurityAPIService', 'VRNavigationService', 'VRNotificationService'];

function ResetPasswordController($scope, SecurityAPIService, VRNavigationService, VRNotificationService) {

   // var parameters;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        //parameters = VRNavigationService.getParameters($scope);
        //console.log(parameters);
    }

    function defineScope() {

        $scope.txtPasswordOld = "";
        $scope.txtPasswordNew = "";
        $scope.txtPasswordConfirmed = "";

        $scope.ResetPassword = function () {

       
            if ($scope.txtPasswordNew.toLowerCase() != $scope.txtPasswordConfirmed.toLowerCase()) {
                return "Mismatched Passwords"
            }
            else {
               
                //var credentialsObject = {
                //    OldPassword: $scope.txtPasswordOld,
                //    NewPassword: $scope.txtPasswordNew
                //};

                return SecurityAPIService.ResetPassword($scope.txtPasswordOld,$scope.txtPasswordNew)
                        .then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("User's password was", response)) {
                               $scope.modalContext.closeModal();
                            }
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                            console.log(error);
                        
                        })
                        .finally(function () {
                            $scope.$hide();
                        });
                }

           
        };

        $scope.hide = function () {
            $scope.$hide();
        };

        $scope.ConfirmPassword = function (text) {

            if ($scope.txtPasswordNew.toLowerCase() != text.toLowerCase())
                return "Mismatched Password";
            else
                return null;
        };
    }

    function load() {
    }

};

appControllers.controller('Security_ResetPasswordController', ResetPasswordController);