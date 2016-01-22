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
                    Name: $scope.name
                };

                return VR_Sec_UserAPIService.EditUserProfile(userProfileObject).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's Name", response)) {
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
        }
    }

    appControllers.controller('VR_Sec_EditProfileController', EditProfileController);

})(appControllers);
