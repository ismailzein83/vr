(function (appControllers) {

    'use strict';

    EditMyLanguageController.$inject = ['$scope', 'VR_Sec_SecurityAPIService', 'VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VR_Sec_UserAPIService'];

    function EditMyLanguageController($scope, VR_Sec_SecurityAPIService, VRNotificationService, UtilsService, VRUIUtilsService, VR_Sec_UserAPIService) {
        var languageSelectorAPI;
        var languageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var languageId;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
        }

        function defineScope() {
            $scope.save = function () {
                updateMyLanguage();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.onLanguageSelectorReady = function (api) {
                languageSelectorAPI = api;
                languageSelectorReadyDeferred.resolve();
            };

        }

        function load() {
            VR_Sec_UserAPIService.GetUserLanguageId().then(function (response) {
                if (response != null)
                    languageId = response;
                loadAllControls();
            });
          
        }

        function loadAllControls() {
            function loadLanguageSelector() {
                var languageSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                languageSelectorReadyDeferred.promise.then(function () {
                    var payload = {};
                    if (languageId != undefined) {
                        payload.selectedIds = languageId;
                    }
                    VRUIUtilsService.callDirectiveLoad(languageSelectorAPI, payload, languageSelectorLoadDeferred);
                });
                return languageSelectorLoadDeferred.promise;
            }
            return UtilsService.waitMultipleAsyncOperations([loadLanguageSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildObjectFromScope() {
            var userSetting = {
                LanguageId: languageSelectorAPI.getSelectedIds()
            };
        return userSetting;
    }
        function updateMyLanguage() {
            var userSetting = buildObjectFromScope();
            VR_Sec_UserAPIService.UpdateMyLanguage(userSetting).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Language", response)) {
                    if ($scope.onLanguageUpdated != undefined) {
                        $scope.onLanguageUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    };

appControllers.controller('VR_Sec_EditMyLanguageController', EditMyLanguageController);

})(appControllers);
