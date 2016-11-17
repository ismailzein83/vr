(function (appControllers) {

    "use strict";

    countryUploaderController.$inject = ['$scope', 'VRCommon_CountryAPIService', 'VRNotificationService', 'UtilsService'];

    function countryUploaderController($scope, VRCommon_CountryAPIService, VRNotificationService, UtilsService) {
        var fileID;

        defineScope();
        load();

        function defineScope() {
            $scope.isUploadingComplete = false;
            $scope.downloadTemplate = function () {
                return VRCommon_CountryAPIService.DownloadCountriesTemplate().then(function (response) {
                    UtilsService.downloadFile(response.data, response.headers);
                });
            };
            $scope.hasDownloadCountryPermission = function () {
                return VRCommon_CountryAPIService.HasDownloadCountryPermission();
            };


            $scope.uploadCountires = function () {
                return VRCommon_CountryAPIService.UploadCountries($scope.file.fileId).then(function (response) {
                    $scope.isUploadingComplete = true;
                    $scope.addedCountries = response.CountOfCountriesAdded;
                    $scope.existsCountries = response.CountOfCountriesExist;
                    fileID = response.fileID;
                    VRNotificationService.showSuccess("Country Finished Upload");
                }).catch(function (error) {
                    VRNotificationService.showError(error.ExceptionMessage);
                });;
            };

            $scope.downloadLog = function () {
                if (fileID != undefined) {
                    return VRCommon_CountryAPIService.DownloadCountryLog(fileID).then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
                }
            };

            $scope.hasUploadCountryPermission = function () {
                return VRCommon_CountryAPIService.HasUploadCountryPermission();
            };
        }

        function load() {
        }
    }

    appControllers.controller('VRCommon_CountryUploaderController', countryUploaderController);
})(appControllers);
