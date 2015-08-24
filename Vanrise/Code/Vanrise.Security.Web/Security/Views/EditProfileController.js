EditProfileController.$inject = ['$scope', 'UsersAPIService', 'VRNotificationService'];

function EditProfileController($scope, UsersAPIService, VRNotificationService) {

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }

    function defineScope() {

        $scope.txtName = "";

        $scope.EditUserProfile = function () {

            return UsersAPIService.EditUserProfile($scope.txtName)
                        .then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("User's Name", response)) {
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

        $scope.hide = function () {
            $scope.$hide();
        };

    }
    
    function load() {
    }

};

appControllers.controller('Security_EditProfileController', EditProfileController);