(function (appControllers) {
    'use strict';

    ReenterPasswordModalController.$inject = ['$scope', 'Sec_CookieService', 'SecurityService', 'VRNotificationService', 'VR_Sec_SecurityProviderAPIService', 'UtilsService', 'VRUIUtilsService'];

    function ReenterPasswordModalController($scope, Sec_CookieService, SecurityService, VRNotificationService, VR_Sec_SecurityProviderAPIService, UtilsService, VRUIUtilsService) {

        var authenticateUserEditorApi;
        var authenticateUserEditorPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {

        }

        function defineScope() {
            $scope.onAuthenticateUserEditorReady = function (api) {
                authenticateUserEditorApi = api;
                authenticateUserEditorPromiseDeferred.resolve();
            };

            $scope.OkClicked = function () {
                SecurityService.authenticate(Sec_CookieService.getLoggedInUserInfo().SecurityProviderId, authenticateUserEditorApi.getData()).then(function () {
                    if ($scope.onAuthenticated != undefined)
                        $scope.onAuthenticated();
                    $scope.modalContext.closeModal();
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        //function load() {
        //    console.log(Sec_CookieService.getLoggedInUserInfo());
        //    $scope.email = Sec_CookieService.getLoggedInUserInfo().UserName;
        //}


        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            function loadSecurityProvider() {
                var securityProviderId = Sec_CookieService.getLoggedInUserInfo().SecurityProviderId;
                return VR_Sec_SecurityProviderAPIService.GetSecurityProviderInfobyId(securityProviderId).then(function (response) {
                    $scope.selectedSecurityProvider = response;
                });
            }

            function loadAuthenticateUserEditor() {
                var authenticateUserEditorLoadDeferred = UtilsService.createPromiseDeferred();
                authenticateUserEditorPromiseDeferred.promise.then(function () {
                    var authenticateUserEditorPayload = { username: Sec_CookieService.getLoggedInUserInfo().UserName };
                    VRUIUtilsService.callDirectiveLoad(authenticateUserEditorApi, authenticateUserEditorPayload, authenticateUserEditorLoadDeferred);
                });
                return authenticateUserEditorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([loadSecurityProvider, loadAuthenticateUserEditor, setTitle]).then(function () { }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function setTitle() {
            $scope.title = 'Session Expired. Please Enter your Password';
        }
    }

    appControllers.controller('VR_ReenterPasswordModalController', ReenterPasswordModalController);

})(appControllers);
