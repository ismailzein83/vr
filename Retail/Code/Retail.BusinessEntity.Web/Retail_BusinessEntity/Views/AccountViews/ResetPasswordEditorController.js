(function (appControllers) {
    'use strict';

    ResetPasswordEditorController.$inject = ['$scope', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_PortalAccountAPIService'];

    function ResetPasswordEditorController($scope, VRNavigationService, VRNotificationService, Retail_BE_PortalAccountAPIService) {

        var userId;
        var connectionId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                userId = parameters.userId;
                var context = parameters.context;

                if (context != undefined) {
                    connectionId = context.getConnectionId();
                }
            }
        }
        function defineScope() {

            $scope.validatePasswords = function () {
                if ($scope.scopeModel.password != $scope.scopeModel.confirmedPassword)
                    return 'Passwords do not match';
                return null;
            };

            $scope.save = function () {
                var resetPasswordInput = {
                    UserId: userId,
                    ConnectionId: connectionId,
                    Password: $scope.scopeModel.password
                };

                return Retail_BE_PortalAccountAPIService.ResetPassword(resetPasswordInput).then(function (response) {
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

    appControllers.controller('Retail_BE_ResetPasswordEditorController', ResetPasswordEditorController);

})(appControllers);
