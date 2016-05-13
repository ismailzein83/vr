(function (appControllers) {

    "use strict";

    BEInternal_LandingPageController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationAPIService','$location'];

    function BEInternal_LandingPageController($scope, CloudPortal_BEInternal_CloudApplicationAPIService, $location) {

        defineScope();
        getUserApplications();
        function defineScope() {
            $scope.modal = {};
            $scope.onApplicationClicked = function (application) {
                window.location.href = application.Settings.OnlineURL;
            }
        }

        function getUserApplications() {
            $scope.isLoading = true;
            CloudPortal_BEInternal_CloudApplicationAPIService.GetCloudApplicationByUser().then(function (response) {
                $scope.currentApplications = response;
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('CloudPortal_BEInternal_LandingPageControllerController', BEInternal_LandingPageController);
})(appControllers);