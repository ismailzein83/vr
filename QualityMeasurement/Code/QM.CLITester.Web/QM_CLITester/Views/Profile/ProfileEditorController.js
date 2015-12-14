(function (appControllers) {

    "use strict";

    QM_CLITester_ProfileEditorController.$inject = ['$scope', 'QM_CLITester_ProfileAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function QM_CLITester_ProfileEditorController($scope, QM_CLITester_ProfileAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var profileId;

        var profileEntity;


        loadParameters();
        defineScope();
        load();
        var profileSettingAPI;
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                profileId = parameters.profileId;
            }

        }

        function defineScope() {

            $scope.scopeModal = {};

            setDirectiveTabs();
            $scope.SaveProfile = function () {
                return updateProfile();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.isLoading = true;
            $scope.title = "Edit Profile";
            getProfile().then(function () {
                loadAllControls()
            }).catch(function () {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }


        function loadAllControls() {
            $scope.scopeModal.name = profileEntity.Name;

            var promises = [];

            for (var i = 0 ; i < $scope.directiveTabs.length ; i++) {
                var promise = $scope.directiveTabs[i].readypromisedeferred.promise;
                promises.push(promise);
            }

            var j = 0;
            angular.forEach(promises, function (promise) {
                promise.then(function () {
                    $scope.directiveTabs[j].directiveAPI.load(profileEntity.Settings.ExtendedSettings[j]);
                    j++;
                });
            })

            $scope.isLoading = false;
        }



        function getProfile() {
            return QM_CLITester_ProfileAPIService.GetProfile(profileId).then(function (profile) {
                profileEntity = profile;
                console.log('profileEntity')
                console.log(profileEntity)
            });
        }

        function buildProfileObjFromScope() {
            var profile = {
                ProfileId: (profileId != null) ? profileId : 0,
                Name: $scope.scopeModal.name
            };


            var extendedSetting = [];
            for (var i = 0 ; i < $scope.directiveTabs.length ; i++) {

                console.log('$scope.directiveTabs[i].directiveAPI.getData()')

                console.log($scope.directiveTabs[i].directiveAPI.getData())

                if ($scope.directiveTabs[i].directiveAPI != undefined)
                    extendedSetting[extendedSetting.length] = $scope.directiveTabs[i].directiveAPI.getData();
            }
            profile.Settings = {
                ExtendedSettings: extendedSetting
            }
            console.log(profile)
            return profile;
        }


        function updateProfile() {
            var profileObject = buildProfileObjFromScope();
            QM_CLITester_ProfileAPIService.UpdateProfile(profileObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Profile", response, "Name")) {
                    if ($scope.onProfileUpdated != undefined)
                        $scope.onProfileUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
        }

        function setDirectiveTabs() {
            $scope.directiveTabs = [];
            var cliTab = {
                title: "Settings",
                directive: "vr-qm-clitester-profilesettings",
                readypromisedeferred: UtilsService.createPromiseDeferred(),
                loadDirective: function (api) {
                    cliTab.profileSettingAPI = api;
                    cliTab.readypromisedeferred.resolve();
                },
                dontLoad: false
            };
            $scope.directiveTabs.push(cliTab);
        }
    }

    appControllers.controller('QM_CLITester_ProfileEditorController', QM_CLITester_ProfileEditorController);
})(appControllers);
