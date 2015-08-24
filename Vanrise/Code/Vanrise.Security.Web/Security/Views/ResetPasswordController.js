ResetPasswordController.$inject = ['$scope', 'SecurityAPIService', 'VRNotificationService'];

function ResetPasswordController($scope, SecurityAPIService, VRNotificationService) {

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
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
               
                return SecurityAPIService.ResetPassword($scope.txtPasswordOld,$scope.txtPasswordNew)
                        .then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("User's password", response)) {
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