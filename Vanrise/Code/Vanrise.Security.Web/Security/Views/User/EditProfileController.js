(function (appControllers) {

    'use strict';

    EditProfileController.$inject = ['$scope', 'VR_Sec_UserAPIService', 'VRNotificationService'];

    function EditProfileController($scope, VR_Sec_UserAPIService, VRNotificationService) {

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
        }

        function defineScope() {
            $scope.save = function () {
                var userProfileObject = {
                    UserId: $scope.userObject.UserId,
                    Name: $scope.name,
                    PhotoFileId: $scope.userPhoto != null ? $scope.userPhoto.fileId : null
                };

                return VR_Sec_UserAPIService.EditUserProfile(userProfileObject).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's Name", response)) {
                        if ($scope.onProfileUpdated != undefined)
                            $scope.onProfileUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
    
        function load() {
            $scope.isLoading = true;

            return VR_Sec_UserAPIService.LoadLoggedInUserProfile().then(function (response) {
                fillScopeFromUserObj(response);
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function fillScopeFromUserObj(userObject) {
            $scope.name = userObject.Name;
            $scope.userObject = userObject;
            if (userObject.PhotoFileId != null)
                $scope.userPhoto = {
                    fileId: userObject.PhotoFileId
                };
            else
                $scope.userPhoto = null;
        }
    }

    appControllers.controller('VR_Sec_EditProfileController', EditProfileController);

})(appControllers);
