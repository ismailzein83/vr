EditProfileController.$inject = ['$scope', 'UsersAPIService', 'VRNotificationService'];

function EditProfileController($scope, UsersAPIService, VRNotificationService) {

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
    }

    function defineScope() {

        $scope.EditUserProfile = function () {

            var userProfileObject = {
                UserId: $scope.userObject.UserId,
                Name: $scope.txtName
            }


            return UsersAPIService.EditUserProfile(userProfileObject)
                        .then(function (response) {
                            if (VRNotificationService.notifyOnItemUpdated("User's Name", response)) {
                                $scope.modalContext.closeModal();
                            }
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                      
        }

        $scope.close = function () {
            $scope.modalContext.closeModal();
        };

    }
    
    function load() {

        $scope.isGettingData = true;
        return UsersAPIService.LoadLoggedInUserProfile()
          .then(function (response) {
              fillScopeFromUserObj(response);
          })
          .catch(function (error) {
              VRNotificationService.notifyExceptionWithClose(error, $scope);
          })
         .finally(function () {
             $scope.isGettingData = false;
         });
     

    }

    function fillScopeFromUserObj(userObject) {
        $scope.txtName = userObject.Name;
        $scope.userObject = userObject;
     
    }

};

appControllers.controller('Security_EditProfileController', EditProfileController);