(function (appControllers) {

    "use strict";

    countryUploaderController.$inject = ['$scope', 'VRCommon_CountryAPIService', 'VRNotificationService', 'UtilsService'];

    function countryUploaderController($scope, VRCommon_CountryAPIService, VRNotificationService, UtilsService) {
        
        defineScope();
        load();

        function defineScope() {
            $scope.downloadTemplate = function () {
                return VRCommon_CountryAPIService.DownloadCountriesTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            }
            $scope.hasDownloadCountryPermission = function () {
                return VRCommon_CountryAPIService.HasDownloadCountryPermission();
            };


            $scope.uploadCountires = function () {                
                return VRCommon_CountryAPIService.UploadCountries($scope.file.fileId).then(function (response) {
                    VRNotificationService.showInformation(response)
                }).catch(function (error) {
                    VRNotificationService.showError(error.ExceptionMessage);
                });;
            }
            $scope.hasUploadCountryPermission = function () {
                return VRCommon_CountryAPIService.HasUploadCountryPermission();
            };
        }

        function load() {
        }
    }

    appControllers.controller('VRCommon_CountryUploaderController', countryUploaderController);
})(appControllers);
