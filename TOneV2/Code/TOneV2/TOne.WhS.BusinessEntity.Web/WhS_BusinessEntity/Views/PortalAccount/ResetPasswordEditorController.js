(function (appControllers) {
    'use strict';

    ResetPasswordEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'WhS_BE_PortalAccountAPIService'];

    function ResetPasswordEditorController($scope, VRNavigationService, VRNotificationService, WhS_BE_PortalAccountAPIService) {

        var userId;
        var context; 
        var carrierProfileId; 

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                carrierProfileId = parameters.carrierProfileId;
                context = parameters.context;
                userId = parameters.userId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.validatePasswords = function () {
                if ($scope.scopeModel.password != $scope.scopeModel.confirmedPassword)
                    return 'Passwords do not match';
                return null;
            };

            $scope.save = function () {
                var resetPasswordInput = {
                    CarrierProfileId: carrierProfileId,
                    Password: $scope.scopeModel.password,
                    UserId: userId
                };

                return WhS_BE_PortalAccountAPIService.ResetPassword(resetPasswordInput).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("User's password is", response)) {
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
            setTitle();
        }

        function setTitle() {
            $scope.title = 'Reset Password';
        }
    }

    appControllers.controller('WhS_BE_ResetPasswordEditorController', ResetPasswordEditorController);

})(appControllers);
